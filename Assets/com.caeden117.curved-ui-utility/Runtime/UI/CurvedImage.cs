using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace CurvedUIUtility
{
	public class CurvedImage : Image
	{
		private Vector3 cachedPosition = Vector3.zero;
		private Vector2 cachedUv = Vector2.zero;

		// Token: 0x060002D6 RID: 726 RVA: 0x0000D041 File Offset: 0x0000B241
		protected override void OnEnable()
		{
			base.OnEnable();
			this.curvedUIHelper.Reset();
		}

        // Token: 0x060002D7 RID: 727 RVA: 0x0000D054 File Offset: 0x0000B254
        protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (base.overrideSprite == null)
			{
				base.OnPopulateMesh(toFill);
				return;
			}

			switch (base.type)
			{
				default:
					this.GenerateSimpleSprite(toFill, base.preserveAspect);
					break;
				case CurvedImage.Type.Sliced:
					this.GenerateSlicedSprite(toFill);
					break;
				case CurvedImage.Type.Tiled:
					this.GenerateTiledSprite(toFill);
					break;
				case CurvedImage.Type.Filled:
					this.GenerateFilledSprite(toFill, base.preserveAspect);
					break;
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006E0C File Offset: 0x0000500C
		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			vh.Clear();
			Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
			Vector4 vector = (base.overrideSprite != null) ? DataUtility.GetOuterUV(base.overrideSprite) : Vector4.zero;

			int horizontalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(drawingDimensions.z - drawingDimensions.x));
			int verticalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(drawingDimensions.w - drawingDimensions.y));

			var color32 = (Color32)color;

			for (int j = 0; j < verticalElements; j++)
			{
				float currrentVertProgress = j / (float)verticalElements;
				float nextVertProgress = (j + 1) / (float)verticalElements;

				float currentPosY = Mathf.LerpUnclamped(drawingDimensions.w, drawingDimensions.y, currrentVertProgress);
				float currentUvY = Mathf.LerpUnclamped(vector.w, vector.y, currrentVertProgress);

				float nextPosY = Mathf.LerpUnclamped(drawingDimensions.w, drawingDimensions.y, nextVertProgress);
				float nextUvY = Mathf.LerpUnclamped(vector.w, vector.y, nextVertProgress);
				for (int i = 0; i < horizontalElements + 1; i++)
				{
					float horizProgress = i / (float)horizontalElements;
					float posX = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, horizProgress);
					float uvX = Mathf.LerpUnclamped(vector.x, vector.z, horizProgress);
					cachedPosition.Set(posX, currentPosY, 0);
					cachedUv.Set(uvX, currentUvY);
					vh.AddVert(cachedPosition, color32, cachedUv, kVec2Zero, kVec2Zero, kVec2Zero, kVec3Zero, kVec4Zero);
					cachedPosition.Set(posX, nextPosY, 0);
					cachedUv.Set(uvX, nextUvY);
					vh.AddVert(cachedPosition, color32, cachedUv, kVec2Zero, kVec2Zero, kVec2Zero, kVec3Zero, kVec4Zero);
				}
			}

			for (int j = 0; j < verticalElements; j++)
			{
				for (int k = 0; k < horizontalElements; k++)
				{
					int num4 = (j * (horizontalElements + 1) * 2) + (k * 2);
					vh.AddTriangle(num4, 1 + num4, 2 + num4);
					vh.AddTriangle(2 + num4, 3 + num4, 1 + num4);
				}
			}
		}



		// Token: 0x06000133 RID: 307 RVA: 0x000070C8 File Offset: 0x000052C8
		private void GenerateSlicedSprite(VertexHelper toFill)
		{
			if (!base.hasBorder)
			{
				this.GenerateSimpleSprite(toFill, false);
				return;
			}
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector4 vector4;
			if (base.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(base.overrideSprite);
				vector2 = DataUtility.GetInnerUV(base.overrideSprite);
				vector3 = DataUtility.GetPadding(base.overrideSprite);
				vector4 = base.overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector4.zero;
			}
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			vector4 = GetAdjustedBorders(vector4 / base.pixelsPerUnit, pixelAdjustedRect);
			vector3 /= base.pixelsPerUnit;
			CurvedImage.s_VertScratch[0] = new Vector2(vector3.x, vector3.y);
			CurvedImage.s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
			CurvedImage.s_VertScratch[1].x = vector4.x;
			CurvedImage.s_VertScratch[1].y = vector4.y;
			CurvedImage.s_VertScratch[2].x = pixelAdjustedRect.width - vector4.z;
			CurvedImage.s_VertScratch[2].y = pixelAdjustedRect.height - vector4.w;
			Vector2 scale = new Vector2(1f / pixelAdjustedRect.width, 1f / pixelAdjustedRect.height);
			for (int i = 0; i < 4; i++)
			{
				CurvedImage.s_UV1Scratch[i] = CurvedImage.s_VertScratch[i];
				CurvedImage.s_UV1Scratch[i].Scale(scale);
			}
			for (int j = 0; j < 4; j++)
			{
				Vector2[] array = CurvedImage.s_VertScratch;
				int num = j;
				array[num].x = array[num].x + pixelAdjustedRect.x;
				Vector2[] array2 = CurvedImage.s_VertScratch;
				int num2 = j;
				array2[num2].y = array2[num2].y + pixelAdjustedRect.y;
			}
			CurvedImage.s_UVScratch[0] = new Vector2(vector.x, vector.y);
			CurvedImage.s_UVScratch[1] = new Vector2(vector2.x, vector2.y);
			CurvedImage.s_UVScratch[2] = new Vector2(vector2.z, vector2.w);
			CurvedImage.s_UVScratch[3] = new Vector2(vector.z, vector.w);
			toFill.Clear();
			float x2 = base.transform.localScale.x;

			for (int k = 0; k < 3; k++)
			{
				int num5 = k + 1;
				for (int l = 0; l < 3; l++)
				{
					if (base.fillCenter || k != 1 || l != 1)
					{
						int num6 = l + 1;
						AddQuad(toFill, new Vector2(CurvedImage.s_VertScratch[k].x, CurvedImage.s_VertScratch[l].y), new Vector2(CurvedImage.s_VertScratch[num5].x, CurvedImage.s_VertScratch[num6].y), this.color, new Vector2(CurvedImage.s_UVScratch[k].x, CurvedImage.s_UVScratch[l].y), new Vector2(CurvedImage.s_UVScratch[num5].x, CurvedImage.s_UVScratch[num6].y), new Vector2(CurvedImage.s_UV1Scratch[k].x, CurvedImage.s_UV1Scratch[l].y), new Vector2(CurvedImage.s_UV1Scratch[num5].x, CurvedImage.s_UV1Scratch[num6].y), x2);
					}
				}
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000073D8 File Offset: 0x000055D8
		private void GenerateTiledSprite(VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector2 vector4;
			if (this.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(this.overrideSprite);
				vector2 = DataUtility.GetInnerUV(this.overrideSprite);
				vector3 = this.overrideSprite.border;
				vector4 = this.overrideSprite.rect.size;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / this.multipliedPixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / this.multipliedPixelsPerUnit;
			vector3 = this.GetAdjustedBorders(vector3 / this.multipliedPixelsPerUnit, pixelAdjustedRect);
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
			if (this.overrideSprite != null && (this.hasBorder || this.overrideSprite.packed || this.overrideSprite.texture.wrapMode != TextureWrapMode.Repeat))
			{
				long num5;
				long num6;
				if (this.fillCenter)
				{
					num5 = (long)Math.Ceiling((double)((num3 - x) / num));
					num6 = (long)Math.Ceiling((double)((num4 - y) / num2));
					double num7;
					if (this.hasBorder)
					{
						num7 = ((double)num5 + 2.0) * ((double)num6 + 2.0) * 4.0;
					}
					else
					{
						num7 = (double)(num5 * num6) * 4.0;
					}
					if (num7 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", this);
						double num8 = 16250.0;
						double num9;
						if (this.hasBorder)
						{
							num9 = ((double)num5 + 2.0) / ((double)num6 + 2.0);
						}
						else
						{
							num9 = (double)num5 / (double)num6;
						}
						double num10 = Math.Sqrt(num8 / num9);
						double num11 = num10 * num9;
						if (this.hasBorder)
						{
							num10 -= 2.0;
							num11 -= 2.0;
						}
						num5 = (long)Math.Floor(num10);
						num6 = (long)Math.Floor(num11);
						num = (num3 - x) / (float)num5;
						num2 = (num4 - y) / (float)num6;
					}
				}
				else if (this.hasBorder)
				{
					num5 = (long)Math.Ceiling((double)((num3 - x) / num));
					num6 = (long)Math.Ceiling((double)((num4 - y) / num2));
					if (((double)(num6 + num5) + 2.0) * 2.0 * 4.0 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", this);
						double num12 = 16250.0;
						double num13 = (double)num5 / (double)num6;
						double num14 = (num12 - 4.0) / (2.0 * (1.0 + num13));
						double d = num14 * num13;
						num5 = (long)Math.Floor(num14);
						num6 = (long)Math.Floor(d);
						num = (num3 - x) / (float)num5;
						num2 = (num4 - y) / (float)num6;
					}
				}
				else
				{
					num5 = (num6 = 0L);
				}
				if (this.fillCenter)
				{
					for (long num15 = 0L; num15 < num6; num15 += 1L)
					{
						float num16 = y + (float)num15 * num2;
						float num17 = y + (float)(num15 + 1L) * num2;
						if (num17 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num16) / (num17 - num16);
							num17 = num4;
						}
						vector7.x = vector6.x;
						for (long num18 = 0L; num18 < num5; num18 += 1L)
						{
							float num19 = x + (float)num18 * num;
							float num20 = x + (float)(num18 + 1L) * num;
							if (num20 > num3)
							{
								vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num19) / (num20 - num19);
								num20 = num3;
							}
							CurvedImage.AddQuad(toFill, new Vector2(num19, num16) + pixelAdjustedRect.position, new Vector2(num20, num17) + pixelAdjustedRect.position, this.color, vector5, vector7);
						}
					}
				}
				if (this.hasBorder)
				{
					vector7 = vector6;
					for (long num21 = 0L; num21 < num6; num21 += 1L)
					{
						float num22 = y + (float)num21 * num2;
						float num23 = y + (float)(num21 + 1L) * num2;
						if (num23 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num22) / (num23 - num22);
							num23 = num4;
						}
						CurvedImage.AddQuad(toFill, new Vector2(0f, num22) + pixelAdjustedRect.position, new Vector2(x, num23) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector5.y), new Vector2(vector5.x, vector7.y));
						CurvedImage.AddQuad(toFill, new Vector2(num3, num22) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num23) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector5.y), new Vector2(vector.z, vector7.y));
					}
					vector7 = vector6;
					for (long num24 = 0L; num24 < num5; num24 += 1L)
					{
						float num25 = x + (float)num24 * num;
						float num26 = x + (float)(num24 + 1L) * num;
						if (num26 > num3)
						{
							vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num25) / (num26 - num25);
							num26 = num3;
						}
						CurvedImage.AddQuad(toFill, new Vector2(num25, 0f) + pixelAdjustedRect.position, new Vector2(num26, y) + pixelAdjustedRect.position, this.color, new Vector2(vector5.x, vector.y), new Vector2(vector7.x, vector5.y));
						CurvedImage.AddQuad(toFill, new Vector2(num25, num4) + pixelAdjustedRect.position, new Vector2(num26, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector5.x, vector6.y), new Vector2(vector7.x, vector.w));
					}
					CurvedImage.AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector.y), new Vector2(vector5.x, vector5.y));
					CurvedImage.AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector.y), new Vector2(vector.z, vector5.y));
					CurvedImage.AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector6.y), new Vector2(vector5.x, vector.w));
					CurvedImage.AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector6.y), new Vector2(vector.z, vector.w));
					return;
				}
			}
			else
			{
				Vector2 b = new Vector2((num3 - x) / num, (num4 - y) / num2);
				if (this.fillCenter)
				{
					CurvedImage.AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num3, num4) + pixelAdjustedRect.position, this.color, Vector2.Scale(vector5, b), Vector2.Scale(vector6, b));
				}
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007D30 File Offset: 0x00005F30
		private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			for (int i = 0; i < 4; i++)
			{
				vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
			}
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007D84 File Offset: 0x00005F84
		private static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
			vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uv0Min, Vector2 uv0Max, Vector2 uv1Min, Vector2 uv1Max, float elementWidthScale)
		{
			int horizontalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(posMin.x - posMax.x));
			int verticalElements = CurvedUIHelper.GetNumberOfElementsForWidth(Mathf.Abs(posMin.y - posMax.y));
			//int numberOfElements = 5;
			int currentVertCount = vertexHelper.currentVertCount;

			var color32 = color;

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
					cachedPosition.Set(posX, currentPosY, 0);
					cachedUv.Set(uvX, currentUvY);
					vertexHelper.AddVert(cachedPosition, color32, cachedUv, kVec2Zero, kVec2Zero, kVec2Zero, kVec3Zero, kVec4Zero);
					cachedPosition.Set(posX, nextPosY, 0);
					cachedUv.Set(uvX, nextUvY);
					vertexHelper.AddVert(cachedPosition, color32, cachedUv, kVec2Zero, kVec2Zero, kVec2Zero, kVec3Zero, kVec4Zero);
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

		// Token: 0x06000137 RID: 311 RVA: 0x00007E74 File Offset: 0x00006074
		private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
		{
			Rect rect = base.rectTransform.rect;
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

		// Token: 0x06000138 RID: 312 RVA: 0x00007F90 File Offset: 0x00006190
		private void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
		{
			toFill.Clear();
			if (this.fillAmount < 0.001f)
			{
				return;
			}
			else if (fillAmount >= 1f)
            {
				GenerateSimpleSprite(toFill, preserveAspect);
				return;
            }
			Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect);
			Vector4 obj = (this.overrideSprite != null) ? DataUtility.GetOuterUV(this.overrideSprite) : Vector4.zero;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			Vector4 obj2 = obj;
			float num = obj2.x;
			float num2 = obj2.y;
			float num3 = obj2.z;
			float num4 = obj2.w;
			if (this.fillMethod == Image.FillMethod.Horizontal || this.fillMethod == Image.FillMethod.Vertical)
			{
				if (this.fillMethod == Image.FillMethod.Horizontal)
				{
					float num5 = (num3 - num) * this.fillAmount;
					if (this.fillOrigin == 1)
					{
						drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * this.fillAmount;
						num = num3 - num5;
					}
					else
					{
						drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * this.fillAmount;
						num3 = num + num5;
					}
				}
				else if (this.fillMethod == Image.FillMethod.Vertical)
				{
					float num6 = (num4 - num2) * this.fillAmount;
					if (this.fillOrigin == 1)
					{
						drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * this.fillAmount;
						num2 = num4 - num6;
					}
					else
					{
						drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * this.fillAmount;
						num4 = num2 + num6;
					}
				}
			}
			CurvedImage.s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
			CurvedImage.s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
			CurvedImage.s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
			CurvedImage.s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
			CurvedImage.s_Uv[0] = new Vector2(num, num2);
			CurvedImage.s_Uv[1] = new Vector2(num, num4);
			CurvedImage.s_Uv[2] = new Vector2(num3, num4);
			CurvedImage.s_Uv[3] = new Vector2(num3, num2);
			if (this.fillAmount < 1f && this.fillMethod != Image.FillMethod.Horizontal && this.fillMethod != Image.FillMethod.Vertical)
			{
				if (this.fillMethod == Image.FillMethod.Radial90)
				{
					if (CurvedImage.RadialCut(CurvedImage.s_Xy, CurvedImage.s_Uv, this.fillAmount, this.fillClockwise, this.fillOrigin))
					{
						CurvedImage.AddQuad(toFill, CurvedImage.s_Xy, this.color, CurvedImage.s_Uv);
						return;
					}
				}
				else
				{
					if (this.fillMethod == Image.FillMethod.Radial180)
					{
						for (int i = 0; i < 2; i++)
						{
							int num7 = (this.fillOrigin > 1) ? 1 : 0;
							float t;
							float t2;
							float t3;
							float t4;
							if (this.fillOrigin == 0 || this.fillOrigin == 2)
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
							CurvedImage.s_Xy[0].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t3);
							CurvedImage.s_Xy[1].x = CurvedImage.s_Xy[0].x;
							CurvedImage.s_Xy[2].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t4);
							CurvedImage.s_Xy[3].x = CurvedImage.s_Xy[2].x;
							CurvedImage.s_Xy[0].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t);
							CurvedImage.s_Xy[1].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t2);
							CurvedImage.s_Xy[2].y = CurvedImage.s_Xy[1].y;
							CurvedImage.s_Xy[3].y = CurvedImage.s_Xy[0].y;
							CurvedImage.s_Uv[0].x = Mathf.LerpUnclamped(num, num3, t3);
							CurvedImage.s_Uv[1].x = CurvedImage.s_Uv[0].x;
							CurvedImage.s_Uv[2].x = Mathf.LerpUnclamped(num, num3, t4);
							CurvedImage.s_Uv[3].x = CurvedImage.s_Uv[2].x;
							CurvedImage.s_Uv[0].y = Mathf.LerpUnclamped(num2, num4, t);
							CurvedImage.s_Uv[1].y = Mathf.LerpUnclamped(num2, num4, t2);
							CurvedImage.s_Uv[2].y = CurvedImage.s_Uv[1].y;
							CurvedImage.s_Uv[3].y = CurvedImage.s_Uv[0].y;
							float value = this.fillClockwise ? (this.fillAmount * 2f - (float)i) : (this.fillAmount * 2f - (float)(1 - i));
							if (CurvedImage.RadialCut(CurvedImage.s_Xy, CurvedImage.s_Uv, Mathf.Clamp01(value), this.fillClockwise, (i + this.fillOrigin + 3) % 4))
							{
								CurvedImage.AddQuad(toFill, CurvedImage.s_Xy, this.color, CurvedImage.s_Uv);
							}
						}
						return;
					}
					if (this.fillMethod == Image.FillMethod.Radial360)
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
							CurvedImage.s_Xy[0].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t5);
							CurvedImage.s_Xy[1].x = CurvedImage.s_Xy[0].x;
							CurvedImage.s_Xy[2].x = Mathf.LerpUnclamped(drawingDimensions.x, drawingDimensions.z, t6);
							CurvedImage.s_Xy[3].x = CurvedImage.s_Xy[2].x;
							CurvedImage.s_Xy[0].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t7);
							CurvedImage.s_Xy[1].y = Mathf.LerpUnclamped(drawingDimensions.y, drawingDimensions.w, t8);
							CurvedImage.s_Xy[2].y = CurvedImage.s_Xy[1].y;
							CurvedImage.s_Xy[3].y = CurvedImage.s_Xy[0].y;
							CurvedImage.s_Uv[0].x = Mathf.LerpUnclamped(num, num3, t5);
							CurvedImage.s_Uv[1].x = CurvedImage.s_Uv[0].x;
							CurvedImage.s_Uv[2].x = Mathf.LerpUnclamped(num, num3, t6);
							CurvedImage.s_Uv[3].x = CurvedImage.s_Uv[2].x;
							CurvedImage.s_Uv[0].y = Mathf.LerpUnclamped(num2, num4, t7);
							CurvedImage.s_Uv[1].y = Mathf.LerpUnclamped(num2, num4, t8);
							CurvedImage.s_Uv[2].y = CurvedImage.s_Uv[1].y;
							CurvedImage.s_Uv[3].y = CurvedImage.s_Uv[0].y;
							float value2 = this.fillClockwise ? (this.fillAmount * 4f - (float)((j + this.fillOrigin) % 4)) : (this.fillAmount * 4f - (float)(3 - (j + this.fillOrigin) % 4));
							if (CurvedImage.RadialCut(CurvedImage.s_Xy, CurvedImage.s_Uv, Mathf.Clamp01(value2), this.fillClockwise, (j + 2) % 4))
							{
								CurvedImage.AddQuad(toFill, CurvedImage.s_Xy, this.color, CurvedImage.s_Uv);
							}
						}
						return;
					}
				}
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000886C File Offset: 0x00006A6C
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
			CurvedImage.RadialCut(xy, cos, sin, invert, corner);
			CurvedImage.RadialCut(uv, cos, sin, invert, corner);
			return true;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000088DC File Offset: 0x00006ADC
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

		// Token: 0x060002E5 RID: 741 RVA: 0x0000F638 File Offset: 0x0000D838
		public virtual Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 vector = (base.overrideSprite == null) ? Vector4.zero : DataUtility.GetPadding(base.overrideSprite);
			Vector2 vector2 = (base.overrideSprite == null) ? Vector2.zero : new Vector2(base.overrideSprite.rect.width, base.overrideSprite.rect.height);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
			if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
			{
				float num3 = vector2.x / vector2.y;
				float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
				if (num3 > num4)
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
				}
			}
			vector3 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
			return vector3;
		}

		// Token: 0x04000205 RID: 517

		protected static readonly Vector2 kVec2Zero = new Vector2(0f, 0f);

		// Token: 0x04000206 RID: 518

		protected static readonly Vector3 kVec3Zero = new Vector3(0f, 0f, 0f);

		// Token: 0x04000207 RID: 519

		protected static readonly Vector4 kVec4Zero = new Vector4(0f, 0f, 0f, 0f);

		// Token: 0x04000208 RID: 520
		protected readonly CurvedUIHelper curvedUIHelper = new CurvedUIHelper();

		// Token: 0x04000209 RID: 521

		protected static readonly Vector2[] s_VertScratch = new Vector2[4];

		// Token: 0x0400020A RID: 522

		protected static readonly Vector2[] s_UVScratch = new Vector2[4];

		// Token: 0x0400020B RID: 523

		protected static readonly Vector2[] s_UV1Scratch = new Vector2[4];

		// Token: 0x0400020C RID: 524

		protected static readonly Color[] s_ColorScratch = new Color[4];

		// Token: 0x0400020D RID: 525

		protected static readonly Vector3[] s_Xy = new Vector3[4];

		// Token: 0x0400020E RID: 526

		protected static readonly Vector3[] s_Uv = new Vector3[4];
	}
}