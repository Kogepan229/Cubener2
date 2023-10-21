using System.Collections.Generic;
using UnityEngine;

public static class TextureManager
{
    private readonly static int ATLAS_MAX_SIZE = 128;

    public static Texture2D BlockAtlas { get; } = new Texture2D(ATLAS_MAX_SIZE, ATLAS_MAX_SIZE);

    private static readonly Dictionary<string, Rect> TextureUVMap = new();

    public static void LoadTextures()
    {
        LoadBlockTextures();
    }

    private static void LoadBlockTextures()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Images/Blocks");
        for (int i = 0; i < textures.Length; i++)
        {
            Debug.Log(textures[i].name);
        }

        //var atlas = new Texture2D(ATLAS_MAX_SIZE, ATLAS_MAX_SIZE);

        BlockAtlas.filterMode = FilterMode.Point;
        Rect[] rects = BlockAtlas.PackTextures(textures, 0, ATLAS_MAX_SIZE);
        for (int i = 0; i < rects.Length; i++)
        {
            TextureUVMap.Add(textures[i].name, rects[i]);
            Debug.Log("rect: " + rects[i].ToString());
        }

    }

    public static Rect GetTextureUV(string textureId)
    {
        return TextureUVMap[textureId];
    }
}
