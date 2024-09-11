using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to write to multiple holding registers.
/// </summary>
public readonly record struct ModbusCommandWriteRegistersRequest
{
    /// <summary>
    /// Gets the address of the starting register to write to.
    /// </summary>
    public ushort StartingAddress { get; init; }

    /// <summary>
    /// Gets the values to put in the registers.
    /// </summary>
    public IEnumerable<ushort> Values { get; init; }
}
