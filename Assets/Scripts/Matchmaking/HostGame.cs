using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class HostGame : MonoBehaviour
{
	List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
	bool matchCreated;
	NetworkMatch networkMatch;

	public Transform roomsRoot;
	public GameObject serverEntryPrefab;

	void Awake()
	{
		networkMatch = gameObject.AddComponent<NetworkMatch>();
	}

	void OnGUI()
	{
		if (GUILayout.Button("Create Room"))
		{
			string matchName = "room";
			uint matchSize = 4;
			bool matchAdvertise = true;
			string matchPassword = "";

			networkMatch.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, "", "", 0, 0, OnMatchCreate);
		}

		if (GUILayout.Button("List rooms"))
		{
			networkMatch.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
		}

		if (GUILayout.Button("Quick Game"))
		{
			networkMatch.ListMatches(0, 20, "", true, 0, 0, OnQuickGame);
		}

		if (matchList.Count > 0)
		{
			GUILayout.Label("Current rooms");
		}
		foreach (var match in matchList)
		{
			if (GUILayout.Button(match.name + " PING : " + Network.GetLastPing(Network.player)))
			{
				networkMatch.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
			}
		}
	}

	public void CreateRoom(string matchName, uint matchSize, bool matchAdvertise, string matchPassword)
	{
		networkMatch.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, "", "", 0, 0, OnMatchCreate);
	}

	public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		if (success)
		{
			Debug.Log("Create match succeeded");
			matchCreated = true;
			NetworkServer.Listen(matchInfo, 9000);
		}
		else
		{
			Debug.LogError("Create match failed: " + extendedInfo);
		}
	}

	public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{
		if (success)
		{
			if(matches != null && matches.Count > 0)
			{
				matchList.Clear();
				matchList = matches;
			}
			else
			{
				CreateRoom(Random.Range(0,99999).ToString(),2,true,"");
			}
		}
		else
		{
			Debug.LogError("List match failed: " + extendedInfo);
		}
	}

	public void OnQuickGame(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{
		if (success)
		{
			if(matches != null && matches.Count > 0)
			{
				networkMatch.JoinMatch(matches[Random.Range(0,matches.Count)].networkId, "", "", "", 0, 0, OnMatchJoined);
			}
			else
			{
				CreateRoom(Random.Range(0,99999).ToString(),2,true,"");
			}
		}
		else
		{
			Debug.LogError("List match failed: " + extendedInfo);
		}
	}

	public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		if (success)
		{
			Debug.Log("Join match succeeded");
			if (matchCreated)
			{
				Debug.LogWarning("Match already set up, aborting...");
				return;
			}
			//Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
			NetworkClient myClient = new NetworkClient();
			myClient.RegisterHandler(MsgType.Connect, OnConnected);
			myClient.Connect(matchInfo);

		}
		else
		{
			Debug.LogError("Join match failed " + extendedInfo);
		}
	}

	public void OnConnected(NetworkMessage msg)
	{
		Debug.Log("Connected!");
	}
}