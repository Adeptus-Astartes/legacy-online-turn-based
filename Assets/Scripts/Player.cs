using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Model;
using Prototype.NetworkLobby;

public delegate void OnBoardChangeHandler(object sender);


public class Player : NetworkBehaviour
{
	[SyncVar]
	public string playerName;
	[SyncVar]
	public Color myColor = Color.red;
	[SyncVar]
	public string myArmy;
	[SyncVar]
	public int Balance = 5;

	[SyncVar]
	public string turnOwner;
	protected bool myTurn = true;
	public int maxHealth = 500;
	[SyncVar]
	public int health = 500;
	public Slider myHealthBar;

	public Renderer baseMesh;

	GameManager m_manager;

	public List<GameObject> Army;
	public List<GameObject> AllUnits;
	public Unit SelectedUnit;
	public List<Unit> Units;
	public GameObject BulletPrefab;
	[Header("UI")]
	public GameObject HealthBarPrefab;

	[SyncVar]
	public float MatchStartOffset;

	[Header("GameBoard")]
	public GameBoard boardInstance;
	public Tile currentTile;
	public LayerMask GridLayer;
	public Material spawnerMaterial;
	public List<TileBehaviour> Spawners;
	//[SyncVar]
	[HideInInspector]
	public Vector3 m_selectedSpawnerPos;
	public Tile First;
	public Tile Last;

	public GameObject Tile;
	public GameObject Line;

	public int Width, Height;

	public bool spawningUnit = false;

	public bool loaded = false;
	 
	//This running on client and say for server that this clien loaded game scene and ready, then server begin spawn game board and other..So this very important
	[Command]
	private void CmdSetReadyStatus()
	{
		RpcSetReadyStatus();
	}
	[ClientRpc]
	private void RpcSetReadyStatus()
	{
		loaded = true;
	}

	// Use this for initialization
	void Start () 
	{
		
		name = playerName;
		GameManager.sInstance.Players.Add(this);

		CreateHealthBars();
		//Hotfix
		if(isLocalPlayer)
		{
			//Debug.Log("LocalReady");
			CmdSetReadyStatus();
		}

		//There need optimization
		if(myArmy == "Axis")
		{
			foreach(GameObject _unit in AllUnits)
			{
				if(_unit.name.Contains("German"))
					Army.Add(_unit);
			}
		}
		if(myArmy == "Allies")
		{
			foreach(GameObject _unit in AllUnits)
			{
				if(_unit.name.Contains("US"))
					Army.Add(_unit);
			}
		}


		if (!isLocalPlayer)
			return;
		m_manager = GameManager.sInstance;
		m_manager.PassTurnButton.onClick.AddListener(PassTurn);
		m_manager.BackInLobbyButton.onClick.AddListener(LobbyManager.s_Singleton.GoBackButton);
	
		foreach(GameObject army in m_manager.Armies)
		{
			if(army.name == myArmy)
			{
				GameObject _army = Instantiate(army) as GameObject;
				_army.transform.SetParent(m_manager.UnitListViewport.transform);
				RectTransform _armyTransform = _army.GetComponent<RectTransform>();
				_armyTransform.sizeDelta = new Vector2(0,_armyTransform.sizeDelta.y);
				_armyTransform.localScale = Vector3.one;
				m_manager.UnitListScrollView.content = _armyTransform;
				m_manager.UnitListScrollView.enabled = true;
				_armyTransform.anchoredPosition = Vector2.zero;
				_army.GetComponent<UnitList>().SetPlayer(this);
			}
		}
		Destroy(GameObject.Find("_Fade"));
		SetupClient();
	}

	public void CreateHealthBars()
	{
		int idx = GameManager.sInstance.Players.IndexOf(this);
		myHealthBar = GameManager.sInstance.PlayersHealthbars[idx];
		myHealthBar.maxValue = maxHealth;
		myHealthBar.value = maxHealth;
		myHealthBar.fillRect.GetComponent<Image>().color = myColor;
		myHealthBar.gameObject.SetActive(true);
		baseMesh.material.SetColor("_Color", myColor);
	}

