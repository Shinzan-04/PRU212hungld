using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PlayerHarvest public API.
/// OnHarvestButtonPressed is the public entry; TryHarvestClose requires Physics2D (PlayMode).
/// We verify that the component wires up cleanly and that the public button method exists.
/// </summary>
public class PlayerHarvestTests
{
    private GameObject go;
    private PlayerHarvest harvest;
    private PlayerMovement movement;
    private PlayerBackpack backpack;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("Player");
        go.AddComponent<Rigidbody2D>();
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<AudioSource>();
        movement = go.AddComponent<PlayerMovement>();
        movement.movementSpeed = 5f;
        backpack = go.AddComponent<PlayerBackpack>();
        backpack.max = 10;
        harvest = go.AddComponent<PlayerHarvest>();

        // Set serialized fields via reflection
        var radiusField = typeof(PlayerHarvest).GetField("harvestRadius",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var harvestTimeField = typeof(PlayerHarvest).GetField("harvestTime",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        radiusField?.SetValue(harvest, 1.5f);
        harvestTimeField?.SetValue(harvest, 0.5f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ---------------------------------------------------------------
    // Component wiring
    // ---------------------------------------------------------------

    [Test]
    public void PlayerHarvest_ComponentExists()
    {
        Assert.IsNotNull(go.GetComponent<PlayerHarvest>());
    }

    [Test]
    public void PlayerMovement_PresentOnSameObject()
    {
        Assert.IsNotNull(go.GetComponent<PlayerMovement>());
    }

    [Test]
    public void PlayerBackpack_PresentOnSameObject()
    {
        Assert.IsNotNull(go.GetComponent<PlayerBackpack>());
    }

    // ---------------------------------------------------------------
    // OnHarvestButtonPressed: no bush in scene → movement stays not harvesting
    // ---------------------------------------------------------------

    [Test]
    public void OnHarvestButtonPressed_WithNoBushNearby_MovementRemainsNotHarvesting()
    {
        // No bushes in test scene, so HarvestStopMovement should NOT be called
        // We call Start manually to wire internal references
        var start = typeof(PlayerHarvest).GetMethod("Start",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        start?.Invoke(harvest, null);

        harvest.OnHarvestButtonPressed();

        Assert.IsFalse(movement.IsHarvesting());
    }

    [Test]
    public void OnHarvestButtonPressed_WithNoBushNearby_BackpackRemainsEmpty()
    {
        var start = typeof(PlayerHarvest).GetMethod("Start",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        start?.Invoke(harvest, null);

        harvest.OnHarvestButtonPressed();

        Assert.AreEqual(0, backpack.current);
    }
}
