using System;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CurvedUIUtility.Editor
{
    public static class CreateObjectsMenu
    {
        private const string standardSprite = "UI/Skin/UISprite.psd";
        private const string backgroundSprite = "UI/Skin/Background.psd";
        private const string inputFieldBackground = "UI/Skin/InputFieldBackground.psd";
        private const string knobPath = "UI/Skin/Knob.psd";
        private const string checkmarkPark = "UI/Skin/Checkmark.psd";
        private const string dropdownArrow = "UI/Skin/DropdownArrow.psd";
        private const string mask = "UI/Skin/UIMask.psd";

        private static TMP_DefaultControls.Resources StandardResources = new TMP_DefaultControls.Resources()
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(standardSprite),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>(backgroundSprite),
            inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(inputFieldBackground),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(knobPath),
            checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(checkmarkPark),
            dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(dropdownArrow),
            mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(mask),
        };

        [MenuItem("GameObject/UI/Curved UI Utility/Curved Text", false, 100)]
        public static void CreateCurvedCanvas(MenuCommand command)
        {
            Debug.Log("fuck");
        }
    }
}