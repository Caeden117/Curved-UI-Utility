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
using System.Collections;
using UnityEngine;

namespace CurvedUIUtility
{
    /*
     * The CurvedUIController exposes public methods for controlling your curved UI.
     */
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class CurvedUIController : MonoBehaviour
    {

        [Tooltip("On Start, automatically set UI curve to this amount.\nRecommended values: 0.1 - 0.3")]
        public float StartingCurve = 0;
        [Tooltip("Easing for the default UI curve transition.")]
        [SerializeField] private AnimationCurve curveTransition = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public float CurrentCurve { get; private set; } = 0;

        public Action<float> CurveChangedEvent;

        private void Start()
        {
            SetCurveInstant(StartingCurve);
        }

        /// <summary>
        /// Sets the curve for the <see cref="curvedUIRenderer"/>, and animates the transition.
        /// </summary>
        /// <param name="curve">Amount of curvature.</param>
        /// <param name="transitionTime">Transition time to animate between the current and new curvature.</param>
        public void SetUICurve(float curve, float transitionTime = 1f)
        {
            // Stop any coroutines, which include the curvature animation.
            StopAllCoroutines();
            if (transitionTime <= 0)
            {
                SetCurveInstant(curve);
                return;
            }
            StartCoroutine(SetZoom(curve, transitionTime));
        }

        /// <summary>
        /// Instantaneously sets the curve for the <see cref="curvedUIRenderer"/>.
        /// </summary>
        /// <param name="curve">Amount of curvature.</param>
        public void SetCurveInstant(float curve)
        {
            CurrentCurve = curve;
            CurveChangedEvent?.Invoke(curve);
        }

        // Internal IEnumerator that animates the curvature change.
        private IEnumerator SetZoom(float targetZoom, float transitionTime)
        {
            float t = 0;
            float oldZoom = CurrentCurve;
            while (t < 1)
            {
                float zoom = Mathf.Lerp(oldZoom, targetZoom, curveTransition.Evaluate(t));
                t += Time.deltaTime / transitionTime;
                SetCurveInstant(zoom);
                yield return new WaitForEndOfFrame();
            }
            SetCurveInstant(targetZoom);
        }
    }
}
