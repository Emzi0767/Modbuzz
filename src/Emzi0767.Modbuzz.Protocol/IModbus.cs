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
