using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

using Prototype.NetworkLobby;

[System.Serializable]
public class UITab
{
	public string name;
	public CanvasGroup tabGroup;
	public string tipText;
}

public class UIControler : MonoBehaviour 
{
	public static UIControler Instance;

	public RectTransform root;
	public List<UITab> Tabs;
	public int currentTab;
	public int targetTab;

	public AnimationCurve transitionCurve;
	public float transitionSpeed;

	[Header("Navigation Bar")]
	public HorizontalLayoutGroup naviBarRoot;
	public GameObject naviObject;

	public Text tipTextObject;

	private bool m_IsActive = false;
	private float m_time = 0;

	private CanvasGroup m_currentGroup;
	private CanvasGroup m_targetGroup;

	[Header("ArmySelection")]
	//public GameObject Lobby;
	public GameObject ArmySelection;
	public bool ClearPF = false;

	LobbyPlayer m_player;
	public void OpenArmySelectionTab(LobbyPlayer player)
	{
		m_player = player;
		//if(Lobby != null)
		//	Lobby.SetActive(false);
		ArmySelection.SetActive(true);
	}

	public void SelectArmy(string name)
	{
		//Lobby.SetActive(true);
		ArmySelection.SetActive(false);
		m_player.CmdArmyChanged(name);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>


	// Use this for initialization
	void Start () 
	{
		Instance = this;
		if(ClearPF)
			PlayerPrefs.DeleteAll();

		if(Tabs.Count > 0)
			SelectTab(0);
	
		#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
		if(!GoogleControler.Instance.IsLogged())
		{
			GoogleControler.Instance.Login();
		}
		#endif

	}

	// Update is called once per frame
	void Update () 
	{
		if(m_IsActive)
		{
			m_time += transitionSpeed * Time.deltaTime;

			if(m_currentGroup == null)
			{
				m_currentGroup = Tabs[currentTab].tabGroup;
			}
			else
			{
				m_currentGroup.alpha = Mathf.Lerp(1,0,transitionCurve.Evaluate(m_time));
				m_currentGroup.interactable = false;
				m_currentGroup.blocksRaycasts = false;
			}

			if(m_targetGroup == null)
			{
				m_targetGroup = Tabs[targetTab].tabGroup;
			}
			else
			{
				m_targetGroup.alpha = Mathf.Lerp(0,1,transitionCurve.Evaluate(m_time));
				m_targetGroup.interactable = true;
				m_targetGroup.blocksRaycasts = true;
			}
				

			if(m_time>1)
			{
				currentTab = targetTab;
				m_currentGroup = null;
				m_targetGroup = null;
				m_time = 0;
				m_IsActive = false;
			}
		}
	}

	public void SelectTab(int id)
	{
		if(id > Tabs.Count || m_IsActive || currentTab == id)
			return;
		targetTab = id;
		m_IsActive = true;
		tipTextObject.text = Tabs[id].tipText;

		MenuObject objNext = Tabs[id].tabGroup.GetComponent<MenuObject>();
		MenuObject objLast = Tabs[currentTab].tabGroup.GetComponent<MenuObject>();
		if(objNext != null)
		{
			objNext.Activate();
		}
		if(objLast != null)
		{
			objLast.DeActivate();
		}

		//NaviBar 
		/*foreach(Transform obj in naviBarRoot.transform)
			Destroy(obj);*/
		//NavibarAddObject(Tabs[id].name);
		
	}


	public void DisableMenu()
	{
		root.gameObject.SetActive(false);
	}
	public void EnableMenu()
	{
		root.gameObject.SetActive(true);
	}


	public void SelectNavibarObject(GameObject obj)
	{
		int id = obj.transform.GetSiblingIndex();
		Debug.Log(id);

		for(int i = naviBarRoot.transform.childCount - 1; i > id; i--)
		{
			Debug.Log(naviBarRoot.transform.GetChild(i).gameObject);
			Destroy(naviBarRoot.transform.GetChild(i).gameObject);
		}
	}
		
	public void NavibarAddObject(string value)
	{
		GameObject obj = Instantiate(naviObject, naviBarRoot.transform) as GameObject;
		obj.transform.localScale = Vector3.one;
		obj.transform.SetAsLastSibling();
		obj.GetComponentInChildren<Text>().text = value.ToUpper() + " / ";
		obj.GetComponent<NaviBarObject>().target = this;
	}


	public void AppQuit()
	{
		Application.Quit();
	}

}
