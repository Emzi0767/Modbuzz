namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Represents a Modbus command code.
/// </summary>
public enum ModbusProtocolCommand : byte
{
    /// <summary>
    /// Specifies that one or more coils should be read.
    /// </summary>
    ReadCoils = 0x01,

    /// <summary>
    /// Specifies that one or more discrete inputs should be read.
    /// </summary>
    ReadDiscreteInputs = 0x02,

    /// <summary>
    /// Specifies that one or more holding registers should be read.
    /// </summary>
    ReadHoldingRegisters = 0x03,

    /// <summary>
    /// Specifies that one or more input registers should be read.
    /// </summary>
    ReadInputRegisters = 0x04,

    /// <summary>
    /// Specifies that a single coil should be written to.
    /// </summary>
    WriteSingleCoil = 0x05,

    /// <summary>
    /// Specifies that a single holding register should be written to.
    /// </summary>
    WriteSingleHoldingRegister = 0x06,

    /// <summary>
    /// Specifies that multiple coils should be written to.
    /// </summary>
    WriteMultipleCoils = 0x0F,

    /// <summary>
    /// Specifies that multiple holding registers should be written to.
    /// </summary>
    WriteMultipleHoldingRegisters = 0x10,
}
