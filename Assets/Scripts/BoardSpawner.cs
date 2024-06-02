using UnityEngine;
using Model;
using System.Collections;

public class BoardSpawner : MonoBehaviour
{

	public TileBehaviour tile;
	public Material Material;
	// Use this for initialization
	void Start () 
	{
		tile = this.GetComponent<TileBehaviour>();
		GetComponent<Renderer>().material = Material;
		Debug.LogError("TILE");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
