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

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a common header for all Modbus commands.
/// </summary>
public readonly record struct ModbusCommandHeader
{
    /// <summary>
    /// Gets the address of the modbus slave the command is directed at or sent from.
    /// </summary>
    public byte SlaveAddress { get; init; }

    /// <summary>
    /// Gets the command to be executed, or one that was executed. This can also indicate errors.
    /// </summary>
    public ModbusProtocolCommand Command { get; init; }
}
