using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System;
using Model;

public class TileBehaviour : NetworkBehaviour
{
	public Tile Tile;
	//public bool Block = false;
	public GameObject ableZone;
	public GameObject meleeZone;
	public GameObject treatmentZone;
	public bool Able = false;
	//public bool engaged = false;

	public void SetMaterial()
	{
		//this.GetComponent<Renderer>().material.color = Tile.CanPass ? new Color(0,1,0,0.1f) : new Color(0,1,0,0.1f);
	}

	// Update is called once per frame
	void Update()
	{
		//if(!Block)
		//	return;
		//BoardBehaviour.Instance.Game.GameBoard[Tile.X,Tile.Y].CanPass = false;
		//SetMaterial();
	}

	public void Block()
	{
		Debug.Log("Block");
		Able = false;
		Tile.CanPass = false;
		gameObject.SetActive(false);
	}

	public void Hightlight(int value)
	{
		// 1 = movement
		// 2 = attack
		// 3 = disable all
		switch(value)
		{
		case 1:
			Able = true;
			ableZone.SetActive(true);
			meleeZone.SetActive(false);
			treatmentZone.SetActive(false);
			break;
		case 2:
			Able = false;
			ableZone.SetActive(false);
			meleeZone.SetActive(true);
			treatmentZone.SetActive(false);
			break;
		case 3:
			Able = false;
			ableZone.SetActive(false);
			meleeZone.SetActive(false);
			treatmentZone.SetActive(false);
			break;
		case 4:
			Able = true;
			ableZone.SetActive(false);
			meleeZone.SetActive(false);
			treatmentZone.SetActive(true);
			break;
		}
	}
}
