using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic; // Cần thiết để dùng List

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

    // --- Ô MỚI THÊM Ở ĐÂY ---
    [Header("Danger Zone & Push")]
    public Collider2D waterDangerZone;
    public Transform playerTransform;
    public float pushSpeed = 10f;
    // -----------------------

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
    private List<Transform> safetyPoints = new List<Transform>();

    void Start()
    {
        // Tìm tất cả các điểm an toàn có Tag là SafetyPoint trong Scene
        GameObject[] points = GameObject.FindGameObjectsWithTag("SafetyPoint");
        foreach (var p in points) safetyPoints.Add(p.transform);
    }

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
        // 1. Nếu triều lên, kiểm tra và đẩy Player ngay lập tức
        if (toHigh) CheckAndPushPlayer();

        float t = 0f;
        float startWaterAlpha = waterTilemap.color.a;
        float startStakesAlpha = groundStakesTilemap.color.a;

        float targetWaterAlpha = toHigh ? 1f : 0f;
        float targetStakesAlpha = toHigh ? 0f : 1f;

        if (audioSource != null)
        {
            audioSource.clip = toHigh ? tideInSound : tideOutSound;
            audioSource.Play();
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            SetTilemapAlpha(waterTilemap, Mathf.Lerp(startWaterAlpha, targetWaterAlpha, p));
            SetTilemapAlpha(groundStakesTilemap, Mathf.Lerp(startStakesAlpha, targetStakesAlpha, p));
            SetTilemapAlpha(lowWaterStakesUpTilemap, Mathf.Lerp(startStakesAlpha, targetStakesAlpha, p));

            yield return null;
        }

        // 🎯 Cập nhật vật lý sau khi fade xong (hoặc có thể đưa lên đầu nếu muốn chặn ngay)
        waterCollider.enabled = toHigh;
        stakesCollider.enabled = !toHigh;
        lowWaterStakesCollider.enabled = !toHigh;
    }

    void CheckAndPushPlayer()
    {
        // Kiểm tra xem Player có đang đứng trong vùng nguy hiểm không
        if (waterDangerZone != null && waterDangerZone.OverlapPoint(playerTransform.position))
        {
            Transform bestPoint = GetClosestSafetyPoint(playerTransform.position);
            if (bestPoint != null)
            {
                StartCoroutine(PushToSafetyRoutine(bestPoint.position));
            }
        }
    }

    Transform GetClosestSafetyPoint(Vector2 currentPos)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;
        foreach (Transform pt in safetyPoints)
        {
            float dist = Vector2.Distance(currentPos, pt.position);
            if (dist < minDist) { minDist = dist; closest = pt; }
        }
        return closest;
    }

    IEnumerator PushToSafetyRoutine(Vector3 targetPos)
    {
        while (Vector3.Distance(playerTransform.position, targetPos) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, pushSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void SetTilemapAlpha(Tilemap tm, float alpha)
    {
        if (tm == null) return;
        Color c = tm.color;
        c.a = alpha;
        tm.color = c;
    }

    void LateUpdate()
    {
        if (isHighTide && waterTilemap != null)
        {
            float offset = Mathf.Sin(Time.time * waveSpeed) * waveAmount;
            waterTilemap.transform.localPosition = new Vector3(0, offset, 0);
        }
    }
}