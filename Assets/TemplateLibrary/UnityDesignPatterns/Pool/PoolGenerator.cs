using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class COnePoolItem
{
	public string ObjectType;
	public MPool GenericPull;

	public bool CheckName( string name )
	{
		var n1 = name.Replace( "(Clone)","" );
		var n2 = ObjectType.Replace( "(Clone)","" );
		return n1 == n2;
	}

	public COnePoolItem( IPoolObject example, GameObject root )
	{
		ObjectType = example.Name;
		GenericPull = new MPool();
		GenericPull.InitializePull( example, root );
		GenericPull.IsExtensible = true;
	}
}

public class PoolGenerator : SingletonDontDestroy<PoolGenerator>
{
	public List<COnePoolItem> Pulls;

	void Awake()
	{
		Pulls = new List<COnePoolItem>();
	}
	public static void FreePull( MPoolObject getPrefab )
	{
		var list = Instance.Pulls.FirstOrDefault( p=>p.CheckName( getPrefab.Name ) );
		if( list != null )
		{
			list.GenericPull.FreePull();
		}
	}

	private IPoolObject GetObject( IPoolObject obj )
	{
		var list = Pulls.FirstOrDefault( p=>p.CheckName( obj.Name ) );
		if( list == null )
		{
			var onePull = new COnePoolItem( obj, gameObject );
			Pulls.Add( onePull );
			return onePull.GenericPull.Pop();
		}
		return list.GenericPull.Pop();
	}

	private void PushObject( IPoolObject obj )
	{
		var list = Pulls.FirstOrDefault( p=>p.CheckName( obj.Name ) );
		if( list == null )
		{
			var onePull = new COnePoolItem( obj, gameObject );
			Pulls.Add( onePull );
			onePull.GenericPull.Release( obj );
		}
		else
		{
			list.GenericPull.Release( obj );
		}
	}

	public static IPoolObject Take( IPoolObject obj )
	{
		return Instance.GetObject( obj );
	}

	public static void Push( IPoolObject obj )
	{
		Instance.PushObject( obj );
	}

	public static void DestroyPools()
	{
		Instance.Pulls = new List<COnePoolItem>();
	}
}