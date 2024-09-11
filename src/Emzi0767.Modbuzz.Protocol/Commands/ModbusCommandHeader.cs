namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a common header for all Modbus commands.
/// </summary>
public readonly record struct ModbusCommandHeader
{
    /// <summary>
    /// Gets the address of the modbus slave the command is directed at or sent from.
    /// </summary>
    public byte SlaveAddress { get; init; }

    /// <summary>
    /// Gets the command to be executed, or one that was executed. This can also indicate errors.
    /// </summary>
    public ModbusProtocolCommand Command { get; init; }
}
