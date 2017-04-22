using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	public List<Vector3> vertices = new List<Vector3>();
	public List<Vector3> verticesCollider = new List<Vector3>();
	public List<int> triangles = new List<int>();
	public List<int> trianglesCollider = new List<int>();
	public List<Vector2> uvs = new List<Vector2>();
	public bool useRenderDataForCol;

	public void AddQuadTriangles()
	{
		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 1);
		if (!useRenderDataForCol)
			return;
		trianglesCollider.Add(verticesCollider.Count - 4);
		trianglesCollider.Add(verticesCollider.Count - 3);
		trianglesCollider.Add(verticesCollider.Count - 2);
		trianglesCollider.Add(verticesCollider.Count - 4);
		trianglesCollider.Add(verticesCollider.Count - 2);
		trianglesCollider.Add(verticesCollider.Count - 1);
	}

	public void AddVertex(Vector3 vertex)
	{
		vertices.Add(vertex);
		if (useRenderDataForCol)
			verticesCollider.Add(vertex);
	}
}