	public void UpdateCurrentTile()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out hit, 100,GridLayer))
		{
			if(hit.collider.GetComponent<TileBehaviour>() != null)
			{
				currentTile = hit.collider.GetComponent<TileBehaviour>().Tile;
				currentTile.CanPass = false;
				Debug.LogError(currentTile);
			}
		}
		DebugGUI.Instance.Text += "\nTile : " + currentTile.ToString();
	}

	[ClientCallback]
	void Update()
	{

		if (!isLocalPlayer)
			return;
		if(MatchStartOffset>0)
		{

			if(m_manager == null)
				return;
			if(m_manager.PreGameWindow)
			if(m_manager.PreGameWindow.activeInHierarchy)
			{
				m_manager.PreGameWindow.SetActive(true);
				m_manager.GameWindow.SetActive(false);
				m_manager.PreGameWindow.transform.SetAsLastSibling();
			}
			else
			{

			}

			MatchStartOffset -= Time.deltaTime;
			m_manager.PreGameTimer.text = "MATCH START OFFSET " + MatchStartOffset.ToString("0");
		}
		else
		{
			if(!m_manager.GameWindow.activeInHierarchy)
			{
				m_manager.PreGameWindow.SetActive(false);
				m_manager.GameWindow.SetActive(true);
				m_manager.GameWindow.transform.SetAsLastSibling();
			}
		}

		if(!myTurn)
			return;

		if(Input.GetMouseButtonDown(0))
		{
			if(!EventSystem.current.IsPointerOverGameObject())
			{
				Control();
			}
		}



	}


	#region Board
	[Command]
	public void CmdSpawnBoard() //call this on the server
	{ 
		boardInstance = new GameObject().AddComponent<GameBoard>();
		boardInstance.name = "GameBoard";
		boardInstance.tag = "GameBoard";
		boardInstance.Tile = Tile;
		boardInstance.Line = Line;
		boardInstance.SpanwerGridMat = spawnerMaterial;

		boardInstance.Game = new Game(Width, Height);
		boardInstance.GameBoardTiles = new TileBehaviour[Width, Height];
		boardInstance.Width = Width;
		boardInstance.Height = Height;

		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				var tile = (GameObject)Instantiate(Tile);
				tile.transform.SetParent(boardInstance.transform);

				var tileTransform = tile.transform;
				tileTransform.position = boardInstance.GetWorldCoordinates(x, 0, y);
				tileTransform.eulerAngles = new Vector3(270, -90, 0);
				var tb = (TileBehaviour)tile.GetComponent("TileBehaviour");
				boardInstance.GameBoardTiles[x, y] = tb;
				tb.Tile = boardInstance.Game.GameBoard[x, y];
				tb.SetMaterial();
	
				NetworkServer.Spawn(tile);
				RpcSpawnBoard(tile, x, y);
			}
		}
	}

	[ClientRpc]
	void RpcSpawnBoard(GameObject tile, int x, int y)
	{
		
		if(boardInstance == null)
		{
			boardInstance = new GameObject().AddComponent<GameBoard>();
			boardInstance.name = "GameBoard";
			boardInstance.tag = "GameBoard";
			boardInstance.Tile = Tile;
			boardInstance.Line = Line;
			boardInstance.SpanwerGridMat = spawnerMaterial;

			boardInstance.Game = new Game(Width, Height);
			boardInstance.GameBoardTiles = new TileBehaviour[Width, Height];

			boardInstance.Width = Width;
			boardInstance.Height = Height;

		}

		if(boardInstance != null)
		{
			tile.transform.SetParent(boardInstance.transform);

			var tileTransform = tile.transform;
			tileTransform.position = boardInstance.GetWorldCoordinates(x, 0, y);
			tileTransform.eulerAngles = new Vector3(270, -90, 0);
			var tb = (TileBehaviour)tile.GetComponent("TileBehaviour");
			boardInstance.GameBoardTiles[x, y] = tb;
			tb.Tile = boardInstance.Game.GameBoard[x, y];

			tb.SetMaterial();
		}

	}
	[Command]
	public void CmdSpawnMapDetails()
	{
		GameBoard.Instance.CreateSpawners();
		GameBoard.Instance.CreateBarricades();
		UpdateCurrentTile();
		RpcSpawnMapDetails();
	}
	[ClientRpc]
	public void RpcSpawnMapDetails()
	{
		GameBoard.Instance.CreateSpawners();
		GameBoard.Instance.CreateBarricades();
		UpdateCurrentTile();
	}

	#endregion

	#region TurnBase

	NetworkClient myClient;

	public class MyMsgType {
		public static short Score = MsgType.Highest + 1;
	};

	public class ScoreMessage : MessageBase
	{
		public string turnOwner;
		public string playerColor;
	}
	[Command]
	public void CmdSetTurnOwner(string name, string color)
	{
		RpcSetTurnOwner(name, color);
	}

	[ClientRpc]
	public void RpcSetTurnOwner(string name, string color)
	{
		if(!isLocalPlayer)
			return;
		if(playerName == name)
		{
			m_manager.TurnOwnerText.text = "TURN " + "<color=" + color + ">" + name + "</color>";
			m_manager.PassTurnButton.gameObject.SetActive(true);
			m_manager.UnitListBlocker.SetActive(false);
		}
		else
		{
			myTurn = false;
			m_manager.TurnOwnerText.text = "TURN " + "<color=" + color + ">" + name + "</color>";
			m_manager.PassTurnButton.gameObject.SetActive(false);
			m_manager.UnitListBlocker.SetActive(true);
		}
		RefreshUI();
	}
	public void PassTurn()
	{
		GameManager managerRef = GameManager.sInstance;
		Player opponent = null;
		for(int i = 0; i<managerRef.Players.Count; i++)
		{
			//Debug.Log(this  + " :: " + managerRef.Players[i]);
			if(this != managerRef.Players[i])
			{
				opponent = managerRef.Players[i];
				//Debug.Log("I : " + gameObject.name + " Opponent : " + opponent.gameObject.name);
				break;
			}
		}
		boardInstance.DisableHightlight();
		CmdPassTurn(opponent.playerName,RGBToHex(opponent.myColor));
	}

	[Command]
	public void CmdPassTurn(string oppnentName,string color)
	{
		RpcPassTurn(oppnentName, color);
	}

	[ClientRpc]
	public void RpcPassTurn(string oppnentName, string color)
	{
		Balance += 5;
		if(isLocalPlayer)
		{
			m_manager.UnitListWindow.SetActive(false);
			foreach(Unit unit in Units)
			{
				unit.PlayerPassTurn();
			}
		}
		OnPassTurn(oppnentName, color);
	}

	public void OnPassTurn(string oppnentName, string color)
	{
		BroadcastTurnOwner(oppnentName, color);
	}

	public void BroadcastTurnOwner(string owner, string color)
	{
		ScoreMessage msg = new ScoreMessage();
		msg.turnOwner = owner;
		msg.playerColor = color;
		NetworkServer.SendToAll(MyMsgType.Score, msg);
	}

	// Create a client and connect to the server port
	public void SetupClient()
	{
		RefreshUI();

		myClient = LobbyManager.singleton.client;
		myClient.RegisterHandler(MyMsgType.Score, OnTurnOwner);
	}

	public void RefreshUI()
	{
		if(isLocalPlayer)
			m_manager.MoneyText.text = "COINS : " + Balance.ToString();
	}

	public void OnTurnOwner(NetworkMessage netMsg)
	{
		ScoreMessage msg = netMsg.ReadMessage<ScoreMessage>();
		if(msg.turnOwner == playerName)
		{
			myTurn = true;
			m_manager.TurnOwnerText.text = "TURN " + "<color=" + msg.playerColor + ">" + msg.turnOwner + "</color>";
			m_manager.PassTurnButton.gameObject.SetActive(true);
			m_manager.UnitListBlocker.SetActive(false);
		}
		else
		{
			myTurn = false;
			m_manager.TurnOwnerText.text = "TURN " + "<color=" + msg.playerColor + ">" + msg.turnOwner + "</color>";
			m_manager.PassTurnButton.gameObject.SetActive(false);
			m_manager.UnitListBlocker.SetActive(true);
		}
		RefreshUI();
	}

	public string RGBToHex(Color color)
	{
		string rgbString = string.Format("#{0:X2}{1:X2}{2:X2}",(int)(color.r * 255),(int)(color.g * 255),(int)(color.b * 255));
		return rgbString;
	}

	#endregion

	#region SpawnUnit
	[Command]
	public void CmdSpawnUnit(string _unitName, Vector3 pos, bool forFree, bool selectAfterSpawn) //call this on the server
	{ 
		GameObject _unit = null;
		foreach(GameObject obj in AllUnits)
		{
			if(obj.name == _unitName)
			{
				_unit = obj;
			}
		}
		if(forFree)
		{
			GameObject spawnedUnit = (GameObject)Instantiate(_unit,pos,transform.rotation);
			NetworkServer.Spawn(spawnedUnit);
			RpcSpawnUnit(spawnedUnit, forFree, selectAfterSpawn);
		}
		else
		{
			if(Balance - _unit.GetComponent<Unit>().SpawnPrice >= 0)
			{
				GameObject spawnedUnit = (GameObject)Instantiate(_unit,pos,transform.rotation);
				NetworkServer.Spawn(spawnedUnit);
				RpcSpawnUnit(spawnedUnit, forFree, selectAfterSpawn);
			}
		}
	}

	[ClientRpc]
	void RpcSpawnUnit(GameObject _spawnedUnit,bool forFree, bool selectAfterSpawn)
	{
		Debug.Log("RPC");
		if(isLocalPlayer)
			m_manager.UnitListWindow.SetActive(false);
		_spawnedUnit.transform.SetParent(transform);
		_spawnedUnit.transform.position += Vector3.up * 0.2f;
		_spawnedUnit.transform.rotation = transform.rotation;
		Unit _unit = _spawnedUnit.GetComponent<Unit>();
		if(!forFree)
			TakeMoney(_unit.SpawnPrice);
		_unit.Health = _unit.MaxHealth;
		_unit.controller = this;
		_unit.UpdateCurrentTile();
		_unit.SetupHealthbar(myColor);
		Units.Add(_unit);
		//if(!isLocalPlayer) // Disable Highlight
		//	_unit.myHealthBar.transform.GetCgameObject.SetActive(false);
		if(SelectedUnit == null)
		{
			if(selectAfterSpawn)
				CmdSelectUnit(_spawnedUnit);
		}
	}
	/*
	void SpawnUnit(GameObject _spawnedUnit,bool forFree, bool selectAfterSpawn)
	{
		Debug.Log("RPC");
		if(isLocalPlayer)
			m_manager.UnitListWindow.SetActive(false);
		_spawnedUnit.transform.SetParent(transform);
		_spawnedUnit.transform.position += Vector3.up * 0.2f;
		_spawnedUnit.transform.rotation = transform.rotation;
		Unit _unit = _spawnedUnit.GetComponent<Unit>();
		if(!forFree)
			TakeMoney(_unit.SpawnPrice);
		_unit.Health = _unit.MaxHealth;
		_unit.controller = this;
		_unit.UpdateCurrentTile();
		_unit.SetupHealthbar(myColor);
		Units.Add(_unit);
		//if(!isLocalPlayer) // Disable Highlight
		//	_unit.myHealthBar.transform.GetCgameObject.SetActive(false);
		if(SelectedUnit == null)
		{
			if(selectAfterSpawn)
				CmdSelectUnit(_spawnedUnit);
		}
	}
*/

	public void TakeMoney(int value)
	{
		Balance -= value;
		if(Balance <= 0)
		{
			PassTurn();
		}
		if(isLocalPlayer)
			m_manager.MoneyText.text = "COINS : " + Balance.ToString();
	}
	#endregion

	#region Control


	public void Control()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100))
		{
			TileBehaviour _temp = hit.collider.GetComponent<TileBehaviour>();

			if(Spawners.Contains(_temp))
			{
				if(boardInstance.Game.GameBoard[_temp.Tile.X,_temp.Tile.Y].CanPass == true) // If Cell Empty, then open spawn menu
				{
					m_selectedSpawnerPos = hit.collider.transform.position;
					m_manager.UnitListWindow.SetActive(!m_manager.UnitListWindow.activeInHierarchy);
				}

			}
			if(hit.collider.GetComponent<Player>() != null)
			{
				if(hit.collider.GetComponent<Player>() != this)
				{
					if(SelectedUnit)
					{
						CmdAttack(hit.collider.gameObject);
					}
				}

			}
			if(hit.collider.GetComponent<Unit>() != null)
			{
				if(Units.Contains(hit.collider.GetComponent<Unit>()))
				{
					CmdSelectUnit(hit.collider.gameObject);
				}
				else
				{

					if(SelectedUnit)
					{
						CmdAttack(hit.collider.gameObject);
					}
				}
			}
			if(hit.collider.GetComponent<Unit>() == null)
			{
				if(_temp)
				if(SelectedUnit && _temp.Tile.CanPass)
				{
					SetUnitTarget();
				}
			}
		}
	}

	public void SetUnitTarget()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100))
		{
			TileBehaviour _tile = hit.collider.GetComponent<TileBehaviour>();
			if(_tile != null)
			{
				Debug.Log("!NULL");
				if(_tile.Able)
				{
					if(!Spawners.Contains(_tile))
					{
						Debug.Log("ABLE");
						CmdMoveTo(hit.collider.GetComponent<NetworkIdentity>().netId);
					}
				}
			}

		}
	}


	[Command]
	public void CmdMoveTo(NetworkInstanceId newPosition) { //call this on the server
		if(SelectedUnit == null)
			return;
		if(Balance - SelectedUnit.MovementPrice >= 0)
		{
			
			RpcMoveTo(newPosition);

		}


	}
	[ClientRpc]
	void RpcMoveTo(NetworkInstanceId newPosition) {
		if(SelectedUnit == null)
			return;
		SelectedUnit.MoveTo(ClientScene.FindLocalObject(newPosition)); //this will run in all clients
		TakeMoney(SelectedUnit.MovementPrice);
	}

	[Command]
	public void CmdAttack(GameObject enemy) { //call this on the server
		if(SelectedUnit == null)
			return;
		if(Balance - SelectedUnit.AttackPrice >= 0)
		{
			RpcAttack(enemy);
		}

	}
	[ClientRpc]
	void RpcAttack(GameObject enemy) {
		if(SelectedUnit == null)
			return;
		SelectedUnit.Attack(enemy);
		TakeMoney(SelectedUnit.AttackPrice);
	}



	//Selecting
	[Command]
	public void CmdSelectUnit(GameObject _unit)
	{
		if(_unit == null)
			return;
		RpcSelectUnit(_unit);
	}
	[ClientRpc]
	void RpcSelectUnit(GameObject _unit)
	{
		if(_unit == null)
			return;
		//Debug.LogError(name + " : " + _unit.name);
		if(SelectedUnit != null)//Disable Previously
		{
			if(isLocalPlayer)
			SelectedUnit.Highlight(myColor,false);
		}
		SelectedUnit = _unit.GetComponent<Unit>();
		if(isLocalPlayer && !SelectedUnit.IsMoving)
			SelectedUnit.Highlight(myColor,true);

	}

	//[Command]
	public void CmdFire(Vector3 shootPoint, GameObject target, int distance, int damage)
	{
		if(isClient)
		{
			Debug.Log("DAMAGE : " + damage);
			Unit _targetUnit = target.GetComponent<Unit>();
			if(_targetUnit != null)
			{
				_targetUnit.Hit(damage);
			}
			Player _targetBase = target.GetComponent<Player>();
			if(_targetBase != null)
			{
				_targetBase.CmdHitBase(damage);
			}
			var bullet = (GameObject)Instantiate(BulletPrefab,  shootPoint,	Quaternion.identity);
			//target.position - transform.position
			bullet.GetComponent<Rigidbody>().velocity = (new Vector3(target.transform.position.x,shootPoint.y,target.transform.position.z)  - shootPoint).normalized * 20;
		}
		else
		{
			RpcFire(target,damage,shootPoint);
		}
	}
		

	[ClientRpc]
	public void RpcFire(GameObject target, int damage, Vector3 shootPoint)
	{
		Debug.Log("DAMAGE : " + damage);
		Unit _target = target.GetComponent<Unit>();
		_target.Hit(damage);

		var bullet = (GameObject)Instantiate(BulletPrefab,  shootPoint,	Quaternion.identity);
		bullet.GetComponent<Rigidbody>().velocity = (new Vector3(target.transform.position.x,shootPoint.y,target.transform.position.z)  - shootPoint).normalized * 20;
	}

	public void DoDamage(GameObject target, int damage)
	{
		if(isClient)
		{
			Debug.Log("DAMAGE : " + damage);
			Unit _target = target.GetComponent<Unit>();
			_target.Hit(damage);
		}
		else
		{
			RpcDoDamage(target,damage);
		}
	}

	[ClientRpc]
	public void RpcDoDamage(GameObject target, int damage)
	{
		Debug.Log("DAMAGE : " + damage);
		Unit _target = target.GetComponent<Unit>();
		_target.Hit(damage);
	}


	[Command]
	public void CmdHit(GameObject unit, int damage)
	{
		RpcHit(unit, damage);
	}
	[Command]
	public void CmdHitBase(int damage)
	{
		RpcHitBase(damage);
	}
	[ClientRpc]
	public void RpcHit(GameObject unit, int damage)
	{
		var _unit = unit.GetComponent<Unit>();
		Unit _myUnit = null;
		foreach(Unit myUnit in Units)
		{
			if(myUnit == _unit)
			{
				_myUnit = myUnit;
			}
		}
		if(_myUnit)
		{
			_myUnit.RecieveDamage(damage);
		}
	}

	[Command]
	public void CmdHeal(GameObject unit, int power)
	{
		RpcHeal(unit, power);
	}

	[ClientRpc]
	public void RpcHeal(GameObject unit, int power)
	{
		unit.GetComponent<Unit>().Heal(power);
	}

	[ClientRpc]
	public void RpcHitBase(int damage)
	{
		health -= damage;
		myHealthBar.value = health;
		if(health <=0)
		{
			if(isLocalPlayer)
			{
				GameManager.sInstance.ResultWindow.SetActive(true);
				GameManager.sInstance.winnerText.text = "DEFEAT!";
			}
			else
			{
				GameManager.sInstance.ResultWindow.SetActive(true);
				GameManager.sInstance.winnerText.text = "VICTORY!";
				#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
				GoogleControler.Instance.PlayerWin();
				#endif
			}
			this.enabled = false;
		}
	}
	#endregion
		
}
