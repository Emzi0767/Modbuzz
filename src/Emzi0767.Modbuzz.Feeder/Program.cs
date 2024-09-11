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
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Emzi0767.Modbuzz.Common;
using Emzi0767.Modbuzz.Protocol;
using RJCP.IO.Ports;

namespace Emzi0767.Modbuzz.Feeder;

public static class Program
{
    private static VirtualModbus1 _vmodbus = new();

    private static IModbus _modbus;
    private static ModbusProtocolCommand _lastCmd;
    private static ushort _lastAddress, _lastCount;

    public static async Task Main(string[] args)
    {
        if (args.Length != 5)
        {
            PrintHelp();
            return;
        }

        var port = args[0];
        if (!int.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var baudRate))
        {
            PrintHelp();
            return;
        }

        if (!int.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var dataBits))
        {
            PrintHelp();
            return;
        }

        if (!Enum.TryParse<Parity>(args[3], true, out var parity))
        {
            PrintHelp();
            return;
        }

        if (!Enum.TryParse<StopBits>(args[4], true, out var stopBits))
        {
            PrintHelp();
            return;
        }

        Console.WriteLine("Connecting to {0} using {1} {2}/{3}/{4}", port, baudRate, dataBits, parity, stopBits);
        _modbus = new ModbusRtu(port, baudRate, dataBits, parity, stopBits);
        _modbus.FrameReceived += Modbus_FrameReceived;
        await _modbus.OpenAsync();

        await Task.Delay(-1);
    }

    private static async Task Modbus_FrameReceived(IModbus sender, ModbusEventArgs e)
    {
        Console.WriteLine("FRAME IN: {0:X2} {1}", e.Frame.Header.SlaveAddress, e.Frame.Header.Command);
        var frame = e.Frame;
        var header = frame.Header;
        if (header.SlaveAddress != _vmodbus.SlaveId)
            return;

        ushort address = 0, count = 0;
        var datau16 = default(IEnumerable<ushort>);
        var databool = default(IEnumerable<bool>);
        var command = header.Command;
        switch (frame)
        {
            case ModbusFrame<ModbusCommandReadCoilsRequest> read:
                address = read.Payload.StartingAddress;
                count = read.Payload.RegisterCount;
                break;

            case ModbusFrame<ModbusCommandReadDiscreteInputsRequest> read:
                address = read.Payload.StartingAddress;
                count = read.Payload.RegisterCount;
                break;

            case ModbusFrame<ModbusCommandReadRegistersRequest> read:
                address = read.Payload.StartingAddress;
                count = read.Payload.RegisterCount;
                break;

            case ModbusFrame<ModbusCommandReadInputsRequest> read:
                address = read.Payload.StartingAddress;
                count = read.Payload.RegisterCount;
                break;

            case ModbusFrame<ModbusCommandWriteCoilRequest> write:
                address = write.Payload.Address;
                databool = new[] { write.Payload.Value };
                break;

            case ModbusFrame<ModbusCommandWriteRegisterRequest> write:
                address = write.Payload.Address;
                datau16 = new[] { write.Payload.Value };
                break;

            case ModbusFrame<ModbusCommandWriteCoilsRequest> writes:
                address = writes.Payload.StartingAddress;
                databool = writes.Payload.Values;
                break;

            case ModbusFrame<ModbusCommandWriteRegistersRequest> writes:
                address = writes.Payload.StartingAddress;
                datau16 = writes.Payload.Values;
                break;
        }

        if (command == ModbusProtocolCommand.ReadCoils
            || command == ModbusProtocolCommand.ReadDiscreteInputs
            || command == ModbusProtocolCommand.ReadHoldingRegisters
            || command == ModbusProtocolCommand.ReadInputRegisters)
        {
            if (_lastCmd == header.Command && address == _lastAddress && count == _lastCount)
                return;
        }

        switch (command)
        {
            case ModbusProtocolCommand.ReadCoils:
            case ModbusProtocolCommand.ReadDiscreteInputs:
            {
                var values = new bool[count];
                for (ushort i = 0; i < count; ++i)
                {
                    if (!_vmodbus.GetCoil((ushort)(address + i), out var val))
                    {
                        await SendError(command, ModbusProtocolError.IllegalDataAddress);
                        return;
                    }

                    values[i] = val;
                }

                await _modbus.SendAsync(command == ModbusProtocolCommand.ReadCoils
                    ? new ModbusFrame<ModbusCommandReadCoilsResponse>
                    {
                        Header = MakeHeader(command),
                        Payload = new()
                        {
                            Values = values,
                        },
                    }
                    : new ModbusFrame<ModbusCommandReadDiscreteInputsResponse>
                    {
                        Header = MakeHeader(command),
                        Payload = new()
                        {
                            Values = values,
                        },
                    }
                );

                break;
            }

            case ModbusProtocolCommand.ReadHoldingRegisters:
            case ModbusProtocolCommand.ReadInputRegisters:
            {
                var values = new ushort[count];
                for (ushort i = 0; i < count; ++i)
                {
                    if (!_vmodbus.GetRegister((ushort)(address + i), out var val))
                    {
                        await SendError(command, ModbusProtocolError.IllegalDataAddress);
                        return;
                    }

                    values[i] = val;
                }

                await _modbus.SendAsync(command == ModbusProtocolCommand.ReadHoldingRegisters
                    ? new ModbusFrame<ModbusCommandReadRegistersResponse>
                    {
                        Header = MakeHeader(command),
                        Payload = new()
                        {
                            Values = values,
                        },
                    }
                    : new ModbusFrame<ModbusCommandReadInputsResponse>
                    {
                        Header = MakeHeader(command),
                        Payload = new()
                        {
                            Values = values,
                        },
                    }
                );

                break;
            }
        }
    }

    private static ModbusCommandHeader MakeHeader(ModbusProtocolCommand command)
        => new()
        {
            SlaveAddress = _vmodbus.SlaveId,
            Command = command,
        };

    private static async Task SendError(ModbusProtocolCommand command, ModbusProtocolError error)
        => await _modbus.SendAsync(
            new ModbusFrame<ModbusCommandError>
            {
                Header = new()
                {
                    SlaveAddress = _vmodbus.SlaveId,
                    Command = (ModbusProtocolCommand)(0x80 | (byte)command),
                },
                Payload = new()
                {
                    ErrorCode = error,
                },
            }
        );

    public static void PrintHelp()
        => Console.WriteLine("Usage: feeder <port> <baud rate> <data bits> <parity:None/Odd/Even/Mark/Space> <stop bits:One/Two/One5>");
}
