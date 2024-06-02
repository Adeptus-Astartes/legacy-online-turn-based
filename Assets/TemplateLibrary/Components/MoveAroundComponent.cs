using UnityEngine;
using System.Collections;

public class MoveAroundComponent : MonoBehaviour 
{
	public Vector3		Direction;
	public Transform	PointOfRotate;
	public float		SpeedOfRotate = 10;

	public bool			Self	= false;
	
	public void Play()
	{
		IsPlay = true;
	}
	
	public void Stop()
	{
		IsPlay = false;
	}
	
	public bool IsPlay = false;

	void Update () 
	{
		if (IsPlay)
		{
			if (Self)
			{
				transform.RotateAround(PointOfRotate.position, transform.TransformDirection(Direction), (SpeedOfRotate));
			}
			else
			{
				transform.RotateAround(PointOfRotate.position, Direction, (SpeedOfRotate));
			}
		}
	}
}