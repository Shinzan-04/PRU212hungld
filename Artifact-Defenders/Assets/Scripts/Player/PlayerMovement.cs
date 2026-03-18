using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible of the player movement and managing the SpriteRenderer.flipX property
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public Joystick joystick;

    new Rigidbody2D rigidbody;
    Vector2 normVector;
    SpriteRenderer sprite;

    float timer;
    bool harvesting;

    bool attacking;
    float attackTimer;

    public Vector2 MoveDirection { get; private set; }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Reset harvesting
        if (harvesting && Time.time > timer)
            harvesting = false;

        // Reset attacking
        if (attacking && Time.time > attackTimer)
            attacking = false;

        FlipSprite();
    }

    private void FlipSprite()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.1f)
        {
            h = joystick.Horizontal;
        }

        if (h > 0.1f)
            sprite.flipX = false;
        else if (h < -0.1f)
            sprite.flipX = true;
    }

    void FixedUpdate()
    {
        // 👉 CHỈ chặn khi harvesting (KHÔNG chặn khi attacking nữa)
        if (harvesting)
        {
            rigidbody.linearVelocity = Vector2.zero;
            MoveDirection = Vector2.zero;
            return;
        }

        // Lấy input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (joystick != null)
        {
            if (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f)
            {
                h = joystick.Horizontal;
                v = joystick.Vertical;
            }
        }

        normVector = new Vector2(h, v);

        if (normVector.sqrMagnitude > 1)
            normVector = normVector.normalized;

        // 👉 Nếu đang đánh thì giảm tốc nhẹ (tùy chọn, có thể xoá nếu không thích)
        float currentSpeed = movementSpeed;
        if (attacking)
        {
            currentSpeed *= 0.7f; // giảm 30% tốc độ khi đánh
        }

        rigidbody.linearVelocity = normVector * currentSpeed;

        MoveDirection = normVector;
    }

    // ===== HARVEST =====
    public void HarvestStopMovement(float time)
    {
        harvesting = true;
        timer = Time.time + time;
    }

    public bool IsHarvesting()
    {
        return harvesting;
    }

    // ===== ATTACK =====
    public bool IsAttacking()
    {
        return attacking;
    }

    public void StopMovementForAttack(float time)
    {
        attacking = true;
        attackTimer = Time.time + time;
    }

    // ===== OTHER =====
    public Vector2 GetVelocity()
    {
        return rigidbody.linearVelocity;
    }
}