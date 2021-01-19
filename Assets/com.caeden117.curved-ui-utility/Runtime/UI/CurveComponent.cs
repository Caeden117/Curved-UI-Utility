using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility
{
    [RequireComponent(typeof(Graphic))]
    [ExecuteAlways]
    public class CurveComponent : MonoBehaviour, IMeshModifier
    {
        // WE ARE ENTERING REFLECTION LAND FOR PERFORMANCE REASONS PLEASE DO NOT BE ALARMED
        private static readonly FieldInfo vertexHelperPositions = typeof(VertexHelper).GetField("m_Positions", BindingFlags.Instance | BindingFlags.NonPublic);

        private Graphic graphic = null;

        private CurvedUIHelper helper = new CurvedUIHelper();
        private CurvedUIController controller = null;

        private List<Vector3> cachedVertices = new List<Vector3>();
        private List<Vector3> newVertices = new List<Vector3>();
        private Mesh cachedMesh;

        private Vector3 cachedPosition = Vector3.positiveInfinity;

        private Matrix4x4 localToCanvasMatrix;
        private Matrix4x4 canvasToLocalMatrix;

        private void OnEnable()
        {
            graphic = GetComponent<Graphic>();
            helper.Reset();
            helper.GetCurvedUIController(graphic.canvas);
            OnTransformParentChanged();
        }

        private void OnValidate()
        {
            OnTransformParentChanged();
            graphic?.SetAllDirty();
            cachedMesh = CreateNewMesh();
        }

        private void OnTransformParentChanged()
        {
            if (graphic is null || graphic.canvas is null) return;
            canvasToLocalMatrix = transform.worldToLocalMatrix * graphic.canvas.transform.localToWorldMatrix;
            localToCanvasMatrix = graphic.canvas.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        }

        private void Start()
        {
            cachedMesh = CreateNewMesh();
            controller = helper.GetCurvedUIController(graphic.canvas);
            if (controller != null)
            {
                controller.CurveSettingsChangedEvent += Controller_CurveSettingsChangedEvent;
            }
            UpdateCurvature();
        }

        private void Controller_CurveSettingsChangedEvent() => UpdateCurvature();

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
            cachedVertices = new List<Vector3>(vertexHelperPositions.GetValue(verts) as List<Vector3>);

            if (cachedMesh == null)
            {
                cachedMesh = CreateNewMesh();
            }

            verts.FillMesh(cachedMesh);

            UpdateCurvature();

            vertexHelperPositions.SetValue(verts, newVertices);
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                UpdateCurvature();
                return;
            }

            if (CurvedUIHelper.ScreenDirty)
            {
                OnTransformParentChanged();
            }

            var currentPosition = graphic.rectTransform.anchoredPosition3D;
            
            if (cachedPosition != currentPosition)
            {
                OnTransformParentChanged();
                cachedPosition = currentPosition;
                UpdateCurvature();
            }
        }

        private void UpdateCurvature()
        {
            if (cachedMesh == null) return;

            helper.PokeScreenSize();

            var settings = controller.CurrentCurveSettings;
            
            newVertices.Clear();

            foreach (var v in cachedVertices)
            {
                newVertices.Add(helper.GetCurvedPosition(v, localToCanvasMatrix, canvasToLocalMatrix, settings));
            }

            cachedMesh.SetVertices(newVertices);

            graphic.canvasRenderer.SetMesh(cachedMesh);
        }

        private Mesh CreateNewMesh()
        {
            return new Mesh
            {
                name = "Temporary Mesh",
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }
}
