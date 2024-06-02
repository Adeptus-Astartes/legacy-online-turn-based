using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoolGeneratorDynamic
{
	public List<COnePoolItem> Pulls;
	private GameObject _root;
	public void InitRoot( GameObject root )
	{
		Pulls = new List<COnePoolItem>();
		_root = root;
	}
	public void PreInstance( IPoolObject example, int count )
	{
		var elements = new List<IPoolObject>();
		for( int i = 0; i < count; i++ )
		{
			elements.Add(GetObject(example));
		}	
		for( int i = 0; i < elements.Count; i++ )
		{
			Push(elements[i]);
		}	
	}
	public void FreePull( MPoolObject getPrefab )
	{
		var list = Pulls.FirstOrDefault( p=>p.CheckName( getPrefab.Name ) );
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
			var onePull = new COnePoolItem( obj, _root );
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
			var onePull = new COnePoolItem( obj, _root );
			Pulls.Add( onePull );
			onePull.GenericPull.Release( obj );
		}
		else
		{
			list.GenericPull.Release( obj );
		}
	}
	public IPoolObject Take( IPoolObject obj )
	{
		return GetObject( obj );
	}
	public void Push( IPoolObject obj )
	{
		PushObject( obj );
	}
	public void DestroyPools()
	{
		Pulls = new List<COnePoolItem>();
	}
	
}