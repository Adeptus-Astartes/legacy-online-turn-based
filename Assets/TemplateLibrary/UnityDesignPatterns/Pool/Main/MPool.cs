using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MPool : IPool
{
	public	int Count
	{
		get
		{
			if( ListPullObjects != null )
			{
				return ListPullObjects.Count;
			}
			return -1;
		}
	}
	public	int StackCount
	{
		get
		{
			if( Pull != null )
			{
				return Pull.Count;
			}
			return -1;
		}
	}
	public	bool				IsExtensible	{ get; set; }
	public	List<IPoolObject>	ListPullObjects	{ get; set; }
	public	Stack<IPoolObject>	Pull			{ get; set; }
	public	IPoolObject			ExampleObject	{ get; set; }
	public	GameObject			RootGameObject	{ get; set; }

	/// <summary>
	/// Initializes the pull.
	/// </summary>
	/// <param name="objExample">Object example</param>
	/// <param name="root">Root for set parent</param>
	public void InitializePull( IPoolObject objExample, GameObject root )
	{
		ExampleObject	= objExample;
		RootGameObject	= root;
		Pull			= new Stack<IPoolObject>();
		ListPullObjects	= new List<IPoolObject>();
	}
	/// <summary>
	/// Initializes the pull.
	/// </summary>
	/// <param name="objExample">Object example</param>
	/// <param name="root">Root for set parent</param>
	/// <param name="preInstanceCount">Pre instance count</param>
	public void InitializePull( IPoolObject objExample, GameObject root, int preInstanceCount )
	{
		InitializePull( objExample, root );
		for( int i = 0; i < preInstanceCount; i++ )
		{
			var t = objExample.Clone() as IPoolObject;
			if( t != null )
			{
				t.PullSource = this;
				t.InitPoolObject();
				Pull.Push( t );
				ListPullObjects.Add( t );
			}
		}
	}

	public void Release( IPoolObject obj )
	{
		obj.BeforeRelease();
		Pull.Push( obj );
		obj.AfterRelease();
	}
	public IPoolObject Pop()
	{
		if( Pull.Count == 0 )
		{
			var t = ExampleObject.Clone() as IPoolObject;
			if( t != null )
			{
				t.PullSource = this;
				t.InitPoolObject();
				Pull.Push( t );
				ListPullObjects.Add( t );
			}
		}
		var result = Pull.Pop();
		result.OnPop();
		return result;
	}

	public void FreePull()
	{
		if( Pull != null )
		{
			foreach( var pullObject in ListPullObjects )
			{
				if( pullObject.Used )
				{
					pullObject.FreeObject();
				}
			}
		}
	}

	public void ClearPull()
	{
		if( Pull != null )
		{
			foreach( var pullObject in Pull )
			{
				pullObject.DestroyObject();
			}
			Pull.Clear();
			ListPullObjects.Clear();
		}
	}

	public void OnDestoryPullObject( IPoolObject obj )
	{
		ListPullObjects.Remove( obj );
	}
}