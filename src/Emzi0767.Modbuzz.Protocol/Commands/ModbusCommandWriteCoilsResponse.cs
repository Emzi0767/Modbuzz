namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to write to multiple coil registers.
/// </summary>
public readonly record struct ModbusCommandWriteCoilsResponse
{
    /// <summary>
    /// Gets the address of the starting register to write to.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the number of coils written.
    /// </summary>
    public ushort Count { get; init; }
}
