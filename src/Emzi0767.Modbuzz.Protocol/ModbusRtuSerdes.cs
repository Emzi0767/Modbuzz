using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Handles serializing and deserializing Modbus RTU frames.
/// </summary>
public sealed class ModbusRtuSerdes : IModbusSerdes
{
    private readonly static int _szHeader = Unsafe.SizeOf<ModbusCommandHeader>();
    private readonly static int _szCrc = sizeof(ushort);

    /// <inheritdoc />
    public ModbusProtocolError TryParseFrame(ReadOnlySpan<byte> raw, out IModbusFrame frame, out int bytesRead)
    {
        frame = null;
        bytesRead = 0;
        if (raw.Length < _szHeader + _szCrc)
            return ModbusProtocolError.SlaveDeviceFailure;

        var header = MemoryMarshal.Read<ModbusCommandHeader>(raw);
        bytesRead += _szHeader;
        var bytePos = bytesRead;

        var payload = raw[_szHeader..];
        var result = header.Command switch
        {
            ModbusProtocolCommand.ReadCoils or
            ModbusProtocolCommand.ReadDiscreteInputs or
            ModbusProtocolCommand.ReadHoldingRegisters or
            ModbusProtocolCommand.ReadInputRegisters => this.TryParseRead(payload, header, ref frame, ref bytesRead),

            ModbusProtocolCommand.WriteSingleCoil or
            ModbusProtocolCommand.WriteSingleHoldingRegister => this.TryParseWriteSingle(payload, header, ref frame, ref bytesRead),

            ModbusProtocolCommand.WriteMultipleCoils or
            ModbusProtocolCommand.WriteMultipleHoldingRegisters => this.TryParseWriteMultiple(payload, header, ref frame, ref bytesRead),

            _ => ModbusProtocolError.IllegalFunction
        };

        if (result != ModbusProtocolError.None && bytesRead == bytePos)
        {
            bytesRead = result == ModbusProtocolError.IllegalFunction ? 1 : 0;
            frame = new ModbusFrame { Header = header };
            return result;
        }

        var crcBytes = raw[bytesRead..];
        if (crcBytes.Length < sizeof(ushort))
        {
            bytesRead = 0;
            frame = new ModbusFrame { Header = header };
            return ModbusProtocolError.SlaveDeviceFailure;
        }

        var crcFrame = BinaryPrimitives.ReadUInt16BigEndian(crcBytes);
        var crcActual = ModbusCrc.Compute(raw[..bytesRead]);
        bytesRead += sizeof(ushort);
        if (crcFrame != crcActual)
        {
            frame = new ModbusFrame { Header = header };
            return ModbusProtocolError.MemoryParityError;
        }

        return ModbusProtocolError.None;
    }

    private ModbusProtocolError TryParseRead(ReadOnlySpan<byte> raw, in ModbusCommandHeader header, ref IModbusFrame frame, ref int bytesRead)
    {
        if (raw.Length < sizeof(ushort) * 2 /* address:u16, count:u16 */)
            return ModbusProtocolError.SlaveDeviceFailure;

        var address = BinaryPrimitives.ReadUInt16BigEndian(raw);
        var count = BinaryPrimitives.ReadUInt16BigEndian(raw[sizeof(ushort)..]);
        bytesRead += sizeof(ushort) * 2;
        switch (header.Command)
        {
            case ModbusProtocolCommand.ReadCoils:
                frame = new ModbusFrame<ModbusCommandReadCoilsRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        RegisterCount = count,
                    },
                };
                break;

