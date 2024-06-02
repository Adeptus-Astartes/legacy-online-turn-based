using UnityEngine;

public class MenuObject : MonoBehaviour
{
	public virtual void Activate()
	{
		Debug.Log(name + " Activated");
	}

	public virtual void DeActivate()
	{
		Debug.Log(name + " Deactivated");
	}
}
