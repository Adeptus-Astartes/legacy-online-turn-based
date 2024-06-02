using UnityEngine;
using System.Collections.Generic;

public interface IPool
{
	List<IPoolObject>	ListPullObjects	{ get; set; }
	Stack<IPoolObject>	Pull			{ get; set; }
	bool				IsExtensible	{ get; set; }
	IPoolObject			ExampleObject	{ get; set; }
	GameObject			RootGameObject	{ get; set; }
	int	Count			{ get; }
	int	StackCount		{ get; }

	void InitializePull( IPoolObject objExample, GameObject root );
	void InitializePull( IPoolObject objExample, GameObject root, int preInstanceCount );

	void Release( IPoolObject obj );
	IPoolObject Pop();

	void ClearPull();
	void FreePull();
	void OnDestoryPullObject( IPoolObject obj );
}