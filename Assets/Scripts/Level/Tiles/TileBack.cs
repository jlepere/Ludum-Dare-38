using UnityEngine;

public class TileBack : Tile
{
    private static System.Random rand = new System.Random();

    public TileBack(bool model)
    {
        if (model)
            texturePos = new Vector2(1, -3);
        else
        {
            int next = rand.Next(0, 2);
            if (next == 0)
                texturePos = new Vector2(1, -2);
            else if (next == 1)
                texturePos = new Vector2(0, -3);
            else if (next == 2)
                texturePos = new Vector2(0, -4);
        }
    }

    public override bool IsSolid
    {
        get { return true; }
    }
}
