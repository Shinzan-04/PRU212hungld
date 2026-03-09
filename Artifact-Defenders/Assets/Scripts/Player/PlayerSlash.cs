using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible of creating and managing the Player's melee attack
/// </summary>
public class PlayerSlash : MonoBehaviour
{
    public GameObject slashPrefab;
    public float cooldown;
    public Transform pivot;
    public int damage = 35;

    [Header("Mana Restore")] // Mana
    [Min(1)] public int manaRestore = 10;

    float timer;
    new Collider2D collider2D;
    public LayerMask enemyMask;
    AudioSource audioSource;

    PlayerMovement playerMovement;
    PlayerSpriteAnim playerAnim;
    PlayerMana playerMana;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerAnim = GetComponentInParent<PlayerSpriteAnim>();
        playerMana = GetComponentInParent<PlayerMana>();
    }
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Mouse0) && Time.time > timer)
        //{
        //    // --- HỒI MANA ---
        //    if (playerMana == null)
        //    {
        //        return;
        //    }
        //    playerMana.RestoreMana(manaRestore);

        //    Slash();
        //    audioSource.Play();
        //    timer = Time.time + cooldown;
        //}
        // Kiểm tra nếu người dùng nhấn chuột trái (nút 0)
        if (Input.GetMouseButtonDown(0))
        {
            OnAttackButtonPressed();
        }
    }
    void Slash()
    {
        // 1. Thực hiện các hiệu ứng hình ảnh/âm thanh/di chuyển trước
        Instantiate(slashPrefab, transform.position, transform.rotation);
        if (playerAnim != null) playerAnim.PlayAttack(0.3f);
        if (playerMovement != null) playerMovement.StopMovementForAttack(0.3f);

        // 2. Kiểm tra va chạm bằng OverlapBox
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2, 1.5f), pivot.rotation.z, enemyMask);

        // 3. LOGIC HỒI MANA: Chỉ chạy khi danh sách trúng đòn không trống
        if (hits.Length != 0)
        {
            // Kiểm tra xem có script mana không trước khi hồi
            if (playerMana != null)
            {
                playerMana.RestoreMana(manaRestore);
                // Debug.Log("Trúng đích! Đã hồi mana.");
            }

            // 4. Gây sát thương cho từng đối tượng trúng đòn
            foreach (Collider2D hit in hits)
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                BossAI boss = hit.GetComponent<BossAI>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                else if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }
    }
    public void OnAttackButtonPressed()
    {
        if (Time.time > timer)
        {
            Slash();
            audioSource.Play();
            timer = Time.time + cooldown;
        }
    }

}
