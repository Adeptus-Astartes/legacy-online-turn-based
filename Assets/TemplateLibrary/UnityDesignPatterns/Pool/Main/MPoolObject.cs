using UnityEngine;
using System.Collections;

public class MPoolObject : MonoBehaviour, IPoolObject
{
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}
	public IPool PullSource { get; set; }
	public bool Used { get; set; }

	public virtual void InitPoolObject()
	{
		gameObject.SetActive( false );
		Used = false;
		transform.SetParent( PullSource.RootGameObject.transform, false );
	}

	public virtual void OnPop()
	{
		gameObject.SetActive( true );
		Used = true;
	}

	public virtual void BeforeRelease()
	{

	}
	public virtual void AfterRelease()
	{
		gameObject.SetActive( false );
		Used = false;
	}

	public virtual void FreeObject()
	{
		PullSource.Release( this );
	}
	public virtual void DestroyObject()
	{
		PullSource.OnDestoryPullObject( this );
		if( Application.isPlaying )
		{
			GameObject.Destroy( gameObject );
		}
		else
		{
			GameObject.DestroyImmediate( gameObject );
		}
	}

	public object Clone()
	{
		return Instantiate( this );
	}
}