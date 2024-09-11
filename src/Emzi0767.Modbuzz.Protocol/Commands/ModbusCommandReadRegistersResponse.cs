using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a response to <see cref="ModbusCommandReadRegistersRequest"/>.
/// </summary>
public readonly record struct ModbusCommandReadRegistersResponse
{
    /// <summary>
    /// Gets the values contained in this response.
    /// </summary>
    public IEnumerable<ushort> Values { get; init; }
}
