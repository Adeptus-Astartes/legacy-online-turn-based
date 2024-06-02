using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Networking;
//using UnityEngine.Networking.Match;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace Prototype.NetworkLobby
//{
//    public class LobbyServerList : MonoBehaviour
//    {
//        public LobbyManager lobbyManager;
//
//        public RectTransform serverListRect;
//        public GameObject serverEntryPrefab;
//        public GameObject noServerFound;
//
//        protected int currentPage = 0;
//        protected int previousPage = 0;
//
//        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
//        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);
//
//        void OnEnable()
//        {
//            currentPage = 0;
//            previousPage = 0;
//
//            foreach (Transform t in serverListRect)
//                Destroy(t.gameObject);
//
//            noServerFound.SetActive(false);
//
//            RequestPage(0);
//        }
//
//	public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
//	{
//		if (matches.Count == 0)
//		{
//			if (currentPage == 0)
//			{
//				noServerFound.SetActive(true);
//			}
//
//		currentPage = previousPage;
//
//		return;
//	}
//
//            noServerFound.SetActive(false);
//            foreach (Transform t in serverListRect)
//                Destroy(t.gameObject);
//
//			for (int i = 0; i < matches.Count; ++i)
//			{
//                GameObject o = Instantiate(serverEntryPrefab) as GameObject;
//
//				o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor);
//
//				o.transform.SetParent(serverListRect, false);
//            }
//        }
//
//        public void ChangePage(int dir)
//        {
//            int newPage = Mathf.Max(0, currentPage + dir);
//
//            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
//            if (noServerFound.activeSelf)
//                newPage = 0;
//
//            RequestPage(newPage);
//        }
//
//        public void RequestPage(int page)
//        {
//            previousPage = currentPage;
//            currentPage = page;
//			lobbyManager.matchMaker.ListMatches(page, 6, "", true, 0, 0, OnGUIMatchList);
//		}
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
	public class LobbyServerList : MonoBehaviour
	{
		public LobbyManager lobbyManager;

		public RectTransform serverListRect;
		public GameObject serverEntryPrefab;
		public GameObject noServerFound;

		protected int currentPage = 0;
		protected int previousPage = 0;

		static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

		public void RunQuickGame()
		{
			lobbyManager.StartMatchMaker();

			currentPage = 0;
			previousPage = 0;

			foreach (Transform t in serverListRect)
				Destroy(t.gameObject);

			noServerFound.SetActive(false);

			RequestPage(0);
		}

		public void RequestPage(int page)
		{
			previousPage = currentPage;
			currentPage = page;
			//	lobbyManager.matchMaker.ListMatches(page, 6, "", OnGUIMatchList);
			lobbyManager.matchMaker.ListMatches(page, 6, "", true, 0, 0, OnGUIMatchList);
		}

		/*void OnEnable()
		{
			currentPage = 0;
			previousPage = 0;

			foreach (Transform t in serverListRect)
				Destroy(t.gameObject);

			noServerFound.SetActive(false);

			RequestPage(0);
		}*/

		public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
		{
			if (matches.Count == 0)
			{
				if (currentPage == 0)
				{
					StartMatchmakingGame();		
					//noServerFound.SetActive(true);
				}

				currentPage = previousPage;

				//return;
			}
			else
			{
				JoinRandom(matches);
			}

			return;
			//
			noServerFound.SetActive(false);
			foreach (Transform t in serverListRect)
				Destroy(t.gameObject);

			for (int i = 0; i < matches.Count; ++i)
			{
				GameObject o = Instantiate(serverEntryPrefab) as GameObject;
				o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i%2 == 0) ? OddServerColor : EvenServerColor);

				o.transform.SetParent(serverListRect, false);
			}
		}

		public void ChangePage(int dir)
		{
			int newPage = Mathf.Max(0, currentPage + dir);

			//if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
			if (noServerFound.activeSelf)
				newPage = 0;

			RequestPage(newPage);
		}



		public void StartMatchmakingGame()
		{
			lobbyManager.StartMatchMaker();
			//

			//lobbyManager.matchMaker.SetProgramAppID((UnityEngine.Networking.Types.AppID)2023702); //SetProgramAppID((UnityEngine.Networking.Types.AppID)12345);

			//
		///	CreateRoom(,2,true,"");
		// networkMatch.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, "", "", 0, 0, OnMatchCreate);
			lobbyManager.matchMaker.CreateMatch(Random.Range(0,99999).ToString(), 2, true, "", "", "", 0,0, lobbyManager.OnMatchCreate);
			/*lobbyManager.matchMaker.CreateMatch(
				Random.Range(0,99999).ToString(), //matchNameInput.text,
				(uint)lobbyManager.maxPlayers,
				true,
				"",
				lobbyManager.OnMatchCreate);*/

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

	}
}