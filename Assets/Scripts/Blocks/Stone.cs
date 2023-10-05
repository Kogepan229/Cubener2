public class Stone : BlockNew
{
    public static readonly string NAME = "stone";
    public Stone() : base(NAME)
    {
        SetTextureAll(NAME);
    }
}