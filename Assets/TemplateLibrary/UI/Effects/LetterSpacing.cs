using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Letter Spacing", 15)]
	public class LetterSpacing : BaseMeshEffect
	{
		private const string SupportedTagRegexPattersn = @"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";
		[SerializeField]
		private bool useRichText;

		[SerializeField]
		private float m_spacing = 0f;

		protected LetterSpacing() { }

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			spacing = m_spacing;
			base.OnValidate();
		}
		#endif

		public float spacing
		{
			get { return m_spacing; }
			set
			{
				if (m_spacing == value) return;
				m_spacing = value;
				if (graphic != null) graphic.SetVerticesDirty();
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (!this.IsActive())
				return;

			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);

			ModifyVertices(list);

			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}   

		public void ModifyVertices(List<UIVertex> verts)
		{
			if (!IsActive())
				return;

			Text text = GetComponent<Text>();
			if (text == null)
			{
				Debug.LogWarning("LetterSpacing: Missing Text component");
				return;
			}

			string[] lines = text.text.Split('\n');
			Vector3 pos;
			float letterOffset = spacing * (float)text.fontSize / 100f;
			float alignmentFactor = 0;
			int glyphIdx = 0;

			bool isRichText = useRichText && text.supportRichText;
			IEnumerator matchedTagCollection = null; //when using RichText this will collect all tags (index, length, value)
			Match currentMatchedTag = null;

			switch (text.alignment)
			{
				case TextAnchor.LowerLeft:
				case TextAnchor.MiddleLeft:
				case TextAnchor.UpperLeft:
					alignmentFactor = 0f;
				break;

				case TextAnchor.LowerCenter:
				case TextAnchor.MiddleCenter:
				case TextAnchor.UpperCenter:
					alignmentFactor = 0.5f;
				break;

				case TextAnchor.LowerRight:
				case TextAnchor.MiddleRight:
				case TextAnchor.UpperRight:
					alignmentFactor = 1f;
				break;
			}

			for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
			{
				string line = lines[lineIdx];
				int lineLength = line.Length;

				if (isRichText)
				{
					matchedTagCollection = GetRegexMatchedTagCollection(line, out lineLength);
					currentMatchedTag = null;
					if (matchedTagCollection.MoveNext())
					{
						currentMatchedTag = (Match)matchedTagCollection.Current;
					}
				}

				float lineOffset = (lineLength - 1) * letterOffset * alignmentFactor;

				for (int charIdx = 0, charPositionIndex = 0; charIdx < line.Length; charIdx++, charPositionIndex++)
				{
					if (isRichText)
					{
						if (currentMatchedTag != null && currentMatchedTag.Index == charIdx)
						{
							charIdx = currentMatchedTag.Length + currentMatchedTag.Index - 1;
							glyphIdx = currentMatchedTag.Length + currentMatchedTag.Index - 1;
							charPositionIndex--;
							currentMatchedTag = null;
							if (matchedTagCollection.MoveNext())
							{
								currentMatchedTag = (Match)matchedTagCollection.Current;
							}
						}
					}

					int idx1 = glyphIdx * 6 + 0;
					int idx2 = glyphIdx * 6 + 1;
					int idx3 = glyphIdx * 6 + 2;
					int idx4 = glyphIdx * 6 + 3;
					int idx5 = glyphIdx * 6 + 4;
					int idx6 = glyphIdx * 6 + 5;

					// Check for truncated text (doesn't generate verts for all characters)
					if (idx6 > verts.Count - 1)
						return;

					UIVertex vert1 = verts[idx1];
					UIVertex vert2 = verts[idx2];
					UIVertex vert3 = verts[idx3];
					UIVertex vert4 = verts[idx4];
					UIVertex vert5 = verts[idx5];
					UIVertex vert6 = verts[idx6];

					pos = Vector3.right * (letterOffset * charPositionIndex - lineOffset);

					vert1.position += pos;
					vert2.position += pos;
					vert3.position += pos;
					vert4.position += pos;
					vert5.position += pos;
					vert6.position += pos;

					verts[idx1] = vert1;
					verts[idx2] = vert2;
					verts[idx3] = vert3;
					verts[idx4] = vert4;
					verts[idx5] = vert5;
					verts[idx6] = vert6;

					glyphIdx++;
				}

				// Offset for carriage return character that still generates verts
				glyphIdx++;
			}
		}

		private IEnumerator GetRegexMatchedTagCollection(string line, out int lineLengthWithoutTags)
		{
			MatchCollection matchedTagCollection = Regex.Matches(line,SupportedTagRegexPattersn);
			lineLengthWithoutTags = 0;
			int tagsLength = 0;

			if (matchedTagCollection.Count > 0)
			{
				foreach (Match matchedTag in matchedTagCollection)
				{
					tagsLength += matchedTag.Length;
				}
			}
			lineLengthWithoutTags = line.Length - tagsLength;
			return matchedTagCollection.GetEnumerator();
		}
	}
}