using System;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class DayTime : MonoBehaviour
{
    public Material morningMaterial;
    public Material nightMaterial;

    public Light sun;

    public bool isChanged = false;

    void Update()
    {
        isChanged = Input.GetKey(KeyCode.Space);

        if (isChanged)
        {
            RenderSettings.skybox = morningMaterial;
            sun.intensity = 1;

        }
        else
        {
            RenderSettings.skybox = nightMaterial;
            sun.intensity = 0;
        }
    }
}
