using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CurvedUIUtility
{
    [ExecuteAlways]
    public partial class CurvedTextMeshPro : TextMeshProUGUI, ICurveable
    {
        public bool HasCurvedThisFrame { get; set; } = false;

        private CurvedUIHelper curvedHelper = new CurvedUIHelper();
        private CurvedUIController controller = null;

        private int cachedCharacterCount = 0;
        private Vector3 cachedPosition = Vector3.zero;

        private List<Vector3> cachedVertices = new List<Vector3>();
        private List<Vector3> modifiedVertices = new List<Vector3>();

        private Matrix4x4 canvasToLocalMatrix;
        private Matrix4x4 localToCanvasMatrix;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(WaitForCanvas());
        }

        private IEnumerator WaitForCanvas()
        {
            yield return new WaitWhile(() => canvas == null);
            curvedHelper.Reset();
            controller = curvedHelper.GetCurvedUIController(canvas);
            controller.CurveSettingsChangedEvent += Controller_CurveSettingsChangedEvent;
            UpdateMatrices();
            UpdateCurvature();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateMatrices();
            SetAllDirty();
        }
#endif

        protected override void OnDisable()
        {
            base.OnDisable();

            if (controller != null)
            {
                controller.CurveSettingsChangedEvent -= Controller_CurveSettingsChangedEvent;
            }
        }

        protected override void GenerateTextMesh()
        {
            base.GenerateTextMesh();

            m_mesh.GetVertices(cachedVertices);

            UpdateCurvature();
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
                    SetAllDirty();
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

        private void Controller_CurveSettingsChangedEvent() => UpdateCurvature();

        public void UpdateMatrices()
        {
            if (canvas == null || m_rectTransform == null) return;

            var canvasTransform = curvedHelper?.CachedCanvas == null ? canvas.transform : curvedHelper.CachedCanvas.transform;

            var transformWorldToLocal = m_rectTransform.worldToLocalMatrix;
            var transformLocalToWorld = m_rectTransform.localToWorldMatrix;
            var canvasWorldToLocal = canvasTransform.worldToLocalMatrix;
            var canvasLocalToWorld = canvasTransform.localToWorldMatrix;

            canvasToLocalMatrix = MatrixUtils.FastMultiplication(ref transformWorldToLocal, ref canvasLocalToWorld);
            localToCanvasMatrix = MatrixUtils.FastMultiplication(ref canvasWorldToLocal, ref transformLocalToWorld);
        }

        public void CheckPosition()
        {
            var position = m_rectTransform.position;

            if (position != cachedPosition || m_characterCount != cachedCharacterCount)
            {
                UpdateMatrices();
                cachedPosition = position;
                cachedCharacterCount = m_characterCount;
                UpdateCurvature();
            }
        }

        public void UpdateCurvature()
        {
            if (controller == null || HasCurvedThisFrame) return;

            curvedHelper.PokeScreenSize();

            modifiedVertices.Clear();

            var settings = controller.CurrentCurveSettings;
            
            foreach (var v in cachedVertices)
            {
                modifiedVertices.Add(curvedHelper.GetCurvedPosition(v, localToCanvasMatrix, canvasToLocalMatrix, settings));
            }

            m_mesh.SetVertices(modifiedVertices);

            canvasRenderer.SetMesh(m_mesh);

            HasCurvedThisFrame = true;
        }
    }
}
