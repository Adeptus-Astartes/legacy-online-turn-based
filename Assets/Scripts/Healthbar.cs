using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Healthbar : MonoBehaviour 
{
	public Renderer fill;
	public float myMaxValue;

	//public float normalSize = 0.2f;
	//public float activeSize = 0.4f;
	public AnimationCurve curve;
	public float amplitude = 1;
	public float speed = 1;
	private bool IsSelected = false;


	private float runTime = 0;
	private Vector3 m_originPos;

	void OnEnable()
	{
		if(fill == null)
		{
			fill = transform.Find("_Fill").GetComponent<Renderer>();
		}
		m_originPos = transform.localPosition;
	}

	public void UpdateHealth(float newValue)
	{
		if(fill != null)
		{
			fill.transform.localScale = Vector3.one * newValue/myMaxValue;
		}
	}

	public void SetUpHealthBar(float maxValue, Color playerColor)
	{
		Debug.Log("Healthbar");
		myMaxValue = maxValue;
		fill.material.SetColor("_Color", playerColor);
	}

	public void Select(bool myValue)
	{
		IsSelected = myValue;
		if(myValue == true)
		{
			
		}
		else
		{
			transform.localPosition = m_originPos;
			runTime = 0;
		}
	}

	void Update()
	{
		if(IsSelected)
		{
			runTime += Time.deltaTime * speed;
			transform.localPosition = m_originPos + Vector3.up * curve.Evaluate(runTime) * amplitude;
		}
	}



}
