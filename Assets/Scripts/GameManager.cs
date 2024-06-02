using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public List<Player> Players = new List<Player>();
	public Player CurrentTurnOwner = null;

	static public GameManager sInstance = null;
	[Header("UI Links")]
	public Button PassTurnButton;
	public Text TurnOwnerText;
	public Text MoneyText;
	public ScrollRect UnitListScrollView;
	public Transform UnitListViewport;
	public GameObject UnitListBlocker;
	public Transform HealthBarRoot;

	public GameObject UnitListWindow;

	public GameObject ResultWindow;
	public Text winnerText;
	public Button BackInLobbyButton;

	public GameObject PreGameWindow;
	public Text PreGameTimer;
	public GameObject GameWindow;

	public List<GameObject> Armies;

	public List<Slider> PlayersHealthbars;


	void Awake()
	{
		sInstance = this;
	}

	public void ColorUpdate(Color newColor)
	{
		GetComponent<Renderer>().material.color = newColor;
	}
	void OnGUI()
	{
		/*
		foreach(Player player in Players)
		{
			if(player == null)
				return;
			GUI.color = player.myColor;
			GUILayout.Label(player.name);
		}*/
	}
}
