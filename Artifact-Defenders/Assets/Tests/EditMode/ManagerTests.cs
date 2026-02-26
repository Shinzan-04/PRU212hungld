using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for Manager.
/// GetTime() is the only pure-logic public method testable without scene loading.
/// Update / Lose / Win require SceneManager and a running scene — PlayMode only.
/// </summary>
public class ManagerTests
{
    private GameObject go;
    private Manager manager;
    private Artifact artifact;

    // Manager needs a SceneManager sibling
    private Misc.SceneManager sceneManagerComponent;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("GameManager");
        // Manager.Awake reads SceneManager from same GameObject
        go.AddComponent<SceneManager>();

        artifact = new GameObject("Artifact").AddComponent<Artifact>();
        artifact.gameObject.AddComponent<AudioSource>();
        artifact.maxHealth = 100;
        var awake = typeof(Artifact).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake?.Invoke(artifact, null);

        manager = go.AddComponent<Manager>();
        manager.timeToWin = 60f;
        manager.artifact  = artifact;

        var mgrAwake = typeof(Manager).GetMethod("Awake",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        mgrAwake?.Invoke(manager, null);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(artifact.gameObject);
    }

    // ---------------------------------------------------------------
    // GetTime initial state
    // ---------------------------------------------------------------

    [Test]
    public void GetTime_AfterAwake_EqualsTimeToWin()
    {
        Assert.AreEqual(60f, manager.GetTime(), 0.001f);
    }

    [Test]
    public void GetTime_IsPositive_WhenTimeToWinIsPositive()
    {
        Assert.Greater(manager.GetTime(), 0f);
    }

    [Test]
    public void ArtifactReference_IsAssigned()
    {
        Assert.IsNotNull(manager.artifact);
    }
}
