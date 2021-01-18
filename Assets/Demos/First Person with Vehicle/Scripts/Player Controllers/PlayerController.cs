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
    public abstract class PlayerController : MonoBehaviour
    {
        public Rigidbody PlayerRigidbody => playerRigidbody;

        public bool IsActive = false;
        public float MaximumSpeed = 10;
        public CurvedUISettingsObject CurvedUISettings;

        [SerializeField] protected Rigidbody playerRigidbody;
        [SerializeField] protected CurvedUIController curvedUIController;

        protected Camera mainCamera;

        private void Update()
        {
            if (!IsActive) return;
            Vector3 movementVector = Vector3.zero;
            
            // Now, if this was an actual game I am making, this would be done through the new Unity Input System.
            // But to simplify the demo, we are using legacy input.
            if (Input.GetKey(KeyCode.W))
            {
                movementVector.z = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                movementVector.z = -1;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                movementVector.y = 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                movementVector.x = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                movementVector.x = 1;
            }

            HandleMovement(movementVector);

            Vector2 mouseRotation = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y"));

            HandleRotation(mouseRotation);

            PlayerControllerUpdate();
        }

        private void LateUpdate()
        {
            // I use absolute value to prevent backwards long jumping from the Source engine LULW
            Vector2 flatVelocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.z);
            if (Mathf.Abs(flatVelocity.magnitude) > MaximumSpeed)
            {
                flatVelocity = flatVelocity.normalized * MaximumSpeed;
                playerRigidbody.velocity = new Vector3(flatVelocity.x, playerRigidbody.velocity.y, flatVelocity.y);
            }
            if (!IsActive) return;
            PlayerControllerLateUpdate();
        }

        protected virtual void PlayerControllerUpdate() { }
        protected virtual void PlayerControllerLateUpdate() { }
        public virtual void OnControllerDisable() { }

        public abstract void OnControllerEnable(Camera mainCamera);
        public abstract void HandleMovement(Vector3 movement);
        public abstract void HandleRotation(Vector2 rotation);
    }
}
