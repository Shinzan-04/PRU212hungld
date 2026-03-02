using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy logic: move, attack artifact or eat bushes, and take damage.
/// Combined from original + extended version.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] bool isEater = false; // nếu true thì ăn cây, nếu false thì đánh trụ
    // --- MỚI ---
    [SerializeField] bool isBoat = false; // Nếu true, quái di chuyển trên nước

    [Header("Stats")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int maxHealth = 10;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float eatTime = 1.2f;

    [Header("Target Masks")]
    [SerializeField] LayerMask bushesMask;
    // --- MỚI ---
    [SerializeField] LayerMask waterMask; // Mask cho vùng nước
    [SerializeField] LayerMask obstacleMask; // Mask cho cọc/vật cản

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool left;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;

    // trạng thái
    int currentHealth;
    float attackTimer;
    float eatTimer;
    bool killingBush;
    bool attacking;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public GameObject[] UpgradeItems;
    // mục tiêu
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
            artifact = GameObject.FindGameObjectWithTag("Artifact").GetComponent<Artifact>();
            attacking = false;
        }

        // Đồng bộ EnemyHealth UI
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
            // vẫn giữ hướng đúng
            if (artifact != null)
                left = artifact.transform.position.x < transform.position.x;
            return;
        }

        // --- MỚI ---
        if (isBoat)
        {
            HandleBoat();
            return;
        }

        if (isEater) HandleEater();
        else HandleAttacker();
    }

    // --- MỚI: BOAT LOGIC ---
    void HandleBoat()
    {
        if (artifact == null) return;

        // 1. Di chuyển về phía trụ
        MoveTowards(artifact.transform.position);

        // 2. Kiểm tra va chạm với cọc
        Collider2D obstacleHit = Physics2D.OverlapCircle(transform.position, 0.3f, obstacleMask);
        if (obstacleHit != null)
        {
            StartCoroutine(DieRoutine()); // Tự hủy khi gặp cọc
            return;
        }

        // 3. Kiểm tra xem còn ở trên nước không
        Collider2D waterHit = Physics2D.OverlapCircle(transform.position, 0.3f, waterMask);
        if (waterHit == null)
        {
            StartCoroutine(DieRoutine()); // Tự hủy khi rời khỏi nước
            return;
        }

        left = artifact.transform.position.x < transform.position.x;
    }

    // === EATER LOGIC ===
    void HandleEater()
    {
        if (target == null || !target.enabled)
        {
            SearchForTarget();
            return;
        }

        float dist = Vector2.Distance(transform.position, target.transform.position);

        // Nếu còn trái và chưa ăn bụi
        if (target.HasFruits() && !killingBush)
        {
            if (dist > 0.5f)
            {
                MoveTowards(target.transform.position);
            }
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
        else
        {
            SearchForTarget();
        }

        if (target != null)
            left = target.transform.position.x < transform.position.x;
    }

    IEnumerator EatRoutine()
    {
        isAttacking = true;


        yield return new WaitForSeconds(0.45f); // frame 4: thực hiện ăn

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

        yield return new WaitForSeconds(0.15f); // frame 5: kết thúc animation
        isAttacking = false;
    }


    // === ATTACKER LOGIC ===
    void HandleAttacker()
    {
        if (artifact == null) return;

        float distance = Vector2.Distance(transform.position, artifact.transform.position);

        if (distance > 1.5f)
        {
            MoveTowards(artifact.transform.position);
        }
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
        yield return null;
        yield return new WaitForSeconds(0.45f);
        Attack();
        yield return new WaitForSeconds(0.15f);
        isAttacking = false;
    }

    void MoveTowards(Vector3 targetPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        isMoving = true;
        isAttacking = false;
    }

    void Attack()
    {
        if (artifact != null)
            artifact.Damage(attackDamage);
    }

    // === TÌM BỤI CÂY GẦN NHẤT ===
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

    // === NHẬN SÁT THƯƠNG ===
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - dmg);

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.current = currentHealth;

        if (currentHealth > 0)
            StartCoroutine(HurtRoutine());
        else
            StartCoroutine(DieRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        isMoving = false;
        isAttacking = false;
        yield return new WaitForSeconds(0.4f);
        isHurt = false;
    }

    IEnumerator DieRoutine()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;
        isHurt = false;

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.current = 0;

        // Tắt collider để không va chạm khi chết
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(1.0f);

        if (UpgradeItems != null && UpgradeItems.Length > 0)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
            int rand = Random.Range(0, UpgradeItems.Length); // chọn ngẫu nhiên 1 loại đá
            GameObject item = Instantiate(UpgradeItems[rand], spawnPos, Quaternion.identity);

            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Văng nhẹ ra hướng ngẫu nhiên
                Vector2 dir = Random.insideUnitCircle.normalized;
                rb.AddForce(dir * 2f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}