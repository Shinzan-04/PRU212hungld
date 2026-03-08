using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound; // Âm thanh khi trúng địch
    [Range(0f, 1f)][SerializeField] private float hitVolume = 1f;

    private Vector2 moveDirection;
    private float speed;
    private int damage;
    private bool hasHit = false;

    public void Setup(Vector2 dir, float _speed, int _damage)
    {
        moveDirection = dir;
        speed = _speed;
        damage = _damage;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        EnemyHealth health = collision.GetComponent<EnemyHealth>();

        if (health != null)
        {
            hasHit = true;

            // PHÁT ÂM THANH KHI TRÚNG ĐÍCH
            if (hitSound != null)
            {
                // Dùng PlayClipAtPoint để âm thanh tiếp tục phát sau khi Arrow bị Destroy
                AudioSource.PlayClipAtPoint(hitSound, transform.position, hitVolume);
            }

            health.DamageEnemy(damage);
            Destroy(gameObject);
        }
    }
}