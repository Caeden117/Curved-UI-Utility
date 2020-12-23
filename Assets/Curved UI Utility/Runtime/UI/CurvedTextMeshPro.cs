using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CurvedUIUtility
{
    public class CurvedTextMeshPro : TextMeshProUGUI
    {
        private CurvedUIHelper curvedHelper = new CurvedUIHelper();
        private List<Vector3> cachedVertices = new List<Vector3>();
        private List<Vector3> modifiedVertices = new List<Vector3>();

        protected override void OnEnable()
        {
            base.OnEnable();
            curvedHelper.Reset();
        }

        protected override void GenerateTextMesh()
        {
            base.GenerateTextMesh();

            m_mesh.GetVertices(cachedVertices);

            UpdateCurvature();
        }

        private void LateUpdate()
        {
            if (!m_layoutAlreadyDirty)
            {
                UpdateCurvature();
            }
        }

        private void UpdateCurvature()
        {
            curvedHelper.GetCurvedUIController(canvas);
            curvedHelper.PokeScreenSize();

            modifiedVertices.Clear();

            foreach (var v in cachedVertices)
            {
                modifiedVertices.Add(curvedHelper.GetCurvedPosition(rectTransform, v));
            }

            m_mesh.SetVertices(modifiedVertices);

            canvasRenderer.SetMesh(m_mesh);
        }
    }
}
