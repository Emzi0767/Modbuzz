using System;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Handles serializing and deserializing of Modbus frames. Abstract class for medium-specific implementations.
/// </summary>
public interface IModbusSerdes
{
    /// <summary>
    /// Attempts to parse a Modbus frame. Returns an error code.
    /// </summary>
    /// <param name="raw">Raw frame to parse.</param>
    /// <param name="frame">Parsed frame, if applicable.</param>
    /// <param name="bytesRead">Number of bytes read.</param>
    /// <returns>Error code.</returns>
    public ModbusProtocolError TryParseFrame(ReadOnlySpan<byte> raw, out IModbusFrame frame, out int bytesRead);

    /// <summary>
    /// Attempts to serialize a Modbus frame.
    /// </summary>
    /// <param name="frame">Frame to serialize.</param>
    /// <param name="raw">Buffer to place the serialized frame into.</param>
    /// <param name="bytesWritten">Number of bytes written to the buffer.</param>
    /// <returns>Whether the operation was a success.</returns>
    public bool TrySerializeFrame(in IModbusFrame frame, Span<byte> raw, out int bytesWritten);
}
