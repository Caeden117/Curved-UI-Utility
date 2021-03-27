using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace CurvedUIUtility
{
	public class CurvedImage : Image
	{
		protected static readonly Vector2 kVec2Zero = new Vector2(0f, 0f);
		protected static readonly Vector3 kVec3Zero = new Vector3(0f, 0f, 0f);
		protected static readonly Vector4 kVec4Zero = new Vector4(0f, 0f, 0f, 0f);
		protected static readonly Vector2[] s_VertScratch = new Vector2[4];
		protected static readonly Vector2[] s_UVScratch = new Vector2[4];
		protected static readonly Vector2[] s_UV1Scratch = new Vector2[4];
		protected static readonly Color[] s_ColorScratch = new Color[4];
		protected static readonly Vector3[] s_Xy = new Vector3[4];
		protected static readonly Vector3[] s_Uv = new Vector3[4];

		protected readonly CurvedUIHelper curvedUIHelper = new CurvedUIHelper();

		private static readonly FieldInfo indicesList = typeof(VertexHelper).GetField("m_Indices", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo positionsList = typeof(VertexHelper).GetField("m_Positions", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo colorsList = typeof(VertexHelper).GetField("m_Colors", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo uv0List = typeof(VertexHelper).GetField("m_Uv0S", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo uv1List = typeof(VertexHelper).GetField("m_Uv1S", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo uv2List = typeof(VertexHelper).GetField("m_Uv2S", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo uv3List = typeof(VertexHelper).GetField("m_Uv3S", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo normalList = typeof(VertexHelper).GetField("m_Normals", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly FieldInfo tangentList = typeof(VertexHelper).GetField("m_Tangents", BindingFlags.Instance | BindingFlags.NonPublic);

		private Vector3 cachedPosition = Vector3.zero;
		private Vector2 cachedUv = Vector2.zero;

		protected override void OnEnable()
		{
			base.OnEnable();
			curvedUIHelper.Reset();
		}

        protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (overrideSprite == null)
			{
				base.OnPopulateMesh(toFill);
				return;
			}

			switch (type)
			{
				default:
					GenerateSimpleSprite(toFill, preserveAspect);
					break;
				case Type.Sliced:
					GenerateSlicedSprite(toFill);
					break;
				case Type.Tiled:
					GenerateTiledSprite(toFill);
					break;
				case Type.Filled:
					GenerateFilledSprite(toFill, preserveAspect);
					break;
			}
		}

		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			vh.Clear();
			Vector4 drawingDimensions = GetDrawingDimensions(lPreserveAspect);
			Vector4 vector = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

			s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
			s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
			s_Uv[0] = new Vector2(vector.x, vector.y);
			s_Uv[2] = new Vector2(vector.z, vector.w);

			AddQuad(vh, s_Xy[0], s_Xy[2], color, s_Uv[0], s_Uv[2], Vector2.zero, Vector2.zero, 0f);
		}

		private void GenerateSlicedSprite(VertexHelper toFill)
		{
			if (!hasBorder)
			{
				GenerateSimpleSprite(toFill, false);
				return;
			}
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector4 vector4;
			if (overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(overrideSprite);
				vector2 = DataUtility.GetInnerUV(overrideSprite);
				vector3 = DataUtility.GetPadding(overrideSprite);
				vector4 = overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector4.zero;
			}
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			vector4 = GetAdjustedBorders(vector4 / pixelsPerUnit, pixelAdjustedRect);
			vector3 /= pixelsPerUnit;
            s_VertScratch[0] = new Vector2(vector3.x, vector3.y);
            s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
            s_VertScratch[1].x = vector4.x;
            s_VertScratch[1].y = vector4.y;
            s_VertScratch[2].x = pixelAdjustedRect.width - vector4.z;
            s_VertScratch[2].y = pixelAdjustedRect.height - vector4.w;
			Vector2 scale = new Vector2(1f / pixelAdjustedRect.width, 1f / pixelAdjustedRect.height);
			for (int i = 0; i < 4; i++)
			{
                s_UV1Scratch[i] = s_VertScratch[i];
                s_UV1Scratch[i].Scale(scale);
			}
			for (int j = 0; j < 4; j++)
			{
				Vector2[] array = s_VertScratch;
				int num = j;
				array[num].x = array[num].x + pixelAdjustedRect.x;
				Vector2[] array2 = s_VertScratch;
				int num2 = j;
				array2[num2].y = array2[num2].y + pixelAdjustedRect.y;
			}
            s_UVScratch[0] = new Vector2(vector.x, vector.y);
            s_UVScratch[1] = new Vector2(vector2.x, vector2.y);
            s_UVScratch[2] = new Vector2(vector2.z, vector2.w);
            s_UVScratch[3] = new Vector2(vector.z, vector.w);
			toFill.Clear();
			float x2 = transform.localScale.x;

			for (int k = 0; k < 3; k++)
			{
				int num5 = k + 1;
				for (int l = 0; l < 3; l++)
				{
					if (fillCenter || k != 1 || l != 1)
					{
						int num6 = l + 1;
						AddQuad(toFill, new Vector2(s_VertScratch[k].x, s_VertScratch[l].y), new Vector2(s_VertScratch[num5].x, s_VertScratch[num6].y), color, new Vector2(s_UVScratch[k].x, s_UVScratch[l].y), new Vector2(s_UVScratch[num5].x, s_UVScratch[num6].y), new Vector2(s_UV1Scratch[k].x, s_UV1Scratch[l].y), new Vector2(s_UV1Scratch[num5].x, s_UV1Scratch[num6].y), x2);
					}
				}
			}
		}

		private void GenerateTiledSprite(VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector2 vector4;
			if (overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(overrideSprite);
				vector2 = DataUtility.GetInnerUV(overrideSprite);
				vector3 = overrideSprite.border;
				vector4 = overrideSprite.rect.size;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / multipliedPixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / multipliedPixelsPerUnit;
			vector3 = GetAdjustedBorders(vector3 / multipliedPixelsPerUnit, pixelAdjustedRect);
			Vector2 vector5 = new Vector2(vector2.x, vector2.y);
			Vector2 vector6 = new Vector2(vector2.z, vector2.w);
			float x = vector3.x;
			float num3 = pixelAdjustedRect.width - vector3.z;
			float y = vector3.y;
			float num4 = pixelAdjustedRect.height - vector3.w;
			toFill.Clear();
			Vector2 vector7 = vector6;
			if (num <= 0f)
			{
				num = num3 - x;
			}
			if (num2 <= 0f)
			{
				num2 = num4 - y;
			}
			if (overrideSprite != null && (hasBorder || overrideSprite.packed || overrideSprite.texture.wrapMode != TextureWrapMode.Repeat))
			{
				long num5;
				long num6;
				if (fillCenter)
				{
					num5 = (long)Math.Ceiling((num3 - x) / num);
					num6 = (long)Math.Ceiling((num4 - y) / num2);
					double num7;
					if (hasBorder)
					{
						num7 = (num5 + 2.0) * (num6 + 2.0) * 4.0;
					}
					else
					{
						num7 = num5 * num6 * 4.0;
					}
					if (num7 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", this);
						double num8 = 16250.0;
						double num9;
						if (hasBorder)
						{
							num9 = (num5 + 2.0) / (num6 + 2.0);
						}
						else
						{
							num9 = num5 / (double)num6;
						}
						double num10 = Math.Sqrt(num8 / num9);
						double num11 = num10 * num9;
						if (hasBorder)
						{
							num10 -= 2.0;
							num11 -= 2.0;
						}
						num5 = (long)Math.Floor(num10);
						num6 = (long)Math.Floor(num11);
						num = (num3 - x) / num5;
						num2 = (num4 - y) / num6;
					}
				}
				else if (hasBorder)
				{
					num5 = (long)Math.Ceiling((num3 - x) / num);
					num6 = (long)Math.Ceiling((num4 - y) / num2);
					if ((num6 + num5 + 2.0) * 2.0 * 4.0 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", this);
						double num12 = 16250.0;
						double num13 = num5 / (double)num6;
						double num14 = (num12 - 4.0) / (2.0 * (1.0 + num13));
						double d = num14 * num13;
						num5 = (long)Math.Floor(num14);
						num6 = (long)Math.Floor(d);
						num = (num3 - x) / num5;
						num2 = (num4 - y) / num6;
					}
				}
				else
				{
					num5 = (num6 = 0L);
				}
				if (fillCenter)
				{
					for (long num15 = 0L; num15 < num6; num15 += 1L)
					{
						float num16 = y + num15 * num2;
						float num17 = y + (num15 + 1L) * num2;
						if (num17 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num16) / (num17 - num16);
							num17 = num4;
						}
						vector7.x = vector6.x;
						for (long num18 = 0L; num18 < num5; num18 += 1L)
						{
							float num19 = x + num18 * num;
							float num20 = x + (num18 + 1L) * num;
							if (num20 > num3)
							{
								vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num19) / (num20 - num19);
								num20 = num3;
							}
                            AddQuad(toFill, new Vector2(num19, num16) + pixelAdjustedRect.position, new Vector2(num20, num17) + pixelAdjustedRect.position, color, vector5, vector7);
						}
					}
				}
				if (hasBorder)
				{
					vector7 = vector6;
					for (long num21 = 0L; num21 < num6; num21 += 1L)
					{
						float num22 = y + num21 * num2;
						float num23 = y + (num21 + 1L) * num2;
						if (num23 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num22) / (num23 - num22);
							num23 = num4;
						}
                        AddQuad(toFill, new Vector2(0f, num22) + pixelAdjustedRect.position, new Vector2(x, num23) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector5.y), new Vector2(vector5.x, vector7.y));
                        AddQuad(toFill, new Vector2(num3, num22) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num23) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector5.y), new Vector2(vector.z, vector7.y));
					}
					vector7 = vector6;
					for (long num24 = 0L; num24 < num5; num24 += 1L)
					{
						float num25 = x + num24 * num;
						float num26 = x + (num24 + 1L) * num;
						if (num26 > num3)
						{
							vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num25) / (num26 - num25);
							num26 = num3;
						}
                        AddQuad(toFill, new Vector2(num25, 0f) + pixelAdjustedRect.position, new Vector2(num26, y) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector.y), new Vector2(vector7.x, vector5.y));
                        AddQuad(toFill, new Vector2(num25, num4) + pixelAdjustedRect.position, new Vector2(num26, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector6.y), new Vector2(vector7.x, vector.w));
					}
                    AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector.y), new Vector2(vector5.x, vector5.y));
                    AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector.y), new Vector2(vector.z, vector5.y));
                    AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector6.y), new Vector2(vector5.x, vector.w));
                    AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector6.y), new Vector2(vector.z, vector.w));
					return;
				}
			}
			else
			{
				Vector2 b = new Vector2((num3 - x) / num, (num4 - y) / num2);
				if (fillCenter)
				{
                    AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num3, num4) + pixelAdjustedRect.position, color, Vector2.Scale(vector5, b), Vector2.Scale(vector6, b));
				}
			}
		}

		private void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			for (int i = 0; i < 4; i++)
			{
				vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
			}
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private int GetNewVertex4(Dictionary<uint, int> newVectices, List<Vector3> vertices, List<Vector2> uvs, int i1, int i2)
		{
			int newIndex = vertices.Count;
			uint t1 = ((uint)i1 << 16) | (uint)i2;
			uint t2 = ((uint)i2 << 16) | (uint)i1;
			if (newVectices.ContainsKey(t2))
				return newVectices[t2];
			if (newVectices.ContainsKey(t1))
				return newVectices[t1];

			newVectices.Add(t1, newIndex);

			vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
			uvs.Add((uvs[i1] + uvs[i2]) * 0.5f);

			return newIndex;
		}


		private void Subdivide4(VertexHelper vertexHelper)
		{
			var newVectices = new Dictionary<uint, int>();
			var indices = new List<int>();

			var vertices = positionsList.GetValue(vertexHelper) as List<Vector3>;
			var uvs = uv0List.GetValue(vertexHelper) as List<Vector2>;

			var triangles = indicesList.GetValue(vertexHelper) as List<int>;
			for (int i = 0; i < triangles.Count; i += 3)
			{
				int i1 = triangles[i + 0];
				int i2 = triangles[i + 1];
				int i3 = triangles[i + 2];

				int a = GetNewVertex4(newVectices, vertices, uvs, i1, i2);
				int b = GetNewVertex4(newVectices, vertices, uvs, i2, i3);
				int c = GetNewVertex4(newVectices, vertices, uvs, i3, i1);
				indices.Add(i1); indices.Add(a); indices.Add(c);
				indices.Add(i2); indices.Add(b); indices.Add(a);
				indices.Add(i3); indices.Add(c); indices.Add(b);
				indices.Add(a); indices.Add(b); indices.Add(c); // center triangle
			}

			positionsList.SetValue(vertexHelper, vertices);
			indicesList.SetValue(vertexHelper, indices);
			colorsList.SetValue(vertexHelper, Enumerable.Repeat((Color32)color, uvs.Count).ToList());
			uv0List.SetValue(vertexHelper, uvs);
			uv1List.SetValue(vertexHelper, Enumerable.Repeat(kVec2Zero, uvs.Count).ToList());
			uv2List.SetValue(vertexHelper, Enumerable.Repeat(kVec2Zero, uvs.Count).ToList());
			uv3List.SetValue(vertexHelper, Enumerable.Repeat(kVec2Zero, uvs.Count).ToList());
			normalList.SetValue(vertexHelper, Enumerable.Repeat(kVec3Zero, uvs.Count).ToList());
			tangentList.SetValue(vertexHelper, Enumerable.Repeat(kVec4Zero, uvs.Count).ToList());
		}

		private void AddVert(VertexHelper vertexHelper, float posX, float posY, float uvX, float uvY)
        {
			cachedPosition.Set(posX, posY, 0);
			cachedUv.Set(uvX, uvY);
			vertexHelper.AddVert(cachedPosition, color, cachedUv, kVec2Zero, kVec2Zero, kVec2Zero, kVec3Zero, kVec4Zero);
        }

		private void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			AddVert(vertexHelper, posMin.x, posMin.y, uvMin.x, uvMin.y);
			AddVert(vertexHelper, posMin.x, posMax.y, uvMin.x, uvMax.y);
			AddVert(vertexHelper, posMax.x, posMax.y, uvMax.x, uvMax.y);
			AddVert(vertexHelper, posMax.x, posMin.y, uvMax.x, uvMin.y);
			
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uv0Min, Vector2 uv0Max, Vector2 uv1Min, Vector2 uv1Max, float elementWidthScale)
		{
			int horizontalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(posMin.x - posMax.x));
			int verticalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(posMin.y - posMax.y));
			
			int currentVertCount = vertexHelper.currentVertCount;

			for (int j = 0; j < verticalElements; j++)
			{
				float currrentVertProgress = j / (float)verticalElements;
				float nextVertProgress = (j + 1) / (float)verticalElements;

				float currentPosY = Mathf.LerpUnclamped(posMin.y, posMax.y, currrentVertProgress);
				float currentUvY = Mathf.LerpUnclamped(uv0Min.y, uv0Max.y, currrentVertProgress);

				float nextPosY = Mathf.LerpUnclamped(posMin.y, posMax.y, nextVertProgress);
				float nextUvY = Mathf.LerpUnclamped(uv0Min.y, uv0Max.y, nextVertProgress);
				for (int i = 0; i < horizontalElements + 1; i++)
				{
					float horizProgress = i / (float)horizontalElements;
					float posX = Mathf.LerpUnclamped(posMin.x, posMax.x, horizProgress);
					float uvX = Mathf.LerpUnclamped(uv0Min.x, uv0Max.x, horizProgress);
					AddVert(vertexHelper, posX, currentPosY, uvX, currentUvY);
					AddVert(vertexHelper, posX, nextPosY, uvX, nextUvY);
				}
			}

			for (int j = 0; j < verticalElements; j++)
			{
				for (int k = 0; k < horizontalElements; k++)
				{
					int num4 = (j * (horizontalElements + 1) * 2) + (k * 2) + currentVertCount;
					vertexHelper.AddTriangle(num4, 1 + num4, 2 + num4);
					vertexHelper.AddTriangle(2 + num4, 3 + num4, 1 + num4);
				}
			}
		}
		
		private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
		{
			Rect rect = rectTransform.rect;
			for (int i = 0; i <= 1; i++)
			{
				if (rect.size[i] != 0f)
				{
					float num = adjustedRect.size[i] / rect.size[i];
					ref Vector4 ptr = ref border;
					int index = i;
					ptr[index] *= num;
					ptr = ref border;
					index = i + 2;
					ptr[index] *= num;
				}
				float num2 = border[i] + border[i + 2];
				if (adjustedRect.size[i] < num2 && num2 != 0f)
				{
					float num = adjustedRect.size[i] / num2;
					ref Vector4 ptr = ref border;
					int index = i;
					ptr[index] *= num;
					ptr = ref border;
					index = i + 2;
					ptr[index] *= num;
				}
			}
			return border;
		}

		private void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
		{
			toFill.Clear();
			if (fillAmount < 0.001f)
			{
				return;
			}
			else if (fillAmount >= 1f)
            {
				GenerateSimpleSprite(toFill, preserveAspect);
				return;
            }
			Vector4 drawingDimensions = GetDrawingDimensions(preserveAspect);
			Vector4 obj = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = color;
			Vector4 obj2 = obj;
			float num = obj2.x;
			float num2 = obj2.y;
			float num3 = obj2.z;
			float num4 = obj2.w;
			if (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
			{
				if (fillMethod == FillMethod.Horizontal)
				{
					float num5 = (num3 - num) * fillAmount;
					if (fillOrigin == 1)
					{
						drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * fillAmount;
						num = num3 - num5;
					}
					else
					{
						drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * fillAmount;
						num3 = num + num5;
					}
				}
				else if (fillMethod == FillMethod.Vertical)
				{
					float num6 = (num4 - num2) * fillAmount;
					if (fillOrigin == 1)
					{
						drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * fillAmount;
						num2 = num4 - num6;
					}
					else
					{
						drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * fillAmount;
						num4 = num2 + num6;
					}
				}
			}
            s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
            s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
            s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
            s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
            s_Uv[0] = new Vector2(num, num2);
            s_Uv[1] = new Vector2(num, num4);
            s_Uv[2] = new Vector2(num3, num4);
            s_Uv[3] = new Vector2(num3, num2);
			if (fillAmount < 1f && fillMethod != FillMethod.Horizontal && fillMethod != FillMethod.Vertical)
			{
				int elements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Min(Mathf.Abs(s_Xy[0].x - s_Xy[2].x), Mathf.Abs(s_Xy[0].y - s_Xy[2].y))) / 3;

				if (fillMethod == FillMethod.Radial90)
				{
					if (RadialCut(s_Xy, s_Uv, fillAmount, fillClockwise, fillOrigin))
					{
                        AddQuad(toFill, s_Xy, color, s_Uv);
						for (int i = 0; i < elements; i++)
						{
							Subdivide4(toFill);
						}
						return;
					}
				}
				else
				{
					if (fillMethod == FillMethod.Radial180)
					{
						for (int i = 0; i < 2; i++)
						{
							int num7 = (fillOrigin > 1) ? 1 : 0;
							float t;
							float t2;
							float t3;
							float t4;
							if (fillOrigin == 0 || fillOrigin == 2)
							{
								t = 0f;
								t2 = 1f;
								if (i == num7)
								{
									t3 = 0f;
									t4 = 0.5f;
								}
								else
								{
									t3 = 0.5f;
									t4 = 1f;
								}
							}
							else
							{
								t3 = 0f;
								t4 = 1f;
								if (i == num7)
								{
									t = 0.5f;
									t2 = 1f;
								}
								else
								{
									t = 0f;
									t2 = 0.5f;
								}
							}
                            s_Xy[0].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t3);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t4);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t);
                            s_Xy[1].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t2);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.LerpUnclamped(num, num3, t3);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.LerpUnclamped(num, num3, t4);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.LerpUnclamped(num2, num4, t);
                            s_Uv[1].y = Mathf.LerpUnclamped(num2, num4, t2);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
							float value = fillClockwise ? (fillAmount * 2f - i) : (fillAmount * 2f - (1 - i));
							if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value), fillClockwise, (i + fillOrigin + 3) % 4))
							{
                                AddQuad(toFill, s_Xy, color, s_Uv);
							}
						}
						for (int i = 0; i < elements; i++)
						{
							Subdivide4(toFill);
						}
						return;
					}
					if (fillMethod == FillMethod.Radial360)
					{
						for (int j = 0; j < 4; j++)
						{
							float t5;
							float t6;
							if (j < 2)
							{
								t5 = 0f;
								t6 = 0.5f;
							}
							else
							{
								t5 = 0.5f;
								t6 = 1f;
							}
							float t7;
							float t8;
							if (j == 0 || j == 3)
							{
								t7 = 0f;
								t8 = 0.5f;
							}
							else
							{
								t7 = 0.5f;
								t8 = 1f;
							}
                            s_Xy[0].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t5);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t6);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t7);
                            s_Xy[1].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t8);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.LerpUnclamped(num, num3, t5);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.LerpUnclamped(num, num3, t6);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.LerpUnclamped(num2, num4, t7);
                            s_Uv[1].y = Mathf.LerpUnclamped(num2, num4, t8);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
							float value2 = fillClockwise ? (fillAmount * 4f - (j + fillOrigin) % 4) : (fillAmount * 4f - (3 - (j + fillOrigin) % 4));
							if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value2), fillClockwise, (j + 2) % 4))
							{
                                AddQuad(toFill, s_Xy, color, s_Uv);
							}
						}
						for (int i = 0; i < elements; i++)
						{
							Subdivide4(toFill);
						}
						return;
					}
				}
            }
            else
            {
				AddQuad(toFill, s_Xy[0], s_Xy[2], color, s_Uv[0], s_Uv[2], Vector2.zero, Vector2.zero, 0f);
				return;
			}
		}

		private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
		{
			if (fill < 0.001f)
			{
				return false;
			}
			if ((corner & 1) == 1)
			{
				invert = !invert;
			}
			if (!invert && fill > 0.999f)
			{
				return true;
			}
			float num = Mathf.Clamp01(fill);
			if (invert)
			{
				num = 1f - num;
			}
			num *= 1.57079637f;
			float cos = Mathf.Cos(num);
			float sin = Mathf.Sin(num);
            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
			return true;
		}

		private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
		{
			int num = (corner + 1) % 4;
			int num2 = (corner + 2) % 4;
			int num3 = (corner + 3) % 4;
			if ((corner & 1) == 1)
			{
				if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					if (invert)
					{
						xy[num].x = Mathf.LerpUnclamped(xy[corner].x, xy[num2].x, cos);
						xy[num2].x = xy[num].x;
					}
				}
				else if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					if (!invert)
					{
						xy[num2].y = Mathf.LerpUnclamped(xy[corner].y, xy[num2].y, sin);
						xy[num3].y = xy[num2].y;
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				if (!invert)
				{
					xy[num3].x = Mathf.LerpUnclamped(xy[corner].x, xy[num2].x, cos);
					return;
				}
				xy[num].y = Mathf.LerpUnclamped(xy[corner].y, xy[num2].y, sin);
				return;
			}
			else
			{
				if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					if (!invert)
					{
						xy[num].y = Mathf.LerpUnclamped(xy[corner].y, xy[num2].y, sin);
						xy[num2].y = xy[num].y;
					}
				}
				else if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					if (invert)
					{
						xy[num2].x = Mathf.LerpUnclamped(xy[corner].x, xy[num2].x, cos);
						xy[num3].x = xy[num2].x;
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				if (invert)
				{
					xy[num3].y = Mathf.LerpUnclamped(xy[corner].y, xy[num2].y, sin);
					return;
				}
				xy[num].x = Mathf.LerpUnclamped(xy[corner].x, xy[num2].x, cos);
				return;
			}
		}

		public virtual Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 vector = (overrideSprite == null) ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
			Vector2 vector2 = (overrideSprite == null) ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / num, vector.y / num2, (num - vector.z) / num, (num2 - vector.w) / num2);
			if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
			{
				float num3 = vector2.x / vector2.y;
				float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
				if (num3 > num4)
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * rectTransform.pivot.x;
				}
			}
			vector3 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
			return vector3;
		}
	}
}