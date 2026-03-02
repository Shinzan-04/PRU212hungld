using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for Artifact component (health management).
/// Trigger-collision healing and bleed-over-time require PlayMode; excluded here.
/// </summary>
public class ArtifactTests
{
    private GameObject go;
    private Artifact artifact;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Artifact");
        go.AddComponent<AudioSource>();
        artifact = go.AddComponent<Artifact>();
        artifact.maxHealth = 100;
        artifact.bleed = 0;

        // Invoke Awake to init health = maxHealth
        var awake = typeof(Artifact).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(artifact, null);
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
    public void Awake_SetsHealthToMaxHealth()
    {
        Assert.AreEqual(100, artifact.health);
    }

    // ---------------------------------------------------------------
    // Damage
    // ---------------------------------------------------------------

    [Test]
    public void Damage_ReducesHealthByGivenAmount()
    {
        artifact.Damage(30);
        Assert.AreEqual(70, artifact.health);
    }

    [Test]
    public void Damage_CanReduceHealthBelowZero()
    {
        // Damage does NOT clamp; Update() clamps per frame
        artifact.Damage(150);
        Assert.AreEqual(-50, artifact.health);
    }

    [Test]
    public void Damage_ZeroAmount_DoesNotChangeHealth()
    {
        artifact.Damage(0);
        Assert.AreEqual(100, artifact.health);
    }

    [Test]
    public void Damage_MultipleHits_Accumulates()
    {
        artifact.Damage(20);
        artifact.Damage(30);
        Assert.AreEqual(50, artifact.health);
    }

    // ---------------------------------------------------------------
    // SetMaxHealth
    // ---------------------------------------------------------------

    [Test]
    public void SetMaxHealth_UpdatesMaxAndResetsHealth()
    {
        artifact.SetMaxHealth(200);

        Assert.AreEqual(200, artifact.maxHealth);
        Assert.AreEqual(200, artifact.health);
    }

    [Test]
    public void SetMaxHealth_ToLowerValue_ResetsHealthToNewMax()
    {
        artifact.Damage(10); // health = 90
        artifact.SetMaxHealth(50);

        Assert.AreEqual(50, artifact.maxHealth);
        Assert.AreEqual(50, artifact.health); // reset by SetMaxHealth
    }
}
