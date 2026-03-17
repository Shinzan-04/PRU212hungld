using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy logic: move, attack artifact or eat bushes, take damage.
/// Special: Boat logic spawns land enemies on destruction.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] bool isEater = false;
    [SerializeField] bool isBoat = false;

    [Header("Boat Settings (Only for Boats)")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int minSpawnCount = 3;
    [SerializeField] int maxSpawnCount = 5;
    [SerializeField] float spawnSpreadRadius = 0.8f;

    [Header("Boat Scale Settings")]
    [SerializeField] private float distanceToStartScaling = 15f; // Khoảng cách bắt đầu to lên
    [SerializeField] private bool scaleOverDistance = true; // Bật/tắt tính năng này
    [SerializeField] private float minScale = 0.5f;       // Kích cỡ khi ở xa
    [SerializeField] private float maxScale = 1.2f;       // Kích cỡ khi đến gần bờ
    [SerializeField] private float distanceToMaxScale = 2f; // Khoảng cách mà tại đó thuyền đạt kích cỡ tối đa

    [Header("Audio Settings")]
    [SerializeField] private AudioSource boatAudioSource; // Kéo AudioSource của thuyền vào đây
    [SerializeField] private AudioClip boatBreakSound;    // Kéo file âm thanh thuyền vỡ vào đây
    [Range(0f, 1f)][SerializeField] private float breakVolume = 0.7f;

    [Header("Stats")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int maxHealth = 10;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float eatTime = 1.2f;

    [Header("Target Masks")]
    [SerializeField] LayerMask bushesMask;
    [SerializeField] LayerMask waterMask;
    [SerializeField] LayerMask obstacleMask;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool left;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;

    int currentHealth;
    float attackTimer;
    float eatTimer;
    bool killingBush;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public GameObject[] UpgradeItems;

    Artifact artifact;
    BushFruits target;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (isEater)
        {
            SearchForTarget();
            killingBush = false;
        }
        else
        {
            GameObject artObj = GameObject.FindGameObjectWithTag("Artifact");
            if (artObj != null) artifact = artObj.GetComponent<Artifact>();
        }

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.max = maxHealth;
            eh.current = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isHurt)
        {
            if (artifact != null)
                left = artifact.transform.position.x < transform.position.x;
            return;
        }

        if (isBoat)
        {
            HandleBoat();
            return;
        }

        if (isEater) HandleEater();
        else HandleAttacker();
    }

    void HandleBoat()
    {
        if (artifact == null) return;

        UpdateBoatScale(); // Gọi ở đây để cập nhật liên tục khi thuyền di chuyển

        if (artifact == null) return;

        // Âm thanh lướt sóng (Loop)
        if (boatAudioSource != null && !boatAudioSource.isPlaying)
            boatAudioSource.Play();

        Vector2 direction = (artifact.transform.position - transform.position).normalized;
        Vector3 checkPos = transform.position + (Vector3)direction * 0.3f;

        // 1. Kiểm tra trạng thái nước từ TideSwitch
        bool isHighTide = TideSwitch.Instance != null ? TideSwitch.Instance.IsHighTide : true;

        // 2. Kiểm tra va chạm
        Collider2D obstacleHit = Physics2D.OverlapCircle(checkPos, 0.3f, obstacleMask);
        Collider2D waterHit = Physics2D.OverlapCircle(checkPos, 0.2f, waterMask);

        // TRƯỜNG HỢP A: Chạm đất liền (Luôn vỡ và sinh quân bất kể thủy triều)
        // Giả sử tileLand của bạn không nằm trong waterMask
        if (waterHit == null)
        {
            SpawnEnemiesAndDestroy(); // Vỡ và thả lính lên bờ
            return;
        }

        // TRƯỜNG HỢP B: Kiểm tra Cọc (Chỉ vỡ khi triều rút)
        if (!isHighTide && obstacleHit != null)
        {
            BreakWithoutSpawning(); // Vỡ do kẹt cọc, không sinh quân (hoặc tùy bạn chỉnh)
            return;
        }

        // Nếu nước sâu và không chạm đất, thuyền đi xuyên qua cọc đang chìm
        MoveTowards(artifact.transform.position);
        left = artifact.transform.position.x < transform.position.x;
    }

    // Tìm đến hàm này trong EnemyAI.cs và thay thế nội dung
    void BreakWithoutSpawning()
    {
        if (isDead) return;

        isDead = true; // Kích hoạt trạng thái chết để WolfAnim nhận diện và chạy deathSprites
        isMoving = false;

        // Tắt va chạm để thuyền không cản đường các thuyền khác khi đang diễn hoạt ảnh vỡ
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        PlayBreakAudio();

        // Thay vì Destroy ngay, ta gọi Coroutine để chờ
        StartCoroutine(WaitAndDestroyBoat());
    }

    IEnumerator WaitAndDestroyBoat()
    {
        // Chờ một khoảng thời gian (ví dụ 0.8 giây hoặc tùy độ dài anim của bạn)
        // Đảm bảo thời gian này khớp với tổng thời gian chạy deathSprites trong WolfAnim
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    // Tách riêng logic âm thanh để tái sử dụng
    void PlayBreakAudio()
    {
        if (boatAudioSource != null) boatAudioSource.Stop();
        if (boatBreakSound != null)
        {
            // Kích âm lượng lên 1.5f như bạn muốn
            AudioSource.PlayClipAtPoint(boatBreakSound, transform.position, 1.5f);
        }
    }

    // Cập nhật lại hàm cũ: Chỉ dùng khi thuyền bị tiêu diệt bằng vũ khí (Sinh quân)
    void SpawnEnemiesAndDestroy()
    {
        if (isDead) return;
        isDead = true;

        PlayBreakAudio();

        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            if (enemyPrefab != null)
            {
                float angle = i * (360f / spawnCount) * Mathf.Deg2Rad;
                Vector3 spawnDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                Vector3 spawnPos = transform.position + spawnDir * spawnSpreadRadius;

                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(spawnDir * 5f, ForceMode2D.Impulse);
                }

                EnemyAI ai = enemy.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.isBoat = false;
                    ai.StartCoroutine(TemporarilyDisableMovement(ai));
                }
            }
        }

        Destroy(gameObject);
    }

    IEnumerator TemporarilyDisableMovement(EnemyAI ai)
    {
        ai.enabled = false;
        yield return new WaitForSeconds(0.4f);
        if (ai != null) ai.enabled = true;
    }

    // === LOGIC ĂN BỤI CÂY ===
    void HandleEater()
    {
        if (target == null || !target.enabled)
        {
            SearchForTarget();
            return;
        }

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (target.HasFruits() && !killingBush)
        {
            if (dist > 0.5f) MoveTowards(target.transform.position);
            else if (!isAttacking)
            {
                isMoving = false;
                StartCoroutine(EatRoutine());
            }
        }
        else if (killingBush)
        {
            if (Time.time > eatTimer && !isAttacking)
            {
                isMoving = false;
                StartCoroutine(EatRoutine());
            }
        }
        else SearchForTarget();

        if (target != null)
            left = target.transform.position.x < transform.position.x;
    }

    IEnumerator EatRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.45f);

        if (target != null && target.enabled)
        {
            if (target.HasFruits())
            {
                target.HarvestFruit();
                eatTimer = Time.time + eatTime;
                killingBush = true;
            }
            else
            {
                target.EatBush();
                killingBush = false;
                SearchForTarget();
            }
        }
        yield return new WaitForSeconds(0.15f);
        isAttacking = false;
    }

    // === LOGIC TẤN CÔNG TRỤ ===
    void HandleAttacker()
    {
        if (artifact == null) return;
        float distance = Vector2.Distance(transform.position, artifact.transform.position);

        if (distance > 1.5f) MoveTowards(artifact.transform.position);
        else
        {
            isMoving = false;
            if (!isAttacking && Time.time > attackTimer)
            {
                StartCoroutine(AttackRoutine());
                attackTimer = Time.time + attackCooldown;
            }
        }
        left = artifact.transform.position.x < transform.position.x;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.45f);
        if (artifact != null) artifact.Damage(attackDamage);
        yield return new WaitForSeconds(0.15f);
        isAttacking = false;
    }

    void MoveTowards(Vector3 targetPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        isMoving = true;
    }

    void SearchForTarget()
    {
        target = null;
        for (int i = 1; i < 50; i++)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Mathf.Exp(i), bushesMask);
            foreach (Collider2D hit in hits)
            {
                BushFruits bush = hit.GetComponent<BushFruits>();
                if (bush != null && bush.enabled && bush.HasFruits())
                {
                    target = bush;
                    return;
                }
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth = Mathf.Max(0, currentHealth - dmg);
        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.current = currentHealth;

        if (currentHealth > 0) StartCoroutine(HurtRoutine());
        else StartCoroutine(DieRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        isMoving = false;
        yield return new WaitForSeconds(0.4f);
        isHurt = false;
    }

    IEnumerator DieRoutine()
    {
        if (isBoat) { SpawnEnemiesAndDestroy(); yield break; }

        isDead = true;
        isMoving = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(1.0f);

        if (UpgradeItems != null && UpgradeItems.Length > 0)
        {
            int rand = Random.Range(0, UpgradeItems.Length);
            GameObject item = Instantiate(UpgradeItems[rand], transform.position, Quaternion.identity);
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null) rb.AddForce(Random.insideUnitCircle.normalized * 2f, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }
    void UpdateBoatScale()
    {
        if (!isBoat || artifact == null || !scaleOverDistance) return;

        float distance = Vector2.Distance(transform.position, artifact.transform.position);

        // Sử dụng biến distanceToStartScaling thay vì số 15 cố định
        float t = Mathf.InverseLerp(distanceToStartScaling, distanceToMaxScale, distance);

        // Clamp t để đảm bảo không bị nhỏ hơn minScale hoặc lớn hơn maxScale
        float currentScale = Mathf.Lerp(minScale, maxScale, t);

        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}