using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class LevelManager : MonoBehaviour
{
	public string levelName;
	public int levelWidth, levelHeight;

	private Tile[,] levelTiles;
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;

	private void Awake()
	{
		meshFilter = gameObject.GetComponent<MeshFilter>();
		meshCollider = gameObject.GetComponent<MeshCollider>();
	}

	private void Start()
	{
		LoadLevel("Data/Levels/test");
	}

	public Tile GetTile(int x, int y)
	{
		return levelTiles[x, y];
	}

	private void LoadLevel(string levelData)
	{
		string[] parseLevelDataLine = new string[] { "\r\n", "\r", "\n" };
		TextAsset data = Resources.Load(levelData) as TextAsset;
		string[] dataLine = data.text.Split(parseLevelDataLine, StringSplitOptions.None);
		SetLevelInfo(dataLine);
		SetLevelData(dataLine);
		UpdateMesh();
	}

	private void SetLevelInfo(string[] dataLine)
	{
		string[] dataSplit = dataLine[0].Split(' ');
		foreach (string data in dataSplit)
		{
			string[] readData = data.Split(':');
			if (readData[0] == "name")
				levelName = readData[1];
			else if (readData[0] == "width")
				levelWidth = Int32.Parse(readData[1]);
			else if (readData[0] == "height")
				levelHeight = Int32.Parse(readData[1]);
		}
	}

	private void SetLevelData(string[] dataLine)
	{
		levelTiles = new Tile[levelWidth, levelHeight];
		for (int y = 0; y < levelHeight; y++)
		{
			//string[] dataSplit = dataLine[y + 1].Split(' ');
			for (int x = 0; x < levelWidth; x++)
			{
				levelTiles[x, y] = new Tile();
			}
		}
	}

	private void UpdateMesh()
	{
		MeshData meshData = new MeshData();
		for (int x = 0; x < levelWidth; x++)
			for (int y = 0; y < levelHeight; y++)
				meshData = levelTiles[x, y].TileData(this, x, y, meshData);
		RenderMesh(meshData);
	}

	private void RenderMesh(MeshData meshData)
	{
		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices = meshData.vertices.ToArray();
		meshFilter.mesh.triangles = meshData.triangles.ToArray();
		meshFilter.mesh.uv = meshData.uvs.ToArray();
		meshFilter.mesh.RecalculateNormals();

		Mesh mesh = new Mesh();
		meshCollider.sharedMesh = null;
		mesh.vertices = meshData.verticesCollider.ToArray();
		mesh.triangles = meshData.trianglesCollider.ToArray();
		mesh.RecalculateNormals();
		meshCollider.sharedMesh = mesh;
	}
}
