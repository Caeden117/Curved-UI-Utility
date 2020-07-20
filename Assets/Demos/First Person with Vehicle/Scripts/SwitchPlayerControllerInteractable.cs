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
using TMPro;
using UnityEngine;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    [RequireComponent(typeof(Collider))]
    public class SwitchPlayerControllerInteractable : MonoBehaviour
    {
        [SerializeField] private PlayerControllerManager controllerManager;
        [SerializeField] private PlayerController controllerToSwap;
        [SerializeField] private TextMeshProUGUI interactionText;

        private bool inTrigger = false;

        private void OnTriggerEnter(Collider other)
        {
            interactionText.gameObject.SetActive(true);
            inTrigger = true;
        }

        private void OnTriggerExit(Collider collision)
        {
            interactionText.gameObject.SetActive(false);
            inTrigger = false;
        }

        private void Update()
        {
            if (controllerManager.EnabledPlayerController == controllerToSwap)
            {
                interactionText.gameObject.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.E) && controllerManager.EnabledPlayerController != controllerToSwap && inTrigger)
            {
                controllerManager.SetPlayerController(controllerToSwap);
            }
        }
    }
}
