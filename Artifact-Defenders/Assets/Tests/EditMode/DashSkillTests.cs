using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for DashSkill component.
/// Tests the mana-consumption guard in TryDash and direction helpers.
/// Coroutine body and physics require PlayMode; those are out of scope here.
/// </summary>
public class DashSkillTests
{
    private GameObject go;
    private DashSkill dash;
    private PlayerMana mana;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        go.AddComponent<Rigidbody2D>();
        mana = go.AddComponent<PlayerMana>();
        mana.maxMana = 100;
        mana.regenRate = 0f;
        // Manually fire Awake on PlayerMana to initialise currentMana
        var awakeInfo = typeof(PlayerMana).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awakeInfo?.Invoke(mana, null);

        dash = go.AddComponent<DashSkill>();
        dash.manaCost = 10;
        dash.cooldown = 1f;
        dash.dashSpeed = 18f;
        dash.dashDur = 0.15f;
        dash.grantIFrame = false; // skip layer name lookup in tests
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // TryUse – mana guard
    // ---------------------------------------------------------------

    [Test]
    public void TryUse_WithSufficientMana_ConsumesCorrectManaAmount()
    {
        int manaBefore = mana.GetCurrentMana();
        dash.TryUse();
        // Mana should be deducted by manaCost when not on cooldown
        Assert.AreEqual(manaBefore - dash.manaCost, mana.GetCurrentMana());
    }

    [Test]
    public void TryUse_WithInsufficientMana_DoesNotConsumeMana()
    {
        // Drain mana below manaCost
        mana.TryUseMana(95); // leaves 5, manaCost = 10
        int manaBeforeDash = mana.GetCurrentMana();

        dash.TryUse();

        Assert.AreEqual(manaBeforeDash, mana.GetCurrentMana());
    }

    [Test]
    public void TryUse_ManaCostZero_StillConsumesZeroMana()
    {
        dash.manaCost = 0;
        dash.TryUse();
        // Spending 0 mana should leave mana unchanged
        Assert.AreEqual(mana.GetMaxMana(), mana.GetCurrentMana());
    }

    // ---------------------------------------------------------------
    // TryUse(Vector2) – direction variant
    // ---------------------------------------------------------------

    [Test]
    public void TryUseWithDirection_WithSufficientMana_ConsumesCorrectManaAmount()
    {
        int manaBefore = mana.GetCurrentMana();
        dash.TryUse(Vector2.right);
        Assert.AreEqual(manaBefore - dash.manaCost, mana.GetCurrentMana());
    }

    [Test]
    public void TryUseWithDirection_WithInsufficientMana_DoesNotConsumeMana()
    {
        mana.TryUseMana(95);
        int manaBeforeDash = mana.GetCurrentMana();

        dash.TryUse(Vector2.up);

        Assert.AreEqual(manaBeforeDash, mana.GetCurrentMana());
    }
}
