using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for UpgradeItem.
/// OnTriggerEnter2D (physics-based pickup) is PlayMode only.
/// We verify the StoneType enum values and public field defaults only.
/// </summary>
public class UpgradeItemTests
{
    private GameObject go;
    private UpgradeItem item;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("UpgradeItem");
        item = go.AddComponent<UpgradeItem>();
    }

    [TearDown]
    public void TearDown()
    {
        if (go != null)
            Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // StoneType enum
    // ---------------------------------------------------------------

    [Test]
    public void StoneType_Tower_HasCorrectIntValue()
    {
        Assert.AreEqual(0, (int)UpgradeItem.StoneType.Tower);
    }

    [Test]
    public void StoneType_Attack_HasCorrectIntValue()
    {
        Assert.AreEqual(1, (int)UpgradeItem.StoneType.Attack);
    }

    // ---------------------------------------------------------------
    // Default config values
    // ---------------------------------------------------------------

    [Test]
    public void DefaultPickupRadius_IsGreaterThanZero()
    {
        Assert.Greater(item.pickupRadius, 0f);
    }

    [Test]
    public void DefaultMoveSpeed_IsGreaterThanZero()
    {
        Assert.Greater(item.moveSpeed, 0f);
    }

    // ---------------------------------------------------------------
    // StoneType assignment
    // ---------------------------------------------------------------

    [Test]
    public void AssignStoneType_Tower_ReflectsCorrectly()
    {
        item.stoneType = UpgradeItem.StoneType.Tower;
        Assert.AreEqual(UpgradeItem.StoneType.Tower, item.stoneType);
    }

    [Test]
    public void AssignStoneType_Attack_ReflectsCorrectly()
    {
        item.stoneType = UpgradeItem.StoneType.Attack;
        Assert.AreEqual(UpgradeItem.StoneType.Attack, item.stoneType);
    }
}
