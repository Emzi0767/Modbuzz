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
/// Represents a response to <see cref="ModbusCommandWriteRegisterRequest"/>.
/// </summary>
public readonly record struct ModbusCommandWriteRegisterResponse
{
    /// <summary>
    /// Gets the address of the coil to write to.
    /// </summary>
    public ushort Address { get; init; }

    /// <summary>
    /// Gets the value to write to the register.
    /// </summary>
    public ushort Value { get; init; }
}
