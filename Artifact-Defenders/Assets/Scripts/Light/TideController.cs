using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TideSwitch : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap waterTilemap;
    public Tilemap groundStakesTilemap;
    public Tilemap lowWaterStakesUpTilemap;

    [Header("Colliders")]
    public TilemapCollider2D waterCollider;
    public TilemapCollider2D stakesCollider;
    public TilemapCollider2D lowWaterStakesCollider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tideInSound;
    public AudioClip tideOutSound;

    [Header("Wave Effect")]
    public float waveSpeed = 2f;
    public float waveAmount = 0.02f;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.P;
    public float fadeDuration = 1f;

    private bool isHighTide = true;
    private Coroutine tideCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isHighTide = !isHighTide;

            if (tideCoroutine != null)
                StopCoroutine(tideCoroutine);

            tideCoroutine = StartCoroutine(TransitionTide(isHighTide));
        }
    }

    IEnumerator TransitionTide(bool toHigh)
    {
        float t = 0f;

        float startWaterAlpha = waterTilemap.color.a;
        float startStakesAlpha = groundStakesTilemap.color.a;

        float targetWaterAlpha = toHigh ? 1f : 0f;
        float targetStakesAlpha = toHigh ? 0f : 1f;

        // 🔊 Play sound
        if (audioSource != null)
        {
            audioSource.clip = toHigh ? tideInSound : tideOutSound;
            audioSource.Play();
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            float waterAlpha = Mathf.Lerp(startWaterAlpha, targetWaterAlpha, p);
            float stakesAlpha = Mathf.Lerp(startStakesAlpha, targetStakesAlpha, p);

            SetTilemapAlpha(waterTilemap, waterAlpha);
            SetTilemapAlpha(groundStakesTilemap, stakesAlpha);
            SetTilemapAlpha(lowWaterStakesUpTilemap, stakesAlpha);

            yield return null;
        }

        // 🎯 Bật/tắt collider theo tide
        waterCollider.enabled = toHigh;
        stakesCollider.enabled = !toHigh;
        lowWaterStakesCollider.enabled = !toHigh;
    }

    void SetTilemapAlpha(Tilemap tm, float alpha)
    {
        if (tm == null) return;
        Color c = tm.color;
        c.a = alpha;
        tm.color = c;
    }

    // 🌊 Wave effect (chỉ áp dụng khi High tide)
    void LateUpdate()
    {
        if (isHighTide && waterTilemap != null)
        {
            float offset = Mathf.Sin(Time.time * waveSpeed) * waveAmount;
            waterTilemap.transform.localPosition = new Vector3(0, offset, 0);
        }
    }
}