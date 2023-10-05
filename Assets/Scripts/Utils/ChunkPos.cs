using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct ChunkPos
{
    public int x;
    public int y;
    public int z;

    public ChunkPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public ChunkPos(BlockPos pos)
    {
        x = Mathf.FloorToInt(pos.x / (float)Chunk.Width);
        y = pos.y / Chunk.Height;
        z = Mathf.FloorToInt(pos.z / (float)Chunk.Width);
    }

    /// <summary>
    /// Return x and z as long type
    /// </summary>
    public static long AsLong(int x, int z)
    {
        return (long)x & 4294967295L | ((long)z & 4294967295L) << 32;
    }

    /// <summary>
    /// Return x and z as long type
    /// </summary>
    public readonly long AsLong()
    {
        return AsLong(x, z);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChunkPos operator +(ChunkPos a, ChunkPos b)
    {
        return new ChunkPos(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChunkPos operator -(ChunkPos a, ChunkPos b)
    {
        return new ChunkPos(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ChunkPos lhs, ChunkPos rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ChunkPos lhs, ChunkPos rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
    }

    public override readonly bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }
        return this == (ChunkPos)obj;
    }

    public override readonly int GetHashCode()
    {
        return System.HashCode.Combine(x, y, z);
    }

    public override readonly string ToString()
    {
        return x.ToString() + " / " + y.ToString() + " / " + z.ToString();
    }

}
