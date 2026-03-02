using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Unit tests for PauseManager component.
/// Tests the public Pause / Resume state-machine, and Time.timeScale side-effects.
/// QuitToMenu (loads a scene) cannot run in EditMode; excluded.
/// </summary>
public class PauseManagerTests
{
    private GameObject go;
    private PauseManager pauseManager;
    private GameObject pauseUI;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("PauseManager");
        pauseManager = go.AddComponent<PauseManager>();

        pauseUI = new GameObject("PauseUI");
        pauseUI.SetActive(false);
        pauseManager.pauseUI = pauseUI;

        // Ensure timeScale is normal before each test
        Time.timeScale = 1f;
    }

    [TearDown]
    public void TearDown()
    {
        // Always restore timeScale so other tests aren't affected
        Time.timeScale = 1f;
        Object.DestroyImmediate(go);
        Object.DestroyImmediate(pauseUI);
    }

    // ---------------------------------------------------------------
    // Pause
    // ---------------------------------------------------------------

    [Test]
    public void Pause_ShowsPauseUI()
    {
        pauseManager.Pause();
        Assert.IsTrue(pauseUI.activeSelf);
    }

    [Test]
    public void Pause_SetsTimeScaleToZero()
    {
        pauseManager.Pause();
        Assert.AreEqual(0f, Time.timeScale);
    }

    // ---------------------------------------------------------------
    // Resume
    // ---------------------------------------------------------------

    [Test]
    public void Resume_HidesPauseUI()
    {
        pauseManager.Pause();
        pauseManager.Resume();
        Assert.IsFalse(pauseUI.activeSelf);
    }

    [Test]
    public void Resume_RestoresTimeScaleToOne()
    {
        pauseManager.Pause();
        pauseManager.Resume();
        Assert.AreEqual(1f, Time.timeScale);
    }

    // ---------------------------------------------------------------
    // Toggle sequence
    // ---------------------------------------------------------------

    [Test]
    public void PauseThenResume_CyclesTwice_UIEndsHidden()
    {
        pauseManager.Pause();
        pauseManager.Resume();
        pauseManager.Pause();
        pauseManager.Resume();
        Assert.IsFalse(pauseUI.activeSelf);
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void Resume_WhenNotPaused_HidesUIAndKeepsTimeScale()
    {
        // Calling Resume before Pause should not break anything
        pauseUI.SetActive(true); // pretend UI was visible
        pauseManager.Resume();
        Assert.IsFalse(pauseUI.activeSelf);
        Assert.AreEqual(1f, Time.timeScale);
    }
}
