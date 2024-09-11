using System;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a base for all Modbus frames.
/// </summary>
public interface IModbusFrame
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; }

    /// <summary>
    /// Gets the type of payload in this frame.
    /// </summary>
    public Type PayloadType { get; }
}

/// <summary>
/// Represents an empty modbus frame.
/// </summary>
public readonly record struct ModbusFrame : IModbusFrame
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; init; }

    Type IModbusFrame.PayloadType
        => null;
}

/// <summary>
/// Represents a Modbus frame.
/// </summary>
/// <typeparam name="T">Type of payload.</typeparam>
public readonly record struct ModbusFrame<T> : IModbusFrame
    where T : struct
{
    /// <summary>
    /// Gets the header of this frame.
    /// </summary>
    public ModbusCommandHeader Header { get; init; }

    /// <summary>
    /// Gets the payload of this frame.
    /// </summary>
    public T Payload { get; init; }

    Type IModbusFrame.PayloadType
        => typeof(T);
}
