using UnityEngine;

public class SinRotateComponent : MonoBehaviour 
{

	public Vector3 RotateOffset = new Vector3(0, 0.3f, 0);
	public Vector3 RotateAngle = new Vector3(0, 10.0f, 0);
	public float RotateSpeed = 15f;
	private float _offsetTime;
	private Vector3 _startRotation;

	void Start () 
	{
		_offsetTime = Random.Range(-1, 1);
		_startRotation = transform.localEulerAngles;
	}
	
	void Update () 
	{
		transform.localEulerAngles = _startRotation + RotateOffset +( RotateAngle * Mathf.Sin ( _offsetTime + Time.unscaledTime * RotateSpeed ) );
	}
}
