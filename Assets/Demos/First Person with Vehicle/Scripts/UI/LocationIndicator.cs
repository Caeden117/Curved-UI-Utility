using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUIUtility.Demos.FirstPersonWithVehicle
{
    public class LocationIndicator : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI locationIndicator;
        [Space(15)]
        [SerializeField] private float collapsedWidth = 0;
        [SerializeField] private float collapsedHeight = 0;
        [SerializeField] private float collapsedTextSize = 0;
        [Space(15)]
        [SerializeField] private float newlyEnteredWidth = 800;
        [SerializeField] private float newlyEnteredHeight = 64;
        [SerializeField] private float newlyEnteredTextSize = 42;
        [Space(15)]
        [SerializeField] private float stayingWidth = 400;
        [SerializeField] private float stayingHeight = 42;
        [SerializeField] private float stayingTextSize = 24;

        private void Start()
        {
            var rectTransform = backgroundImage.rectTransform;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, collapsedWidth);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, collapsedHeight);
            locationIndicator.fontSize = collapsedTextSize;
        }

        private void OnEnable()
        {
            Location.OnPlayerEnterLocation += Location_OnPlayerEnterLocation;
            Location.OnPlayerLeaveLocation += Location_OnPlayerLeaveLocation;
        }

        private void Location_OnPlayerEnterLocation(Location obj)
        {
            var color = obj.LocationColor;
            color.a = 0.5f;
            backgroundImage.color = color;
            locationIndicator.text = obj.LocationName;
            StopAllCoroutines();
            StartCoroutine(EnteredLocation());
        }

        private void Location_OnPlayerLeaveLocation(Location obj)
        {
            StopAllCoroutines();
            StartCoroutine(LeftLocation());
        }

        private void OnDisable()
        {
            Location.OnPlayerEnterLocation -= Location_OnPlayerEnterLocation;
            Location.OnPlayerLeaveLocation -= Location_OnPlayerLeaveLocation;
        }

        private IEnumerator EnteredLocation()
        {
            yield return StartCoroutine(ChangeSpecs(newlyEnteredWidth, newlyEnteredHeight, newlyEnteredTextSize));
            yield return new WaitForSeconds(5f);
            yield return StartCoroutine(ChangeSpecs(stayingWidth, stayingHeight, stayingTextSize));
        }

        private IEnumerator LeftLocation()
        {
            yield return StartCoroutine(ChangeSpecs(collapsedWidth, collapsedHeight, collapsedTextSize));
        }

        private IEnumerator ChangeSpecs(float width, float height, float textSize)
        {
            var rectTransform = backgroundImage.rectTransform;

            var endVector = new Vector3(width, height, textSize);

            var t = 0f;

            while (t < 1)
            {
                var startVector = new Vector3(
                    backgroundImage.rectTransform.rect.width,
                    backgroundImage.rectTransform.rect.height,
                    locationIndicator.fontSize);

                var lerp = Vector3.Slerp(startVector, endVector, 0.1f);

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerp.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lerp.y);
                locationIndicator.fontSize = lerp.z;

                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            locationIndicator.fontSize = textSize;
        }
    }
}