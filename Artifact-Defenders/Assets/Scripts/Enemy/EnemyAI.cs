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
    [SerializeField] GameObject enemyPrefab; // Kéo Prefab Enemy00 vào đây
    [SerializeField] int minSpawnCount = 3;
    [SerializeField] int maxSpawnCount = 5;
    [SerializeField] float spawnSpreadRadius = 0.8f; // Bán kính tủa ra

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

    // --- LOGIC THUYỀN ---
    // --- Cập nhật HandleBoat để nhạy hơn với va chạm trên/dưới ---
    // --- BOAT LOGIC CẢI TIẾN ---
    void HandleBoat()
    {
        if (artifact == null) return;

        // 1. Xác định hướng di chuyển tiếp theo
        Vector2 direction = (artifact.transform.position - transform.position).normalized;

        // 2. Dự đoán vị trí tiếp theo (Check ahead)
        // Kiểm tra một điểm nhỏ ở phía trước thuyền 0.3 đơn vị để xử lý trước khi lọt vào đất
        Vector3 checkPos = transform.position + (Vector3)direction * 0.3f;

        // 3. Kiểm tra va chạm cọc tại vị trí dự đoán
        Collider2D obstacleHit = Physics2D.OverlapCircle(checkPos, 0.3f, obstacleMask);

        // 4. Kiểm tra xem điểm dự đoán còn ở trên nước không
        Collider2D waterHit = Physics2D.OverlapCircle(checkPos, 0.2f, waterMask);

        // NẾU chạm cọc HOẶC điểm phía trước KHÔNG PHẢI LÀ NƯỚC -> Vỡ thuyền ngay
        if (obstacleHit != null || waterHit == null)
        {
            SpawnEnemiesAndDestroy();
            return;
        }

        // 5. Nếu an toàn thì mới thực hiện di chuyển
        MoveTowards(artifact.transform.position);

        left = artifact.transform.position.x < transform.position.x;
    }

    void SpawnEnemiesAndDestroy()
    {
        if (isDead) return;
        isDead = true;

        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        // Tính toán để lính tủa ra các ô xung quanh
        for (int i = 0; i < spawnCount; i++)
        {
            if (enemyPrefab != null)
            {
                float angle = i * (360f / spawnCount) * Mathf.Deg2Rad;
                Vector3 spawnDir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

                // Spawn lính cách thuyền 1 khoảng rộng để mỗi con 1 ô riêng biệt
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
        // Vô hiệu hóa script di chuyển của lính trong chốc lát để chúng văng ra vị trí riêng
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
}