﻿/*
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
using UnityEngine.UI;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class SpeedometerController : MonoBehaviour
    {
        [SerializeField] private PlayerControllerManager controllerManager;
        [SerializeField] private Image speedMeasurement;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private float maxSpeedForMeasurement;

        private PlayerController activePlayerController;
        private float lastSpeed = 0;

        private void Start()
        {
            controllerManager.OnPlayerControllerChanged += PlayerControllerChanged;
        }

        private void PlayerControllerChanged(PlayerController controller)
        {
            activePlayerController = controller;
        }

        private void LateUpdate()
        {
            float speed = activePlayerController.PlayerRigidbody.velocity.magnitude;
            speedText.text = speed.ToString("F1");
            
            float lerpedSpeed = Mathf.Lerp(lastSpeed, speed, 0.1f);
            speedMeasurement.fillAmount = Mathf.Clamp(lerpedSpeed / maxSpeedForMeasurement / 2f, 0, 0.5f);
            
            lastSpeed = lerpedSpeed;
        }

        private void OnDestroy()
        {
            controllerManager.OnPlayerControllerChanged -= PlayerControllerChanged;
        }
    }
}
