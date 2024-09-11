namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to read one or more input registers.
/// </summary>
public readonly record struct ModbusCommandReadInputsRequest
{
    /// <summary>
    /// Gets the address of the starting register to read from.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the total number of registers to read.
    /// </summary>
    public ushort RegisterCount { get; init; }
}
