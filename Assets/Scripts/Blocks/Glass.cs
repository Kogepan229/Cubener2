public class Glass : Block
{
    public static readonly string NAME = "glass";
    public Glass() : base(NAME)
    {
        SetTextureAll(NAME);
    }

    public override bool IsTransparent(Faces face)
    {
        return true;
    }
}