using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TideSwitch : MonoBehaviour
{
    public static TideSwitch Instance;

    [Header("Main Swap Tilemaps")]
    public Tilemap tilemap1; // Nước (Mặc định ban đầu)
    public Tilemap tilemap2; // Đất/Trạng thái khác

    [Header("Sub Stakes Tilemaps")]
    public Tilemap groundStakesTilemap;
    public Tilemap lowWaterStakesUpTilemap;

    [Header("Colliders")]
    public TilemapCollider2D waterCollider;
    public TilemapCollider2D stakesCollider;
    public TilemapCollider2D lowWaterStakesCollider;

    [Header("Settings & Mana")]
    public KeyCode toggleKey = KeyCode.P;
    public float fadeDuration = 1f;
    public float autoReturnDelay = 15f; // Thời gian tự động quay về
    public PlayerMana playerMana;
    public int manaCost = 50;

    [Header("VFX & Audio")]
    public AudioSource audioSource;
    public AudioClip tideInSound;
    public AudioClip tideOutSound;
    public float waveSpeed = 2f;
    public float waveAmount = 0.02f;

    [Header("Danger Zone & Push")]
    public Collider2D waterDangerZone;
    public Transform playerTransform;
    public float pushSpeed = 10f;

    private bool isHighTide = true;
    public bool IsHighTide => isHighTide;
    private Coroutine tideCoroutine;
    private Coroutine autoReturnCoroutine; // Coroutine quản lý việc tự động quay về
    private List<Transform> safetyPoints = new List<Transform>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        ForceAllActive();
    }

    void ForceAllActive()
    {
        if (tilemap1) tilemap1.gameObject.SetActive(true);
        if (tilemap2) tilemap2.gameObject.SetActive(true);
        if (groundStakesTilemap) groundStakesTilemap.gameObject.SetActive(true);
        if (lowWaterStakesUpTilemap) lowWaterStakesUpTilemap.gameObject.SetActive(true);
    }

    void Start()
    {
        if (playerMana == null)
            playerMana = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMana>();

        GameObject[] points = GameObject.FindGameObjectsWithTag("SafetyPoint");
        safetyPoints.Clear();
        foreach (var p in points) safetyPoints.Add(p.transform);

        SyncStateImmediate(true);
    }

    void SyncStateImmediate(bool toHigh)
    {
        isHighTide = toHigh;
        SetTilemapAlpha(tilemap1, toHigh ? 1f : 0f);
        SetTilemapAlpha(tilemap2, toHigh ? 0f : 1f);
        SetTilemapAlpha(groundStakesTilemap, toHigh ? 0f : 1f);
        SetTilemapAlpha(lowWaterStakesUpTilemap, toHigh ? 0f : 1f);

        if (waterCollider) waterCollider.enabled = toHigh;
        if (stakesCollider) stakesCollider.enabled = !toHigh;
        if (lowWaterStakesCollider) lowWaterStakesCollider.enabled = !toHigh;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // Chỉ cho phép bấm đổi nếu đang ở trạng thái mặc định (High Tide)
            // Hoặc nếu bạn muốn bấm để đổi qua lại liên tục thì bỏ điều kiện !isHighTide
            if (isHighTide && playerMana != null && playerMana.TryUseMana(manaCost))
            {
                ToggleTide();
            }
        }
    }

    public void ToggleTide()
    {
        isHighTide = !isHighTide;

        // Xử lý Coroutine chuyển đổi hiệu ứng
        if (tideCoroutine != null) StopCoroutine(tideCoroutine);
        tideCoroutine = StartCoroutine(TransitionTide(isHighTide));

        // Xử lý Logic tự động quay về
        if (autoReturnCoroutine != null) StopCoroutine(autoReturnCoroutine);

        if (!isHighTide) // Nếu vừa chuyển sang Low Tide
        {
            autoReturnCoroutine = StartCoroutine(AutoReturnTimer());
        }

        StartCoroutine(ShakeCamera(0.2f, 0.1f));
    }

    IEnumerator AutoReturnTimer()
    {
        yield return new WaitForSeconds(autoReturnDelay);

        // Nếu hiện tại vẫn đang là Low Tide thì mới tự động chuyển về
        if (!isHighTide)
        {
            ToggleTide();
        }
    }

    IEnumerator TransitionTide(bool toHigh)
    {
        if (toHigh) CheckAndPushPlayer();

        float t = 0f;
        if (audioSource != null)
        {
            audioSource.clip = toHigh ? tideInSound : tideOutSound;
            audioSource.Play();
        }

        if (waterCollider) waterCollider.enabled = toHigh;
        if (stakesCollider) stakesCollider.enabled = !toHigh;
        if (lowWaterStakesCollider) lowWaterStakesCollider.enabled = !toHigh;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / fadeDuration);

            float alpha1 = toHigh ? p : 1f - p;
            float alpha2 = 1f - alpha1;

            SetTilemapAlpha(tilemap1, alpha1);
            SetTilemapAlpha(tilemap2, alpha2);
            SetTilemapAlpha(groundStakesTilemap, alpha2);
            SetTilemapAlpha(lowWaterStakesUpTilemap, alpha2);

            yield return null;
        }

        SyncStateImmediate(toHigh);
    }

    void SetTilemapAlpha(Tilemap tm, float alpha)
    {
        if (tm == null) return;
        Color c = tm.color;
        c.a = alpha;
        tm.color = c;

        TilemapRenderer tr = tm.GetComponent<TilemapRenderer>();
        if (tr != null && tr.material != null)
        {
            if (tr.material.HasProperty("_MainColour"))
            {
                Color shaderColor = tr.material.GetColor("_MainColour");
                shaderColor.a = alpha;
                tr.material.SetColor("_MainColour", shaderColor);
            }
        }
    }

    // --- CÁC HÀM CŨ GIỮ NGUYÊN ---

    void CheckAndPushPlayer()
    {
        if (waterDangerZone != null && playerTransform != null && waterDangerZone.OverlapPoint(playerTransform.position))
        {
            Transform bestPoint = GetClosestSafetyPoint(playerTransform.position);
            if (bestPoint != null) StartCoroutine(PushToSafetyRoutine(bestPoint.position));
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

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }

    void LateUpdate()
    {
        // Hiệu ứng dập dềnh vẫn hoạt động dựa trên logic sóng
        if (isHighTide && tilemap1 != null)
        {
            float offset = Mathf.Sin(Time.time * waveSpeed) * waveAmount;
            tilemap1.transform.localPosition = new Vector3(0, offset, 0);
        }
    }
}