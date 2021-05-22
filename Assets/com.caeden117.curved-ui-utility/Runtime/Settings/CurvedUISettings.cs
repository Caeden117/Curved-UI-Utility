using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CurvedUIUtility
{
    [Serializable]
    public class CurvedUISettings : ICloneable
    {
        public static readonly CurvedUISettings EmptyCurveSettings = new CurvedUISettings();

        [Space(25f)]
        [Header("The Z components for these are unused.")]

        /// <summary>
        /// Curved HUD closer to the center of the screen.
        /// </summary>
        [Tooltip("Curves HUD closer to the center of the screen.")]
        [FormerlySerializedAs("curve")]
        public Vector3 Curve = Vector3.zero;

        /// <summary>
        /// Pulls the HUD towards a certain direction.
        /// </summary>
        [Tooltip("Pulls the HUD towards a certain direction.")]
        [FormerlySerializedAs("pull")]
        public Vector3 Pull = Vector3.zero;


        [Tooltip("General scale of the HUD.")]
        [FormerlySerializedAs("scale")]
        /// <summary>
        /// General scale of the HUD.
        /// </summary>
        public Vector3 Scale = Vector3.one;

        /// <summary>
        /// Added offset to the HUD.
        /// </summary>
        [Tooltip("Added offset to the HUD.")]
        [FormerlySerializedAs("offset")]
        public Vector3 Offset = Vector3.zero;

        public event Action SettingsChanged;

        public object Clone()
        {
            return new CurvedUISettings
            {
                Curve = Curve,
                Pull = Pull,
                Scale = Scale,
                Offset = Offset
            };
        }

        public void Set(Vector3 curve, Vector3 pull, Vector3 scale, Vector3 offset)
        {
            Curve = curve;
            Pull = pull;
            Scale = scale;
            Offset = offset;

            NotifySettingsChanged();
        }

        public void NotifySettingsChanged()
        {
            SettingsChanged?.Invoke();
        }
    }
}
