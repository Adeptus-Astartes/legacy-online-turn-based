using UnityEngine;
using System.Collections;

/// <summary>Abstract base class for thread-safe singleton objects</summary>
/// <typeparam name="T">Instance type</typeparam>

public class SingletonDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	private static object _lock = new object();

	/// <summary>
	/// This instanse form SingletonMonoBehaviour.
	/// </summary>
	public static T Instance
	{
		get
		{
			lock( _lock )
			{
				if( instance == null )
				{
					instance = ( T ) FindObjectOfType( typeof( T ) );
					if( instance != null )
					{
						DontDestroyOnLoad( instance.gameObject );
					}
					if( FindObjectsOfType( typeof( T ) ).Length > 1 )
					{
						Debug.LogError( "[Singleton] Something went really wrong " +
										" - there should never be more than 1 singleton!" +
										" Reopening the scene might fix it." );
						return instance;
					}

					if( instance == null )
					{
						GameObject singleton = new GameObject();
						instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof( T );

						DontDestroyOnLoad( singleton );

						Debug.Log( "[Singleton] An instance of " + typeof( T ) +
								   " is needed in the scene, so '" + singleton +
								   "' was created with DontDestroyOnLoad." );
					}
					else
					{
						Debug.Log( "[Singleton] Using instance already created: " +
								   instance.gameObject.name );
					}
				}

				return instance;
			}
		}
	}

	public virtual void OnApplicationQuit()
	{
		instance = null;
	}

	public static void Set( T type )
	{
		instance = type;
	}

    public static void ResetInstance()
    {
        if (instance != null && instance.gameObject != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }
}