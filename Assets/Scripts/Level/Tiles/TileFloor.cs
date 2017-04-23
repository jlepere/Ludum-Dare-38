using UnityEngine;

public class TileFloor : Tile
{
    public TileFloor()
    {
        texturePos = new Vector2(0, -2);
    }

    public override bool IsSolid
    {
        get { return true; }
    }
}
