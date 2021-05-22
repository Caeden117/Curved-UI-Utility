using UnityEngine;

namespace CurvedUIUtility
{
    [CreateAssetMenu(fileName = "New Curved UI Settings", menuName = "Curved UI Utility/Curved UI Settings Object")]
    public class CurvedUISettingsObject : ScriptableObject
    {
        public CurvedUISettings Settings;

        private void OnValidate()
        {
            Settings.NotifySettingsChanged();
        }
    }
}