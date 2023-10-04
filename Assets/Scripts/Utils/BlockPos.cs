using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct BlockPos
{
    public int x;
    public int y;
    public int z;

    public BlockPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public BlockPos(Vector3 pos)
    {
        x = Mathf.FloorToInt(pos.x);
        y = Mathf.FloorToInt(pos.y);
        z = Mathf.FloorToInt(pos.z);

        x = Mathf.Approximately(pos.x, x + 1) ? x + 1 : x;
        y = Mathf.Approximately(pos.y, y + 1) ? y + 1 : y;
        z = Mathf.Approximately(pos.z, z + 1) ? z + 1 : z;
    }

    public Vector3Int AsVector3Int()
    {
        return new Vector3Int(this.x, this.y, this.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BlockPos lhs, BlockPos rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BlockPos lhs, BlockPos rhs)
    {
        return !(lhs == rhs);
    }

    public override string ToString()
    {
        return x.ToString() + " / " + y.ToString() + " / " + z.ToString();
    }
}
