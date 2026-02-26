using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for SpinAttackSkill mana-guard and cooldown logic.
/// The SpinRoutine coroutine and Physics2D damage ticks are PlayMode concerns.
/// </summary>
public class SpinAttackSkillTests
{
    private GameObject go;
    private SpinAttackSkill skill;
    private PlayerMana mana;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        mana = go.AddComponent<PlayerMana>();
        mana.maxMana = 100;
        mana.regenRate = 0f;
        var awake = typeof(PlayerMana).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(mana, null);

        skill = go.AddComponent<SpinAttackSkill>();
        skill.manaCost     = 25;
        skill.cooldown     = 0.8f;
        skill.radius       = 1.3f;
        skill.duration     = 0.45f;
        skill.ticksPerSecond = 8f;
        // bodyRenderer / spinFrames left null – TryUse guards are before renderer code
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Mana guard
    // ---------------------------------------------------------------

    [Test]
    public void TryUse_WithSufficientMana_ConsumesCorrectMana()
    {
        int before = mana.GetCurrentMana();
        skill.TryUse();
        Assert.AreEqual(before - skill.manaCost, mana.GetCurrentMana());
    }

    [Test]
    public void TryUse_WithInsufficientMana_DoesNotConsumeMana()
    {
        mana.TryUseMana(80); // leaves 20, manaCost = 25
        int before = mana.GetCurrentMana();
        skill.TryUse();
        Assert.AreEqual(before, mana.GetCurrentMana());
    }

    // ---------------------------------------------------------------
    // Cooldown guard
    // ---------------------------------------------------------------

    [Test]
    public void TryUse_SecondCallBeforeCooldownExpires_DoesNotConsumeManaAgain()
    {
        skill.cooldown = 9999f;

        skill.TryUse(); // first use – ok
        int manaAfterFirst = mana.GetCurrentMana();

        skill.TryUse(); // blocked by cooldown
        Assert.AreEqual(manaAfterFirst, mana.GetCurrentMana());
    }
}
