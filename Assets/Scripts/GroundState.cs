using UnityEngine;

public class GroundState
{
    private GameObject player;
    private float rayLength = 0.05f;
    private float rayWidth, rayHeight;

    public GroundState(GameObject player)
    {
        this.player = player;
        rayWidth = player.GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        rayHeight = player.GetComponent<Collider2D>().bounds.extents.y + 0.1f;
    }

    public bool IsWall()
    {
        if (Physics2D.Raycast(new Vector2(player.transform.position.x - rayWidth, player.transform.position.y), -Vector2.right, rayLength) ||
            Physics2D.Raycast(new Vector2(player.transform.position.x + rayWidth, player.transform.position.y), Vector2.right, rayLength))
            return true;
        return false;
    }

    public bool IsGround()
    {
        if (Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - rayHeight), -Vector2.up, rayLength))
            return true;
        return false;
    }

    public bool IsTouching()
    {
        if (IsGround() || IsWall())
            return true;
        return false;
    }

    public int WallDirection()
    {
        if (Physics2D.Raycast(new Vector2(player.transform.position.x - rayWidth, player.transform.position.y), -Vector2.right, rayLength))
            return -1;
        else if (Physics2D.Raycast(new Vector2(player.transform.position.x + rayWidth, player.transform.position.y), -Vector2.right, rayLength))
            return 1;
        return 0;
    }
}
