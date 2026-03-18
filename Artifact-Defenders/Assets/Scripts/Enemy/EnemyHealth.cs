using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public int max;
    public int current;

    private EnemyAI enemyAI;
    private BossAI bossAI;

    private void Awake()
    {
        // Khởi tạo máu
        current = max;

        // Lấy component
        enemyAI = GetComponent<EnemyAI>();
        bossAI = GetComponent<BossAI>();
    }

    // Gọi khi enemy bị gây damage
    public void DamageEnemy(int amount)
    {
        // Nếu có EnemyAI thì để AI xử lý
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(amount);
            current = Mathf.Max(0, enemyAI.CurrentHealth);
            return;
        }

        // Nếu là Boss
        if (bossAI != null)
        {
            bossAI.TakeDamage(amount);
            current = Mathf.Max(0, bossAI.CurrentHealth);
            return;
        }

        // Nếu không có AI thì xử lý trực tiếp
        current -= amount;

        if (current <= 0)
        {
            current = 0;
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}