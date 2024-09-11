// This file is part of Modbuzz project
//
// Copyright © 2024 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
