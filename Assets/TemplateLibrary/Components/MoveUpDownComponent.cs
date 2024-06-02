using UnityEngine;
using System.Collections;

public class MoveUpDownComponent : MonoBehaviour 
{
	public Vector3 MoveOffset = new Vector3(0, 0.3f, 0);
	public Vector3 MoveDistance = new Vector3(0, 0.03f, 0);
	public float MoveSpeed = 15f;
	private float _offsetTime;
	private Vector3 _startPosition;

	void Start () 
	{
		_offsetTime = Random.Range(-1, 1);
		_startPosition = transform.localPosition;
	}
	
	void Update () 
	{
		transform.localPosition = _startPosition + MoveOffset +( MoveDistance * Mathf.Sin ( _offsetTime + Time.unscaledTime * MoveSpeed ) );
	}
}
