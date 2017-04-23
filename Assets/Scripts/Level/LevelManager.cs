using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject enemiesPrefab;

    [SerializeField]
    private Text gameScore;

    [SerializeField]
    private float cameraMoveOffset = 5f;

    public string levelName;
	public int levelWidth, levelHeight;
    private int mapHitboxBegin, mapHitboxEnd, mapEnemiesCount, mapEnemiesBegin, mapDataCount;
    public Vector3 playerSpawn;

	private Tile[,] levelTiles;
	private MeshFilter meshFilter;
    private PolygonCollider2D meshCollider;
    private List<Vector2>[] levelHitbox;
    private Enemy[] levelEnemies;

	private void Awake()
	{
		meshFilter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<PolygonCollider2D>();
        LoadLevel("Data/Levels/Level_1");
    }

    private void Update()
    {
        int cameraLeftWall = (levelWidth / 2) - 10;
        int cameraRightWall = -cameraLeftWall;
        if (levelCamera.transform.position.x > cameraRightWall && levelCamera.transform.position.x > levelPlayer.transform.position.x + cameraMoveOffset)
            levelCamera.transform.position = new Vector3(levelPlayer.transform.position.x + cameraMoveOffset, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x < cameraLeftWall && levelCamera.transform.position.x < levelPlayer.transform.position.x - cameraMoveOffset)
            levelCamera.transform.position = new Vector3(levelPlayer.transform.position.x - cameraMoveOffset, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x <= cameraRightWall)
            levelCamera.transform.position = new Vector3(cameraRightWall, levelCamera.transform.position.y, levelCamera.transform.position.z);
        else if (levelCamera.transform.position.x >= cameraLeftWall)
            levelCamera.transform.position = new Vector3(cameraLeftWall, levelCamera.transform.position.y, levelCamera.transform.position.z);
        gameScore.text = string.Format("Score : {0}", GameManager.GetScore);
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
        SetLevelHitbox(dataLine);
        SetLevelEnemies(dataLine);
		SetLevelData(dataLine);
		UpdateMesh();
	}

	private void SetLevelInfo(string[] dataLine)
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
            else if (readData[0] == "player_x")
                playerSpawn.x = -(levelWidth / 2) + Int32.Parse(readData[1]);
            else if (readData[0] == "player_y")
                playerSpawn.y = -Int32.Parse(readData[1]) / 2;
            else if (readData[0] == "hitbox_b")
                mapHitboxBegin = Int32.Parse(readData[1]);
            else if (readData[0] == "hitbox_e")
                mapHitboxEnd = Int32.Parse(readData[1]);
            else if (readData[0] == "enemies")
                mapEnemiesCount = Int32.Parse(readData[1]);
            else if (readData[0] == "enemies_b")
                mapEnemiesBegin = Int32.Parse(readData[1]);
            else if (readData[0] == "map_b")
                mapDataCount = Int32.Parse(readData[1]);
		}
	}

    private void SetLevelHitbox(string[] dataLine)
    {
        levelHitbox = new List<Vector2>[mapHitboxEnd + 1 - mapHitboxBegin];
        for (int i = mapHitboxBegin; i <= mapHitboxEnd; i++)
        {
            string[] dataSplit = dataLine[i - 1].Split(' ');
            levelHitbox[mapHitboxEnd - i] = new List<Vector2>();
            foreach (string dataSplited in dataSplit)
            {
                string[] readData = dataSplited.Split(':');
                levelHitbox[mapHitboxEnd - i].Add(new Vector2(float.Parse(readData[0]) - (levelWidth / 2), -(float.Parse(readData[1]) - (levelHeight / 2) - 1)));
            }
        }
    }

    private void SetLevelEnemies(string[] dataLine)
    {
        int count = 0;
        levelEnemies = new Enemy[mapEnemiesCount];
        string[] dataSplit = dataLine[mapEnemiesBegin - 1].Split(' ');
        foreach (string data in dataSplit)
        {
            string[] enemyData = data.Split(':');
            var enemyObject = Instantiate(enemiesPrefab, new Vector3(float.Parse(enemyData[0]), float.Parse(enemyData[1]), 0), Quaternion.Euler(Vector3.zero));
            enemyObject.name = string.Format("Enemy [{0}, {1}]", enemyData[0], enemyData[1]);
            enemyObject.transform.parent = GameObject.Find("Enemies Behaviour").transform;
            levelEnemies[count] = enemyObject.GetComponent<Enemy>();
            if (enemyData.Length == 3)
                levelEnemies[count].GetComponentInChildren<EnemyController2D>().MovementOffset = float.Parse(enemyData[2]);
            count++;
        }
    }

	private void SetLevelData(string[] dataLine)
	{
		levelTiles = new Tile[levelWidth, levelHeight];
		for (int y = 0; y < levelHeight; y++)
		{
			string[] dataSplit = dataLine[y + mapDataCount - 1].Split(' ');
			for (int x = 0; x < levelWidth; x++)
			{
                if (dataSplit[x] == "0")
                    levelTiles[x, levelHeight - y - 1] = new TileBack(false);
                else if (dataSplit[x] == "1")
                    levelTiles[x, levelHeight - y - 1] = new TileFloor();
                else if (dataSplit[x] == "2")
                    levelTiles[x, levelHeight - y - 1] = new TileSlab(false);
                else if (dataSplit[x] == "3")
                    levelTiles[x, levelHeight - y - 1] = new TileSlab(true);
                else if (dataSplit[x] == "4")
                    levelTiles[x, levelHeight - y - 1] = new TileBack(true);
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
        meshCollider.pathCount = 0;
        for (int i = 0; i < mapHitboxEnd + 1 - mapHitboxBegin; i++)
            meshCollider.SetPath(meshCollider.pathCount++, levelHitbox[i].ToArray());
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
