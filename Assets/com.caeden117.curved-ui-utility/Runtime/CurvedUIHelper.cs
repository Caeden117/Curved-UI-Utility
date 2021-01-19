using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CurvedUIUtility
{
    public class CurvedUIHelper
    {
        internal static bool ScreenDirty = false;

        private static Dictionary<Canvas, CurvedUIController> curvedControllerCache = new Dictionary<Canvas, CurvedUIController>();

        private bool hasCachedData;
        private bool cachedCanvasIsRootCanvas;
        private Canvas cachedCanvas;
        private Vector2 cachedCanvasSize;
        private CurvedUIController cachedController;

        private Vector3 screenSizeOffset = Vector3.zero;
        private int lastCheckedFrameCount = 0;

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
        public Vector3 GetCurvedPosition(Vector3 position, Matrix4x4 localToCanvas, Matrix4x4 canvasToLocal, CurvedUISettings settings)
        {
            var screenSpace = localToCanvas.MultiplyPoint(position);

            ModifyCurvedPosition(ref screenSpace, settings);

            return canvasToLocal.MultiplyPoint(screenSpace);
        }

        public void ModifyCurvedPosition(ref Vector3 screenSpace, CurvedUISettings settings)
        {
            var xDist = DistanceFromCenter(screenSpace.y, cachedCanvasSize.y / 2);
            var yDist = DistanceFromCenter(screenSpace.x, cachedCanvasSize.x / 2);

            if (settings.UsingCurve)
            {
                var curve = settings.Curve;
                screenSpace.Set(
                    screenSpace.x + (0 - screenSpace.x) * xDist * curve.x,
                    screenSpace.y + (0 - screenSpace.y) * yDist * curve.y,
                    screenSpace.z
                    );
            }

            if (settings.UsingPull)
            {
                var pull = settings.Pull;
                screenSpace.Set(
                    screenSpace.x + (xDist * pull.x),
                    screenSpace.y + (yDist * pull.y),
                    screenSpace.z
                    );
            }

            if (settings.UsingScale)
            {
                MultiplyVectorValues(ref screenSpace, settings.Scale);
            }

            if (settings.UsingOffset)
            {
                MultiplyVectorValuesIntoResult(ref screenSizeOffset, settings.Offset, cachedCanvasSize);
                AddVectorValues(ref screenSpace, screenSizeOffset);
            }
        }

        public void PokeScreenSize()
        {
            var rectSize = (cachedCanvas.transform as RectTransform).rect.size;

            if (lastCheckedFrameCount != Time.frameCount)
            {
                ScreenDirty = rectSize != cachedCanvasSize;
                lastCheckedFrameCount = Time.frameCount;
            }

            cachedCanvasSize = rectSize;
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
            if (Application.isPlaying && curvedControllerCache.TryGetValue(canvas, out var component))
            {
                return component;
            }
            component = canvas.GetComponent<CurvedUIController>();
            curvedControllerCache.Add(canvas, component);
            return component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MultiplyVectorValues(ref Vector3 a, Vector3 b) => a.Set(a.x * b.x, a.y * b.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MultiplyVectorValuesIntoResult(ref Vector3 result, Vector3 a, Vector2 b) => result.Set(a.x * b.x, a.y * b.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddVectorValues(ref Vector3 a, Vector3 b) => a.Set(a.x + b.x, a.y + b.y, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float DistanceFromCenter(float x, float c) => 1 - (x / c* x / c);
    }
}