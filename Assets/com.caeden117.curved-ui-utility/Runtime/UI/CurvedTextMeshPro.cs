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

        private Matrix4x4 cachedCanvasWorldToLocalMatrix;
        private Matrix4x4 cachedCanvasLocalToWorldMatrix;

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
            OnTransformParentChanged();
            UpdateCurvature();
        }

        protected override void OnTransformParentChanged()
        {
            if (canvas == null) return;

            base.OnTransformParentChanged();

            if (curvedHelper.CachedCanvas == null)
            {
                cachedCanvasWorldToLocalMatrix = canvas.transform.worldToLocalMatrix;
                cachedCanvasLocalToWorldMatrix = canvas.transform.localToWorldMatrix;
            }
            else
            {
                cachedCanvasWorldToLocalMatrix = curvedHelper.CachedCanvas.transform.worldToLocalMatrix;
                cachedCanvasLocalToWorldMatrix = curvedHelper.CachedCanvas.transform.localToWorldMatrix;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnTransformParentChanged();
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

            HasCurvedThisFrame = false;

            UpdateCurvature();
        }

        private void Update()
        {
            HasCurvedThisFrame = false;
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                if (CurvedUIHelper.ScreenDirty)
                {
                    OnTransformParentChanged();
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
                OnTransformParentChanged();
            }

            CheckPosition();
        }

        private void Controller_CurveSettingsChangedEvent() => UpdateCurvature();

        public void CheckPosition()
        {
            var position = m_rectTransform.position;

            if (position != cachedPosition || m_characterCount != cachedCharacterCount)
            {
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
            var worldToLocal = m_rectTransform.worldToLocalMatrix;
            var localToWorld = m_rectTransform.localToWorldMatrix;
            
            foreach (var v in cachedVertices)
            {
                var canvasSpace = cachedCanvasWorldToLocalMatrix.MultiplyPoint(localToWorld.MultiplyPoint(v));
                curvedHelper.ModifyCurvedPosition(ref canvasSpace, settings);
                modifiedVertices.Add(worldToLocal.MultiplyPoint(cachedCanvasLocalToWorldMatrix.MultiplyPoint(canvasSpace)));
            }

            m_mesh.SetVertices(modifiedVertices);

            canvasRenderer.SetMesh(m_mesh);

            HasCurvedThisFrame = true;
        }
    }
}
