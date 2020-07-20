/*
 * Released under the MIT License
 *
 * Copyright (c) 2020 Caeden Statia
 * https://caeden.dev/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility
{
    /*
     * The CurvedUIController exposes public methods for controlling your curved UI.
     */
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class CurvedUIController : MonoBehaviour
    {
        private static readonly int ZoomMultiplier = Shader.PropertyToID("_CurvedUIMultiplier");
        private static readonly int RadialScale = Shader.PropertyToID("_CurvedUIRadialScale");

        [Tooltip("On Start, automatically set the zoom to this amount.\nRecommended values: 0.1 - 0.3")]
        [SerializeField] private float startingZoom = 0;
        [Tooltip("Controls the area of effect for zoom.\nA larger number means smaller area.\nRecommended value: 0.75")]
        [SerializeField] private float radialScale = 0.75f;
        [Tooltip("Curved UI Renderer that will be layered on top of this canvas.")]
        [SerializeField] private CurvedUIRenderer curvedUIRenderer;

        private float currentZoom = 0;
        private Material curvedUIMaterial;
        private RawImage curvedUIRawImage;

        private void Start()
        {
            // To prevent extra setup from the end user, we create the RawImage for our CurvedUIRenderer to output to.
            GameObject mirror = new GameObject("UI Mirror", typeof(CanvasRenderer), typeof(RawImage));
            mirror.transform.SetParent(transform);

            RectTransform mirrorRect = mirror.transform as RectTransform;
            mirrorRect.anchorMin = Vector2.zero;
            mirrorRect.anchorMax = Vector2.one;
            mirrorRect.offsetMin = mirrorRect.offsetMax = Vector2.zero;
            mirrorRect.localScale = Vector3.one;

            // Essentially, the curved UI effect is done via a Shader Graph.
            // Let's go find that shader and apply it to our Raw Image.
            curvedUIMaterial = new Material(Shader.Find("Shader Graphs/CurvedUI"));
            curvedUIMaterial.name = "Curved UI Material";
            curvedUIMaterial.SetTexture("_MainTex", curvedUIRenderer.RenderTexture);

            curvedUIRawImage = mirrorRect.GetComponent<RawImage>();
            curvedUIRawImage.texture = curvedUIRenderer.RenderTexture;
            curvedUIRawImage.material = curvedUIMaterial;
            SetZoomInternal(startingZoom);

            curvedUIRenderer.OnDimensionsChanged += CurvedUIDimensionsChanged;
        }

        /// <summary>
        /// Sets the curve for the <see cref="curvedUIRenderer"/>.
        /// </summary>
        /// <param name="curve">Amount of curvature.</param>
        /// <param name="transitionTime">Transition time to animate between the current and new curvature.</param>
        public void SetUICurve(float curve, float transitionTime = 1f)
        {
            if (curvedUIMaterial is null)
            {
                StartCoroutine(WaitForMaterialThenSetCurve(curve, transitionTime));
                return;
            }
            // Stop any coroutines, which include the curvature animation.
            StopAllCoroutines();
            if (transitionTime <= 0)
            {
                SetZoomInternal(curve);
                return;
            }
            StartCoroutine(SetZoom(curve, transitionTime));
        }

        // Called when the CurvedUIRenderer detects a change in screen resolution.
        private void CurvedUIDimensionsChanged()
        {
            curvedUIRawImage.texture = curvedUIRenderer.RenderTexture;
            curvedUIMaterial.SetTexture("_MainTex", curvedUIRenderer.RenderTexture);
        }

        // Internal IEnumerator that animates the curvature change.
        private IEnumerator SetZoom(float targetZoom, float transitionTime)
        {
            float t = 0;
            while (t < 1)
            {
                float zoom = Mathf.Lerp(currentZoom, targetZoom, t);
                t += Time.deltaTime / transitionTime;
                SetZoomInternal(zoom);
                yield return new WaitForEndOfFrame();
            }
            SetZoomInternal(targetZoom);
        }

        private IEnumerator WaitForMaterialThenSetCurve(float targetZoom, float transitionTime)
        {
            yield return new WaitUntil(() => curvedUIMaterial != null);
            StartCoroutine(SetZoom(targetZoom, transitionTime));
        }

        // Sets stored zoom/curvature level and updates our material.
        private void SetZoomInternal(float zoom)
        {
            currentZoom = zoom;
            curvedUIMaterial.SetFloat(ZoomMultiplier, zoom);
            curvedUIMaterial.SetFloat(RadialScale, radialScale);
        }

        private void OnDestroy()
        {
            curvedUIRenderer.OnDimensionsChanged -= CurvedUIDimensionsChanged;
        }
    }
}
