using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerMana component.
/// These run in EditMode using the Unity Test Framework.
/// </summary>
public class PlayerManaTests
{
    private GameObject go;
    private PlayerMana playerMana;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        playerMana = go.AddComponent<PlayerMana>();
        playerMana.maxMana = 100;
        playerMana.regenRate = 5f;
        // Simulate Awake manually (EditMode does not call Awake automatically)
        var awake = typeof(PlayerMana).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(playerMana, null);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // GetCurrentMana / GetMaxMana
    // ---------------------------------------------------------------

    [Test]
    public void InitialMana_EqualsMaxMana()
    {
        Assert.AreEqual(100, playerMana.GetCurrentMana());
    }

    [Test]
    public void GetMaxMana_ReturnsCorrectValue()
    {
        Assert.AreEqual(100, playerMana.GetMaxMana());
    }

    // ---------------------------------------------------------------
    // TryUseMana
    // ---------------------------------------------------------------

    [Test]
    public void TryUseMana_WithSufficientMana_ReturnsTrueAndReducesMana()
    {
        bool result = playerMana.TryUseMana(30);

        Assert.IsTrue(result);
        Assert.AreEqual(70, playerMana.GetCurrentMana());
    }

    [Test]
    public void TryUseMana_WithExactMana_ReturnsTrueAndZerosMana()
    {
        bool result = playerMana.TryUseMana(100);

        Assert.IsTrue(result);
        Assert.AreEqual(0, playerMana.GetCurrentMana());
    }

    [Test]
    public void TryUseMana_WithInsufficientMana_ReturnsFalseAndKeepsMana()
    {
        bool result = playerMana.TryUseMana(150);

        Assert.IsFalse(result);
        Assert.AreEqual(100, playerMana.GetCurrentMana());
    }

    [Test]
    public void TryUseMana_WithZeroAmount_ReturnsTrueAndKeepsMana()
    {
        bool result = playerMana.TryUseMana(0);

        Assert.IsTrue(result);
        Assert.AreEqual(100, playerMana.GetCurrentMana());
    }

    [Test]
    public void TryUseMana_MultipleConsecutiveCalls_AccumulatesDeduction()
    {
        playerMana.TryUseMana(30);
        playerMana.TryUseMana(30);

        Assert.AreEqual(40, playerMana.GetCurrentMana());
    }

    // ---------------------------------------------------------------
    // RestoreMana
    // ---------------------------------------------------------------

    [Test]
    public void RestoreMana_BelowMax_IncreasesMana()
    {
        playerMana.TryUseMana(50); // currentMana = 50
        playerMana.RestoreMana(20);

        Assert.AreEqual(70, playerMana.GetCurrentMana());
    }

    [Test]
    public void RestoreMana_ExceedingMax_ClampedToMax()
    {
        playerMana.TryUseMana(10); // currentMana = 90
        playerMana.RestoreMana(50); // would be 140, but clamped to 100

        Assert.AreEqual(100, playerMana.GetCurrentMana());
    }

    [Test]
    public void RestoreMana_WhenFull_StaysAtMax()
    {
        playerMana.RestoreMana(30);

        Assert.AreEqual(100, playerMana.GetCurrentMana());
    }
}
