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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class PlayerControllerManager : MonoBehaviour
    {
        public PlayerController EnabledPlayerController { get; private set; }

        public Action<PlayerController> OnPlayerControllerChanged;

        [SerializeField] private PlayerController beginningPlayerController;
        [SerializeField] private List<PlayerController> allPlayerControllers;
        [SerializeField] private CurvedUIController curvedUIController;

        private Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            mainCamera = Camera.main;
            SetPlayerController(beginningPlayerController);
        }
        
        public void SetPlayerController(PlayerController controller)
        {
            EnabledPlayerController = controller;
            EnabledPlayerController.OnControllerEnable(mainCamera);
            EnabledPlayerController.IsActive = true;

            curvedUIController.SetCurveSettings(EnabledPlayerController.CurvedUISettings.Settings);
            
            foreach (PlayerController toDisable in allPlayerControllers.Where(x => x != controller))
            {
                if (toDisable.IsActive)
                {
                    toDisable.OnControllerDisable();
                }
                toDisable.IsActive = false;
            }

            OnPlayerControllerChanged?.Invoke(EnabledPlayerController);
        }
    }
}
