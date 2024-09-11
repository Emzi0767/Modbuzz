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

using System.Collections.Generic;
using System.Linq;

namespace Emzi0767.Modbuzz.Protocol;

/// <summary>
/// Handles packing and unpacking bits to and from byte arrays.
/// </summary>
public static class BitPacker
{
    /// <summary>
    /// Packs a given bistream into a bytestream.
    /// </summary>
    /// <param name="bits">Bits to pack.</param>
    /// <returns>Enumerator over packed bytestream.</returns>
    public static IEnumerable<byte> Pack(this IEnumerable<bool> bits)
    {
        byte packed = 0;
        var idx = 0;
        foreach (var bit in bits)
        {
            packed |= (byte)((bit ? 1 : 0) << idx++);
            if ((idx &= 0x7) == 0)
            {
                yield return packed;
                packed = 0;
            }
        }

        if (idx != 0)
            yield return packed;
    }

    /// <summary>
    /// Unpacks a given bytestream into a bitstream.
    /// </summary>
    /// <param name="bytes">Bytes to unpack.</param>
    /// <returns>Enumerator over unpacked bitstream.</returns>
    public static IEnumerable<bool> Unpack(this IEnumerable<byte> bytes)
    {
        foreach (var packed in bytes)
        {
            for (var i = 0; i < 8; i++)
                yield return ((packed >> i) & 0x1) == 1;
        }
    }

    /// <summary>
    /// Unpacks a given bytestream into a bitstream. Returns at most <paramref name="maxCount"> items.
    /// </summary>
    /// <param name="bytes">Bytes to unpack.</param>
    /// <param name="maxCount">Maximum number of bits to unpack.</param>
    /// <returns>Enumerator over unpacked bitstream.</returns>
    public static IEnumerable<bool> Unpack(this IEnumerable<byte> bytes, int maxCount)
        => bytes.Unpack().Take(maxCount);
}
