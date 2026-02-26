using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightSwitch : MonoBehaviour
{
    public Light2D globalLight;
    public KeyCode toggleKey = KeyCode.O;
    public float fadeDuration = 1.2f;

    [Header("Settings")]
    public float dayIntensity = 1f;
    public float nightIntensity = 0.3f;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.2f, 0.2f, 0.5f);

    private bool isDay = true;
    private Coroutine lightCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isDay = !isDay;
            if (lightCoroutine != null) StopCoroutine(lightCoroutine);
            lightCoroutine = StartCoroutine(TransitionLight(isDay));
        }
    }

    System.Collections.IEnumerator TransitionLight(bool toDay)
    {
        float t = 0;
        float startInt = globalLight.intensity;
        Color startCol = globalLight.color;
        float targetInt = toDay ? dayIntensity : nightIntensity;
        Color targetCol = toDay ? dayColor : nightColor;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / fadeDuration);
            globalLight.intensity = Mathf.Lerp(startInt, targetInt, p);
            globalLight.color = Color.Lerp(startCol, targetCol, p);
            yield return null;
        }
    }
}