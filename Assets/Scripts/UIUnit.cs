using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIUnit : MonoBehaviour
{
	public Player owner;
	public GameObject UnitPrefab;
	public Button Button;
	public Text Description;

	void Start()
	{
		if(Button == null)
		{
			Button = this.GetComponent<Button>();
		}
		else
		{
			//Button.onClick.AddListener(DoSpawn);
		}
		UpdateInfo();
	}

	public void DoSpawn()
	{
		owner.CmdSpawnUnit(UnitPrefab.name, owner.m_selectedSpawnerPos,false,true);
	}

	public void UpdateInfo()
	{
		//Add red overlay if player have no enought money;
		if(UnitPrefab.GetComponent<General>() != null)
		{
			General general = UnitPrefab.GetComponent<General>();
			Description.text = 
				general.MaxHealth.ToString() +       " ARMOR\n" +
				general.Damage.ToString() + 	     " DAMAGE\n" +
				general.explosionDamage.ToString() + " BLAST POWER\n" +
				general.damageRadius.ToString() +    " BLAST RADIUS\n" +
				general.MovementRange.ToString() +   " MOVE DIST\n" +
				general.SpawnPrice.ToString() +      " PRICE";
		}
		else if(UnitPrefab.GetComponent<Medic>() != null)
		{
			Medic medic = UnitPrefab.GetComponent<Medic>();
			Description.text = 
				medic.MaxHealth.ToString() +      " ARMOR\n" +
				medic.treatmentPower.ToString() + " HEAL POWER\n" +
				medic.MovementRange.ToString() +  " MOVE DIST\n" +
				medic.treatmentRange.ToString() + " HEAL RANGE\n" +
				medic.SpawnPrice.ToString() +     " PRICE";
		}
		else if(UnitPrefab.GetComponent<Unit>() != null)
		{
			Unit unit = UnitPrefab.GetComponent<Unit>();
			Description.text = 
				unit.MaxHealth.ToString() +     " ARMOR\n" +
				unit.Damage.ToString() +        " DAMAGE\n" +
				unit.MovementRange.ToString() +	" MOVE DIST\n" +
				unit.AttackRange.ToString() +   " ATTACK DIST\n" +
				unit.SpawnPrice.ToString() +    " PRICE";
		}
	}
}
