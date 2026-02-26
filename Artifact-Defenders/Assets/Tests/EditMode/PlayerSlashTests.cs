using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerSlash.
/// OnAttackButtonPressed is the public entry-point; we verify cooldown enforcement
/// and mana restore side-effect without needing Physics2D (no enemies in scene).
/// </summary>
public class PlayerSlashTests
{
    private GameObject go;
    private PlayerSlash slash;
    private PlayerMana mana;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        // PlayerSlash.Start reads these via GetComponentInParent
        go.AddComponent<Rigidbody2D>();
        mana = go.AddComponent<PlayerMana>();
        mana.maxMana  = 100;
        mana.regenRate = 0f;
        var awake = typeof(PlayerMana).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(mana, null);
        mana.TryUseMana(60); // start at 40 so RestoreMana is visible

        // AudioSource is required by OnAttackButtonPressed (audioSource.Play())
        go.AddComponent<AudioSource>();

        // Create child for PlayerSlash (mirrors the real prefab hierarchy)
        var childGo = new GameObject("SlashPivot");
        childGo.transform.SetParent(go.transform);
        slash = childGo.AddComponent<PlayerSlash>();
        slash.cooldown    = 1f;
        slash.damage      = 35;
        slash.manaRestore = 10;

        // Assign a pivot (required by Slash() for rotation check)
        var pivot = new GameObject("Pivot");
        pivot.transform.SetParent(childGo.transform);
        slash.pivot = pivot.transform;

        // slashPrefab left null so Instantiate early-exits after mana restore and before OverlapBoxAll
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Mana restore on slash
    // ---------------------------------------------------------------

    [Test]
    public void OnAttackButtonPressed_RestoresManaByManaRestoreAmount()
    {
        int before = mana.GetCurrentMana(); // 40
        slash.OnAttackButtonPressed();
        // manaRestore = 10 → 50
        Assert.AreEqual(before + slash.manaRestore, mana.GetCurrentMana());
    }

    [Test]
    public void OnAttackButtonPressed_ManaRestoreDoesNotExceedMax()
    {
        // Set mana close to full
        mana.RestoreMana(55); // mana = min(100, 40+55) = 95
        slash.manaRestore = 20;
        slash.OnAttackButtonPressed();
        Assert.AreEqual(mana.GetMaxMana(), mana.GetCurrentMana());
    }

    // ---------------------------------------------------------------
    // Cooldown enforcement
    // ---------------------------------------------------------------

    [Test]
    public void OnAttackButtonPressed_SecondCallBeforeCooldown_DoesNotRestoreManaAgain()
    {
        slash.cooldown = 9999f;

        slash.OnAttackButtonPressed(); // first – ok
        int manaAfterFirst = mana.GetCurrentMana();

        slash.OnAttackButtonPressed(); // blocked by cooldown
        Assert.AreEqual(manaAfterFirst, mana.GetCurrentMana());
    }
}
