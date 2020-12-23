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
        [SerializeField] private RectTransform cardinalDirectionTransform;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI waypointMarker;
        [SerializeField] private Location[] locations;
        [SerializeField] private PlayerControllerManager playerControllerManager;

        private List<(Location, List<TextMeshProUGUI>)> locationWaypoints = new List<(Location, List<TextMeshProUGUI>)>();

        private void Start()
        {
            var halfIndex = Mathf.FloorToInt((cardinalDirections.Count / 2) - 0.1f);

            for (int i = 0; i < cardinalDirections.Count; i++)
            {
                var direction = cardinalDirections[i];
                var anchoredX = (i - halfIndex) * textScale;
                direction.rectTransform.anchoredPosition += new Vector2(anchoredX, 0);

                for (int j = 1; j <= ticksInBetweenCardinalDirections; j++)
                {
                    var newTick = Instantiate(tick.gameObject, tick.transform.parent).GetComponent<TextMeshProUGUI>();
                    var tickX = anchoredX + (textScale * ((float)j / (ticksInBetweenCardinalDirections + 1)));
                    newTick.rectTransform.anchoredPosition += new Vector2(tickX, 0);
                }
            }

            for (int i = 0; i < locations.Length; i++)
            {
                var location = locations[i];
                var markers = new List<TextMeshProUGUI>();
                
                for (int j = 0; j < 2; j++)
                {
                    var marker = Instantiate(waypointMarker.gameObject, waypointMarker.transform.parent).GetComponent<TextMeshProUGUI>();
                    marker.color = location.LocationColor;
                    markers.Add(marker);
                }

                locationWaypoints.Add((location, markers));
            }

            waypointMarker.gameObject.SetActive(false);
            tick.gameObject.SetActive(false);
        }

        private void Update()
        {
            var y = mainCamera.transform.eulerAngles.y * -1;
            var newAnchoredPosX = y / 90 * textScale;
            var loop = textScale * 4;
            var controller = playerControllerManager.EnabledPlayerController;

            cardinalDirectionTransform.anchoredPosition = new Vector2(newAnchoredPosX, cardinalDirectionTransform.anchoredPosition.y);

            foreach (var waypoint in locationWaypoints)
            {
                var location = waypoint.Item1;
                var targetDir = location.LocationCenter - controller.transform.position;
                targetDir.Set(targetDir.x, 0, targetDir.z);
                var forward = mainCamera.transform.forward;
                forward.Set(forward.x, 0, forward.z);

                float angle = Vector3.SignedAngle(targetDir.normalized, forward, Vector3.up) * -1;

                float waypointAnchoredPosX = angle / 90 * textScale;

                for (int i = 0; i < waypoint.Item2.Count; i++)
                {
                    var text = waypoint.Item2[i];
                    var color = text.color;

                    color.a = Mathf.Lerp(text.color.a, location.ContainsPlayer ? 0 : 1, Time.deltaTime);
                    text.color = color;

                    text.rectTransform.anchoredPosition = new Vector2(
                        (loop * i) + waypointAnchoredPosX,
                        text.rectTransform.anchoredPosition.y);
                }
            }
        }

        private float BetterModulo(float x, float m) => (x % m + m) % m; // thanks stackoverflow
    }
}
