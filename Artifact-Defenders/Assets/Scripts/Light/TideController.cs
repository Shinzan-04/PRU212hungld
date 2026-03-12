using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TideSwitch : MonoBehaviour
{
    public static TideSwitch Instance;

    [Header("Main Swap Tilemaps")]
    public Tilemap tilemap1; // Nước (Gắn Material chứa _MainColour)
    public Tilemap tilemap2; // Đất hoặc trạng thái khác

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

    private bool isHighTide = true; // Bắt đầu là Map 2 bật, Map 1 tắt
    public bool IsHighTide => isHighTide;
    private Coroutine tideCoroutine;
    private List<Transform> safetyPoints = new List<Transform>();

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Ép ẩn/hiện ngay lập tức từ lúc load script để tránh bị nháy map
        ForceImmediateState();
    }

    void ForceImmediateState()
    {
        // --- THAY ĐỔI 2: Logic bật Map 1 (Water), tắt Map 2 (Ground) ---
        isHighTide = true;

        // HIỆN object Nước (Map 1)
        if (tilemap1 != null)
        {
            SetTilemapAlpha(tilemap1, 1f);
            tilemap1.gameObject.SetActive(true);
        }

        // TẮT object Đất/Cọc (Map 2)
        if (tilemap2 != null)
        {
            SetTilemapAlpha(tilemap2, 0f);
            tilemap2.gameObject.SetActive(false);
        }

        // Tắt các cọc phụ
        if (groundStakesTilemap) groundStakesTilemap.gameObject.SetActive(false);
        if (lowWaterStakesUpTilemap) lowWaterStakesUpTilemap.gameObject.SetActive(false);

        // Cập nhật Collider: Bật nước, tắt cọc
        if (waterCollider) waterCollider.enabled = true;
        if (stakesCollider) stakesCollider.enabled = false;
        if (lowWaterStakesCollider) lowWaterStakesCollider.enabled = false;
    }

    void Start()
    {
        if (playerMana == null)
            playerMana = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMana>();

        GameObject[] points = GameObject.FindGameObjectsWithTag("SafetyPoint");
        safetyPoints.Clear();
        foreach (var p in points) safetyPoints.Add(p.transform);

        // Gọi lại để đảm bảo mọi thứ đồng bộ sau khi Start
        InitialState();
    }

    void InitialState()
    {
        // --- THAY ĐỔI 3: Đồng bộ lại hàm InitialState ---
        // Hiện Nước (Map 1)
        SetTilemapAlpha(tilemap1, 1f);
        if (tilemap1 != null) tilemap1.gameObject.SetActive(true);
        if (waterCollider != null) waterCollider.enabled = true;

        // Tắt Đất/Cọc (Map 2)
        SetTilemapAlpha(tilemap2, 0f);
        if (tilemap2 != null) tilemap2.gameObject.SetActive(false);

        SetTilemapAlpha(groundStakesTilemap, 0f);
        SetTilemapAlpha(lowWaterStakesUpTilemap, 0f);
        if (groundStakesTilemap != null) groundStakesTilemap.gameObject.SetActive(false);
        if (lowWaterStakesUpTilemap != null) lowWaterStakesUpTilemap.gameObject.SetActive(false);

        if (stakesCollider != null) stakesCollider.enabled = false;
        if (lowWaterStakesCollider != null) lowWaterStakesCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (playerMana != null && playerMana.TryUseMana(manaCost))
            {
                ToggleTide();
            }
        }
    }

    public void ToggleTide()
    {
        isHighTide = !isHighTide;
        if (tideCoroutine != null) StopCoroutine(tideCoroutine);
        tideCoroutine = StartCoroutine(TransitionTide(isHighTide));

        StartCoroutine(ShakeCamera(0.2f, 0.1f));
    }
    IEnumerator TransitionTide(bool toHigh)
    {
        if (toHigh) CheckAndPushPlayer();

        // Bật GameObject lên để có thể thấy hiệu ứng Fade
        if (tilemap1) tilemap1.gameObject.SetActive(true);
        if (tilemap2) tilemap2.gameObject.SetActive(true);
        if (groundStakesTilemap) groundStakesTilemap.gameObject.SetActive(true);
        if (lowWaterStakesUpTilemap) lowWaterStakesUpTilemap.gameObject.SetActive(true);

        float t = 0f;
        if (audioSource != null)
        {
            audioSource.clip = toHigh ? tideInSound : tideOutSound;
            audioSource.Play();
        }

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

        // Sau khi Fade xong, tắt hẳn GameObject không sử dụng để tối ưu và tránh lỗi hiển thị
        if (tilemap1) tilemap1.gameObject.SetActive(toHigh);
        if (tilemap2) tilemap2.gameObject.SetActive(!toHigh);
        if (groundStakesTilemap) groundStakesTilemap.gameObject.SetActive(!toHigh);
        if (lowWaterStakesUpTilemap) lowWaterStakesUpTilemap.gameObject.SetActive(!toHigh);

        if (waterCollider != null) waterCollider.enabled = toHigh;
        if (stakesCollider != null) stakesCollider.enabled = !toHigh;
        if (lowWaterStakesCollider != null) lowWaterStakesCollider.enabled = !toHigh;
    }

    void SetTilemapAlpha(Tilemap tm, float alpha)
    {
        if (tm == null) return;

        // 1. Chỉnh màu trên Component Tilemap
        Color c = tm.color;
        c.a = alpha;
        tm.color = c;

        // 2. Chỉnh màu trực tiếp vào Shader (Dành cho Shader Nước của bạn)
        TilemapRenderer tr = tm.GetComponent<TilemapRenderer>();
        if (tr != null && tr.material != null)
        {
            // Kiểm tra xem Material có biến _MainColour không
            if (tr.material.HasProperty("_MainColour"))
            {
                Color shaderColor = tr.material.GetColor("_MainColour");
                shaderColor.a = alpha;
                tr.material.SetColor("_MainColour", shaderColor);
            }
        }
    }

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
        if (isHighTide && tilemap1 != null)
        {
            float offset = Mathf.Sin(Time.time * waveSpeed) * waveAmount;
            tilemap1.transform.localPosition = new Vector3(0, offset, 0);
        }
    }
}