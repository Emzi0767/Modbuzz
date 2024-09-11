namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents an error response to a request.
/// </summary>
public readonly record struct ModbusCommandError
{
    /// <summary>
    /// Gets the error code indicating what happened.
    /// </summary>
    public ModbusProtocolError ErrorCode { get; init; }
}
