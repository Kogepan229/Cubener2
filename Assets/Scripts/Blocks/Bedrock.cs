public class Bedrock : Block
{
    public static readonly string NAME = "bedrock";
    public Bedrock() : base(NAME)
    {
        SetTextureAll(NAME);
    }
}
