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
using System.Threading;
using System.Threading.Tasks;
using Emzi0767.Utilities;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents an abstract base for all Modbus implementations.
/// </summary>
public interface IModbus : IAsyncDisposable
{
    /// <summary>
    /// Starts the Modbus interface and begins processing events.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Task representing the operation.</returns>
    ValueTask OpenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the Modbus interface and stops processing events.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Task representing the operation.</returns>
    ValueTask CloseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a given frame to the Modbus network.
    /// </summary>
    /// <param name="frame">Frame to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Task representing the operation.</returns>
    ValueTask SendAsync(IModbusFrame frame, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fired whenever a Modbus frame is received from the network.
    /// </summary>
    event AsyncEventHandler<IModbus, ModbusEventArgs> FrameReceived;

    /// <summary>
    /// Fired whenever an exception is thrown by any Modbus event handler.
    /// </summary>
    public event EventHandler<ModbusErrorEventArgs> EventExceptionThrown;
}
