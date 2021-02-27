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
        public CurvedUISettings CurrentCurveSettings
        {
            get
            {
                if (currentCurveSettings != null && Application.isPlaying) return currentCurveSettings;

                switch (settingsSource)
                {
                    case SettingsSource.FromScriptableObject:
                        return startingCurveObject?.Settings ?? CurvedUISettings.EmptyCurveSettings;
                    case SettingsSource.FromStartingSettings:
                        return startingCurveSettings;
                    default:
                        return CurvedUISettings.EmptyCurveSettings;
                }
            }
        }


        public event Action CurveSettingsChangedEvent;

        [SerializeField] private AnimationCurve curveTransition = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private SettingsSource settingsSource = SettingsSource.FromStartingSettings;
        [SerializeField] private CurvedUISettings startingCurveSettings = new CurvedUISettings();
        [SerializeField] private CurvedUISettingsObject startingCurveObject = null;

        private CurvedUISettings currentCurveSettings = null;

        private void OnValidate()
        {
            if (currentCurveSettings != null) currentCurveSettings.PropertyChanged -= CurrentCurveSettings_PropertyChanged;
            Awake();
        }

        private void Awake()
        {
            currentCurveSettings = CurrentCurveSettings;
            currentCurveSettings.RefreshBooleans();

            currentCurveSettings.PropertyChanged += CurrentCurveSettings_PropertyChanged;
            CurveSettingsChangedEvent?.Invoke();
        }

        public void SetCurveSettings(CurvedUISettings newSettings, float transitionTime = 1f)
        {
            StopAllCoroutines();

            if (transitionTime <= 0)
            {
                SetCurveSettingsInstant(newSettings);
                return;
            }

            StartCoroutine(AnimateSettingsChange(newSettings, transitionTime));
        }

        public void SetCurveSettingsInstant(CurvedUISettings newSettings)
        {
            currentCurveSettings.PropertyChanged -= CurrentCurveSettings_PropertyChanged;
            currentCurveSettings = newSettings;
            currentCurveSettings.PropertyChanged += CurrentCurveSettings_PropertyChanged;
            CurveSettingsChangedEvent?.Invoke();
        }

        private void CurrentCurveSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CurveSettingsChangedEvent?.Invoke();
        }

        private IEnumerator AnimateSettingsChange(CurvedUISettings target, float time)
        {
            var t = 0f;
            var original = currentCurveSettings.Clone() as CurvedUISettings;
            
            // Creating a new copy so that the current settings (if they happen to be a scriptable object) aren't overwritten.
            SetCurveSettingsInstant(original.Clone() as CurvedUISettings);

            var cachedCurve = Vector3.zero;
            var cachedPull = Vector3.zero;
            var cachedScale = Vector3.zero;
            var cachedOffset = Vector3.zero;

            while (t < 1)
            {
                var animatedT = curveTransition.Evaluate(t);

                LerpVectorsIntoResult(ref cachedCurve, original.Curve, target.Curve, animatedT);
                LerpVectorsIntoResult(ref cachedPull, original.Pull, target.Pull, animatedT);
                LerpVectorsIntoResult(ref cachedScale, original.Scale, target.Scale, animatedT);
                LerpVectorsIntoResult(ref cachedOffset, original.Offset, target.Offset, animatedT);
                
                currentCurveSettings.Set(cachedCurve, cachedPull, cachedScale, cachedOffset);

                t += Time.deltaTime / time;
                yield return new WaitForEndOfFrame();
            }

            SetCurveSettingsInstant(target.Clone() as CurvedUISettings);
        }

        private void LerpVectorsIntoResult(ref Vector3 res, Vector3 a, Vector3 b, float t)
        {
            res.Set(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
    }
}
