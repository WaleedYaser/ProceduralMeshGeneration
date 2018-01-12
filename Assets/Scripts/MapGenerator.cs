using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	[Range(0, 100)]
	public int randomFillPercent;

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;
	int[,] map;

	private void Start()
	{
		GenerateMap();
	}

	private void GenerateMap()
	{
		map = new int[width, height];

		RandomFillMap();
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
				map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent)? 0: 1;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if(map == null)
			return;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Gizmos.color = (map[x, y] == 1)? Color.black: Color.white;
				Vector3 pos = new Vector3(-width/2 + x + 0.5f, 0, -height/2 + y + 0.5f);
				Gizmos.DrawCube(pos, Vector3.one);
			}
		}		
	}
}
