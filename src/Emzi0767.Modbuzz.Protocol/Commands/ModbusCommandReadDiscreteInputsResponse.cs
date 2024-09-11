using System.Collections.Generic;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a response to <see cref="ModbusCommandReadDiscreteInputsRequest"/>.
/// </summary>
public readonly record struct ModbusCommandReadDiscreteInputsResponse
{
    /// <summary>
    /// Gets the values contained in this response.
    /// </summary>
    public IEnumerable<bool> Values { get; init; }
}
