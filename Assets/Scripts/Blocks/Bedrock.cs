using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bedrock : BlockNew
{
    public static readonly string NAME = "bedrock";
    public Bedrock() : base(NAME)
    {
        SetTextureAll(NAME);
    }
}
