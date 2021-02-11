using CurvedUIUtility;
using UnityEngine;
using UnityEngine.Rendering;

public class InvertedMaskCurvedImage : CurvedImage
{
    private Material cachedInverseMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (cachedInverseMaterial == null)
            {
                cachedInverseMaterial = new Material(base.materialForRendering);
                cachedInverseMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return cachedInverseMaterial;
        }
    }
}