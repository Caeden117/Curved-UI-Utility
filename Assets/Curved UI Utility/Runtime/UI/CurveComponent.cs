using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility
{
    [RequireComponent(typeof(Graphic))]
    [ExecuteInEditMode]
    public class CurveComponent : MonoBehaviour, IMeshModifier
    {
        private Graphic graphic = null;
        private CurvedUIHelper helper = new CurvedUIHelper();
        private float cachedCurve = 0;

        private List<UIVertex> cachedVertices = new List<UIVertex>();
        private List<UIVertex> newVertices = new List<UIVertex>();

        private void OnEnable()
        {
            graphic = GetComponent<Graphic>();
            helper.Reset();
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
            float currentCurve = helper.GetCurvedUIController(graphic.canvas).CurrentCurve;
            if (!Mathf.Approximately(currentCurve, cachedCurve))
            {
                cachedCurve = currentCurve;
                graphic.SetAllDirty();
            }
        }
    }
}
