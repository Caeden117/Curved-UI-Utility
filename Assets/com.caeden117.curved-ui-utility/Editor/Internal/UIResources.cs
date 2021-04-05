using UnityEditor;
using UnityEngine;

namespace CurvedUIUtility.Editor
{
    internal class UIResources
    {
        public const float Width = 160f;
        public const float ThickHeight = 30f;
        public const float ThinHeight = 20f;
        public static readonly Vector2 TextElementSize = new Vector2(100f, 100f);
        public static readonly Vector2 ThickElementSize = new Vector2(Width, ThickHeight);
        public static readonly Vector2 ThinElementSize = new Vector2(Width, ThinHeight);
        public static readonly Color DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        public static readonly Color TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        private const string standardSpritePath = "UI/Skin/UISprite.psd";
        private const string backgroundSpritePath = "UI/Skin/Background.psd";
        private const string inputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string knobPath = "UI/Skin/Knob.psd";
        private const string checkmarkPath = "UI/Skin/Checkmark.psd";
        private const string dropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string maskPath = "UI/Skin/UIMask.psd";

        public static UIResources DefaultResources = new UIResources()
        {
            Standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(standardSpritePath),
            Background = AssetDatabase.GetBuiltinExtraResource<Sprite>(backgroundSpritePath),
            InputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(inputFieldBackgroundPath),
            Knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(knobPath),
            Checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(checkmarkPath),
            Dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(dropdownArrowPath),
            Mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(maskPath),
        };

        public Sprite Standard;
        public Sprite Background;
        public Sprite InputField;
        public Sprite Knob;
        public Sprite Checkmark;
        public Sprite Dropdown;
        public Sprite Mask;
    }
}
