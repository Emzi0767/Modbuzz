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
/// Specifies an error code for a fail response.
/// </summary>
public enum ModbusProtocolError : byte
{
    /// <summary>
    /// Indicates no error occured.
    /// </summary>
    None = 0,

    /// <summary>
    /// The function code received in the request is not an authorized action for the slave. The slave may be in the wrong state to process a specific request.
    /// </summary>
    IllegalFunction = 0x01,

    /// <summary>
    /// The data address received by the slave is not an authorized address for the slave.
    /// </summary>
    IllegalDataAddress = 0x02,

    /// <summary>
    /// The value in the request data field is not an authorized value for the slave.
    /// </summary>
    IllegalDataValue = 0x03,

    /// <summary>
    /// The slave fails to perform a requested action because of an unrecoverable error.
    /// </summary>
    SlaveDeviceFailure = 0x04,

    /// <summary>
    /// The slave accepts the request but needs a long time to process it.
    /// </summary>
    Acknowledge = 0x05,

    /// <summary>
    /// The slave is busy processing another command. The master must send the request once the slave is available.
    /// </summary>
    SlaveDeviceBusy = 0x06,

    /// <summary>
    /// The slave cannot perform the programming request sent by the master.
    /// </summary>
    NegativeAcknowledgement = 0x07,

    /// <summary>
    /// The slave detects a parity error in the memory when attempting to read extended memory.
    /// </summary>
    MemoryParityError = 0x08,

    /// <summary>
    /// The gateway is overloaded or not correctly configured.
    /// </summary>
    GatewayPathUnavailable = 0x0A,

    /// <summary>
    /// The slave is not present on the network.
    /// </summary>
    GatewayTargetFailedToRespond = 0x0B,
}
