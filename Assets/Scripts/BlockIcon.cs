using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlockIcon : Graphic
{
    public const int DEFAULT_BLOCK_ICON_SIZE = 40;
    public const int DEFAULT_BLOCK_ICON_PADDING = 5;
    public int BlockID;
    public int IconPaddingSize;

    public static GameObject CreateBlockIcon(int blockID, int iconSize = DEFAULT_BLOCK_ICON_SIZE, int iconPadding = DEFAULT_BLOCK_ICON_PADDING)
    {
        GameObject blockIconObj = new GameObject();
        var rect = blockIconObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2 (iconSize, iconSize);
        var blockIcon = blockIconObj.AddComponent<BlockIcon>();
        blockIcon.BlockID = blockID;
        blockIcon.IconPaddingSize = iconPadding;
        blockIconObj.AddComponent<CanvasRenderer>();
        Debug.Log("create");
        return blockIconObj;
    }

    public override Texture mainTexture
    {
        get
        {
            var texture = TextureManager.BlockAtlas;
            if (texture != null)
            {
                return texture;
            }
            return base.mainTexture;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Debug.Log("populate" + BlockID.ToString());
        vh.Clear();
        Vector3 position = transform.GetComponent<RectTransform>().anchoredPosition;
        var rectTransform = transform.GetComponent<RectTransform>();
        float iconSize = (rectTransform.sizeDelta.x < rectTransform.sizeDelta.y ? rectTransform.sizeDelta.x : rectTransform.sizeDelta.y) - IconPaddingSize * 2;
        float sizeA = iconSize * 0.2f;
        float sizeB = iconSize / 2 - sizeA;
        float sizeC = (iconSize / 2) * 0.95f;

        Vector3[] vertices = new Vector3[]{
            // Top
            new Vector3(position.x,         position.y + iconSize / 2, position.z),             // Top:1,4
            new Vector3(position.x,         position.y + iconSize / 2 - sizeA * 2, position.z), // Top:2,5
            new Vector3(position.x - sizeC, position.y + sizeB, position.z),                    // Top:3
            new Vector3(position.x + sizeC, position.y + sizeB, position.z),                    // Top:6

            // Front
            new Vector3(position.x - sizeC, position.y + sizeB, position.z),                    // Front:1,4
            new Vector3(position.x,         position.y - iconSize / 2, position.z),             // Front:2,5
            new Vector3(position.x - sizeC, position.y - sizeB, position.z),                    // Front:3
            new Vector3(position.x,         position.y + iconSize / 2 - sizeA * 2, position.z), // Front:6

            // Right
            new Vector3(position.x,         position.y + iconSize / 2 - sizeA * 2, position.z), // Right:1,4
            new Vector3(position.x + sizeC, position.y - sizeB, position.z),                    // Right:2,5
            new Vector3(position.x,         position.y - iconSize / 2, position.z),             // Right:3
            new Vector3(position.x + sizeC, position.y + sizeB, position.x),                    // Right:6
        };

        var block = BlockManager.GetBlock(BlockID);
        var textureTop = block.GetTexture(Block.Faces.Top);
        var textureFront = block.GetTexture(Block.Faces.Front);
        var textureRight = block.GetTexture(Block.Faces.Right);
        var uvTop = TextureManager.GetTextureUV(textureTop);
        var uvFront = TextureManager.GetTextureUV(textureFront);
        var uvRight = TextureManager.GetTextureUV(textureRight);
        const float prefix = 0.002f;
        Vector2[] uvs = new Vector2[]
        {
            // Top
            new Vector2(uvTop.x + prefix, uvTop.y + uvTop.height - prefix),
            new Vector2(uvTop.x + uvTop.width - prefix, uvTop.y + prefix),
            new Vector2(uvTop.x + prefix, uvTop.y + prefix),
            new Vector2(uvTop.x + uvTop.width - prefix, uvTop.y + uvTop.height - prefix),

            // Front
            new Vector2(uvFront.x + prefix, uvFront.y + uvFront.height - prefix),
            new Vector2(uvFront.x + uvFront.width - prefix, uvFront.y + prefix),
            new Vector2(uvFront.x + prefix, uvFront.y + prefix),
            new Vector2(uvFront.x + uvFront.width - prefix, uvFront.y + uvFront.height - prefix),

            // Right
            new Vector2(uvRight.x + prefix, uvRight.y + uvRight.height - prefix),
            new Vector2(uvRight.x + uvRight.width - prefix, uvRight.y + prefix),
            new Vector2(uvRight.x + prefix, uvRight.y + prefix),
            new Vector2(uvRight.x + uvRight.width - prefix, uvRight.y + uvRight.height - prefix),
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            Color color;
            if (i < 4)
            {
                color = new Color(1, 1, 1, 1);
            }
            else if (i < 8)
            {
                color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
            else
            {
                color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            var v = new UIVertex
            {
                position = vertices[i],
                color = color,
                uv0 = uvs[i],
            };
            vh.AddVert(v);
        }

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 1, 3);
        vh.AddTriangle(4, 5, 6);
        vh.AddTriangle(4, 5, 7);
        vh.AddTriangle(8, 9, 10);
        vh.AddTriangle(8, 9, 11);
    }
}
