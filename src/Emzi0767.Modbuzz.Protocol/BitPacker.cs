using System;
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
