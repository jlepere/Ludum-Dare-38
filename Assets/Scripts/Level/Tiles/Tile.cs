using UnityEngine;

public class Tile
{
	private const float textureSize = 0.5f;
	protected Vector2 texturePos;

	public virtual bool IsSolid
	{
		get { return true; }
	}

	public virtual Vector2[] TilesUVs()
	{
		Vector2[] tileUVs = new Vector2[4];
		tileUVs[0] = new Vector2(textureSize * texturePos.x, textureSize * texturePos.y);
		tileUVs[1] = new Vector2(textureSize * texturePos.x, textureSize * texturePos.y + textureSize);
		tileUVs[2] = new Vector2(textureSize * texturePos.x + textureSize, textureSize * texturePos.y + textureSize);
		tileUVs[3] = new Vector2(textureSize * texturePos.x + textureSize, textureSize * texturePos.y);
		return tileUVs;
	}

	public virtual MeshData TileData(LevelManager level, int x, int y, MeshData meshData)
	{
		meshData.useRenderDataForCol = true;
		if (!level.GetTile(x, y).IsSolid)
			return meshData;
		meshData.AddVertex(new Vector3(x - (level.levelWidth / 2), y - 0.5f - (level.levelHeight / 2), 0.5f));
		meshData.AddVertex(new Vector3(x - (level.levelWidth / 2), y + 0.5f - (level.levelHeight / 2), 0.5f));
		meshData.AddVertex(new Vector3(x + 1 - (level.levelWidth / 2), y + 0.5f - (level.levelHeight / 2), 0.5f));
		meshData.AddVertex(new Vector3(x + 1 - (level.levelWidth / 2), y - 0.5f - (level.levelHeight / 2), 0.5f));
		meshData.AddQuadTriangles();
		meshData.uvs.AddRange(TilesUVs());
		return meshData;
	}
}
