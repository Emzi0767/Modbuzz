﻿namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a request to read discrete inputs.
/// </summary>
public readonly record struct ModbusCommandReadDiscreteInputsRequest
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
