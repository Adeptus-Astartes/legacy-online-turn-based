using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model;

public class Unit : NetworkBehaviour
{
	[HideInInspector]
	public Player controller;
	public int SpawnPrice = 2;
	public int MovementPrice = 1;
	public int AttackPrice = 3;

	public int MaxHealth;
	public int Health;
	public Healthbar myHealthBar;

	private TransparentControl TransparentController;

	public Animator animator;
	//public Renderer highlight;
	//PathFinding&Movement
	[HideInInspector]
	public GameBoard BoardInstance;
	public LayerMask hexagonLayer;
	Tile targetTile;
	public Tile currentTile;
	[HideInInspector]
	public TileBehaviour currentTileBehaviour;
	List<GameObject> waypoints;
	int currentId = 0;
	Transform currentWaypoing;
	bool isMoving = false;

	public bool IsMoving
	{
		get
		{
			return isMoving;

		}
	}


	public int MovementRange = 5;
	public int AttackRange = 5;
	public int Damage = 10;
	public float MovementSpeed = 3.5f;
	public float RotateSpeed = 10f;

	public GameObject BulletPrefab;
	public float ShootDelay;
	[HideInInspector]
	public float _shootDelayTemp = 0;
	public GameObject ShootPoint;
	[HideInInspector]
	public bool attacking = false;

	protected GameObject m_target;
	[HideInInspector]
	public List<Tile> AttackZone;
	[HideInInspector]
	public bool haveTurnRight = false;
	[HideInInspector]
	public bool endingTurn = false;

	public float DestroyTimeout = 2f;

	void OnEnable()
	{
		animator = this.GetComponent<Animator>();
		TransparentController = this.GetComponent<TransparentControl>();
		if(BoardInstance == null)
		{
			BoardInstance = GameBoard.Instance;
		}
	}

	public void SetupHealthbar(Color _color)
	{
		if(myHealthBar == null)
			myHealthBar = transform.Find("_HealthBar").GetComponent<Healthbar>();
		else
			myHealthBar.SetUpHealthBar(MaxHealth, _color);
	}

