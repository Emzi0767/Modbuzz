// This file is part of Modbuzz project
//
// Copyright © 2024 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Emzi0767.Types;
using Emzi0767.Utilities;
using RJCP.IO.Ports;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Implements the Modbus RTU protocol.
/// </summary>
public sealed class ModbusRtu : IModbus
{
    private readonly AsyncEvent<ModbusRtu, ModbusEventArgs> _frameReceived;
    private readonly SerialPortStream _port;
    private readonly TimeSpan _tickGap;
    private readonly Channel<IModbusFrame> _frameQueue;
    private readonly ChannelReader<IModbusFrame> _frameQueueReader;
    private readonly ChannelWriter<IModbusFrame> _frameQueueWriter;
    private readonly IModbusSerdes _serdes;

    private Task _frameProcessingTask, _queueProcessingTask;
    private CancellationTokenSource _frameProcessingCancellationTokenSource;

    /// <summary>
    /// Creates a new Modbus RTU protocol adapter. For serial parameters, see <see cref="SerialPort"/> constructor.
    /// </summary>
    /// <param name="port">Serial port to use for communication.</param>
    /// <param name="baudRate">Baud rate of the serial port to use.</param>
    /// <param name="dataBits">Data bits for the serial port to use.</param>
    /// <param name="parity">Parity for the serial port to use.</param>
    /// <param name="stopBits">Stop bits for the serial port to use.</param>
    public ModbusRtu(string port, int baudRate, int dataBits, Parity parity, StopBits stopBits)
    {
        this._frameReceived = new("MODBUS_FRAME_RECEIVED", TimeSpan.FromSeconds(1), this.EventExceptionHandler);
        this._port = new SerialPortStream(port, baudRate, dataBits, parity, stopBits);
        this._tickGap = this.ComputeGapTicks(baudRate, dataBits, parity, stopBits);
        this._frameQueue = Channel.CreateBounded<IModbusFrame>(8);
        this._frameQueueReader = this._frameQueue.Reader;
        this._frameQueueWriter = this._frameQueue.Writer;
        this._serdes = new ModbusRtuSerdes();
    }

    /// <inheritdoc />
    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        if (this._port.IsOpen)
            return ValueTask.CompletedTask;

        this._port.DiscardNull = false;
        this._port.DtrEnable = true;
        this._port.Open();
        this._frameProcessingCancellationTokenSource = new();
        this._frameProcessingTask = this.ProcessEvents(this._frameProcessingCancellationTokenSource.Token);
        this._queueProcessingTask = this.ProcessQueue(this._frameProcessingCancellationTokenSource.Token);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!this._port.IsOpen)
            return ValueTask.CompletedTask;

        this._frameProcessingCancellationTokenSource.Cancel();
        this._port.Close();
        this._frameProcessingTask = null;
        this._queueProcessingTask = null;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask SendAsync(IModbusFrame frame, CancellationToken cancellationToken = default)
        => this._frameQueueWriter.WriteAsync(frame, cancellationToken);

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        try
        {
            this._frameProcessingCancellationTokenSource?.Cancel();
            this._frameProcessingCancellationTokenSource?.Dispose();
        }
        catch { }

        this._frameQueueWriter.Complete();
        this._port.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public event AsyncEventHandler<IModbus, ModbusEventArgs> FrameReceived
    {
        add => this._frameReceived.Register(value);
        remove => this._frameReceived.Unregister(value);
    }

    /// <inheritdoc />
    public event EventHandler<ModbusErrorEventArgs> EventExceptionThrown;

    private void EventExceptionHandler<T>(AsyncEvent<ModbusRtu, T> asyncEvent, Exception exception, AsyncEventHandler<ModbusRtu, T> handler, ModbusRtu sender, T eventArgs)
        where T : AsyncEventArgs
    {
        if (this.EventExceptionThrown != null)
            this.EventExceptionThrown(this, new(exception, sender, eventArgs));
    }

    private async Task ProcessEvents(CancellationToken cancellationToken)
    {
        await Task.Yield();
        using var tmp = MemoryPool<byte>.Shared.Rent(4096);
        using var buff = new ContinuousMemoryBuffer<byte>(initialSize: 4096);
        var mem = tmp.Memory;
        var cursor = 0;
        var port = this._port;
        while (!cancellationToken.IsCancellationRequested)
        {
            var br = await port.ReadAsync(mem[..256], cancellationToken);
            buff.Write(mem.Span[..br]);
            //cursor = 0;
            while (cursor < (int)buff.Count)
            {
                var result = this._serdes.TryParseFrame(buff.Span[cursor..], out var frame, out var bytesRead);
                cursor += bytesRead;

                if (result == ModbusProtocolError.None)
                {
                    await this._frameReceived.InvokeAsync(this, new ModbusEventArgs(frame));
                }
                else if (frame is not null)
                {
                    Console.WriteLine("Failed to parse frame: {0} (command {1})", result, frame.Header.Command);
                    if (frame.Header.Command != 0)
                    {
                        await this._frameQueueWriter.WriteAsync(
                            new ModbusFrame<ModbusCommandError>
                            {
                                Header = new()
                                {
                                    SlaveAddress = frame.Header.SlaveAddress,
                                    Command = (ModbusProtocolCommand)(0x80 | (byte)frame.Header.Command),
                                },
                                Payload = new()
                                {
                                    ErrorCode = result,
                                }
                            },
                            cancellationToken
                        );
                    }

                    if (bytesRead == 1 && frame.Header.Command == 0)
                        --cursor;

                    break;
                }
                else
                {
                    Console.WriteLine("Failed to parse frame (completely): {0}", result);
                    cursor -= bytesRead;
                    break;
                }
            }

            if (cursor > 0)
            {
                if (cursor < (int)buff.Count)
                {
                    var len = (int)buff.Count - cursor;
                    buff.Span[cursor..].CopyTo(mem.Span);
                    buff.Clear();
                    buff.Write(mem.Span[..len]);
                    cursor = 0;
                }
            }
        }
    }

    private async Task ProcessQueue(CancellationToken cancellationToken)
    {
        await Task.Yield();
        var buff = new byte[256];
        var mem = buff.AsMemory();
        var port = this._port;
        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = await this._frameQueueReader.ReadAsync(cancellationToken);
            if (this._serdes.TrySerializeFrame(frame, buff, out var written))
            {
                await port.WriteAsync(mem[..written], cancellationToken);
                //await port.FlushAsync(cancellationToken);
            }

            await Task.Delay(this._tickGap, cancellationToken);
        }
    }

    private TimeSpan ComputeGapTicks(int baudRate, int dataBits, Parity parity, StopBits stopBits)
    {
        var gapSize = 28.0 /* 3.5 bytes */;
        var words = (int)Math.Ceiling(gapSize / dataBits);
        var overhead = 8 /* start bits */
            + stopBits switch
            {
                StopBits.One => 1,
                StopBits.Two => 2,
                StopBits.One5 => 2,
                _ => 0,
            }
            + (parity == Parity.None ? 0 : 1);
        var sz = words * (overhead + dataBits);
        var seconds = (double)sz / baudRate;
        return TimeSpan.FromSeconds(seconds * 2);
    }
}
