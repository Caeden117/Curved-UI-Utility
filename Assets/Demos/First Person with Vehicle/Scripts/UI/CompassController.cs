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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class CompassController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        // Yes, the amount of cardinal directions I have in this list is greater than 4.
        // It's so that I can take advantage of some Unity quirks to mimick a "wrap around" effect.
        [SerializeField] private List<TextMeshProUGUI> cardinalDirections;
        [SerializeField] private float textScale;
        [SerializeField] private int ticksInBetweenCardinalDirections = 4;
        [SerializeField] private TextMeshProUGUI tick;

        private List<TextMeshProUGUI> allTexts = new List<TextMeshProUGUI>();
        private List<float> startingAnchoredXs = new List<float>();

        private void Start()
        {
            int halfIndex = Mathf.FloorToInt((cardinalDirections.Count / 2) - 0.1f);
            for (int i = 0; i < cardinalDirections.Count; i++)
            {
                TextMeshProUGUI direction = cardinalDirections[i];
                float anchoredX = (i - halfIndex) * textScale;
                direction.rectTransform.anchoredPosition += new Vector2(anchoredX, 0);

                allTexts.Add(direction);
                startingAnchoredXs.Add(anchoredX);

                for (int j = 1; j <= ticksInBetweenCardinalDirections; j++)
                {
                    TextMeshProUGUI newTick = Instantiate(tick.gameObject, transform).GetComponent<TextMeshProUGUI>();
                    float tickX = anchoredX + (textScale * ((float)j / (ticksInBetweenCardinalDirections + 1)));
                    newTick.rectTransform.anchoredPosition += new Vector2(tickX, 0);
                    allTexts.Add(newTick);
                    startingAnchoredXs.Add(tickX);
                }
            }
            tick.gameObject.SetActive(false);
        }

        private void Update()
        {
            float y = mainCamera.transform.eulerAngles.y * -1;
            for (int i = 0; i < allTexts.Count; i++)
            {
                TextMeshProUGUI text = allTexts[i];
                float baseAnchoredPosX = startingAnchoredXs[i];
                text.rectTransform.anchoredPosition *= new Vector2(0, 1);

                float newAnchoredPosX = baseAnchoredPosX + (y / 90 * textScale);
                text.rectTransform.anchoredPosition += new Vector2(newAnchoredPosX, 0);
            }
        }
    }
}
