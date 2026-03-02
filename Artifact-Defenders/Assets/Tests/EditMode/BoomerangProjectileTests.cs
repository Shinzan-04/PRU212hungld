using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for BoomerangProjectile.Launch() — verifies that launch
/// parameters are wired correctly and inspectable via public state.
/// Update/physics tick tested in PlayMode only.
/// </summary>
public class BoomerangProjectileTests
{
    private GameObject ownerGo;
    private GameObject projGo;
    private BoomerangSkill skill;
    private BoomerangProjectile proj;

    [SetUp]
    public void SetUp()
    {
        ownerGo = new GameObject("Owner");
        skill    = ownerGo.AddComponent<BoomerangSkill>();
        skill.speed       = 10f;
        skill.maxDistance = 5f;
        skill.damage      = 3;
        skill.knockback   = 2f;

        projGo = new GameObject("Projectile");
        projGo.AddComponent<Rigidbody2D>();
        proj = projGo.AddComponent<BoomerangProjectile>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(ownerGo);
        Object.DestroyImmediate(projGo);
    }

    // ---------------------------------------------------------------
    // Launch sets returning = false
    // ---------------------------------------------------------------

    [Test]
    public void Launch_SetsReturningToFalse()
    {
        // Reflect on 'returning' field
        var returningField = typeof(BoomerangProjectile).GetField("returning",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Force returning = true first
        returningField?.SetValue(proj, true);

        proj.Launch(skill, Vector2.zero, Vector2.right, 10f, 5f, 3, 2f, 0);

        bool returning = (bool)(returningField?.GetValue(proj) ?? false);
        Assert.IsFalse(returning);
    }

    [Test]
    public void Launch_ClearsLastHitDictionary()
    {
        var dictField = typeof(BoomerangProjectile).GetField("lastHitTime",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        proj.Launch(skill, Vector2.zero, Vector2.right, 10f, 5f, 3, 2f, 0);

        var dict = dictField?.GetValue(proj) as System.Collections.Generic.Dictionary<Transform, float>;
        Assert.IsNotNull(dict);
        Assert.AreEqual(0, dict.Count);
    }

    [Test]
    public void Launch_NormalisesDirection()
    {
        var dirField = typeof(BoomerangProjectile).GetField("dir",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        proj.Launch(skill, Vector2.zero, new Vector2(3, 4), 10f, 5f, 3, 2f, 0);

        Vector2 dir = (Vector2)(dirField?.GetValue(proj) ?? Vector2.zero);
        Assert.AreEqual(1f, dir.magnitude, 0.001f);
    }
}
