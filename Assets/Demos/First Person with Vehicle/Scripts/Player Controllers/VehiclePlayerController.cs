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

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class VehiclePlayerController : PlayerController
    {
        [SerializeField] private PlayerControllerManager controllerManager;
        [SerializeField] private PlayerController firstPersonController;
        [SerializeField] private Transform playerEjectionPoint;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform cameraFollowPoint;
        [SerializeField] private GameObject vehicleEnterTrigger;
        [SerializeField] private float accelleration = 1;
        [SerializeField] private float torqueForce = 10;
        [SerializeField] private float jumpForce = 10;

        private float timeInVehicle = 0;

        /*
         * This is absolutely not how any sane person should implement vehicle movement in Unity.
         * 
         * But hey, I'm developing a package for curving HUD, not a realistic driving simulator.
         */
        public override void HandleMovement(Vector3 movement)
        {
            if (Mathf.Abs(playerRigidbody.velocity.y) >= 0.2f) movement.y *= 0;
            if (movement.y > 0)
            {
                playerRigidbody.velocity = Vector3.up * jumpForce;
            }
            Vector3 velocity = playerTransform.rotation * (Vector3.forward * movement.z) * accelleration;
            playerRigidbody.AddForce(velocity, ForceMode.Force);
            playerRigidbody.AddTorque(Vector3.up * movement.x * torqueForce, ForceMode.Force);
        }

        public override void HandleRotation(Vector2 rotation)
        {
        }

        public override void OnControllerEnable(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            mainCamera.transform.SetParent(null);

            firstPersonController.transform.parent.gameObject.SetActive(false);
            timeInVehicle = Time.time;
        }

        protected override void PlayerControllerUpdate()
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.time - timeInVehicle >= 0.5f)
            {
                firstPersonController.transform.parent.gameObject.SetActive(true);
                firstPersonController.transform.parent.position = playerEjectionPoint.position;
                firstPersonController.transform.forward = transform.forward;
                controllerManager.SetPlayerController(firstPersonController);
            }

            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                cameraFollowPoint.position, Time.deltaTime * 10);
            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                cameraFollowPoint.rotation, Time.deltaTime * 10);
        }

        public override void OnControllerDisable()
        {
            StartCoroutine(TemporarilyHideTrigger());
        }

        private IEnumerator TemporarilyHideTrigger()
        {
            vehicleEnterTrigger.SetActive(false);
            yield return new WaitForSeconds(0.25f);
            vehicleEnterTrigger.SetActive(true);
        }
    }
}
