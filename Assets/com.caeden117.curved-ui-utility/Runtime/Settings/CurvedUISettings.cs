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
                NotifyPropertyChanged();
            }
        }

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
                NotifyPropertyChanged();
            }
        }
        
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
                NotifyPropertyChanged();
            }
        }
        
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
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            var @new = new CurvedUISettings();
            @new.curve = curve;
            @new.pull = pull;
            @new.scale = scale;
            @new.offset = offset;
            return @new;
        }

        public void Set(Vector3 curve, Vector3 pull, Vector3 scale, Vector3 offset)
        {
            this.curve = curve;
            this.pull = pull;
            this.scale = scale;
            this.offset = offset;

            NotifyPropertyChanged();
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
