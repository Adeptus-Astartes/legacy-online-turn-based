using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VertBlend : BaseMeshEffect
{

	public AnimationCurve curve;
	public float time = 0f;
	public float multiplier;
	public float noise = 0.3f;

	public Color top = Color.white;
	public Color bottom = Color.black;

	public override void ModifyMesh (VertexHelper vh)
	{
		
		if (!IsActive() || vh.currentVertCount == 0)
			return;
		
		//List<UIVertex> _vertexList = new List<UIVertex>();
		//vh.GetUIVertexStream(_vertexList);

		//curve.keys[1].time = vh.currentVertCount/4;

		Mesh mesh = new Mesh();
		vh.FillMesh(mesh);

		List<UIVertex> _vertexList = new List<UIVertex>();
		vh.GetUIVertexStream(_vertexList);

		Color[] colors = new Color[_vertexList.Count];

		UIVertex vert = new UIVertex();

		int i = -1;

		for(int a = 1; a < 1 + vh.currentVertCount/4; a++)
		{
			for(int b = 1; b<5; b++)
			{
				i++;
				//Debug.Log(i);
				vh.PopulateUIVertex(ref vert, i);
				vert.color = Color.Lerp(bottom, top, vert.position.y);

				vert.position = vert.position + Vector3.up * curve.Evaluate(time + a) * multiplier;

				vh.SetUIVertex(vert, i);
			}

		}
		/*
		for (int i = 0; i < vh.currentVertCount; i++)
		{



			/if((i%4) != 0)
			{
				
				vh.PopulateUIVertex(ref vert, i);
				vert.color = Color.Lerp(Color.red, Color.green, vert.position.y);

				vert.position = vert.position + Vector3.up * curve.Evaluate(time) * multiplier;

				vh.SetUIVertex(vert, i);
			}
		}*/

	}



}