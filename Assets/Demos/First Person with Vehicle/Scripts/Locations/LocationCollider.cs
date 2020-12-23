using System;
using UnityEngine;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    [RequireComponent(typeof(Collider))]
    public class LocationCollider : MonoBehaviour
    {
        [SerializeField] private PlayerControllerManager playerControllerManager;

        private void OnEnable()
        {
            playerControllerManager.OnPlayerControllerChanged += OnPlayerControllerChanged;
        }

        private void OnPlayerControllerChanged(PlayerController obj)
        {
            if (obj.GetType() == typeof(FirstPersonPlayerController)) SendMessageUpwards("PlayerLeftLocation");
        }

        private void OnDisable()
        {
            playerControllerManager.OnPlayerControllerChanged -= OnPlayerControllerChanged;
        }

        public void OnTriggerEnter(Collider other)
        {
            var controller = GetPlayerController(other);

            if (controller == null) return;

            if (controller == playerControllerManager.EnabledPlayerController)
            {
                SendMessageUpwards("PlayerEnteredLocation");
            }
        }

        public void OnTriggerExit(Collider other)
        {
            var controller = GetPlayerController(other);

            if (controller == null) return;

            if (controller == playerControllerManager.EnabledPlayerController)
            {
                SendMessageUpwards("PlayerLeftLocation");
            }
        }

        private PlayerController GetPlayerController(Collider other)
        {
            var controller = other.GetComponentInChildren<PlayerController>();

            if (controller == null)
            {
                controller = other.GetComponentInParent<PlayerController>();
            }

            if (controller == null)
            {
                return null;
            }
            else
            {
                return controller;
            }
        }
    }
}
