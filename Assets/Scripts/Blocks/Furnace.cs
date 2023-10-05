public class Furnace : Block
{
    public static readonly string NAME = "furnace";
    public Furnace() : base(NAME)
    {
        SetTextureAll("furnace_side", "furnace_front", "furnace_top", "furnace_top", "furnace_side", "furnace_side");
    }
}