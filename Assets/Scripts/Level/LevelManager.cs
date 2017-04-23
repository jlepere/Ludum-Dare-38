using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject levelCamera;

    [SerializeField]
    private GameObject levelPlayer;

    [SerializeField]
    private float cameraMoveOffset = 5f;

    public string levelName;
	public int levelWidth, levelHeight;
    public Vector3 playerSpawn;

	private Tile[,] levelTiles;
	private MeshFilter meshFilter;
    private PolygonCollider2D meshCollider;
    private List<Vector2> levelHitbox;

	private void Awake()
	{
		meshFilter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<PolygonCollider2D>();
        LoadLevel("Data/Levels/test");
    }

    private void Update()
    {
        int cameraLeftWall = levelWidth / 4;
        int cameraRightWall = -cameraLeftWall;
        if (levelCamera.transform.position.x > cameraRightWall && levelCamera.transform.position.x > levelPlayer.transform.position.x + cameraMoveOffset)
            levelCamera.transform.position = new Vector3(levelPlayer.transform.position.x + cameraMoveOffset, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x < cameraLeftWall && levelCamera.transform.position.x < levelPlayer.transform.position.x - cameraMoveOffset)
            levelCamera.transform.position = new Vector3(levelPlayer.transform.position.x - cameraMoveOffset, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x <= cameraRightWall)
            levelCamera.transform.position = new Vector3(cameraRightWall, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x >= cameraLeftWall)
            levelCamera.transform.position = new Vector3(cameraLeftWall, levelCamera.transform.position.y, levelCamera.transform.position.z);
    }

    public Tile GetTile(int x, int y)
	{
		return levelTiles[x, y];
	}

	private void LoadLevel(string levelData)
	{
        int levelDataIndex = 0;
		string[] parseLevelDataLine = new string[] { "\r\n", "\r", "\n" };
		TextAsset data = Resources.Load(levelData) as TextAsset;
		string[] dataLine = data.text.Split(parseLevelDataLine, StringSplitOptions.None);
		levelDataIndex = SetLevelInfo(dataLine);
        levelDataIndex = SetLevelHitbox(dataLine, levelDataIndex);
		SetLevelData(dataLine, levelDataIndex);
		UpdateMesh();
	}

	private int SetLevelInfo(string[] dataLine)
	{
        playerSpawn = new Vector3(0, 0, -5);
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
            else if (readData[0] == "playerx")
                playerSpawn.x = Int32.Parse(readData[1]);
            else if (readData[0] == "playery")
                playerSpawn.y = -Int32.Parse(readData[1]) / 2;
		}
        return 1;
	}

    private int SetLevelHitbox(string[] dataLine, int levelDataIndex)
    {
        levelHitbox = new List<Vector2>();
        string[] dataSplit = dataLine[levelDataIndex].Split(' ');
        foreach (string dataSplited in dataSplit)
        {
            string[] hitboxPosition = dataSplited.Split(';');
            foreach (string data in hitboxPosition)
            {
                string[] readData = data.Split(':');
                levelHitbox.Add(new Vector2(Int32.Parse(readData[0]) - (levelWidth / 2), -(Int32.Parse(readData[1]) - (levelHeight / 2) - 1)));
            }
        }
        return ++levelDataIndex;
    }

	private void SetLevelData(string[] dataLine, int levelDataIndex)
	{
		levelTiles = new Tile[levelWidth, levelHeight];
		for (int y = 0; y < levelHeight; y++)
		{
			string[] dataSplit = dataLine[y + levelDataIndex].Split(' ');
			for (int x = 0; x < levelWidth; x++)
			{
                if (dataSplit[x] == "0")
                    levelTiles[x, levelHeight - y - 1] = new Tile();
                else if (dataSplit[x] == "1")
                    levelTiles[x, levelHeight - y - 1] = new TileFloor();
            }
		}
	}

	private void UpdateMesh()
	{
		MeshData meshData = new MeshData();
		for (int x = 0; x < levelWidth; x++)
			for (int y = 0; y < levelHeight; y++)
				meshData = levelTiles[x, y].TileData(this, x, y, meshData);
        UpdateCollider(meshData);
		RenderMesh(meshData);
	}

    private void UpdateCollider(MeshData meshData)
    {
        int count = 0;
        meshCollider.pathCount = 0;
        Vector2[] colliderPath = new Vector2[4];
        while (count < levelHitbox.Count)
        {
            levelHitbox.CopyTo(count, colliderPath, 0, 4);
            meshCollider.SetPath(meshCollider.pathCount++, colliderPath);
            count += 4;
        }
    }

	private void RenderMesh(MeshData meshData)
	{
		meshFilter.mesh.Clear();
		meshFilter.mesh.vertices = meshData.vertices.ToArray();
		meshFilter.mesh.triangles = meshData.triangles.ToArray();
		meshFilter.mesh.uv = meshData.uvs.ToArray();
		meshFilter.mesh.RecalculateNormals();
	}
}