            case ModbusProtocolCommand.ReadDiscreteInputs:
                frame = new ModbusFrame<ModbusCommandReadDiscreteInputsRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        RegisterCount = count,
                    },
                };
                break;

            case ModbusProtocolCommand.ReadHoldingRegisters:
                frame = new ModbusFrame<ModbusCommandReadRegistersRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        RegisterCount = count,
                    },
                };
                break;

            case ModbusProtocolCommand.ReadInputRegisters:
                frame = new ModbusFrame<ModbusCommandReadInputsRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        RegisterCount = count,
                    },
                };
                break;
        }
        

        return ModbusProtocolError.None;
    }

    private ModbusProtocolError TryParseWriteSingle(ReadOnlySpan<byte> raw, in ModbusCommandHeader header, ref IModbusFrame frame, ref int bytesRead)
    {
        if (raw.Length < sizeof(ushort) * 2 /* address:u16, count:u16 */)
            return ModbusProtocolError.SlaveDeviceFailure;

        var address = BinaryPrimitives.ReadUInt16BigEndian(raw);
        var data = BinaryPrimitives.ReadUInt16BigEndian(raw[sizeof(ushort)..]);
        bytesRead += sizeof(ushort) * 2;

        switch (header.Command)
        {
            case ModbusProtocolCommand.WriteSingleCoil:
                if (data != 0xFF00u && data != 0x0000u)
                    return ModbusProtocolError.IllegalDataValue;

                frame = new ModbusFrame<ModbusCommandWriteCoilRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        Address = address,
                        Value = data == 0xFF00u,
                    },
                };
                break;

            case ModbusProtocolCommand.WriteSingleHoldingRegister:
                frame = new ModbusFrame<ModbusCommandWriteRegisterRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        Address = address,
                        Value = data,
                    },
                };
                break;
        }

        return ModbusProtocolError.None;
    }

    private ModbusProtocolError TryParseWriteMultiple(ReadOnlySpan<byte> raw, in ModbusCommandHeader header, ref IModbusFrame frame, ref int bytesRead)
    {
        if (raw.Length < (sizeof(ushort) * 2) + 1 /* address:u16, count:u16, bytes:u8 */)
            return ModbusProtocolError.SlaveDeviceFailure;

        var address = BinaryPrimitives.ReadUInt16BigEndian(raw);
        var count = BinaryPrimitives.ReadUInt16BigEndian(raw[sizeof(ushort)..]);
        var byteCount = raw[sizeof(ushort) * 2];
        var preambleSize = (sizeof(ushort) * 2) + 1;
        bytesRead += preambleSize;

        if (preambleSize + byteCount >= raw.Length)
        {
            bytesRead = 0;
            return ModbusProtocolError.IllegalDataValue;
        }

        var data = raw.Slice(preambleSize, byteCount);
        switch (header.Command)
        {
            case ModbusProtocolCommand.WriteMultipleCoils:
                frame = new ModbusFrame<ModbusCommandWriteCoilsRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        Values = data.ToArray().Unpack(count)
                    },
                };
                break;

            case ModbusProtocolCommand.WriteMultipleHoldingRegisters:
                var values = new ushort[byteCount / 2];
                for (var i = 0; i < values.Length; ++i)
                    values[i] = BinaryPrimitives.ReadUInt16BigEndian(data[(i * 2)..]);

                frame = new ModbusFrame<ModbusCommandWriteRegistersRequest>
                {
                    Header = header,
                    Payload = new()
                    {
                        StartingAddress = address,
                        Values = values,
                    },
                };
                break;
        }

        bytesRead += byteCount;
        return ModbusProtocolError.None;
    }

    /// <inheritdoc />
    public bool TrySerializeFrame(in IModbusFrame frame, Span<byte> raw, out int bytesWritten)
    {
        bytesWritten = 0;
        if (raw.Length < _szHeader + _szCrc)
            return false;

        var header = frame.Header;
        MemoryMarshal.Write(raw, header);
        bytesWritten += _szHeader;
        if (frame is ModbusFrame<ModbusCommandError> errorFrame)
        {
            MemoryMarshal.Write(raw[_szHeader..], errorFrame);
            bytesWritten += Unsafe.SizeOf<ModbusCommandError>();
        }
        else
        {
            switch (frame)
            {
                case ModbusFrame<ModbusCommandReadCoilsResponse> coils:
                    if (!this.TrySerializeCoils(coils.Payload.Values, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandReadDiscreteInputsResponse> coils:
                    if (!this.TrySerializeCoils(coils.Payload.Values, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandReadRegistersResponse> registers:
                    if (!this.TrySerializeRegisters(registers.Payload.Values, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandReadInputsResponse> registers:
                    if (!this.TrySerializeRegisters(registers.Payload.Values, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandWriteCoilResponse> write:
                    if (!this.TrySerializeWrite(write.Payload.Address, (ushort)(write.Payload.Value ? 0xFF00u : 0x0000u), raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandWriteRegisterResponse> write:
                    if (!this.TrySerializeWrite(write.Payload.Address, write.Payload.Value, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandWriteCoilsResponse> writes:
                    if (!this.TrySerializeWrite(writes.Payload.StartingAddress, writes.Payload.Count, raw, ref bytesWritten))
                        return false;

                    break;

                case ModbusFrame<ModbusCommandWriteRegistersResponse> writes:
                    if (!this.TrySerializeWrite(writes.Payload.StartingAddress, writes.Payload.Count, raw, ref bytesWritten))
                        return false;

                    break;

                default:
                    return false;
            }
        }

        var crc = ModbusCrc.Compute(raw[..bytesWritten]);
        BinaryPrimitives.WriteUInt16BigEndian(raw[bytesWritten..], crc);
        bytesWritten += 2;
        return true;
    }

    private bool TrySerializeCoils(IEnumerable<bool> values, Span<byte> raw, ref int bytesWritten)
    {
        var @base = bytesWritten++;
        foreach (var value in values.Pack())
        {
            if (raw.Length <= bytesWritten)
                return false;

            raw[bytesWritten++] = value;
        }

        raw[@base] = (byte)(bytesWritten - @base - 1);
        return true;
    }

    private bool TrySerializeRegisters(IEnumerable<ushort> values, Span<byte> raw, ref int bytesWritten)
    {
        var @base = bytesWritten++;
        foreach (var value in values)
        {
            if (raw.Length <= bytesWritten)
                return false;

            BinaryPrimitives.WriteUInt16BigEndian(raw[bytesWritten..], value);
            bytesWritten += sizeof(ushort);
        }

        raw[@base] = (byte)(bytesWritten - @base - 1);
        return true;
    }

    private bool TrySerializeWrite(ushort address, ushort value, Span<byte> raw, ref int bytesWritten)
    {
        if (raw.Length < sizeof(ushort) * 2)
            return false;

        BinaryPrimitives.WriteUInt16BigEndian(raw, address);
        BinaryPrimitives.WriteUInt16BigEndian(raw[sizeof(ushort)..], value);
        bytesWritten += sizeof(ushort) * 2;
        return true;
    }
}
