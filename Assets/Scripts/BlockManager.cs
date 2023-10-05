using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BlockManager
{
    private static readonly List<Block> RegisteredBlocks = new();
    private static readonly Dictionary<string, int> RegisteredBlockNameMap = new();

    public static void ResistAllBlock()
    {
        ResisterBlock("air", null);
        ResisterBlock(Bedrock.NAME, new Bedrock());
        ResisterBlock(Stone.NAME, new Stone());
        ResisterBlock(Dirt.NAME, new Dirt());
        ResisterBlock(Grass.NAME, new Grass());
        ResisterBlock(Glass.NAME, new Glass());
        ResisterBlock(Furnace.NAME, new Furnace());
        ResisterBlock(Beacon.NAME, new Beacon());
    }

    public static Block GetBlock(int id)
    {
        return RegisteredBlocks[id];
    }

    public static Block GetBlock(string name)
    {
        return RegisteredBlocks[GetBlockId(name)];
    }

    public static string GetBlockName(int id)
    {
        return RegisteredBlockNameMap.FirstOrDefault(x => x.Value.Equals(id)).Key;
    }

    public static int GetBlockId(string name)
    {
        return RegisteredBlockNameMap[name];
    }

    private static void ResisterBlock(string name, Block block)
    {
        int id = RegisteredBlocks.Count;
        RegisteredBlocks.Add(block);
        RegisteredBlockNameMap.Add(name, id);
    }
}
