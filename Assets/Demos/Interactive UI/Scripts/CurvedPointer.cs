using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CurvedPointer : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
#if UNITY_EDITOR
        Vector2 gameSize = UnityEditor.Handles.GetMainGameViewSize();
        float screenWidth = gameSize.x;
        float screenHeight = gameSize.y;
#else
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
#endif

        Vector2 clamped = new Vector2(
            Mathf.Clamp(Input.mousePosition.x, 0, screenWidth),
            Mathf.Clamp(Input.mousePosition.y, 0, screenHeight)
            );
        transform.position = clamped;
    }
}
