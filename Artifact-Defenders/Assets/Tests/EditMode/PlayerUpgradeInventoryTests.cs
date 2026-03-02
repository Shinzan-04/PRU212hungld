using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerUpgradeInventory component.
/// UI-dependent parts (UpdateUI, ShowFloatingText) are excluded; they need a Canvas.
/// </summary>
public class PlayerUpgradeInventoryTests
{
    private GameObject go;
    private PlayerUpgradeInventory inv;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        inv = go.AddComponent<PlayerUpgradeInventory>();
        inv.upgradeStones = 0;
        inv.maxStones = 10;
        // stoneText / floatingTextPrefab left null → UpdateUI guards safely
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // AddStone
    // ---------------------------------------------------------------

    [Test]
    public void AddStone_NormalAmount_IncreasesStoneCount()
    {
        inv.AddStone(3);
        Assert.AreEqual(3, inv.upgradeStones);
    }

    [Test]
    public void AddStone_ExceedingMax_ClampsToMax()
    {
        inv.AddStone(15);
        Assert.AreEqual(10, inv.upgradeStones);
    }

    [Test]
    public void AddStone_MultipleAdds_Accumulates()
    {
        inv.AddStone(3);
        inv.AddStone(4);
        Assert.AreEqual(7, inv.upgradeStones);
    }

    [Test]
    public void AddStone_ExactlyMax_EqualsMax()
    {
        inv.AddStone(10);
        Assert.AreEqual(10, inv.upgradeStones);
    }

    [Test]
    public void AddStone_WhenFull_StaysAtMax()
    {
        inv.AddStone(10);
        inv.AddStone(5);
        Assert.AreEqual(10, inv.upgradeStones);
    }

    // ---------------------------------------------------------------
    // UseStones
    // ---------------------------------------------------------------

    [Test]
    public void UseStones_WithSufficientStones_ReturnsTrueAndDeducts()
    {
        inv.AddStone(8);
        bool result = inv.UseStones(5);
        Assert.IsTrue(result);
        Assert.AreEqual(3, inv.upgradeStones);
    }

    [Test]
    public void UseStones_WithExactStones_ReturnsTrueAndZeros()
    {
        inv.AddStone(5);
        bool result = inv.UseStones(5);
        Assert.IsTrue(result);
        Assert.AreEqual(0, inv.upgradeStones);
    }

    [Test]
    public void UseStones_WithInsufficientStones_ReturnsFalseAndKeepsStones()
    {
        inv.AddStone(3);
        bool result = inv.UseStones(5);
        Assert.IsFalse(result);
        Assert.AreEqual(3, inv.upgradeStones);
    }

    [Test]
    public void UseStones_ZeroAmount_ReturnsTrueAndKeepsStones()
    {
        inv.AddStone(5);
        bool result = inv.UseStones(0);
        Assert.IsTrue(result);
        Assert.AreEqual(5, inv.upgradeStones);
    }
}
