using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public Block[] Blocks;
}


[System.Serializable]
public class Block
{
    public string blockName;
    public bool isSolid;
    public bool isTransparent;

    [Header("Texture ID")]
    public int BackTexture;
    public int FrontTexture;
    public int TopTexture;
    public int BottomTexture;
    public int LeftTexture;
    public int RightTexture;

    public enum EnumFace
    {
        Back = 0,
        Front,
        Top,
        Bottom,
        Left,
        Right,
    }

    // Back, Front, Top, Bottom, Left, Right
    public int GetTextureID(EnumFace face)
    {

        switch (face)
        {

            case EnumFace.Back:
                return BackTexture;
            case EnumFace.Front:
                return FrontTexture;
            case EnumFace.Top:
                return TopTexture;
            case EnumFace.Bottom:
                return BottomTexture;
            case EnumFace.Left:
                return LeftTexture;
            case EnumFace.Right:
                return RightTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;


        }

    }
}
