using System.Collections;
using UnityEngine;

public class BossAnim : MonoBehaviour
{
    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite[] hurtSprites;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private Sprite[] castSprites;

    [Header("Animation Settings")]
    [SerializeField] private float frameTime = 0.15f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip castSound;

    private bool hasPlayedDeathSound = false;

    private SpriteRenderer spriteRenderer;
    private BossAI bossAI;
    private int frameIndex = 0;
    private float timer;
    private bool isDead = false;

    private Sprite[] lastAnim = null;
    private bool isPlayingOnce = false;
    private Coroutine playOnceCoroutine = null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossAI = GetComponent<BossAI>();
    }

    void Update()
    {
        if (isDead) return;

        Sprite[] currentAnim = GetCurrentAnim();
        if (currentAnim == null || currentAnim.Length == 0) return;

        if (currentAnim != lastAnim)
        {
            lastAnim = currentAnim;
            frameIndex = 0;
            timer = Time.time + frameTime;
            spriteRenderer.sprite = currentAnim[frameIndex];
        }

        if (!isPlayingOnce)
        {
            if (Time.time >= timer)
            {
                frameIndex = (frameIndex + 1) % currentAnim.Length;
                spriteRenderer.sprite = currentAnim[frameIndex];
                timer = Time.time + frameTime;
            }
        }

        spriteRenderer.flipX = bossAI.left;

        HandleSound(); // 👈 xử lý sound
    }

    private Sprite[] GetCurrentAnim()
    {
        if (bossAI.isDead)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(deathSprites, true));
            return deathSprites;
        }

        if (bossAI.isHurt)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(hurtSprites, false));
            return hurtSprites;
        }

        if (bossAI.isCasting)
            return castSprites;

        if (bossAI.isAttacking)
            return attackSprites;

        if (bossAI.isMoving)
            return walkSprites;

        return idleSprites;
    }

    private IEnumerator PlayOnce(Sprite[] anim, bool dieAfter)
    {
        if (anim == null || anim.Length == 0)
        {
            if (dieAfter) isDead = true;
            else bossAI.isHurt = false;
            playOnceCoroutine = null;
            yield break;
        }

        isPlayingOnce = true;

        for (int i = 0; i < anim.Length; i++)
        {
            spriteRenderer.sprite = anim[i];

            // 🔥 SOUND THEO FRAME

            if (anim == attackSprites && i == 1)
                audioSource.PlayOneShot(attackSound);

            if (anim == hurtSprites && i == 0)
                audioSource.PlayOneShot(hurtSound);

            if (anim == castSprites && i == 0)
                audioSource.PlayOneShot(castSound);

            if (anim == deathSprites && i == 0 && !hasPlayedDeathSound)
            {
                audioSource.PlayOneShot(deathSound);
                hasPlayedDeathSound = true;
            }

            yield return new WaitForSeconds(frameTime);
        }

        isPlayingOnce = false;
        playOnceCoroutine = null;

        if (dieAfter)
        {
            isDead = true;
            spriteRenderer.sprite = anim[anim.Length - 1];
        }
        else
        {
            bossAI.isHurt = false;
        }

        frameIndex = 0;
        timer = Time.time + frameTime;
    }

    // 🔊 WALK LOOP
    void HandleSound()
    {
        if (bossAI.isDead) return;

        if (bossAI.isMoving && !bossAI.isAttacking && !bossAI.isCasting && !bossAI.isHurt)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.loop)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
        }
    }
}