using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitList : MonoBehaviour
{
	public List<UIUnit> UnitButtons;
	// Use this for initialization
	void Start () 
	{
		
	}

	public void SetPlayer(Player player)
	{
		foreach(UIUnit button in UnitButtons)
		{
			button.owner = player;
		}
	}

	public void UpdateInfo()
	{
		foreach(UIUnit button in UnitButtons)
		{
			button.UpdateInfo();
		}
	}


}
