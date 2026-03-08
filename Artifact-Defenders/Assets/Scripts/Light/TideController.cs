using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TideSwitch : MonoBehaviour
{
    // Singleton để EnemyAI có thể gọi: TideSwitch.Instance.IsHighTide
    public static TideSwitch Instance;

    [Header("Artifact Decay Settings")]
    public int damageToArtifact = 5; // Sát thương gây ra cho trụ mỗi đợt
    public float decayInterval = 1.0f; // Khoảng thời gian giữa mỗi lần trừ máu (1 giây)
    private float decayTimer;

    [Header("Tilemaps")]
    public Tilemap waterTilemap;
    public Tilemap groundStakesTilemap;
    public Tilemap lowWaterStakesUpTilemap;

    [Header("Colliders")]
    public TilemapCollider2D waterCollider;
    public TilemapCollider2D stakesCollider;
    public TilemapCollider2D lowWaterStakesCollider;

    [Header("Danger Zone & Push")]
    public Collider2D waterDangerZone;
    public Transform playerTransform;
    public float pushSpeed = 10f;

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

    [Header("Mana Settings")]
    public PlayerMana playerMana; // Kéo thả đối tượng Player có script PlayerMana vào đây
    public int manaCost = 50;

    private bool isHighTide = true;
    private Coroutine tideCoroutine;
    private List<Transform> safetyPoints = new List<Transform>();
    private bool isPushingPlayer = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Tự động tìm PlayerMana nếu chưa kéo thả trong Inspector
        if (playerMana == null)
        {
            playerMana = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMana>();
        }

        GameObject[] points = GameObject.FindGameObjectsWithTag("SafetyPoint");
        foreach (var p in points) safetyPoints.Add(p.transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            // 1. Kiểm tra xem có đủ 50 mana không thông qua hàm TryUseMana đã viết
            // Giả sử bạn đã kéo thả PlayerMana vào biến playerMana trong Inspector
            if (playerMana != null && playerMana.TryUseMana(50))
            {
                // 2. Nếu đủ (hàm trả về true), thực hiện đổi thủy triều
                ToggleTide();
            }
            else
            {
                // 3. Nếu không đủ, có thể thêm hiệu ứng âm thanh hoặc thông báo ở đây
                Debug.Log("Không đủ Mana để thực hiện phép thuật!");
            }
        }
        // --- LOGIC MỚI: GIẢM MÁU TRỤ THEO THỜI GIAN ---
        if (!isHighTide)
        {
            HandleArtifactDecay();
        }
    }
    void HandleArtifactDecay()
    {
        decayTimer += Time.deltaTime;

        if (decayTimer >= decayInterval)
        {
            // Tìm tất cả các Artifact có trong Scene
            Artifact[] artifacts = GameObject.FindObjectsOfType<Artifact>();

            foreach (Artifact art in artifacts)
            {
                art.TakeDamage(damageToArtifact);
            }

            decayTimer = 0f; // Reset bộ đếm để chờ giây tiếp theo
        }
    }

    public void ToggleTide()
    {
        isHighTide = !isHighTide;
        if (tideCoroutine != null) StopCoroutine(tideCoroutine);
        tideCoroutine = StartCoroutine(TransitionTide(isHighTide));
    }

    // Hàm để các script khác kiểm tra trạng thái nước
    public bool IsHighTide() => isHighTide;

    IEnumerator TransitionTide(bool toHigh)
    {
        // 1. Nếu triều lên (toHigh = true), đẩy người chơi ngay lập tức
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

        // Cập nhật vật lý
        waterCollider.enabled = toHigh;
        stakesCollider.enabled = !toHigh;
        lowWaterStakesCollider.enabled = !toHigh;
    }

    void CheckAndPushPlayer()
    {
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
        isPushingPlayer = true;

        // Vô hiệu hóa script di chuyển của người chơi nếu cần (Ví dụ: PlayerMovement)
        // playerTransform.GetComponent<PlayerMovement>().enabled = false;

        while (Vector3.Distance(playerTransform.position, targetPos) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, pushSpeed * Time.deltaTime);
            yield return null;
        }

        // Trả lại quyền điều khiển
        // playerTransform.GetComponent<PlayerMovement>().enabled = true;
        isPushingPlayer = false;
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