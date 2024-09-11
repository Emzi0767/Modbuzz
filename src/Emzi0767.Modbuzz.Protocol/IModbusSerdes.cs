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
