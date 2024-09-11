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
