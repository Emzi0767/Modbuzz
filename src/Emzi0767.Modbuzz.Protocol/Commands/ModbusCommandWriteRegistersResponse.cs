namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to write to multiple holding registers.
/// </summary>
public readonly record struct ModbusCommandWriteRegistersResponse
{
    /// <summary>
    /// Gets the address of the starting register to write to.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the number of holding registers written.
    /// </summary>
    public ushort Count { get; init; }
}
