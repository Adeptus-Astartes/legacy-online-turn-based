using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class LayoutElementControler : MonoBehaviour 
{
	private RectTransform m_transform;
	private HorizontalOrVerticalLayoutGroup layoutGroup;
	public float AdditionalVerticalOffset;
	public float AdditionalHorizontalOffset;
	// Use this for initialization
	void Start () 
	{
		m_transform = this.GetComponent<RectTransform>();
		layoutGroup = this.GetComponent<HorizontalOrVerticalLayoutGroup>();
		//if(this.GetComponent<HorizontalLayoutGroup>() != null)
		//{
			Debug.Log(m_transform.GetSize());
			foreach(Transform child in transform)
			{
				float height = m_transform.GetHeight() - layoutGroup.padding.vertical - AdditionalVerticalOffset;
				child.GetComponent<LayoutElement>().preferredHeight = height;
				child.GetComponent<LayoutElement>().preferredWidth = (height/3)*2;
			}
		//}
		/*if(this.GetComponent<VerticalLayoutGroup>() != null)
		{
			Debug.Log(m_transform.GetSize());
			foreach(Transform child in transform)
			{
				float height = m_transform.GetHeight() - layoutGroup.padding.vertical - AdditionalVerticalOffset;
				child.GetComponent<LayoutElement>().preferredHeight = height;
				child.GetComponent<LayoutElement>().preferredWidth = (height/3)*2;
			}
		}*/
	}
}
