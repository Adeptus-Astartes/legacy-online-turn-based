using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace Prototype.NetworkLobby
{
	//Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
	public class LobbyMainMenu : MonoBehaviour 
	{
		public LobbyManager lobbyManager;

		public RectTransform lobbyServerList;
		public RectTransform lobbyPanel;

		public InputField ipInput;
		public InputField matchNameInput;

		public void OnEnable()
		{
			lobbyManager.topPanel.ToggleVisibility(true);

			ipInput.onEndEdit.RemoveAllListeners();
			ipInput.onEndEdit.AddListener(onEndEditIP);

			matchNameInput.onEndEdit.RemoveAllListeners();
			matchNameInput.onEndEdit.AddListener(onEndEditGameName);
		}

		public void OnClickHost()
		{
			lobbyManager.StartHost();
		}

		public void OnClickJoin()
		{
			lobbyManager.ChangeTo(lobbyPanel);

			lobbyManager.networkAddress = ipInput.text;
			lobbyManager.StartClient();

			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
			lobbyManager.DisplayIsConnecting();

			lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
		}

		public void OnClickDedicated()
		{
			lobbyManager.ChangeTo(null);
			lobbyManager.StartServer();

			lobbyManager.backDelegate = lobbyManager.StopServerClbk;

			lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
		}
		//
		public void OnQuickGame()
		{
			Debug.Log("QG");
			lobbyManager.StartMatchMaker();
			lobbyManager.matchMaker.ListMatches(0, 50, "", true, 0, 0, OnGUIMatchList);
			lobbyManager.mainMenuPanel.gameObject.SetActive(false);
		}
		public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
		{
			if (matches.Count == 0)
			{
				Debug.LogError("CREATE");
				StartMatchmakingGame();	
			}
			else
			{
				Debug.LogError("JOIN");
				JoinRandom(matches);
			}
		}

		public void StartMatchmakingGame()
		{
			//lobbyManager.StartMatchMaker();
			lobbyManager.matchMaker.CreateMatch(Random.Range(0,99999).ToString(), 2, true, "", "", "", 0,0, lobbyManager.OnMatchCreate);
			lobbyManager.backDelegate = lobbyManager.StopHost;
			lobbyManager._isMatchmaking = true;
			lobbyManager.DisplayIsConnecting();

			lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
		}

		public void JoinRandom(List<MatchInfoSnapshot> matches)
		{
			MatchInfoSnapshot match = matches[Random.Range(0,matches.Count)];

			lobbyManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
			lobbyManager._isMatchmaking = true;
			lobbyManager.DisplayIsConnecting();
		}


		//
		public void OnClickCreateMatchmakingGame()
		{
			lobbyManager.StartMatchMaker();
			lobbyManager.matchMaker.CreateMatch(
			Random.Range(0,99999).ToString(),
			(uint)lobbyManager.maxPlayers,
			true,
			"", "", "", 0, 0,
			lobbyManager.OnMatchCreate);

			lobbyManager.backDelegate = lobbyManager.StopHost;
			lobbyManager._isMatchmaking = true;
			lobbyManager.DisplayIsConnecting();

			lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
		}

		public void OnClickOpenServerList()
		{
			lobbyManager.StartMatchMaker();
			lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
			lobbyManager.ChangeTo(lobbyServerList);
		}

		void onEndEditIP(string text)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				OnClickJoin();
			}
		}

		void onEndEditGameName(string text)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				OnClickCreateMatchmakingGame();
			}
		}

	}
}
