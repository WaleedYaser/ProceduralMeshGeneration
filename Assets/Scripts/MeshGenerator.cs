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

	public void GenerateMesh(int[,] map, float squareSize)
	{
		squareGrid = new SquareGrid(map, squareSize);
	}

	private void OnDrawGizmos()
	{
		if(squareGrid != null)
		{
			for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
			{
				for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
				{
					Gizmos.color = (squareGrid.squares[x, y].topLeft.active)? Color.black: Color.white;
					Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

					Gizmos.color = (squareGrid.squares[x, y].topRight.active)? Color.black: Color.white;
					Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

					Gizmos.color = (squareGrid.squares[x, y].buttomRight.active)? Color.black: Color.white;
					Gizmos.DrawCube(squareGrid.squares[x, y].buttomRight.position, Vector3.one * 0.4f);

					Gizmos.color = (squareGrid.squares[x, y].buttomLeft.active)? Color.black: Color.white;
					Gizmos.DrawCube(squareGrid.squares[x, y].buttomLeft.position, Vector3.one * 0.4f);

					Gizmos.color = Color.gray;
					Gizmos.DrawCube(squareGrid.squares[x, y].centerTop.position, Vector3.one * 0.15f);
					Gizmos.DrawCube(squareGrid.squares[x, y].centerRight.position, Vector3.one * 0.15f);
					Gizmos.DrawCube(squareGrid.squares[x, y].centerButtom.position, Vector3.one * 0.15f);
					Gizmos.DrawCube(squareGrid.squares[x, y].centerLeft.position, Vector3.one * 0.15f);
				}
			}
		}
	}
}
