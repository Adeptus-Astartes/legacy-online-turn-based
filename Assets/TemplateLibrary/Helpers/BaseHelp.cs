using UnityEngine;
using System.Collections;

public class BaseHelp
{
	public static Color RandomColor() 
	{
		return new Color(Random.value, Random.value, Random.value);
	}

}
public static class TrainRendererExtensions
{
	/// <summary>
	/// Reset the trail so it can be moved without streaking
	/// </summary>
	public static void Reset(this TrailRenderer trail, MonoBehaviour instance)
	{
		instance.StartCoroutine(ResetTrail(trail));   
	}
	/// <summary>
	/// Coroutine to reset a trail renderer trail
	/// </summary>
	/// <param name="trail"></param>
	/// <returns></returns>
	static IEnumerator ResetTrail(TrailRenderer trail)
	{
		var trailTime = trail.time;
		trail.time = 0;
		yield return 0;
		trail.time = trailTime;
	}
}