using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to write to multiple coil registers.
/// </summary>
public readonly record struct ModbusCommandWriteCoilsRequest
{
    /// <summary>
    /// Gets the address of the starting register to write to.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the values to put in the registers.
    /// </summary>
    public IEnumerable<bool> Values { get; init; }
}
