using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CurvedUIUtility
{
    public class CurvedUIHelper
    {
        public static bool ScreenDirty = false;

        private static readonly ConcurrentDictionary<Canvas, CurvedUIController> curvedControllerCache = new ConcurrentDictionary<Canvas, CurvedUIController>();

        public Canvas CachedCanvas;

        private bool hasCachedData;
        private bool cachedCanvasIsRootCanvas;
        private Vector2 cachedCanvasSize;
        private CurvedUIController cachedController;

        private int lastCheckedFrameCount = 0;

        /// <summary>
        /// Returns the amount of faces that should be drawn for an item of the given width.
        /// </summary>
        /// <param name="width">Total width of the item.</param>
        /// <returns>The amount of faces that should be drawn for an item of the given width</returns>
        public static int GetNumberOfElementsForWidth(float width)
        {
            if (width < 1) return 1;

            int num = Mathf.CeilToInt(width / 100);

            return Mathf.Max(num, 1);
        }

        public void Reset()
        {
            curvedControllerCache.Clear();
            CachedCanvas = null;
            cachedCanvasIsRootCanvas = false;
            cachedController = null;
            hasCachedData = false;
        }

        // We're saving a bunch of implicit Vector2 conversions by having everything as a Vector3
        public Vector3 GetCurvedPosition(Vector3 position, ref Matrix4x4 localToCanvas, ref Matrix4x4 canvasToLocal, CurvedUISettings settings)
        {
            var screenSpace = localToCanvas.MultiplyPoint(position);

            ModifyCurvedPosition(ref screenSpace, settings);

            return canvasToLocal.MultiplyPoint(screenSpace);
        }

        public void ModifyCurvedPosition(ref Vector3 screenSpace, CurvedUISettings settings)
        {
            var xDist = DistanceFromCenter(screenSpace.y, cachedCanvasSize.y / 2);
            var yDist = DistanceFromCenter(screenSpace.x, cachedCanvasSize.x / 2);

            // Curve
            screenSpace.x -= screenSpace.x * xDist * settings.Curve.x;
            screenSpace.y -= screenSpace.y * yDist * settings.Curve.y;

            // Pull
            screenSpace.x += xDist * settings.Pull.x;
            screenSpace.y += yDist * settings.Pull.y;

            // Scale
            screenSpace.x *= settings.Scale.x;
            screenSpace.y *= settings.Scale.y;

            // Offset
            screenSpace.x += settings.Offset.x * cachedCanvasSize.x;
            screenSpace.y += settings.Offset.y * cachedCanvasSize.y;
        }

        public void PokeScreenSize()
        {
            if (CachedCanvas is null) return;

            var rectSize = (CachedCanvas.transform as RectTransform).rect.size;

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
                if (!cachedCanvasIsRootCanvas && CachedCanvas == canvas)
                {
                    return cachedController;
                }
                rootCanvas = canvas.rootCanvas;
                if (cachedCanvasIsRootCanvas && CachedCanvas == rootCanvas)
                {
                    return cachedController;
                }
            }
            cachedController = GetCurvedControllerForCanvas(canvas);
            if (cachedController != null)
            {
                CachedCanvas = canvas;
                cachedCanvasIsRootCanvas = false;
                cachedCanvasSize = (CachedCanvas.transform as RectTransform).rect.size;
                hasCachedData = true;
                return cachedController;
            }
            rootCanvas = canvas.rootCanvas;
            cachedController = GetCurvedControllerForCanvas(rootCanvas);
            cachedCanvasIsRootCanvas = true;
            CachedCanvas = rootCanvas;
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

            if (component != null)
            {
                curvedControllerCache.TryAdd(canvas, component);
            }

            return component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float DistanceFromCenter(float x, float c) => 1 - (x / c * x / c);
    }
}