using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class Bullet : NetworkBehaviour
{
	public int damage = 0;
	public GameObject HitEffect;
	void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
		var hitUnit = hit.GetComponent<Unit>();
		if (hitUnit != null)
		{
			//hitUnit.Hit(damage);
			Destroy(gameObject);
		}
		var hitPlayer = hit.GetComponent<Player>();
		if (hitPlayer != null)
		{
			//hitPlayer.CmdHitBase(damage);
			Destroy(gameObject);
		}
	}
}
