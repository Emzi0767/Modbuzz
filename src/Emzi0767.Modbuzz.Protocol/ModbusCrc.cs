using System;
using System.Buffers.Binary;

namespace Emzi0767.Modbuzz.Protocol;

// based on https://ctlsys.com/support/how_to_compute_the_modbus_rtu_message_crc/

/// <summary>
/// Provides an implementation of the Modbus RTU variant of the CRC-16 algorithm.
/// </summary>
public static class ModbusCrc
{
    /// <summary>
    /// Computes the CRC-16-Modbus for a given buffer.
    /// </summary>
    /// <param name="data">Buffer to compute CRC for.</param>
    /// <returns>Computed CRC value.</returns>
    public static ushort Compute(ReadOnlySpan<byte> data)
    {
        ushort crc = 0xFFFF;
        for (var i = 0; i < data.Length; ++i)
        {
            crc ^= data[i];
            for (var j = 8; j > 0; --j)
            {
                var lsb = (crc & 0x1) != 0;
                crc >>= 1;

                if (lsb)
                    crc ^= 0xA001;
            }
        }

        return BinaryPrimitives.ReverseEndianness(crc);
    }
}
