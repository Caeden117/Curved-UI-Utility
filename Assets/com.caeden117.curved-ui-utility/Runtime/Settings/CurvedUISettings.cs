using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CurvedUIUtility
{
    [Serializable]
    public class CurvedUISettings : ICloneable, INotifyPropertyChanged
    {
        public static readonly CurvedUISettings EmptyCurveSettings = new CurvedUISettings();

        private static readonly PropertyChangedEventArgs blankArgs = new PropertyChangedEventArgs("");

        [Space(25f)]
        [Header("The Z components for these are unused.")]

        [Tooltip("Curves HUD closer to the center of the screen.")]
        [SerializeField] private Vector3 curve = Vector3.zero;

        /// <summary>
        /// Curved HUD closer to the center of the screen.
        /// </summary>
        public Vector3 Curve
        {
            get => curve;
            set
            {
                curve = value;
                UsingCurve = value.magnitude > 0.01f;
                NotifyPropertyChanged();
            }
        }

        public bool UsingCurve { get; private set; } = false;

        [Tooltip("Pulls the HUD towards a certain direction.")]
        [SerializeField] private Vector3 pull = Vector3.zero;

        /// <summary>
        /// Pulls the HUD towards a certain direction.
        /// </summary>
        public Vector3 Pull
        {
            get => pull;
            set
            {
                pull = value;
                UsingPull = value.magnitude > 0.01f;
                NotifyPropertyChanged();
            }
        }

        public bool UsingPull { get; private set; } = false;

        [Tooltip("General scale of the HUD.")]
        [SerializeField] private Vector3 scale = Vector3.one;

        /// <summary>
        /// General scale of the HUD.
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                UsingScale = value.magnitude > 0.01f;
                NotifyPropertyChanged();
            }
        }

        public bool UsingScale { get; private set; } = false;

        [Tooltip("Added offset to the HUD.")]
        [SerializeField] private Vector3 offset = Vector3.zero;
        /// <summary>
        /// Added offset to the HUD.
        /// </summary>
        public Vector3 Offset
        {
            get => offset;
            set
            {
                offset = value;
                UsingOffset = value.magnitude > 0.01f;
                NotifyPropertyChanged();
            }
        }

        public bool UsingOffset { get; private set; } = false;


        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            var @new = new CurvedUISettings
            {
                curve = curve,
                pull = pull,
                scale = scale,
                offset = offset
            };
            @new.RefreshBooleans();

            return @new;
        }

        public void Set(Vector3 curve, Vector3 pull, Vector3 scale, Vector3 offset)
        {
            this.curve = curve;
            this.pull = pull;
            this.scale = scale;
            this.offset = offset;

            RefreshBooleans();

            NotifyPropertyChanged();
        }

        public void RefreshBooleans()
        {
            UsingCurve = curve.magnitude != 0;
            UsingPull = pull.magnitude != 0;
            UsingScale = scale.magnitude != 0;
            UsingOffset = offset.magnitude != 0;
        }

        public void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, blankArgs);
        }
    }
}
