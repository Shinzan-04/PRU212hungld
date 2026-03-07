using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 moveDirection;
    private float speed;
    private int damage;
    private bool hasHit = false;

    // Đây là hàm mà ArtifactPointLauncher đang gọi tới
    public void Setup(Vector2 dir, float _speed, int _damage)
    {
        moveDirection = dir;
        speed = _speed;
        damage = _damage;

        // Tự hủy sau 3 giây để tránh làm rác bộ nhớ nếu bay trượt
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Di chuyển mũi tên theo hướng đã định
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // Tìm component sức khỏe trên mục tiêu
        EnemyHealth health = collision.GetComponent<EnemyHealth>();

        if (health != null)
        {
            hasHit = true;
            health.DamageEnemy(damage); // Gây sát thương
            Destroy(gameObject);        // Mũi tên biến mất khi trúng đích
        }
    }
}