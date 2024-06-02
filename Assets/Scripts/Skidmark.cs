using UnityEngine;
using System.Collections;

public class Skidmark : MonoBehaviour
{
	public AnimationCurve curve;
	public float playOffset;
	public float speed;
	public float amplitude = 1;

	public float rotSpeed;

	float playTime = 0;
	//float rotSpeedRnd = 0;
	Transform mesh;

	// Use this for initialization
	void Start () 
	{
		mesh = transform.GetChild(0);
		//rotSpeedRnd = Random.Range(-rotSpeed,rotSpeed);
		//BeginAnimation(playOffset);
	}

	public void BeginAnimation(float offset)
	{
		playTime = offset;
	}

	// Update is called once per frame
	void Update () 
	{
		playTime += speed * Time.deltaTime;

		mesh.localPosition = Vector3.up * curve.Evaluate(playTime) * amplitude;
		transform.Rotate(Vector3.up * rotSpeed);
		/*if(playTime > 1)
			playTime = 0;*/
	}
}
