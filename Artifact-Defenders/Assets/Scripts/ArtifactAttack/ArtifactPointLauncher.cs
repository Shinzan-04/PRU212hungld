using UnityEngine;

// Đảm bảo tên này khớp hệt với tên file bạn đặt trong thư mục Project
public class ArtifactPointLauncher : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private int damage = 2;
    [SerializeField] private float arrowSpeed = 12f;

    private float fireCountdown = 0f;

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Transform target = GetNearestTarget();
            if (target != null)
            {
                Shoot(target);
                fireCountdown = 1f / fireRate;
            }
        }
    }

    Transform GetNearestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetMask);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D col in colliders)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = col.transform;
            }
        }
        return nearest;
    }

    void Shoot(Transform target)
    {
        if (arrowPrefab == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject arrowObj = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        Arrow projectile = arrowObj.GetComponent<Arrow>();

        if (projectile != null)
        {
            projectile.Setup(dir, arrowSpeed, damage);
        }
    }
}