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
