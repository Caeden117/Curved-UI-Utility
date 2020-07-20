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
using UnityEditorInternal;
using UnityEngine;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class FirstPersonPlayerController : PlayerController
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform cameraRotationTransform;
        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float jumpForce = 10;

        public override void HandleMovement(Vector3 movement)
        {
            if (Mathf.Abs(playerRigidbody.velocity.y) >= 0.2f) movement.y *= 0;
            if (movement.y > 0)
            {
                playerRigidbody.velocity = Vector3.up * jumpForce;
            }
            Vector3 velocity = playerTransform.rotation * movement * movementSpeed;
            playerRigidbody.velocity = new Vector3(velocity.x, playerRigidbody.velocity.y, velocity.z);
        }

        public override void HandleRotation(Vector2 rotation)
        {
            playerTransform.eulerAngles += Vector3.up * rotation.x * rotationSpeed;

            float newX = cameraRotationTransform.transform.localEulerAngles.x + (rotation.y * -1 * rotationSpeed);
            cameraRotationTransform.localEulerAngles = Vector3.right * newX;
        }

        public override void OnControllerEnable(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            StartCoroutine(TransitionToPlayer());
            curvedUIController.SetUICurve(0.3f);
        }

        protected override void PlayerControllerUpdate()
        {
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        private IEnumerator TransitionToPlayer()
        {
            AnimationCurve curveTransition = AnimationCurve.EaseInOut(0, 0, 1, 1);
            mainCamera.transform.SetParent(null);

            float t = 0;
            Vector3 oldPosition = mainCamera.transform.position;
            Quaternion oldRotation = mainCamera.transform.rotation;

            while (t < 1)
            {
                Vector3 newPosition = transform.TransformPoint(new Vector3(0, 0.85f, 0));
                Vector3 position = Vector3.Lerp(oldPosition, newPosition, curveTransition.Evaluate(t));
                mainCamera.transform.position = position;

                Quaternion newRotation = cameraRotationTransform.rotation;
                Quaternion rotation = Quaternion.Lerp(oldRotation, newRotation, curveTransition.Evaluate(t));
                mainCamera.transform.localRotation = rotation;

                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            mainCamera.transform.SetParent(cameraRotationTransform, false);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
