using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboadLoader : MenuObject 
{
	public GameObject LeaderboardEntryPrefab;
	public Transform LeaderboardRoot;

	public static LeaderboadLoader Instance;

	private bool m_scoresLoaded = false;

	void Start()
	{
		Instance = this;
	}
	#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
	public override void Activate ()
	{
		base.Activate ();
		DebugGUI.Instance.Text += "\nACTIVATED";
		if(!m_scoresLoaded)
			GoogleControler.Instance.LoadScore();
		
	}
	#endif
	public void Load(GPUserData[] data)
	{
		DebugGUI.Instance.Text += "\nDATA COMES";

		for(int i = 0; i < data.Length; i++)
		{
			GameObject obj = Instantiate(LeaderboardEntryPrefab) as GameObject;
			obj.transform.SetParent(LeaderboardRoot);
			obj.transform.localScale = Vector3.one;
			LeaderboarEntry entry = obj.GetComponent<LeaderboarEntry>();
			GPUserData user = data[i];
			entry.Rank.text = user.rank.ToString();
			entry.Avatar.texture = user.avatar;
			entry.UserName.text = user.username;
			entry.Score.text = user.score;
		}
		m_scoresLoaded = true;
	}


}
