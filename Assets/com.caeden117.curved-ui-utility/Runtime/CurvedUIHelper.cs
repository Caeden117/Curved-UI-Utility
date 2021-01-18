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

        private Vector3 screenSizeOffset = Vector3.zero;

        /// <summary>
        /// Returns the amount of faces that should be drawn for an item of the given width.
        /// </summary>
        /// <param name="width">Total width of the item.</param>
        /// <returns>The amount of faces that should be drawn for an item of the given width</returns>
        public static int GetNumberOfElementsForWidth(float width)
        {
            if (width < 1) return 1;

            int num = Mathf.CeilToInt(Mathf.Pow(width, 1 / 4f));

            return Mathf.Max(num, 1);
        }

        public void Reset()
        {
            curvedControllerCache.Clear();
            cachedCanvas = null;
            cachedCanvasIsRootCanvas = false;
            cachedController = null;
            hasCachedData = false;
        }

        // We're saving a bunch of implicit Vector2 conversions by having everything as a Vector3
        public Vector3 GetCurvedPosition(RectTransform transform, Vector3 position)
        {
            var settings = cachedController.CurrentCurveSettings;

            var screenSpace = cachedCanvasWorldToLocalMatrix.MultiplyPoint(transform.TransformPoint(position));

            screenSpace.Set(
                Mathf.LerpUnclamped(screenSpace.x, 0, DistanceFromCenter(screenSpace.y, cachedCanvasSize.y / 2, settings.Curve.x)),
                Mathf.LerpUnclamped(screenSpace.y, 0, DistanceFromCenter(screenSpace.x, cachedCanvasSize.x / 2, settings.Curve.y)),
                screenSpace.z
                );

            screenSpace.Set(
                screenSpace.x + DistanceFromCenter(screenSpace.y, cachedCanvasSize.y / 2, settings.Pull.x),
                screenSpace.y + DistanceFromCenter(screenSpace.x, cachedCanvasSize.x / 2, settings.Pull.y),
                screenSpace.z
                );

            MultiplyVectorValues(ref screenSpace, settings.Scale);
            MultiplyVectorValuesIntoResult(ref screenSizeOffset, settings.Offset, cachedCanvasSize);
            AddVectorValues(ref screenSpace, screenSizeOffset);

            return transform.InverseTransformPoint(cachedCanvasLocalToWorldMatrix.MultiplyPoint(screenSpace));
        }

        public void PokeScreenSize()
        {
            var rect = (cachedCanvas.transform as RectTransform);

            cachedCanvasSize = rect.rect.size;
            cachedCanvasLocalToWorldMatrix = rect.localToWorldMatrix;
            cachedCanvasWorldToLocalMatrix = rect.worldToLocalMatrix;
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

        private void MultiplyVectorValuesIntoResult(ref Vector3 result, Vector3 a, Vector3 b)
        {
            result.Set(a.x * b.x, a.y * b.y, 0);
        }

        private void AddVectorValues(ref Vector3 a, Vector3 b)
        {
            a.Set(a.x + b.x, a.y + b.y, 0);
        }

        private float DistanceFromCenter(float x, float c, float curve)
        {
            return (1 - Mathf.Pow(x / c, 2)) * curve;
        }
    }
}