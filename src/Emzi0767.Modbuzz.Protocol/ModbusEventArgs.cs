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

using Emzi0767.Utilities;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents event arguments for Modbus events.
/// </summary>
public sealed class ModbusEventArgs : AsyncEventArgs
{
    /// <summary>
    /// Gets the received frame.
    /// </summary>
    public IModbusFrame Frame { get; }

    /// <summary>
    /// Creates new Modbus event arguments.
    /// </summary>
    /// <param name="frame">Frame that was received.</param>
    public ModbusEventArgs(IModbusFrame frame)
    {
        this.Frame = frame;
    }
}
