using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviBarObject : MonoBehaviour 
{
	[HideInInspector]
	public UIControler target;

	public void SendSelectEvent()
	{
		if(target!= null)
			target.SelectNavibarObject(gameObject);
	}
}
