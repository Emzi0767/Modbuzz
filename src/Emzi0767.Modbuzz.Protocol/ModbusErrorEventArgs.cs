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
using Emzi0767.Utilities;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents Modbus exception event arguments.
/// </summary>
public sealed class ModbusErrorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the exception that occured.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets the event sender instance.
    /// </summary>
    public ModbusRtu Sender { get; }

    /// <summary>
    /// Gets the event arguments supplied to the event handler.
    /// </summary>
    public AsyncEventArgs EventArgs { get; }

    /// <summary>
    /// Creates new Modbus exception event arguments.
    /// </summary>
    /// <param name="exception">Exception that occured.</param>
    /// <param name="sender">Sender instance.</param>
    /// <param name="eventArgs">Arguments supplied to the handler.</param>
    public ModbusErrorEventArgs(Exception exception, ModbusRtu sender, AsyncEventArgs eventArgs)
    {
        this.Exception = exception;
        this.Sender = sender;
        this.EventArgs = eventArgs;
    }
}