	public virtual void UpdateCurrentTile()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out hit, 100,hexagonLayer))
		{
			if(hit.collider.GetComponent<TileBehaviour>() != null)
			{
				if(currentTileBehaviour != null)
				{
					BoardInstance.Game.GameBoard[currentTile.X,currentTile.Y].CanPass = true;
				}
				currentTileBehaviour = hit.collider.GetComponent<TileBehaviour>();
				currentTile = currentTileBehaviour.Tile;
				BoardInstance.Game.GameBoard[currentTile.X,currentTile.Y].CanPass = false;
			}
		}
	}

	public void MoveTo(GameObject target)
	{
		if(target == null)
			return;
		UpdateCurrentTile();
		targetTile = target.GetComponent<TileBehaviour>().Tile;
		BoardInstance.BuildPath(currentTile, targetTile, OnPathBuild, true);
	}
	[HideInInspector]
	public bool animationTrigger = false;

	public void Attack(GameObject target)
	{
		if(target == null)
			return;

		//AttackZone = BoardInstance.ShowAttackRange(currentTile.X, currentTile.Y,AttackRange);
		//BoardInstance.DisableHightlight();
		UpdateCurrentTile();

		m_target = target;

		if(target.GetComponent<Unit>() != null)
		{
			
			targetTile = m_target.GetComponent<Unit>().currentTile;
			if(targetTile != null)
			{
				//Unlock tile for get able build path to the enemy, after path builded, lock it again..
				GameBoard.Instance.Game.GameBoard[targetTile.X,targetTile.Y].CanPass = true;
			}
			else
			{
				Debug.LogError("!TARGET");
			}
				/*if(meleeZone.Contains(targetTile))
				{
					Debug.LogError("MELEE");
				}
				else
				{
					Debug.LogError("DISTANCE");
				}*/
		}
		if(m_target.GetComponent<Player>() != null)
		{
			targetTile = m_target.GetComponent<Player>().currentTile;
			targetTile.CanPass = true;
		}

		AttackZone = new List<Tile>();
		List<TileBehaviour> tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y, AttackRange);

		for(int i = 0; i<tiles.Count; i++)
		{
			AttackZone.Add(tiles[i].Tile);
			//tiles[i].Hightlight(2);
		}
		tiles.Clear();


		Debug.LogError("TT : " + targetTile);
		Debug.LogError("AZ : " + AttackZone);

		if(AttackZone.Contains(targetTile))
		{
			BoardInstance.BuildPath(currentTile, targetTile, OnPathBuild, false);
			targetTile.CanPass = false;
			attacking = true;
			animationTrigger = true;
		}
	}

	protected int pathDistance = -1;

	public void OnPathBuild(bool automove)
	{
		BoardInstance.DisableHightlight();
		waypoints = BoardInstance.Path;
		waypoints.Reverse();
		currentId = 0;
		currentWaypoing = waypoints[currentId].transform;
		pathDistance = waypoints.Count;
		//Locking again.

		if(automove)
			isMoving = true;
	}
	Player opponent = null;

	[ClientCallback]
	void Update()
	{
		if(isMoving)
		{
			Movement();
		}
		if(attacking)
		{
			Attack(pathDistance);
		}
		if(endingTurn)
		{

			if(opponent == null)
			{
				for(int i = 0; i<GameManager.sInstance.Players.Count; i++)
				{
					//Debug.Log(this  + " :: " + managerRef.Players[i]);
					if(controller != GameManager.sInstance.Players[i])
					{
						opponent = GameManager.sInstance.Players[i];
						//Debug.Log("I : " + gameObject.name + " Opponent : " + opponent.gameObject.name);
						break;
					}
				}

			}
			else
			{
				Vector3 targetDir = new Vector3(opponent.transform.position.x, transform.position.y, opponent.transform.position.z) - transform.position;
				float rotateStep = RotateSpeed * Time.deltaTime;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateStep, 0.0F);
				transform.rotation = Quaternion.LookRotation(newDir);
				if(targetDir.normalized == newDir)
				{
					endingTurn = false;
				}
			}
		}
	}

	void Movement()
	{
		Vector3 correctTarget = new Vector3(currentWaypoing.position.x, transform.position.y, currentWaypoing.position.z);

		//Movement
		float movementStep = MovementSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, correctTarget, movementStep);

		//Rotating
		Vector3 targetDir = correctTarget - transform.position;
		float rotateStep = RotateSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateStep, 0.0F);
		transform.rotation = Quaternion.LookRotation(newDir);

		//Checking Road
		if(Vector3.Distance(transform.position, correctTarget) < 0.1f)
		{
			if(currentId + 1 < waypoints.Count)
			{
				
				animator.SetBool("Run",true);
				currentId++;
				currentWaypoing = waypoints[currentId].transform;
			}
			else
			{
				FinishTurn();
			}
		}
	}

	public virtual void Attack(int distance)
	{
		Vector3 correctTarget = new Vector3(m_target.transform.position.x, transform.position.y, m_target.transform.position.z);

		//Rotating
		Vector3 targetDir = correctTarget - transform.position;
		float rotateStep = RotateSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateStep, 0.0F);
		transform.rotation = Quaternion.LookRotation(newDir);
		if(targetDir.normalized == newDir)
		{
			if(animationTrigger)
			{
				animator.SetTrigger("Fire");
				animationTrigger = false;
			}
			_shootDelayTemp += Time.deltaTime;
			if(_shootDelayTemp > ShootDelay)
			{
				controller.CmdFire(ShootPoint.transform.position, m_target, distance, Damage);
				_shootDelayTemp = 0;
				FinishTurn();
			}
		}
	}

	public void FinishTurn()
	{
		UpdateCurrentTile();
		Highlight(controller.myColor,false);
		//controller.PassTurn();
		animator.SetBool("Run",false);
		isMoving = false;
		attacking = false;
		endingTurn = true;
		//Clean Path
		BoardInstance.Path.ForEach(Destroy);
		haveTurnRight = false;
		//BoardInstance.DisableHightlight();
		//transform.rotation = 
	}

	public virtual void PlayerPassTurn()
	{
		haveTurnRight = true;
		//Do something..
	}

	public void Hit(int damage)
	{
		
		controller.CmdHit(gameObject, damage);
	}

	public void RecieveDamage(int damage)
	{
		Health -= damage;
		myHealthBar.UpdateHealth(Health);
		if(Health <=0)
		{
			Destroy(myHealthBar.gameObject);
			animator.SetTrigger("Dead");
			controller.Units.Remove(this);
			//!I dont destroy unit, because maybe in future will add "phoneix" feature!
			myHealthBar.Select(false);
			GetComponent<BoxCollider>().enabled = false;
			this.enabled = false;
			currentTile.CanPass = true;
			TransparentController.Fade(1.0f, 0.0f, 0.6f,UnitDestroy);
		}


	}

	public void Heal(int power)
	{
		Health += power;
		if(Health > MaxHealth)
			Health = MaxHealth;
		myHealthBar.UpdateHealth(Health);
	}

	public void UnitDestroy()
	{
		NetworkServer.Destroy(gameObject);
	}

	public virtual void Highlight(Color color, bool value)
	{
		myHealthBar.Select(value);
		if(value)
		{
			myHealthBar.Select(true);

			BoardInstance.DisableHightlight();

			AttackZone = new List<Tile>();
			List<TileBehaviour> tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y, AttackRange);

			for(int i = 0; i<tiles.Count; i++)
			{
				AttackZone.Add(tiles[i].Tile);
				tiles[i].Hightlight(2);
			}
			tiles.Clear();

			tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y, MovementRange);
			for(int i = 0; i<tiles.Count; i++)
			{
				tiles[i].Hightlight(1);
			}

		}
	}
}
