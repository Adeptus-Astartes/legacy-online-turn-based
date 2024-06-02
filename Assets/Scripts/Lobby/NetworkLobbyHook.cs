using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkLobbyHook : LobbyHook 
{
	protected int count;

	public List<Player> _players;
	public Player localPlayer;
	public bool pass = false;
	public float offset = 2;
	float temp = 0;
	public bool isTest = false;

	public static NetworkLobbyHook Instance;

	void Awake()
	{
		Instance = this;
		StartCoroutine(WaitingOnPlayers());
	}

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
	{
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();

		Player player = gamePlayer.GetComponent<Player>();
		player.playerName = lobby.playerName;
		player.myColor = lobby.playerColor;
		player.myArmy = lobby.playerArmy;
		player.MatchStartOffset = offset;
		_players.Add(player);

	}



	public IEnumerator WaitingOnPlayers()
	{
		yield return new WaitUntil(() => _players.Count >= 2);
		if(!isTest)
		{
		yield return new WaitUntil(() => _players[1].loaded == true);
		DebugGUI.Instance.Text += "\nLoaded";
		}
		temp = offset; // Set Start Game Delay
		//NOTE Timers working, but I need to waiting when client load game scene and will be ready. Need make some method to do this.
		yield return new WaitForSeconds(3);

		_players[0].CmdSpawnBoard(); //Spawn board, 0 bcos is always server in any case, 1 - client

		yield return new WaitForSeconds(2);
		GameBoard.Instance.SpawnBasicArmy();
		_players[0].CmdSpawnMapDetails();
		/*GameBoard.Instance.SpawnBasicArmy();
		GameBoard.Instance.CreateSpawners();
		GameBoard.Instance.CreateBarricades();*/
		int rnd = 1;//Random.Range(0,2);
		if(rnd > 1)
			rnd = 1;
		for(int i = 0; i<2; i++)
		{
			_players[i].CmdSetTurnOwner(_players[rnd].playerName, _players[rnd].RGBToHex(_players[rnd].myColor));
		}

		Debug.Log("LobbyHookCompleteJob");
	}
}
