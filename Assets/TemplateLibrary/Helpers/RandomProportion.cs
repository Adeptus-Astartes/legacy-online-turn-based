using UnityEngine;
using System.Collections.Generic;

public class RandomProportion<T>
{
	public float Proportion { get; set; }
	public T Value { get; set; }
}

 public static class RandomProportion
 {
    public static RandomProportion<T> Create<T>(float proportion, T value)
	{
        return new RandomProportion<T> { Proportion = proportion, Value = value };
	}
    public static T ChooseByRandom<T>( this IEnumerable<RandomProportion<T>> collection )
	{
		var rnd = Random.value;
		foreach (var item in collection)
		{
            if (rnd < item.Proportion)
            {
                return item.Value;
            }
			rnd -= item.Proportion;
		}
		Debug.Log("The proportions in the collection do not add up to 1.");
		return default(T);
	}
 }


/*
        float d = 1f / 3f;
        var slot = new List<RandomProportion<ESlotMachineStats>>
        {
            RandomProportion.Create(d, ESlotMachineStats.A),
            RandomProportion.Create(d, ESlotMachineStats.D),
            RandomProportion.Create(d, ESlotMachineStats.S)
        };

        SlotStat = slot.ChooseByRandom();
*/