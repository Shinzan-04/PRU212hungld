using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerSpriteAnim component.
/// Verifies the PlayAttack / IsAttacking methods in isolation.
/// </summary>
public class PlayerSpriteAnimTests
{
    private GameObject go;
    private PlayerSpriteAnim anim;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        // PlayerSpriteAnim needs SpriteRenderer and PlayerMovement
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Rigidbody2D>();
        go.AddComponent<PlayerMovement>();
        anim = go.AddComponent<PlayerSpriteAnim>();

        // Provide minimal sprite arrays so UpdateCurrentAnim doesn't NRE
        anim.idleSprites   = new Sprite[1] { Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), Vector2.one * 0.5f) };
        anim.runSprites    = new Sprite[1] { Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), Vector2.one * 0.5f) };
        anim.attackSprites = new Sprite[1] { Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), Vector2.one * 0.5f) };
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Initial state
    // ---------------------------------------------------------------

    [Test]
    public void InitialState_IsNotAttacking()
    {
        Assert.IsFalse(anim.IsAttacking());
    }

    // ---------------------------------------------------------------
    // PlayAttack
    // ---------------------------------------------------------------

    [Test]
    public void PlayAttack_SetsIsAttackingTrue()
    {
        anim.PlayAttack(0.3f);

        Assert.IsTrue(anim.IsAttacking());
    }

    [Test]
    public void PlayAttack_CalledMultipleTimes_StaysAttacking()
    {
        anim.PlayAttack(0.3f);
        anim.PlayAttack(0.5f);

        Assert.IsTrue(anim.IsAttacking());
    }

    [Test]
    public void PlayAttack_WithZeroDuration_IsStillAttackingImmediately()
    {
        // attackEndTime = Time.time + 0 → expires next Update tick, not yet
        anim.PlayAttack(0f);

        Assert.IsTrue(anim.IsAttacking());
    }
}
