using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurvedUIUtility;

public class CycleThroughCurveSettings : MonoBehaviour
{
    [SerializeField] private CurvedUIController controller;
    [SerializeField] private List<CurvedUISettingsObject> curvedUISettings;

    private int i = 0;

    private IEnumerator Start()
    {
        while (true)
        {
            controller.SetCurveSettings(curvedUISettings[i].Settings);

            i = (++i) % curvedUISettings.Count;

            yield return new WaitForSeconds(2.5f);
        }
    }
}
