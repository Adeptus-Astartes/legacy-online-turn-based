using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LineControler : MonoBehaviour 
{
	public RectTransform currentElement;
	public RectTransform targetElement;
	public RectTransform lineTransform;

	public RectTransform firstFocus;

	public float speed = 1;
	public AnimationCurve curve;
	private bool IsActive = false;
	private float time = 0;

	void Start()
	{
		SetTarget(firstFocus);
	}

	public void SetTarget(RectTransform newTarget)
	{
		if(currentElement == newTarget || targetElement == newTarget || IsActive)
			return;
		
		targetElement = newTarget;
		IsActive = true;
	}


	void Update ()
	{
		if(IsActive)
		{
			time += speed * Time.deltaTime;
			Vector2 pos = Vector2.Lerp(currentElement.position,targetElement.position,curve.Evaluate(time));
			pos.y = lineTransform.position.y;
			lineTransform.position = pos;

			Vector2 size = Vector2.Lerp(currentElement.GetSize(),targetElement.GetSize(),curve.Evaluate(time));
			size.y = lineTransform.sizeDelta.y;
			lineTransform.SetSize(size);
			if(time > 1)
			{
				currentElement = targetElement;
				time = 0;
				IsActive = false;
			}
		}

	}


}
