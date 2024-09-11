namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a response to <see cref="ModbusCommandWriteCoilRequest"/>.
/// </summary>
public readonly record struct ModbusCommandWriteCoilResponse
{
    /// <summary>
    /// Gets the address of the coil to write to.
    /// </summary>
    public ushort Address { get; init; }

    /// <summary>
    /// Gets the value to write to the register.
    /// </summary>
    public bool Value { get; init; }
}
