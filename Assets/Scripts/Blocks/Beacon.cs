public class Beacon : Block
{
    public static readonly string NAME = "beacon";
    public Beacon(): base(NAME)
    {
        SetTextureAll(NAME);
    }
}
