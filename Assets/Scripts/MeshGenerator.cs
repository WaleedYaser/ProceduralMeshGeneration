using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {


	public class Node
	{
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 position)
		{
			this.position = position;
		}
	}

	public class ControlNode: Node
	{
		public bool active;
		public Node above, right;

		public ControlNode(Vector3 position, bool active, float squareSize): base(position)
		{
			this.active = active;

			above = new Node(position + Vector3.forward * squareSize/2);
			right = new Node(position + Vector3.right * squareSize / 2);
		}
	}

	public class Square
	{
		public ControlNode topLeft, topRight, buttomRight, buttomLeft;
		public Node centerTop, centerRight, centerButtom, centerLeft;
		public int configuration;

		public Square(ControlNode topLeft, ControlNode topRight, ControlNode buttomRight, ControlNode buttomLeft)
		{
			this.topLeft = topLeft;
			this.topRight = topRight;
			this.buttomRight = buttomRight;
			this.buttomLeft = buttomLeft;

			centerTop = this.topLeft.right;
			centerRight = this.buttomRight.above;
			centerButtom = this.buttomLeft.right;
			centerLeft = this.buttomLeft.above;

			if (topLeft.active) configuration += 8;
			if (topRight.active) configuration += 4;
			if (buttomRight.active) configuration += 2;
			if(buttomLeft.active) configuration += 1;
		}
	}

	public class SquareGrid
	{
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize)
		{
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);

			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for (int x = 0; x < nodeCountX; x++)
			{
				for (int y = 0; y < nodeCountY; y++)
				{
					Vector3 pos = new Vector3(-mapWidth/2 + x*squareSize + squareSize/2, 0, -mapHeight/2 + y*squareSize + squareSize/2);
					controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX-1, nodeCountY-1];

			for (int x = 0; x < nodeCountX-1; x++)
			{
				for (int y = 0; y < nodeCountY-1; y++)
				{
					squares[x, y] = new Square(controlNodes[x, y+1], controlNodes[x+1, y+1], controlNodes[x+1, y], controlNodes[x, y]);
				}
			}			
		}
	}

	public SquareGrid squareGrid;
	public List<Vector3> vertices;
	public List<int> triangles;

	public void GenerateMesh(int[,] map, float squareSize)
	{
		squareGrid = new SquareGrid(map, squareSize);

		for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
		{
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
			{
				TriangulateSquare(squareGrid.squares[x, y]);
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = mesh;

	}
	
	private void TriangulateSquare(Square square)
	{
		switch(square.configuration)
		{
			case 0:
			break;

			// 1 point
			case 1: // 0001
			MeshFromPoints(square.centerButtom, square.buttomLeft, square.centerLeft);
			break;

			case 2: // 0010
			MeshFromPoints(square.centerRight, square.buttomRight, square.centerButtom);
			break;

			case 4: // 0100
			MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
			break;

			case 8: // 1000
			MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
			break;

			// 2 points
			case 3: // 0011
			MeshFromPoints(square.centerRight, square.buttomRight, square.buttomLeft, square.centerLeft);
			break;

			case 5: // 0101
			MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerButtom, square.buttomLeft, square.centerLeft);
			break;

			case 6: // 0110
			MeshFromPoints(square.centerTop, square.topRight, square.buttomRight,square.centerButtom);
			break;

			case 9: // 1001
			MeshFromPoints(square.topLeft, square.centerTop, square.centerButtom, square.buttomLeft);
			break;

			case 10: // 1010
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.buttomRight, square.centerButtom, square.centerLeft);
			break;

			case 12:
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);	
			break;

			// 3 points
			case 7: // 0111
			MeshFromPoints(square.centerTop, square.topRight, square.buttomRight, square.buttomLeft, square.centerLeft);	
			break;

			case 11:// 1011
			MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.buttomRight, square.buttomLeft);
			break;

			case 13: // 1101
			MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerButtom, square.buttomLeft);
			break;

			case 14: // 1110
			MeshFromPoints(square.topLeft, square.topRight, square.buttomRight, square.centerButtom, square.centerLeft);
			break;

			// 4 points
			case 15: // 1111
			MeshFromPoints(square.topLeft, square.topRight, square.buttomRight, square.buttomLeft);
			break;
		}	
	}

	private void MeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if(points.Length >= 3) CreateTriangle(points[0], points[1], points[2]);
		if(points.Length >= 4) CreateTriangle(points[0], points[2], points[3]);
		if(points.Length >= 5) CreateTriangle(points[0], points[3], points[4]);
		if(points.Length >= 6) CreateTriangle(points[0], points[4], points[5]);
	}

	private void AssignVertices(Node[] points)
	{
		for (int i = 0; i < points.Length; i++)
		{
			if(points[i].vertexIndex == -1)
			{
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	private void CreateTriangle(Node a, Node b, Node c)
	{
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);
	}

	private void OnDrawGizmos()
	{
		// if(squareGrid != null)
		// {
		// 	for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
		// 	{
		// 		for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
		// 		{
		// 			Gizmos.color = (squareGrid.squares[x, y].topLeft.active)? Color.black: Color.white;
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

		// 			Gizmos.color = (squareGrid.squares[x, y].topRight.active)? Color.black: Color.white;
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

		// 			Gizmos.color = (squareGrid.squares[x, y].buttomRight.active)? Color.black: Color.white;
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].buttomRight.position, Vector3.one * 0.4f);

		// 			Gizmos.color = (squareGrid.squares[x, y].buttomLeft.active)? Color.black: Color.white;
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].buttomLeft.position, Vector3.one * 0.4f);

		// 			Gizmos.color = Color.gray;
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].centerTop.position, Vector3.one * 0.15f);
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].centerRight.position, Vector3.one * 0.15f);
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].centerButtom.position, Vector3.one * 0.15f);
		// 			Gizmos.DrawCube(squareGrid.squares[x, y].centerLeft.position, Vector3.one * 0.15f);
		// 		}
		// 	}
		// }
	}
}
