using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerMovement component.
/// Tests the public methods: HarvestStopMovement, IsHarvesting, IsAttacking, StopMovementForAttack.
/// Note: FixedUpdate / FlipSprite logic relies on Input & Rigidbody2D and is better
/// covered in PlayMode tests; only pure state-machine methods are tested here.
/// </summary>
public class PlayerMovementTests
{
    private GameObject go;
    private PlayerMovement movement;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        // PlayerMovement caches Rigidbody2D in Start – add it first
        go.AddComponent<Rigidbody2D>();
        go.AddComponent<SpriteRenderer>();
        movement = go.AddComponent<PlayerMovement>();
        movement.movementSpeed = 5f;
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
    public void InitialState_IsNotHarvestingAndNotAttacking()
    {
        Assert.IsFalse(movement.IsHarvesting());
        Assert.IsFalse(movement.IsAttacking());
    }

    // ---------------------------------------------------------------
    // HarvestStopMovement / IsHarvesting
    // ---------------------------------------------------------------

    [Test]
    public void HarvestStopMovement_SetsHarvestingTrue()
    {
        movement.HarvestStopMovement(1f);

        Assert.IsTrue(movement.IsHarvesting());
    }

    [Test]
    public void HarvestStopMovement_CalledWithZeroTime_StillSetsHarvesting()
    {
        // Edge case: timer = Time.time + 0 means it will expire next frame,
        // but immediately after the call it should still be true.
        movement.HarvestStopMovement(0f);

        Assert.IsTrue(movement.IsHarvesting());
    }

    // ---------------------------------------------------------------
    // StopMovementForAttack / IsAttacking
    // ---------------------------------------------------------------

    [Test]
    public void StopMovementForAttack_SetsAttackingTrue()
    {
        movement.StopMovementForAttack(0.5f);

        Assert.IsTrue(movement.IsAttacking());
    }

    [Test]
    public void StopMovementForAttack_DoesNotAffectHarvestingState()
    {
        movement.StopMovementForAttack(0.5f);

        Assert.IsFalse(movement.IsHarvesting());
    }

    [Test]
    public void HarvestStopMovement_DoesNotAffectAttackingState()
    {
        movement.HarvestStopMovement(1f);

        Assert.IsFalse(movement.IsAttacking());
    }

    [Test]
    public void BothHarvestAndAttack_CanBeSetIndependently()
    {
        movement.HarvestStopMovement(1f);
        movement.StopMovementForAttack(0.5f);

        Assert.IsTrue(movement.IsHarvesting());
        Assert.IsTrue(movement.IsAttacking());
    }
}
