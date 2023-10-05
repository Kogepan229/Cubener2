public class Grass : Block
{
    public static readonly string NAME = "grass";
    public Grass() : base(NAME)
    {
        SetTextureAll("grass_side", "grass_side", "grass_top", "dirt", "grass_side", "grass_side");
    }
}