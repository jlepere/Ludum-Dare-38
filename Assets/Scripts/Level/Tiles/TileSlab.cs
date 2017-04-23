using UnityEngine;

public class TileSlab : Tile
{
    public TileSlab(bool top)
    {
        if (top)
            texturePos = new Vector2(3, -2);
        else
            texturePos = new Vector2(2, -2);
    }

    public override bool IsSolid
    {
        get { return true; }
    }
}
