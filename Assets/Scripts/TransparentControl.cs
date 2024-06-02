using UnityEngine;
using System;
using System.Collections;

public class TransparentControl : MonoBehaviour
{
	public Renderer[] Meshes;
	bool TweenBegin = false;

	float m_alpha;
	float m_beginFrom;
	float m_targetAlpha;
	float m_step;
	float m_time;

	public void Start()
	{
		for(int i = 0; i<Meshes.Length; i++) //Prepare Standard Shader
		{
			//Meshes[i].material.SetInt("_Mode", 2); //Set Fade Mode
			Meshes[i].material.SetFloat("_ZWrite", 1.0f);
		}
	}

	void Update () 
	{
		if(TweenBegin)
		{
			m_time += m_step * Time.deltaTime;
			m_alpha = Mathf.MoveTowards(m_beginFrom,m_targetAlpha,m_time);
			for(int i = 0; i<Meshes.Length; i++)
			{
				Meshes[i].material.SetColor("_Color", new Color(1f,1f,1f, m_alpha));
			}
			if(m_time > 1)
			{
				myCallback.Invoke();
				TweenBegin = false;
			}
		}
	}

	Action myCallback;

	public void Fade(float beginFrom, float target, float speed, Action callback)
	{
		m_beginFrom = beginFrom;
		m_targetAlpha = target;
		m_step = speed;
		myCallback = callback;

		TweenBegin = true;
	}

	public void Finish()
	{
		Debug.Log("Finish");
	}
}
