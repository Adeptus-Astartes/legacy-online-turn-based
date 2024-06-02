using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour {


	public static DebugGUI Instance;
	public string Text;
	// Use this for initialization
	void Start () {
		Instance = this;
	}

	void OnGUI()
	{
		GUI.color = Color.white;
		if(GUI.Button(new Rect(0,0,100,30), "CLEAR"))
		{
			Text = "";
		}
		GUILayout.Label(Text);
	}
}
