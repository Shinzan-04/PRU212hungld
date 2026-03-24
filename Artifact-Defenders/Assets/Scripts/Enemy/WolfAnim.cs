using System.Collections;
using UnityEngine;

public class WolfAnim : MonoBehaviour
{
    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite[] hurtSprites;
    [SerializeField] private Sprite[] deathSprites;

    [Header("Animation Settings")]
    [SerializeField] private float frameTime = 0.15f;

    private SpriteRenderer spriteRenderer;
    private EnemyAI enemyAI;
    private int frameIndex = 0;
    private float timer;
    private bool isDead = false;

    private Sprite[] lastAnim = null;
    private bool isPlayingOnce = false;
    private Coroutine playOnceCoroutine = null;

    void Awake()
    {
        // Tự động tìm Component để tránh lỗi Unassigned
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAI = GetComponent<EnemyAI>();
    }

    void Update()
    {
        if (isDead || enemyAI == null || spriteRenderer == null) return;

        Sprite[] currentAnim = GetCurrentAnim();
        if (currentAnim == null || currentAnim.Length == 0) return;

        // Reset frame khi chuyển đổi bộ Animation mới
        if (currentAnim != lastAnim)
        {
            lastAnim = currentAnim;
            frameIndex = 0;
            timer = Time.time + frameTime;
            spriteRenderer.sprite = currentAnim[frameIndex];
        }

        // Chạy animation lặp lại (Idle, Walk, Attack)
        if (!isPlayingOnce)
        {
            if (Time.time >= timer)
            {
                frameIndex = (frameIndex + 1) % currentAnim.Length;
                spriteRenderer.sprite = currentAnim[frameIndex];
                timer = Time.time + frameTime;
            }
        }

        // Lật mặt nhân vật theo hướng di chuyển
        spriteRenderer.flipX = enemyAI.left;
    }

    private Sprite[] GetCurrentAnim()
    {
        if (enemyAI.isDead)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(deathSprites, true));
            return deathSprites;
        }

        if (enemyAI.isHurt)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(hurtSprites, false));
            return hurtSprites;
        }

        if (enemyAI.isAttacking)
            return attackSprites;

        if (enemyAI.isMoving)
            return walkSprites;

        return idleSprites;
    }

    // Coroutine để chạy các animation chỉ diễn ra 1 lần (Hurt, Death)
    private IEnumerator PlayOnce(Sprite[] anim, bool dieAfter)
    {
        if (anim == null || anim.Length == 0)
        {
            if (dieAfter) isDead = true;
            else enemyAI.isHurt = false;
            playOnceCoroutine = null;
            yield break;
        }

        isPlayingOnce = true;

        for (int i = 0; i < anim.Length; i++)
        {
            spriteRenderer.sprite = anim[i];
            yield return new WaitForSeconds(frameTime);
        }

        isPlayingOnce = false;
        playOnceCoroutine = null;

        if (dieAfter)
        {
            isDead = true;
            // Giữ lại frame cuối cùng của animation chết
            spriteRenderer.sprite = anim[anim.Length - 1];
        }
        else
        {
            enemyAI.isHurt = false;
        }

        frameIndex = 0;
        timer = Time.time + frameTime;
    }
}