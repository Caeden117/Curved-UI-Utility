/*
 * Released under the MIT License
 *
 * Copyright (c) 2020 Caeden Statia
 * https://caeden.dev/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using UnityEditor;
using UnityEngine;

namespace CurvedUIUtility
{
    /*
     * The CurvedUIRenderer takes the Camera its attached to and prepares it for use in the CurvedUIController.
     * 
     * The camera is forced to render to a RenderTexture, which is updated whenever the resolution
     * of the screen changes.
     * 
     * The camera is also forced to render nothing but the UI to save on performance.
     */
    [RequireComponent(typeof(Camera))]
    public class CurvedUIRenderer : MonoBehaviour
    {
        public RenderTexture RenderTexture { get; private set; }
        public Action OnDimensionsChanged;

        private Vector2 oldGameSize = Vector2.zero;
        private Camera attachedCamera;

        // Start is called before the first frame update
        void Start()
        {
            attachedCamera = GetComponent<Camera>();
            oldGameSize = GetGameViewSize();
            attachedCamera.forceIntoRenderTexture = true;
            attachedCamera.cullingMask = 1 << 5;
            RegenerateRenderTexture();
        }

        // Update is called once per frame
        void Update()
        {
            if (GetGameViewSize() != oldGameSize)
            {
                RegenerateRenderTexture();
                OnDimensionsChanged?.Invoke();
            }
        }

        private void OnDestroy()
        {
            RenderTexture.DiscardContents();
            Destroy(RenderTexture);
        }

        private void RegenerateRenderTexture()
        {
            // Prevent memory leaking by discarding and destroying our existing render texture.
            if (RenderTexture != null)
            {
                RenderTexture.DiscardContents();
                Destroy(RenderTexture);
            }
            // Create a new one.
            RenderTexture = new RenderTexture(Mathf.CeilToInt(oldGameSize.x), Mathf.CeilToInt(oldGameSize.y), 24);
            attachedCamera.targetTexture = RenderTexture;
        }

        private Vector2 GetGameViewSize()
        {
#if UNITY_EDITOR
            Vector2 gameSize = Handles.GetMainGameViewSize();
            return gameSize;
#else
            return new Vector2(Screen.width, Screen.height);
#endif
        }
    }
}


