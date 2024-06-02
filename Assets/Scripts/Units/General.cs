using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
public class General : Unit 
{

	public int damageRadius = 1;
	public int explosionDamage = 2;

	public override void Attack (int distance)
	{
		Vector3 correctTarget = new Vector3(m_target.transform.position.x, transform.position.y, m_target.transform.position.z);

		//Rotating
		Vector3 targetDir = correctTarget - transform.position;
		float rotateStep = RotateSpeed * Time.deltaTime;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotateStep, 0.0F);
		transform.rotation = Quaternion.LookRotation(newDir);
		if(targetDir.normalized == newDir)
		{
			if(animationTrigger)
			{
				animator.SetTrigger("Fire");
				animationTrigger = false;
			}
			_shootDelayTemp += Time.deltaTime;
			if(_shootDelayTemp > ShootDelay)
			{
				//Build Circle
				Tile tile = null;
				Unit unitTarget = m_target.GetComponent<Unit>();
				if(unitTarget!= null)
					tile = unitTarget.currentTile;
				Player baseTarget = m_target.GetComponent<Player>();
				if(baseTarget!= null)
					tile = baseTarget.currentTile;
				List<Tile> damageZone = new List<Tile>();
				if(tile != null)
				{
					List<TileBehaviour> _temp = BoardInstance.GetTileBehaviours(tile.X,tile.Y,damageRadius);
					for(int i = 0; i<_temp.Count; i++)
					{
						damageZone.Add(_temp[i].Tile);
					}
				}
				//GetOpponent
				GameManager managerRef = GameManager.sInstance;
				Player opponent = null;
				for(int i = 0; i<managerRef.Players.Count; i++)
				{
					if(controller != managerRef.Players[i])
					{
						opponent = managerRef.Players[i];
						break;
					}
				}


				foreach(Unit enemy in opponent.Units)
				{
					if(damageZone.Contains(enemy.currentTile) && m_target != enemy.gameObject)
					{
						controller.DoDamage(enemy.gameObject, explosionDamage);
					}
				}

				controller.CmdFire(ShootPoint.transform.position, m_target, distance, Damage);
				_shootDelayTemp = 0;
				FinishTurn();
			}
		}
	}
}
