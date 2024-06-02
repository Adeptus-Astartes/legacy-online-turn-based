using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/ModifiedShadow", 15)]
    public class ModifiedShadow : Shadow
    {
        [SerializeField]
        private Color32 EndColor = Color.black;
    
        [SerializeField]
        [Range(-1.5f, 1.5f)]
        public float Offset = 0f;

        protected new void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;
			if (verts == null || verts.Count == 0)
			{
				return;
			}
            // The capacity calculation of the original version seems wrong.
            var neededCpacity = verts.Count + (end - start);
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            float fBottomY = verts[0].position.y;
            float fTopY = verts[0].position.y;

            for (int i = verts.Count - 1; i >= 1; --i)
            {
                var fYPos = verts[i].position.y;
                if (fYPos > fTopY)
                    fTopY = fYPos;
                else if (fYPos < fBottomY)
                    fBottomY = fYPos;
            }

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;

                v.x += x;
                v.y += y;
                vt.position = v;
                var newColor = color;
                float fUIElementHeight = 1f / (fTopY - fBottomY);

                newColor = Color32.Lerp(EndColor, effectColor, (v.y - fBottomY) * fUIElementHeight - Offset);

                if (useGraphicAlpha)
                    newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

#if UNITY_5_2 && !UNITY_5_2_1pX
    public override void ModifyMesh(Mesh mesh)
    {
        if (!this.IsActive())
            return;

        using (var vh = new VertexHelper(mesh))
        {
            ModifyMesh(vh);
            vh.FillMesh(mesh);
        }
    }
#endif

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
#if UNITY_5_2_1pX || UNITY_5_3
        public override void ModifyMesh(VertexHelper vh)
#else
    public void ModifyMesh(VertexHelper vh)
#endif
        {
            if (!this.IsActive())
                return;

            List<UIVertex> list = new List<UIVertex>();
            vh.GetUIVertexStream(list);

            ModifyVertices(list);

#if UNITY_5_2_1pX || UNITY_5_3
            vh.Clear();
#endif
            vh.AddUIVertexTriangleStream(list);
        }

        public virtual void ModifyVertices(List<UIVertex> verts)
        {
        }
#endif
    }
}