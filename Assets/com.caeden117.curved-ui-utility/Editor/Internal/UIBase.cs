using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace CurvedUIUtility.Editor
{
    internal class UIBase : IDisposable
    {
        private const string uiLayerName = "UI";

        internal readonly GameObject GameObject;

        private readonly MenuCommand menuCommand;

        public UIBase(string name, MenuCommand menuCommand, Vector2 size)
        {
            GameObject = new GameObject(name, typeof(RectTransform));
            GameObject.layer = LayerMask.NameToLayer(uiLayerName);
            GameObject.SetActive(false);

            var rectTransform = GameObject.transform as RectTransform;
            rectTransform.sizeDelta = size;

            this.menuCommand = menuCommand;
        }

        public UIBase AddChildObject(string name, Action<UIObjectBuilder> configureChild)
        {
            var child = new UIObjectBuilder(name, this);
            configureChild?.Invoke(child);
            return this;
        }

        public UIBase AddChildObject<T>(string name, Action<UIObjectBuilder<T>> configureChild) where T : Component
        {
            var child = new UIObjectBuilder<T>(name, this);
            configureChild?.Invoke(child);
            return this;
        }

        public void Dispose()
        {
            var parent = menuCommand.context as GameObject;
            var explicitParent = parent != null;

            if (!explicitParent)
            {
                parent = UIHelper.GetOrCreateCanvasGameObject();

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                    parent = prefabStage.prefabContentsRoot;
            }

            if (!parent.GetComponentsInParent<Canvas>(true).Any())
            {
                var canvas = UIHelper.CreateNewUI();
                Undo.SetTransformParent(canvas.transform, parent.transform, "");
                parent = canvas;
            }

            GameObjectUtility.EnsureUniqueNameForSibling(GameObject);

            UIHelper.SetParentAndAlign(GameObject, parent);
            if (!explicitParent) // not a context click, so center in sceneview
                UIHelper.SetPositionVisibleinSceneView(parent.transform as RectTransform, GameObject.transform as RectTransform);

            // This call ensure any change made to created Objects after they where registered will be part of the Undo.
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? GameObject : parent, "");

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + GameObject.name);

            Selection.activeGameObject = GameObject;

            GameObject.SetActive(true);
        }
    }
}