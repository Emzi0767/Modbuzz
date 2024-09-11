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

using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to write to multiple coil registers.
/// </summary>
public readonly record struct ModbusCommandWriteCoilsRequest
{
    /// <summary>
    /// Gets the address of the starting register to write to.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the values to put in the registers.
    /// </summary>
    public IEnumerable<bool> Values { get; init; }
}
