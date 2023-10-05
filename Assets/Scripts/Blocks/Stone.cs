public class Stone : Block
{
    public static readonly string NAME = "stone";
    public Stone() : base(NAME)
    {
        SetTextureAll(NAME);
    }
}