using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerBackpack component.
/// </summary>
public class PlayerBackpackTests
{
    private GameObject go;
    private PlayerBackpack backpack;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        backpack = go.AddComponent<PlayerBackpack>();
        backpack.max = 10;
        backpack.current = 0;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // AddFruits
    // ---------------------------------------------------------------

    [Test]
    public void AddFruits_NormalAmount_IncreasesCurrentCount()
    {
        backpack.AddFruits(3);

        Assert.AreEqual(3, backpack.current);
    }

    [Test]
    public void AddFruits_ExceedingMax_ClampsToMax()
    {
        backpack.AddFruits(15);

        Assert.AreEqual(10, backpack.current);
    }

    [Test]
    public void AddFruits_MultipleAdds_Accumulates()
    {
        backpack.AddFruits(3);
        backpack.AddFruits(4);

        Assert.AreEqual(7, backpack.current);
    }

    [Test]
    public void AddFruits_ExactlyMax_EqualsMax()
    {
        backpack.AddFruits(10);

        Assert.AreEqual(10, backpack.current);
    }

    [Test]
    public void AddFruits_WhenFull_StaysAtMax()
    {
        backpack.AddFruits(10);
        backpack.AddFruits(5);

        Assert.AreEqual(10, backpack.current);
    }

    [Test]
    public void AddFruits_Zero_DoesNotChange()
    {
        backpack.AddFruits(5);
        backpack.AddFruits(0);

        Assert.AreEqual(5, backpack.current);
    }

    // ---------------------------------------------------------------
    // TakeFruits
    // ---------------------------------------------------------------

    [Test]
    public void TakeFruits_ReturnsCurrentAndResetsToZero()
    {
        backpack.AddFruits(7);

        int taken = backpack.TakeFruits();

        Assert.AreEqual(7, taken);
        Assert.AreEqual(0, backpack.current);
    }

    [Test]
    public void TakeFruits_WhenEmpty_ReturnsZero()
    {
        int taken = backpack.TakeFruits();

        Assert.AreEqual(0, taken);
    }

    [Test]
    public void TakeFruits_CalledTwice_SecondCallReturnsZero()
    {
        backpack.AddFruits(5);
        backpack.TakeFruits();
        int secondTake = backpack.TakeFruits();

        Assert.AreEqual(0, secondTake);
    }
}
