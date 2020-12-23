using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class Location : MonoBehaviour
    {
        public static event Action<Location> OnPlayerEnterLocation;
        public static event Action<Location> OnPlayerLeaveLocation;

        public string LocationName => locationName;
        public Color LocationColor => locationColor;
        public Vector3 LocationCenter { get; private set; } = Vector3.zero;
        public bool ContainsPlayer { get; private set; } = false;

        [SerializeField] private string locationName;
        [SerializeField] private Color locationColor;

        private int activatedTriggers = 0;

        public void PlayerEnteredLocation()
        {
            activatedTriggers++;

            if (activatedTriggers == 1)
            {
                OnPlayerEnterLocation?.Invoke(this);
                ContainsPlayer = true;
            }
        }

        public void PlayerLeftLocation()
        {
            activatedTriggers--;

            if (activatedTriggers == 0)
            {
                OnPlayerLeaveLocation?.Invoke(this);
                ContainsPlayer = false;
            }

            if (activatedTriggers < 0) activatedTriggers = 0;
        }

        private void OnEnable()
        {
            LocationCenter = GetMeanPosition(GetComponentsInChildren<LocationCollider>().Select(c => c.transform.position));
        }

        private Vector3 GetMeanPosition(IEnumerable<Vector3> positions)
        {
            if (positions.Count() == 0) return Vector3.zero;

            Vector3 meanVector = Vector3.zero;

            foreach (Vector3 pos in positions)
            {
                meanVector += pos;
            }

            return meanVector / positions.Count();
        }

    }
}
