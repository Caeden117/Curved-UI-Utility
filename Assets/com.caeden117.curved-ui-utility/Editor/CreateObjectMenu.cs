using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility.Editor
{
    public static class CreateObjectMenu
    {
        [MenuItem("GameObject/UI/Curved UI Utility/Curved Image", false, 100)]
        public static void CreateCurvedImage(MenuCommand menuCommand)
        {
            using (var uiBase = new UIBase("Curved Image", menuCommand, UIResources.TextElementSize))
            {
                var image = uiBase.GameObject.AddComponent<CurvedImage>();
                image.sprite = UIResources.DefaultResources.Standard;

                uiBase.GameObject.AddComponent<CurveComponent>();
            }
        }

        [MenuItem("GameObject/UI/Curved UI Utility/Curved Text", false, 200)]
        public static void CreateCurvedText(MenuCommand menuCommand)
        {
            using (var uiBase = new UIBase("Curved TextMeshPro", menuCommand, UIResources.TextElementSize))
            {
                var text = uiBase.GameObject.AddComponent<CurvedTextMeshPro>();
                text.text = "New Text";
                text.fontSize = 14;
                text.color = UIResources.TextColor;
            }
        }

        [MenuItem("GameObject/UI/Curved UI Utility/Button", false, 300)]
        public static void CreateButton(MenuCommand menuCommand)
        {
            using (var uiBase = new UIBase("Button", menuCommand, UIResources.ThickElementSize))
            {
                var image = uiBase.GameObject.AddComponent<CurvedImage>();
                image.sprite = UIResources.DefaultResources.Standard;
                image.type = Image.Type.Sliced;
                image.color = UIResources.DefaultSelectableColor;

                uiBase.AddChildObject<CurvedTextMeshPro>("Curved Text", (objectBuilder) =>
                {
                    objectBuilder.RectTransform.anchorMin = Vector2.zero;
                    objectBuilder.RectTransform.anchorMax = Vector2.one;
                    objectBuilder.RectTransform.sizeDelta = Vector2.zero;

                    var text = objectBuilder.Component;
                    text.text = "Button";
                    text.alignment = TMPro.TextAlignmentOptions.Center;
                    text.fontSize = 24;
                    text.color = UIResources.TextColor;
                });

                var button = uiBase.GameObject.AddComponent<Button>();
                SetDefaultColorTransitionValues(button);

                uiBase.GameObject.AddComponent<CurveComponent>();
            }
        }

        [MenuItem("GameObject/UI/Curved UI Utility/Input Field", false, 400)]
        public static void CreateInputField(MenuCommand menuCommand)
        {
            using (var uiBase = new UIBase("Input Field", menuCommand, UIResources.ThickElementSize))
            {
                var image = uiBase.GameObject.AddComponent<CurvedImage>();
                image.sprite = UIResources.DefaultResources.InputField;
                image.type = Image.Type.Sliced;
                image.color = UIResources.DefaultSelectableColor;

                var inputField = uiBase.GameObject.AddComponent<TMP_InputField>();

                uiBase.AddChildObject<CurvedImage>("Text Area", (textArea) =>
                {
                    textArea.RectTransform.anchorMin = Vector2.zero;
                    textArea.RectTransform.anchorMax = Vector2.one;
                    textArea.RectTransform.sizeDelta = Vector2.zero;
                    textArea.RectTransform.offsetMin = new Vector2(10, 6);
                    textArea.RectTransform.offsetMax = new Vector2(-10, -7);

                    textArea.Component.sprite = UIResources.DefaultResources.Mask;
                    textArea.Component.type = Image.Type.Sliced;


                    var mask = textArea.GameObject.AddComponent<Mask>();
                    mask.showMaskGraphic = false;
                    
                    textArea.GameObject.AddComponent<CurveComponent>();

                    textArea.AddChildObject<CurvedTextMeshPro>("Placeholder", (placeholder) =>
                    {
                        placeholder.Component.text = "Enter text...";
                        placeholder.Component.enableWordWrapping = false;
                        placeholder.Component.extraPadding = true;
                        placeholder.Component.fontSize = 14;
                        placeholder.Component.fontStyle = FontStyles.Italic;

                        placeholder.RectTransform.anchorMin = Vector2.zero;
                        placeholder.RectTransform.anchorMax = Vector2.one;
                        placeholder.RectTransform.sizeDelta = Vector2.zero;
                        placeholder.RectTransform.offsetMin = Vector2.zero;
                        placeholder.RectTransform.offsetMax = Vector2.zero;

                        var textColor = UIResources.TextColor;
                        textColor.a *= 0.5f;
                        placeholder.Component.color = textColor;

                        placeholder.GameObject.AddComponent<LayoutElement>().ignoreLayout = true;

                        inputField.placeholder = placeholder.Component;
                    })
                    .AddChildObject<CurvedTextMeshPro>("Text", (text) =>
                    {
                        text.Component.text = "";
                        text.Component.enableWordWrapping = false;
                        text.Component.extraPadding = true;
                        text.Component.richText = true;
                        text.Component.fontSize = 14;
                        text.Component.color = UIResources.TextColor;

                        text.RectTransform.anchorMin = Vector2.zero;
                        text.RectTransform.anchorMax = Vector2.one;
                        text.RectTransform.sizeDelta = Vector2.zero;
                        text.RectTransform.offsetMin = Vector2.zero;
                        text.RectTransform.offsetMax = Vector2.zero;

                        inputField.textComponent = text.Component;
                        inputField.fontAsset = text.Component.font;
                    });

                    inputField.textViewport = textArea.RectTransform;
                });

                SetDefaultColorTransitionValues(inputField);

                uiBase.GameObject.AddComponent<CurveComponent>();
            }
        }

        [MenuItem("GameObject/UI/Curved UI Utility/Dropdown", false, 500)]
        public static void CreateDropdown(MenuCommand menuCommand)
        {
            using (var uiBase = new UIBase("Dropdown", menuCommand, UIResources.ThickElementSize))
            {
                var image = uiBase.GameObject.AddComponent<CurvedImage>();
                image.sprite = UIResources.DefaultResources.InputField;
                image.type = Image.Type.Sliced;
                image.color = UIResources.DefaultSelectableColor;

                var dropdown = uiBase.GameObject.AddComponent<TMP_Dropdown>();
                dropdown.targetGraphic = image;

                uiBase.AddChildObject<CurvedTextMeshPro>("Label", (label) =>
                {
                    label.Component.fontSize = 14;
                    label.Component.color = UIResources.TextColor;
                    label.Component.alignment = TextAlignmentOptions.Left;

                    label.RectTransform.anchorMin = Vector2.zero;
                    label.RectTransform.anchorMax = Vector2.one;
                    label.RectTransform.offsetMin = new Vector2(10, 6);
                    label.RectTransform.offsetMax = new Vector2(-25, -7);

                    dropdown.captionText = label.Component;
                })
                .AddChildObject<CurvedImage>("Arrow", (arrow) =>
                {
                    arrow.Component.sprite = UIResources.DefaultResources.Dropdown;

                    arrow.RectTransform.anchorMin = new Vector2(1, 0.5f);
                    arrow.RectTransform.anchorMax = new Vector2(1, 0.5f);
                    arrow.RectTransform.sizeDelta = new Vector2(20, 20);
                    arrow.RectTransform.anchoredPosition = new Vector2(-15, 0);
                    
                    arrow.GameObject.AddComponent<CurveComponent>();
                })
                .AddChildObject<CurvedImage>("Template", (template) =>
                {
                    template.RectTransform.anchorMin = new Vector2(0, 0);
                    template.RectTransform.anchorMax = new Vector2(1, 0);
                    template.RectTransform.pivot = new Vector2(0.5f, 1);
                    template.RectTransform.anchoredPosition = new Vector2(0, 2);
                    template.RectTransform.sizeDelta = new Vector2(0, 150);

                    template.Component.sprite = UIResources.DefaultResources.Standard;
                    template.Component.type = Image.Type.Sliced;

                    var scrollRect = template.GameObject.AddComponent<ScrollRect>();

                    template.AddChildObject<Scrollbar>("Scrollbar", (scrollbar) =>
                    {
                        scrollbar.RectTransform.sizeDelta = UIResources.ThinElementSize;

                        var scrollbarImage = scrollbar.GameObject.AddComponent<CurvedImage>();
                        scrollbarImage.sprite = UIResources.DefaultResources.Standard;
                        scrollbarImage.type = Image.Type.Sliced;
                        scrollbarImage.color = UIResources.DefaultSelectableColor;

                        scrollbar.AddChildObject("Sliding Area", (sliderArea) =>
                        {
                            sliderArea.RectTransform.sizeDelta = new Vector2(-20, -20);
                            sliderArea.RectTransform.anchorMin = Vector2.zero;
                            sliderArea.RectTransform.anchorMax = Vector2.one;

                            scrollbar.Component.handleRect = sliderArea.RectTransform;

                            sliderArea.AddChildObject<CurvedImage>("Handle", (handle) =>
                            {
                                handle.Component.sprite = UIResources.DefaultResources.Standard;
                                handle.Component.type = Image.Type.Sliced;
                                handle.Component.color = UIResources.DefaultSelectableColor;

                                handle.RectTransform.sizeDelta = new Vector2(20, 20);

                                scrollbar.Component.handleRect = handle.RectTransform;
                                scrollbar.Component.targetGraphic = handle.Component;

                                handle.GameObject.AddComponent<CurveComponent>();
                            });
                        });

                        scrollbar.Component.SetDirection(Scrollbar.Direction.BottomToTop, true);
                        
                        scrollbar.RectTransform.anchorMin = Vector2.right;
                        scrollbar.RectTransform.anchorMax = Vector2.one;
                        scrollbar.RectTransform.pivot = Vector2.one;
                        scrollbar.RectTransform.sizeDelta = new Vector2(scrollbar.RectTransform.sizeDelta.x, 0);

                        scrollRect.verticalScrollbar = scrollbar.Component;

                        SetDefaultColorTransitionValues(scrollbar.Component);

                        scrollbar.GameObject.AddComponent<CurveComponent>();
                    })
                    .AddChildObject<CurvedImage>("Viewport", (viewport) =>
                    {
                        viewport.RectTransform.anchorMin = new Vector2(0, 0);
                        viewport.RectTransform.anchorMax = new Vector2(1, 1);
                        viewport.RectTransform.sizeDelta = new Vector2(-18, 0);
                        viewport.RectTransform.pivot = new Vector2(0, 1);

                        viewport.Component.sprite = UIResources.DefaultResources.Mask;
                        viewport.Component.type = Image.Type.Sliced;

                        var mask = viewport.GameObject.AddComponent<Mask>();
                        mask.showMaskGraphic = false;

                        viewport.AddChildObject("Content", (content) =>
                        {
                            content.RectTransform.anchorMin = new Vector2(0f, 1);
                            content.RectTransform.anchorMax = new Vector2(1f, 1);
                            content.RectTransform.pivot = new Vector2(0.5f, 1);
                            content.RectTransform.anchoredPosition = new Vector2(0, 0);
                            content.RectTransform.sizeDelta = new Vector2(0, 28);

                            content.AddChildObject<Toggle>("Item", (item) =>
                            {
                                item.RectTransform.anchorMin = new Vector2(0, 0.5f);
                                item.RectTransform.anchorMax = new Vector2(1, 0.5f);
                                item.RectTransform.sizeDelta = new Vector2(0, 20);

                                item.AddChildObject<CurvedImage>("Item Background", (itemBackground) =>
                                {
                                    itemBackground.RectTransform.anchorMin = Vector2.zero;
                                    itemBackground.RectTransform.anchorMax = Vector2.one;
                                    itemBackground.RectTransform.sizeDelta = Vector2.zero;

                                    itemBackground.Component.sprite = UIResources.DefaultResources.Standard;
                                    itemBackground.Component.type = Image.Type.Sliced;
                                    itemBackground.Component.color = new Color32(245, 245, 245, 0);

                                    itemBackground.GameObject.AddComponent<CurveComponent>();

                                    item.Component.targetGraphic = itemBackground.Component;
                                })
                                .AddChildObject<CurvedImage>("Item Checkmark", (itemCheckmark) =>
                                {
                                    itemCheckmark.RectTransform.anchorMin = new Vector2(0, 0.5f);
                                    itemCheckmark.RectTransform.anchorMax = new Vector2(0, 0.5f);
                                    itemCheckmark.RectTransform.sizeDelta = new Vector2(20, 20);
                                    itemCheckmark.RectTransform.anchoredPosition = new Vector2(10, 0);

                                    itemCheckmark.Component.sprite = UIResources.DefaultResources.Checkmark;
                                    itemCheckmark.Component.type = Image.Type.Sliced;

                                    itemCheckmark.GameObject.AddComponent<CurveComponent>();

                                    item.Component.graphic = itemCheckmark.Component;
                                })
                                .AddChildObject<CurvedTextMeshPro>("Item Label", (itemLabel) =>
                                {
                                    itemLabel.RectTransform.anchorMin = Vector2.zero;
                                    itemLabel.RectTransform.anchorMax = Vector2.one;
                                    itemLabel.RectTransform.offsetMin = new Vector2(20, 1);
                                    itemLabel.RectTransform.offsetMax = new Vector2(-10, -2);

                                    itemLabel.Component.fontSize = 14;
                                    itemLabel.Component.color = UIResources.TextColor;
                                    itemLabel.Component.alignment = TextAlignmentOptions.Left;
                                    itemLabel.Component.text = "Option A";

                                    dropdown.itemText = itemLabel.Component;
                                });

                                item.Component.isOn = true;
                            });

                            scrollRect.content = content.RectTransform;
                        });

                        scrollRect.viewport = viewport.RectTransform;

                        viewport.GameObject.AddComponent<CurveComponent>();
                    });

                    scrollRect.horizontal = false;
                    scrollRect.movementType = ScrollRect.MovementType.Clamped;
                    scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                    scrollRect.verticalScrollbarSpacing = -3;

                    template.GameObject.AddComponent<CurveComponent>();

                    dropdown.template = template.RectTransform;
                });

                SetDefaultColorTransitionValues(dropdown);

                dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option A" });
                dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option B" });
                dropdown.options.Add(new TMP_Dropdown.OptionData { text = "Option C" });
                dropdown.RefreshShownValue();

                uiBase.GameObject.AddComponent<CurveComponent>();
            }
        }

        private static void SetDefaultColorTransitionValues(Selectable selectable, float alpha = 1)
        {
            ColorBlock colors = selectable.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f, alpha);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f, alpha);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f, alpha);
        }
    }
}