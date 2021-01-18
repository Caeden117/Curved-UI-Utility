using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility
{
    [RequireComponent(typeof(Graphic))]
    [ExecuteAlways]
    public class CurveComponent : MonoBehaviour, IMeshModifier
    {
        private Graphic graphic = null;

        private CurvedUIHelper helper = new CurvedUIHelper();
        private CurvedUIController controller = null;

        private Vector3 cachedPosition = Vector3.positiveInfinity;

        private List<UIVertex> cachedVertices = new List<UIVertex>();
        private List<UIVertex> newVertices = new List<UIVertex>();

        private void OnEnable()
        {
            graphic = GetComponent<Graphic>();
            helper.Reset();
            helper.GetCurvedUIController(graphic.canvas);
        }

        private void Start()
        {
            controller = helper.GetCurvedUIController(graphic.canvas);
            if (controller != null)
            {
                controller.CurveSettingsChangedEvent += Controller_CurveSettingsChangedEvent;
            }
        }

        private void Controller_CurveSettingsChangedEvent()
        {
            graphic.SetVerticesDirty();
        }

        private void OnDestroy()
        {
            if (controller != null)
            {
                controller.CurveSettingsChangedEvent -= Controller_CurveSettingsChangedEvent;
            }
        }

        public void ModifyMesh(Mesh mesh) { }

        public void ModifyMesh(VertexHelper verts)
        {
            if (!enabled) return;
            helper.PokeScreenSize();
            verts.GetUIVertexStream(cachedVertices);
            newVertices.Clear();
            verts.Clear();
            for (int i = 0; i < cachedVertices.Count; i++)
            {
                var v = cachedVertices[i];
                v.position = helper.GetCurvedPosition(graphic.rectTransform, v.position);
                newVertices.Add(v);
            }
            verts.AddUIVertexTriangleStream(newVertices);
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                graphic.SetVerticesDirty();
                return;
            }

            var currentPosition = graphic.rectTransform.anchoredPosition3D;
            
            if (cachedPosition != currentPosition)
            {
                cachedPosition = currentPosition;
                graphic.SetAllDirty();
            }
        }
    }
}
