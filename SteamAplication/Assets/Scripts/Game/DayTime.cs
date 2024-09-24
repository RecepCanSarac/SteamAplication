using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class DayTime : MonoBehaviour, IDayTimeCycle
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
            SetDayTime();
        }
        else
        {
            SetNightTime();
        }
    }

    public void SetDayTime()
    {
        RenderSettings.skybox = morningMaterial;
        sun.intensity = 1;
    }

    public void SetNightTime()
    {
        RenderSettings.skybox = nightMaterial;
        sun.intensity = 0;
    }
}

public interface IDayTimeCycle
{
    void SetDayTime();

    void SetNightTime();
}