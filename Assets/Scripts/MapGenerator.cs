using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	[Range(0, 100)]
	public int randomFillPercent = 45;
	public int smoothIterations = 5;
	public int smoothWallCount = 4;
	public int width = 60;
	public int height = 80;

	public string seed = "Waleed";
	public bool useRandomSeed;
	int[,] map;

	private void Start()
	{
		GenerateMap();
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
			GenerateMap();
	}

	private void GenerateMap()
	{
		map = new int[width, height];

		RandomFillMap();

		for (int i = 0; i < smoothIterations; i++)
		{
			SmoothMap();
		}

		MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
		meshGenerator.GenerateMesh(map, 1);
	}

	private void RandomFillMap()
	{
		if(useRandomSeed)
			seed = Time.time.ToString();

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if(x == 0 || x == width-1 || y == 0 || y == height-1)
					map[x, y] = 1;
				else
					map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent)? 1: 0;
			}
		}
	}

	private void SmoothMap()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int neighbours = GetSurroundingWallCount(x, y);

				if(neighbours > smoothWallCount)
				{
					map[x, y] = 1;
				}
				else if (neighbours < smoothWallCount)
				{
					map[x, y] = 0;
				}
			}
		}
	}
	
	private int GetSurroundingWallCount(int x, int y)
	{
		int wallCount = 0;

		for(int neighbourX = x - 1; neighbourX <= x + 1; neighbourX ++)
		{
			for(int neighbourY = y-1; neighbourY <= y +1; neighbourY ++)
			{
				if(neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
				{
					if(neighbourX != x || neighbourY != y)
					{
						wallCount += map[neighbourX, neighbourY];
					}
				}
				else
				{
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

	
	// private void OnDrawGizmos()
	// {
	// 	if(map == null)
	// 		return;

	// 	for (int x = 0; x < width; x++)
	// 	{
	// 		for (int y = 0; y < height; y++)
	// 		{
	// 			Gizmos.color = (map[x, y] == 1)? Color.black: Color.white;
	// 			Vector3 pos = new Vector3(-width/2 + x + 0.5f, 0, -height/2 + y + 0.5f);
	// 			Gizmos.DrawCube(pos, Vector3.one);
	// 		}
	// 	}		
	// }
}
