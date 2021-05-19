using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility
{
    [RequireComponent(typeof(Graphic))]
    [ExecuteAlways]
    public class CurveComponent : MonoBehaviour, IMeshModifier, ICurveable
    {
        // WE ARE ENTERING REFLECTION LAND FOR PERFORMANCE REASONS PLEASE DO NOT BE ALARMED
        private static readonly FieldInfo vertexHelperPositions = typeof(VertexHelper).GetField("m_Positions", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool HasCurvedThisFrame { get; set; } = false;

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

            if (controller != null)
            {
                controller.CurveSettingsChangedEvent -= Controller_CurveSettingsChangedEvent;
                controller.CurveSettingsChangedEvent += Controller_CurveSettingsChangedEvent;
            }

            UpdateMatrices();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateMatrices();
            graphic?.SetAllDirty();
        }
#endif

        private void Start()
        {
            cachedMesh = CreateNewMesh();
            controller = helper.GetCurvedUIController(graphic.canvas);
            
            if (controller != null)
            {
                controller.CurveSettingsChangedEvent -= Controller_CurveSettingsChangedEvent;
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

        private void Update()
        {
            HasCurvedThisFrame = false;
        }

        private void OnRenderObject()
        {
            if (!Application.isPlaying)
            {
                if (CurvedUIHelper.ScreenDirty)
                {
                    UpdateMatrices();
                    graphic.SetAllDirty();
                }
                else
                {
                    CheckPosition();
                }
                return;
            }

            if (CurvedUIHelper.ScreenDirty)
            {
                UpdateMatrices();
            }

            CheckPosition();
        }

        public void UpdateMatrices()
        {

            if (graphic == null || graphic.canvas == null) return;

            var canvasTransform = helper?.CachedCanvas == null ? graphic.canvas.transform : helper.CachedCanvas.transform;

            var transformWorldToLocal = transform.worldToLocalMatrix;
            var transformLocalToWorld = transform.localToWorldMatrix;
            var canvasWorldToLocal = canvasTransform.worldToLocalMatrix;
            var canvasLocalToWorld = canvasTransform.localToWorldMatrix;

            canvasToLocalMatrix = MatrixUtils.FastMultiplication(ref transformWorldToLocal, ref canvasLocalToWorld);
            localToCanvasMatrix = MatrixUtils.FastMultiplication(ref canvasWorldToLocal, ref transformLocalToWorld);
        }

        public void CheckPosition()
        {
            var currentPosition = graphic.rectTransform.position;

            if (cachedPosition != currentPosition)
            {
                UpdateMatrices();
                cachedPosition = currentPosition;
                UpdateCurvature();
            }
        }

        public void UpdateCurvature()
        {
            if (cachedMesh == null || controller == null || HasCurvedThisFrame) return;

            helper.PokeScreenSize();

            var settings = controller.CurrentCurveSettings;
            
            newVertices.Clear();

            foreach (var v in cachedVertices)
            {
                newVertices.Add(helper.GetCurvedPosition(v, localToCanvasMatrix, canvasToLocalMatrix, settings));
            }

            cachedMesh.SetVertices(newVertices);

            graphic.canvasRenderer.SetMesh(cachedMesh);

            HasCurvedThisFrame = true;
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
