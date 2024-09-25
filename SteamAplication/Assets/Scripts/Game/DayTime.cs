using System.Collections;
using UnityEngine;
using RenderSettings = UnityEngine.RenderSettings;

public class DayTime : MonoBehaviour
{
    [Header("Skybox Materials")]
    [Space]
    public Material morningMaterial;
    public Material nightMaterial;
    [Space]

    [Header("Sun Settings")]
    [Space]
    public Light sun;
    public float transitionSpeed = 1.0f;
    [Space]

    [Header("Sun Rotation")]
    [Space]
    public Vector3 dayRotation = new Vector3(50, 0, 0);
    public Vector3 nightRotation = new Vector3(-50, 0, 0);

    private bool isNight = false;
    private bool isTransitioning = false;

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.Space) && !isNight && !isTransitioning)
        {
            StartTransitionToNight();
        }
        else if (!Input.GetKey(KeyCode.Space) && isNight && !isTransitioning)
        {
            StartTransitionToDay();
        }
    }

    void StartTransitionToNight()
    {
        StartCoroutine(TransitionTime(nightMaterial, 0f, nightRotation, () => isNight = true));
    }

    void StartTransitionToDay()
    {
        StartCoroutine(TransitionTime(morningMaterial, 1f, dayRotation, () => isNight = false));
    }

    IEnumerator TransitionTime(Material targetSkybox, float targetIntensity, Vector3 targetRotation, System.Action onComplete)
    {
        isTransitioning = true;

        float initialIntensity = sun.intensity;
        Vector3 initialRotation = sun.transform.eulerAngles;

        float elapsedTime = 0f;
        float duration = Mathf.Abs(targetIntensity - initialIntensity) / transitionSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            sun.intensity = Mathf.Lerp(initialIntensity, targetIntensity, progress);
            sun.transform.eulerAngles = Vector3.Lerp(initialRotation, targetRotation, progress);

            yield return null;
        }

        RenderSettings.skybox = targetSkybox;
        onComplete?.Invoke();
        isTransitioning = false;
    }
}