using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for BoomerangSkill mana-guard and cooldown logic.
/// Projectile instantiation / physics are PlayMode concerns.
/// </summary>
public class BoomerangSkillTests
{
    private GameObject go;
    private BoomerangSkill skill;
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

        skill = go.AddComponent<BoomerangSkill>();
        skill.manaCost  = 15;
        skill.cooldown  = 0.6f;
        skill.speed     = 10f;
        skill.maxDistance = 5f;
        skill.damage    = 2;
        skill.aimAtMouse = false;
        // projectilePrefab left null; TryUse will early-out before Instantiate
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
        mana.TryUseMana(90); // leaves 10, manaCost = 15
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
        skill.cooldown = 9999f; // very long cooldown

        skill.TryUse(); // first use – ok, consumes mana
        int manaAfterFirst = mana.GetCurrentMana();

        skill.TryUse(); // should be blocked by cooldown
        Assert.AreEqual(manaAfterFirst, mana.GetCurrentMana());
    }
}
