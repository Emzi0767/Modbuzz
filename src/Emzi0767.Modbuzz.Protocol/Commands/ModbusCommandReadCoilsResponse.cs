using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a response to <see cref="ModbusCommandReadCoilsRequest"/>.
/// </summary>
public readonly record struct ModbusCommandReadCoilsResponse
{
    /// <summary>
    /// Gets the values contained in this response.
    /// </summary>
    public IEnumerable<bool> Values { get; init; }
}
