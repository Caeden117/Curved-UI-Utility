using System;
using UnityEngine;

namespace CurvedUIUtility.Editor
{
    internal class UIObjectBuilder
    {
        internal readonly GameObject GameObject;
        internal readonly RectTransform RectTransform;

        public UIObjectBuilder(string name, UIBase parent) : this(name, parent.GameObject) { }

        public UIObjectBuilder(string name, GameObject parent) : this(name)
        {
            SetParentAndAlign(GameObject, parent);
        }

        public UIObjectBuilder(string name)
        {
            GameObject = CreateDefaultObject(name);
            RectTransform = GameObject.GetComponent<RectTransform>();
        }

        public UIObjectBuilder AddChildObject(string name, Action<UIObjectBuilder> configureChild)
        {
            var child = new UIObjectBuilder(name, GameObject);
            configureChild?.Invoke(child);
            return this;
        }

        public UIObjectBuilder AddChildObject<TComponent>(string name, Action<UIObjectBuilder<TComponent>> configureChild) where TComponent : Component
        {
            var child = new UIObjectBuilder<TComponent>(name, GameObject);
            configureChild?.Invoke(child);
            return this;
        }

        private GameObject CreateDefaultObject(string name) => new GameObject(name, typeof(RectTransform));

        private void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }
    }

    internal class UIObjectBuilder<T> : UIObjectBuilder where T : Component
    {
        internal readonly T Component;

        public UIObjectBuilder(string name, UIBase parent) : base(name, parent) => Component = GameObject.AddComponent<T>();

        public UIObjectBuilder(string name, GameObject parent) : base(name, parent) => Component = GameObject.AddComponent<T>();

        public UIObjectBuilder(string name) : base(name) => Component = GameObject.AddComponent<T>();
    }
}