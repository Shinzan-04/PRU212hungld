using UnityEngine;
using TMPro;
using System.Collections;

public class GameTutorial : MonoBehaviour
{
    [Header("UI")]
    public GameObject ui;
    public TextMeshProUGUI text;

    [Header("Spawner")]
    public GameObject enemySpawner;

    [Header("Voice")]
    public AudioSource voiceSource;
    public AudioClip moveVoice;
    public AudioClip attackVoice;
    public AudioClip qVoice;
    public AudioClip eVoice;
    public AudioClip pVoice;
    public AudioClip doneVoice;

    private int step = 0;

    private bool w, a, s, d;
    private int lastCount = -1;

    private Coroutine typingCoroutine;
    private string currentMessage = "";

    void Start()
    {
        ui.SetActive(true);

        if (enemySpawner != null)
            enemySpawner.SetActive(false);

        ShowText("Chiến trận không chờ kẻ chậm chân… Di chuyển đi!");
        PlayVoice(moveVoice);
    }

    void Update()
    {
        switch (step)
        {
            case 0:
                MoveStep();
                break;
            case 1:
                AttackStep();
                break;
            case 2:
                QStep();
                break;
            case 3:
                EStep();
                break;
            case 4:
                PStep();
                break;
        }
    }

    // ================= TEXT =================
    void ShowText(string message)
    {
        if (currentMessage == message) return;

        currentMessage = message;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        text.text = "";

        foreach (char c in message)
        {
            text.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    // ================= VOICE =================
    void PlayVoice(AudioClip clip)
    {
        if (clip == null || voiceSource == null) return;

        voiceSource.Stop(); // tránh chồng âm
        voiceSource.PlayOneShot(clip);
    }

    // ================= MOVE =================
    void MoveStep()
    {
        if (Input.GetKeyDown(KeyCode.W)) w = true;
        if (Input.GetKeyDown(KeyCode.A)) a = true;
        if (Input.GetKeyDown(KeyCode.S)) s = true;
        if (Input.GetKeyDown(KeyCode.D)) d = true;

        int count = (w ? 1 : 0) + (a ? 1 : 0) + (s ? 1 : 0) + (d ? 1 : 0);

        if (count != lastCount)
        {
            lastCount = count;
            ShowText($"Chiến trận không chờ kẻ chậm chân… Dùng WASD để làm chủ bước chân! ({count}/4)");
        }

        if (w && a && s && d)
        {
            step = 1;
            ShowText("Đừng do dự! Click chuột trái Tấn công ngay!");
            PlayVoice(attackVoice);
        }
    }

    // ================= ATTACK =================
    void AttackStep()
    {
        if (Input.GetMouseButtonDown(0))
        {
            step = 2;
            ShowText("Giải phóng nội lực! Dùng tuyệt kỹ ấn phím Q!");
            PlayVoice(qVoice);
        }
    }

    // ================= Q =================
    void QStep()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            step = 3;
            ShowText("Không cho chúng cơ hội! Dùng E!");
            PlayVoice(eVoice);
        }
    }

    // ================= E =================
    void EStep()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            step = 4;
            ShowText("Đây là đòn quyết định!Giăng bẫy cọc Nhấn P để để tiêu diệt thuyền chiến quân Nam Hán!");
            PlayVoice(pVoice);
        }
    }

    // ================= P =================
    void PStep()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            step = 5;
            EndTutorial();
        }
    }

    // ================= DONE =================
    void EndTutorial()
    {
        ShowText("Hãy cùng ta bảo vệ Thành Bạch Đằng, tiêu diệt toàn bộ quân địch!");
        PlayVoice(doneVoice);

        if (enemySpawner != null)
            enemySpawner.SetActive(true);

        Invoke("HideUI", 2f);
    }

    void HideUI()
    {
        ui.SetActive(false);
    }

    // ================= CHECK =================
    public bool CanMove() => step >= 0;
    public bool CanAttack() => step >= 1;
    public bool CanUseQ() => step >= 2;
    public bool CanUseE() => step >= 3;
    public bool CanUseP() => step >= 4;
}