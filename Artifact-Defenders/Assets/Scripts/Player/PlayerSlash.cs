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
        if (playerMana != null)
            playerMana.RestoreMana(manaRestore);
        Instantiate(slashPrefab, transform.position, transform.rotation);
        if (playerAnim != null) playerAnim.PlayAttack(0.3f);
        if (playerMovement != null) playerMovement.StopMovementForAttack(0.3f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2,1.5f), pivot.rotation.z, enemyMask);
        if(hits.Length != 0)
        {
            foreach (Collider2D hit in hits)
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                BossAI boss = hit.GetComponent<BossAI>();   
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // hoặc damage nếu bạn muốn tính theo Player
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
