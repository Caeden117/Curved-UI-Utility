using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CurvedUIUtility.Editor
{
    internal static class UIHelper
    {
        private const string uiLayerName = "UI";

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        public static GameObject GetOrCreateCanvasGameObject()
        {
            var selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            var canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (IsValidCanvas(canvas))
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use any valid canvas.
            // We have to find all loaded Canvases, not just the ones in main scenes.
            var canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();

            return canvasArray.Where(x => IsValidCanvas(x)).FirstOrDefault()?.gameObject ?? CreateNewUI();
        }

        public static GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(uiLayerName);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<CurvedUIController>();

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            var customScene = prefabStage != null;

            if (customScene)
            {
                root.transform.SetParent(prefabStage.prefabContentsRoot.transform, false);
            }
            else
            {
                // Create an event system if not in a prefab scene
                CreateEventSystem(false);
            }

            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            return root;
        }

        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null) return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            var rectTransform = child.transform as RectTransform;

            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }

            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        public static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            var sceneView = SceneView.lastActiveSceneView;

            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            var camera = sceneView.camera;
            var position = Vector3.zero;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out Vector2 localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x += canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y += canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        private static void CreateEventSystem(bool select) => CreateEventSystem(null, select);

        private static void CreateEventSystem(GameObject parent, bool select)
        {
            var eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
                GameObjectUtility.SetParentAndAlign(eventSystem.gameObject, parent);
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Created " + eventSystem.name);
            }

            if (select && eventSystem != null) Selection.activeGameObject = eventSystem.gameObject;
        }

        private static bool IsValidCanvas(Canvas canvas)
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy)
                return false;

            // It's important that the non-editable canvas from a prefab scene won't be rejected,
            // but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
            if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
                return false;

            if (StageUtility.GetStageHandle(canvas.gameObject) != StageUtility.GetCurrentStageHandle())
                return false;

            return true;
        }
    }
}