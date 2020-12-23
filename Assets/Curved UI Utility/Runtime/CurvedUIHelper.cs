using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CurvedUIUtility
{
    public class CurvedUIHelper
    {
        private static Dictionary<Canvas, CurvedUIController> curvedControllerCache = new Dictionary<Canvas, CurvedUIController>();

        private bool hasCachedData;
        private bool cachedCanvasIsRootCanvas;
        private Canvas cachedCanvas;
        private Vector3 cachedCanvasSize;
        private CurvedUIController cachedController;
        private Matrix4x4 cachedCanvasLocalToWorldMatrix;
        private Matrix4x4 cachedCanvasWorldToLocalMatrix;

        private Vector3 screenSize = new Vector3(100, 100, 0);

        public void Reset()
        {
            cachedCanvas = null;
            cachedCanvasIsRootCanvas = false;
            cachedController = null;
            hasCachedData = false;
        }

        // We're saving a bunch of implicit Vector2 conversions by having everything as a Vector3
        public Vector3 GetCurvedPosition(RectTransform transform, Vector3 position)
        {
            float curve = Application.isPlaying ? cachedController.CurrentCurve : cachedController.StartingCurve;

            var screenSpace = cachedCanvasWorldToLocalMatrix.MultiplyPoint(transform.TransformPoint(position));
            DivideVectorValues(ref screenSpace, cachedCanvasSize);
            MultiplyVectorValues(ref screenSpace, screenSize);

            var distance = screenSpace.magnitude / screenSize.magnitude;

            LerpVectors(ref screenSpace, Vector3.zero, (1f - distance) * curve);
            DivideVectorValues(ref screenSpace, screenSize);
            MultiplyVectorValues(ref screenSpace, cachedCanvasSize);

            return transform.InverseTransformPoint(cachedCanvasLocalToWorldMatrix.MultiplyPoint(screenSpace));
        }

        public void PokeScreenSize()
        {
#if UNITY_EDITOR
            screenSize = Handles.GetMainGameViewSize() / 2f;
#else
            screenSize = new Vector2(Screen.width, Screen.height) / 2f;
#endif
            cachedCanvasSize = (cachedCanvas.transform as RectTransform).rect.size * cachedCanvas.transform.localScale;
            cachedCanvasLocalToWorldMatrix = cachedCanvas.transform.localToWorldMatrix;
            cachedCanvasWorldToLocalMatrix = cachedCanvas.transform.worldToLocalMatrix;
        }

        public CurvedUIController GetCurvedUIController(Canvas canvas)
        {
            if (canvas == null)
            {
                return null;
            }
            Canvas rootCanvas;
            if (hasCachedData)
            {
                if (!canvas.transform.hasChanged)
                {
                    return cachedController;
                }
                if (!cachedCanvasIsRootCanvas && cachedCanvas == canvas)
                {
                    return cachedController;
                }
                rootCanvas = canvas.rootCanvas;
                if (cachedCanvasIsRootCanvas && cachedCanvas == rootCanvas)
                {
                    return cachedController;
                }
            }
            cachedController = GetCurvedControllerForCanvas(canvas);
            if (cachedController != null)
            {
                cachedCanvas = canvas;
                cachedCanvasIsRootCanvas = false;
                cachedCanvasSize = (cachedCanvas.transform as RectTransform).rect.size;
                cachedCanvasLocalToWorldMatrix = cachedCanvas.transform.localToWorldMatrix;
                cachedCanvasWorldToLocalMatrix = cachedCanvas.transform.worldToLocalMatrix;
                hasCachedData = true;
                return cachedController;
            }
            rootCanvas = canvas.rootCanvas;
            cachedController = GetCurvedControllerForCanvas(rootCanvas);
            cachedCanvasIsRootCanvas = true;
            cachedCanvas = rootCanvas;
            cachedCanvasSize = (rootCanvas.transform as RectTransform).rect.size;
            hasCachedData = true;
            return cachedController;
        }

        private static CurvedUIController GetCurvedControllerForCanvas(Canvas canvas)
        {
            CurvedUIController component;
            if (Application.isPlaying && curvedControllerCache.TryGetValue(canvas, out component))
            {
                return component;
            }
            component = canvas.GetComponent<CurvedUIController>();
            curvedControllerCache[canvas] = component;
            return component;
        }


        // These two helper functions prevent unnecessary allocations
        private void DivideVectorValues(ref Vector3 a, Vector3 b)
        {
            a.Set(a.x / b.x, a.y / b.y, 0);
        }

        private void MultiplyVectorValues(ref Vector3 a, Vector3 b)
        {
            a.Set(a.x * b.x, a.y * b.y, 0);
        }

        private void LerpVectors(ref Vector3 a, Vector3 b, float t)
        {
            a.Set(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
    }
}