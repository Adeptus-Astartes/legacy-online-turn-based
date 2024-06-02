using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
public class Medic : Unit
{

	public int treatmentRange = 2;
	public int treatmentPower = 1;
	public List<Tile> TreatmentZone;



	public override void PlayerPassTurn ()
	{
		base.PlayerPassTurn ();

		BoardInstance.DisableHightlight();
		//TreatmentZone = base.BoardInstance.ShowTreatmentRange(currentTile.Location.X,currentTile.Location.Y,treatmentRange);
		List<TileBehaviour> tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y,treatmentRange);
		TreatmentZone = new List<Tile>();

		for(int i = 0; i<tiles.Count; i++)
		{
			TreatmentZone.Add(tiles[i].Tile);
			tiles[i].Hightlight(4);
		}

		foreach(Unit unit in controller.Units)
		{
			if(TreatmentZone.Contains(unit.currentTile))
			{
				if(unit.Health < unit.MaxHealth)
				{
					controller.CmdHeal(unit.gameObject, treatmentPower);
				}
			}
		}
	}

	public override void Highlight (Color color, bool value)
	{
		myHealthBar.Select(value);
		if(value)
		{

			BoardInstance.DisableHightlight();
			//AttackZone = new List<Tile>();
			List<TileBehaviour> tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y, MovementRange);
			for(int i = 0; i<tiles.Count; i++)
			{
				tiles[i].Hightlight(1);
			}

			tiles.Clear();

			tiles = BoardInstance.GetTileBehaviours(currentTile.X,currentTile.Y,treatmentRange);
			TreatmentZone = new List<Tile>();

			for(int i = 0; i<tiles.Count; i++)
			{
				TreatmentZone.Add(tiles[i].Tile);
				tiles[i].Hightlight(4);
			}
		}
	}
}
