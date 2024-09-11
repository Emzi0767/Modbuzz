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
/// Represents a base for all Modbus frames.
/// </summary>
public interface IModbusFrame
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; }

    /// <summary>
    /// Gets the type of payload in this frame.
    /// </summary>
    public Type PayloadType { get; }
}

/// <summary>
/// Represents an empty modbus frame.
/// </summary>
public readonly record struct ModbusFrame : IModbusFrame
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; init; }

    Type IModbusFrame.PayloadType
        => null;
}

/// <summary>
/// Represents a Modbus frame.
/// </summary>
/// <typeparam name="T">Type of payload.</typeparam>
public readonly record struct ModbusFrame<T> : IModbusFrame
    where T : struct
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; init; }

    /// <summary>
    /// Gets the payload of this frame.
    /// </summary>
    public T Payload { get; init; }

    Type IModbusFrame.PayloadType
        => typeof(T);
}
