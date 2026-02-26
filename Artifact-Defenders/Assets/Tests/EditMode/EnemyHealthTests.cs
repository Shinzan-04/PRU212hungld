using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for EnemyHealth fallback path (no EnemyAI / BossAI on the same GameObject).
/// AI-delegated paths require PlayMode because they instantiate heavy components.
/// </summary>
public class EnemyHealthTests
{
    private GameObject go;
    private EnemyHealth health;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Enemy");
        health = go.AddComponent<EnemyHealth>();
        health.max = 50;
        health.current = 50; // Set manually; Awake sets current=max but max=0 until set
    }

    [TearDown]
    public void TearDown()
    {
        // go may have been destroyed by Destroy(gameObject) inside DamageEnemy
        if (go != null)
            Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Fallback damage path (no EnemyAI / BossAI)
    // ---------------------------------------------------------------

    [Test]
    public void DamageEnemy_ReducesCurrentHealth()
    {
        health.DamageEnemy(10);
        Assert.AreEqual(40, health.current);
    }

    [Test]
    public void DamageEnemy_MultipleHits_AccumulatesDamage()
    {
        health.DamageEnemy(10);
        health.DamageEnemy(15);
        Assert.AreEqual(25, health.current);
    }

    [Test]
    public void DamageEnemy_ZeroAmount_DoesNotChangeHealth()
    {
        health.DamageEnemy(0);
        Assert.AreEqual(50, health.current);
    }

    [Test]
    public void DamageEnemy_MoreThanCurrentHealth_ClampedToZeroAndDestroys()
    {
        // GameObject will be Destroyed inside DamageEnemy when health <= 0
        health.DamageEnemy(60);
        // After destroy, current should have been set to 0 before Destroy is called
        Assert.AreEqual(0, health.current);
        // The GameObject is queued for destruction; it still exists within the same frame
        go = null; // prevent double-DestroyImmediate in TearDown
    }

    [Test]
    public void DamageEnemy_ExactlyLethal_SetsCurrentToZero()
    {
        health.DamageEnemy(50);
        Assert.AreEqual(0, health.current);
        go = null;
    }
}
