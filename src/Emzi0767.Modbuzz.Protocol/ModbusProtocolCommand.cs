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
/// Represents a Modbus command code.
/// </summary>
public enum ModbusProtocolCommand : byte
{
    /// <summary>
    /// Specifies that one or more coils should be read.
    /// </summary>
    ReadCoils = 0x01,

    /// <summary>
    /// Specifies that one or more discrete inputs should be read.
    /// </summary>
    ReadDiscreteInputs = 0x02,

    /// <summary>
    /// Specifies that one or more holding registers should be read.
    /// </summary>
    ReadHoldingRegisters = 0x03,

    /// <summary>
    /// Specifies that one or more input registers should be read.
    /// </summary>
    ReadInputRegisters = 0x04,

    /// <summary>
    /// Specifies that a single coil should be written to.
    /// </summary>
    WriteSingleCoil = 0x05,

    /// <summary>
    /// Specifies that a single holding register should be written to.
    /// </summary>
    WriteSingleHoldingRegister = 0x06,

    /// <summary>
    /// Specifies that multiple coils should be written to.
    /// </summary>
    WriteMultipleCoils = 0x0F,

    /// <summary>
    /// Specifies that multiple holding registers should be written to.
    /// </summary>
    WriteMultipleHoldingRegisters = 0x10,
}
