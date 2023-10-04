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

    public static long AsLong(int x, int z)
    {
        return (long)x & 4294967295L | ((long)z & 4294967295L) << 32;
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
        return !(lhs == rhs);
    }

    public override string ToString()
    {
        return x.ToString() + " / " + y.ToString() + " / " + z.ToString();
    }

}
