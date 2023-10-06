using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public abstract class Block
{
    public enum Faces
    {
        Back = 0,
        Front,
        Top,
        Bottom,
        Left,
        Right,
    }

    public string Name { get; }
    protected readonly string[] Textures = new string[6];

    protected Block(string name)
    {
        Name = name;
    }

    protected void SetTextureAll(string textureId)
    {
        Array.Fill(this.Textures, textureId);
    }

    protected void SetTextureAll(string textureIdBack, string textureIdFront, string textureIdTop, string textureIdBottom, string textureIdLeft, string textureIdRight)
    {
        this.Textures[0] = textureIdBack;
        this.Textures[1] = textureIdFront;
        this.Textures[2] = textureIdTop;
        this.Textures[3] = textureIdBottom;
        this.Textures[4] = textureIdLeft;
        this.Textures[5] = textureIdRight;
    }

    protected void SetTexture(string textureId, Faces face)
    {
        this.Textures[(int)face] = textureId;
    }

    public string GetTexture(Faces face)
    {
        return this.Textures[(int)face];
    }

    public virtual bool IsTransparent(Faces face)
    {
        return false;
    }
}
