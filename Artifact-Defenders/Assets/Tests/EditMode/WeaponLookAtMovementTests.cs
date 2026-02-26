using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for WeaponLookAtMovement.
/// Rotation and flipY logic require an Update cycle (PlayMode).
/// Here we test setup wiring and that the component exists cleanly.
/// </summary>
public class WeaponLookAtMovementTests
{
    private GameObject parentGo;
    private GameObject weaponGo;
    private WeaponLookAtMovement weaponLook;
    private PlayerMovement playerMovement;

    [SetUp]
    public void SetUp()
    {
        parentGo = new GameObject("Player");
        parentGo.AddComponent<Rigidbody2D>();
        parentGo.AddComponent<SpriteRenderer>();
        playerMovement = parentGo.AddComponent<PlayerMovement>();
        playerMovement.movementSpeed = 5f;

        weaponGo = new GameObject("WeaponPivot");
        weaponGo.transform.SetParent(parentGo.transform);
        weaponGo.AddComponent<SpriteRenderer>(); // required by Start()
        weaponLook = weaponGo.AddComponent<WeaponLookAtMovement>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(parentGo);
    }

    // ---------------------------------------------------------------
    // Component health checks
    // ---------------------------------------------------------------

    [Test]
    public void WeaponLookAtMovement_ExistsOnGameObject()
    {
        Assert.IsNotNull(weaponGo.GetComponent<WeaponLookAtMovement>());
    }

    [Test]
    public void PlayerMovement_ExistsOnParent()
    {
        Assert.IsNotNull(parentGo.GetComponent<PlayerMovement>());
    }

    [Test]
    public void InitialMoveDirection_IsZero()
    {
        // MoveDirection defaults to Vector2.zero before FixedUpdate
        Assert.AreEqual(Vector2.zero, playerMovement.MoveDirection);
    }

    [Test]
    public void InitialRotation_IsIdentity()
    {
        Assert.AreEqual(Quaternion.identity, weaponGo.transform.rotation);
    }
}
