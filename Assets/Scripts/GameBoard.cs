using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Model;
public class GameBoard : MonoBehaviour 
{
	public static GameBoard Instance;
	public GameObject Tile;
	public GameObject Line;
	public TileBehaviour[,] GameBoardTiles;

	public Material SpanwerGridMat;

	public int Width, Height;
	const float Spacing = 2f;
	Game _game;
	public Game Game
	{
		get
		{
			return _game;
		}
		set
		{
			_game = value;
		}
	}
	List<GameObject> _path;
	public List<GameObject> Path
	{
		get
		{
			return _path;
		}
		set
		{
			_path = value;
		}
	}

	// Use this for initialization
	void Start () 
	{
		
		Instance = this;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject pl in players)
		{
			pl.GetComponent<Player>().boardInstance = this;
		}


	}

	public void CreateSpawners()
	{
		Player player0 = GameManager.sInstance.Players[0];
		Player player1 = GameManager.sInstance.Players[1];

		Material mat0 = new Material(Shader.Find("Standard"));
		mat0.SetFloat("_Glossiness",1f);
		mat0.color = GameManager.sInstance.Players[0].myColor;
		GameBoardTiles[3,1].gameObject.AddComponent<BoardSpawner>().Material = mat0;
		GameBoardTiles[5,1].gameObject.AddComponent<BoardSpawner>().Material = mat0;
		player0.Spawners.Add(GameBoardTiles[3,1]);
		player0.Spawners.Add(GameBoardTiles[5,1]);

		Material mat1 = new Material(Shader.Find("Standard"));
		mat1.SetFloat("_Glossiness",1f);
		mat1.color = GameManager.sInstance.Players[1].myColor;
		GameBoardTiles[3,9].gameObject.AddComponent<BoardSpawner>().Material = mat1;
		GameBoardTiles[5,9].gameObject.AddComponent<BoardSpawner>().Material = mat1;
		player1.Spawners.Add(GameBoardTiles[3,9]);
		player1.Spawners.Add(GameBoardTiles[5,9]);

	}

	public void CreateBarricades()
	{
		GameBoardTiles[3,4].Block();
		GameBoardTiles[3,5].Block();

		GameBoardTiles[6,5].Block();
		GameBoardTiles[7,5].Block();

		GameBoardTiles[0,1].Block();
		GameBoardTiles[0,2].Block();
		GameBoardTiles[1,2].Block();

		GameBoardTiles[8,8].Block();
		GameBoardTiles[7,8].Block();
		GameBoardTiles[8,7].Block();

	}

	public void SpawnBasicArmy()
	{
		Player player0 = GameManager.sInstance.Players[0];
		Player player1 = GameManager.sInstance.Players[1];

		List<Vector3> player0row = new List<Vector3>();
		player0row.Add(GameBoardTiles[0,0].transform.position);
		player0row.Add(GameBoardTiles[2,0].transform.position);
		player0row.Add(GameBoardTiles[Width - 1,0].transform.position);
		player0row.Add(GameBoardTiles[Width - 3,0].transform.position);

		for(int i = 0; i<player0row.Count; i++)
		{
			player0.CmdSpawnUnit(player0.Army[i].name,player0row[i],true,false);
		}



		List<Vector3> player1row = new List<Vector3>();
		player1row.Add(GameBoardTiles[0,Height-1].transform.position);
		player1row.Add(GameBoardTiles[2,Height-1].transform.position);
		player1row.Add(GameBoardTiles[Width - 1,Height-1].transform.position);
		player1row.Add(GameBoardTiles[Width - 3,Height-1].transform.position);

		for(int i = 0; i<player1row.Count; i++)
		{
			player1.CmdSpawnUnit(player1.Army[i].name,player1row[i],true,false);
		}
	}

	public Vector3 GetWorldCoordinates(int x, float y, int z)
	{
		var yOffset = x % 2 == 0 ? 0 : -Spacing / 2;
		return new Vector3( (/*transform.position.x +*/ x * 1.75f) * Tile.transform.localScale.x, /*transform.position.y +*/ y,( /*transform.position.z + */z * Spacing + yOffset) * Tile.transform.localScale.x);
	}

	public IEnumerable<Tile> BuildPath(Tile _from, Tile _to)
	{
		var start = Game.AllTiles.Single(o => o.X == _from.Location.X && o.Y == _from.Location.Y);
		var destination = Game.AllTiles.Single(o => o.X == _to.Location.X && o.Y == _to.Location.Y);

		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destination.X, 2) + Math.Pow(t.Y - destination.Y, 2));

		var path = PathFind.PathFind.FindPath(start, destination, distance, estimate);

		if (Path == null)
			Path = new List<GameObject>();

		Path.ForEach(Destroy);
		Path = new List<GameObject>();
		path.ToList().ForEach(CreateLine);

		return path;
	}
	public IEnumerable<Tile> BuildPath(Tile _from, Tile _to, Action<bool> callback, bool value)
	{
		var start = Game.AllTiles.Single(o => o.X == _from.Location.X && o.Y == _from.Location.Y);
		var destination = Game.AllTiles.Single(o => o.X == _to.Location.X && o.Y == _to.Location.Y);

		Func<Tile, Tile, double> distance = (node1, node2) => 1;
		Func<Tile, double> estimate = t => Math.Sqrt(Math.Pow(t.X - destination.X, 2) + Math.Pow(t.Y - destination.Y, 2));

		var path = PathFind.PathFind.FindPath(start, destination, distance, estimate);

		if (Path == null)
			Path = new List<GameObject>();

		Path.ForEach(Destroy);
		Path = new List<GameObject>();
		path.ToList().ForEach(CreateLine);
		animationOffset = 0;
		callback(value);
		return path;
	}

	float animationOffset = 0;

	void CreateLine(Tile tile)
	{
		var line = (GameObject)Instantiate(Line);
		line.transform.SetParent(transform);
		line.transform.position = GetWorldCoordinates(tile.Location.X, 1f, tile.Location.Y);

		line.SendMessage("BeginAnimation",animationOffset += 0.5f,SendMessageOptions.DontRequireReceiver);
		Path.Add(line);

	}	
		
	List<TileBehaviour> tileBehaviours = new List<TileBehaviour>();

	public List<TileBehaviour> GetTileBehaviours(int posX, int posY, int range)
	{
		List<TileBehaviour> Tiles = new List<TileBehaviour>();
		float increment = 0;

		if((posX % 2) == 0)
		{
			for(int x = posX - range; x<=posX + range; x++)
			{
				if(x >= posX - range &&  x < posX)
				{
					increment += 0.5f;
					if((range%2) == 0)
					{

						for(int y = posY + Mathf.CeilToInt(range/2) + Mathf.FloorToInt(increment); y > posY - Mathf.FloorToInt(range/2) - Mathf.CeilToInt(increment); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
					else
					{
						for(int y = posY + Mathf.CeilToInt(range/2) + Mathf.CeilToInt(increment); y > posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment + 0.1f); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
				}
				if(x > posX &&  x <= posX + range)
				{
					increment -= 0.5f;
					if((range%2) == 0)
					{

						for(int y = posY + Mathf.CeilToInt(range/2) + Mathf.CeilToInt(increment); y > posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment + 0.1f); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
					else
					{
						for(int y = posY + Mathf.CeilToInt(range/2) + Mathf.CeilToInt(increment + 0.1f); y > posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment + 0.9f); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
				}

				if(x == posX)
				{
					for(int y = posY + range; y>=posY - range; y--)
					{
						if(x >= 0 && y >= 0 && x < Width && y < Height)
						{
							Tiles.Add(GameBoardTiles[x, y]);
						}
					}
				}
			}

		}
		else
		{
			for(int x = posX - range; x<=posX + range; x++)
			{

				if(x >= posX - range &&  x < posX)
				{
					increment += 0.5f;
					if((range%2) == 0)
					{

						for(int y = posY + Mathf.FloorToInt(range/2) + Mathf.FloorToInt(increment - 0.1f); y > posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment + 0.1f); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
					else
					{
						for(int y = posY + Mathf.FloorToInt(range/2) + Mathf.FloorToInt(increment); y >= posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
				}

				if(x > posX &&  x <= posX + range)
				{
					increment -= 0.5f;
					if((range%2) == 0)
					{

						for(int y = posY + Mathf.CeilToInt(range/2) + Mathf.FloorToInt(increment); y >= posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment); y--)
						{
							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
					else
					{
						for(int y = posY + Mathf.FloorToInt(range/2) + Mathf.CeilToInt(increment); y >= posY - Mathf.CeilToInt(range/2) - Mathf.CeilToInt(increment + 0.1f); y--)
						{

							if(x >= 0 && y >= 0 && x < Width && y < Height)
							{
								Tiles.Add(GameBoardTiles[x, y]);
							}
						}
					}
				}

				if(x == posX)
				{
					for(int y = posY + range ; y >= posY - range; y--)
					{
						if(x >= 0 && y >= 0 && x < Width && y < Height)
						{
							Tiles.Add(GameBoardTiles[x, y]);
						}
					}
				}
			}

		}
		return Tiles;
	}

	public void DisableHightlight()
	{
		for(int x = 0; x < Width; x++)
		{
			for(int y = 0; y < Height; y++)
			{
				GameBoardTiles[x, y].Hightlight(3);
			}
		}
	}
}
