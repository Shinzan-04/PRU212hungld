using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for SkillManager.
/// Verifies that the manager wires through correctly to each skill,
/// and that null-safe delegates (UseDash / UseSpin / UseBoomerang) do not throw.
/// </summary>
public class SkillManagerTests
{
    private GameObject go;
    private SkillManager manager;
    private PlayerMana mana;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        go.AddComponent<Rigidbody2D>();
        mana = go.AddComponent<PlayerMana>();
        mana.maxMana = 200;
        mana.regenRate = 0f;
        var awake = typeof(PlayerMana).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(mana, null);

        manager = go.AddComponent<SkillManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Null-safe delegates: skills not assigned → no exception
    // ---------------------------------------------------------------

    [Test]
    public void UseDash_WhenDashIsNull_DoesNotThrow()
    {
        manager.dash = null;
        Assert.DoesNotThrow(() => manager.UseDash());
    }

    [Test]
    public void UseSpin_WhenSpinIsNull_DoesNotThrow()
    {
        manager.spin = null;
        Assert.DoesNotThrow(() => manager.UseSpin());
    }

    [Test]
    public void UseBoomerang_WhenBoomerangIsNull_DoesNotThrow()
    {
        manager.boomerang = null;
        Assert.DoesNotThrow(() => manager.UseBoomerang());
    }

    // ---------------------------------------------------------------
    // Delegation to actual skill components
    // ---------------------------------------------------------------

    [Test]
    public void UseDash_WhenAssigned_ConsumesMana()
    {
        var dash = go.AddComponent<DashSkill>();
        dash.manaCost = 10;
        dash.cooldown = 1f;
        dash.grantIFrame = false;
        manager.dash = dash;

        int before = mana.GetCurrentMana();
        manager.UseDash();

        Assert.AreEqual(before - 10, mana.GetCurrentMana());
    }

    [Test]
    public void UseBoomerang_WhenAssigned_ConsumesMana()
    {
        var boomerang = go.AddComponent<BoomerangSkill>();
        boomerang.manaCost  = 15;
        boomerang.cooldown  = 0.6f;
        boomerang.aimAtMouse = false;
        // projectilePrefab = null → TryUse early-exits after mana deduction
        manager.boomerang = boomerang;

        int before = mana.GetCurrentMana();
        manager.UseBoomerang();

        Assert.AreEqual(before - 15, mana.GetCurrentMana());
    }

    [Test]
    public void UseSpin_WhenAssigned_ConsumesMana()
    {
        var spin = go.AddComponent<SpinAttackSkill>();
        spin.manaCost = 25;
        spin.cooldown = 0.8f;
        manager.spin = spin;

        int before = mana.GetCurrentMana();
        manager.UseSpin();

        Assert.AreEqual(before - 25, mana.GetCurrentMana());
    }
}
