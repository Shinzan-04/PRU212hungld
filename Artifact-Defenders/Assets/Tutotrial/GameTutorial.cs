using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameTutorial : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject ui;
    public TextMeshProUGUI text;
    public Button skipButton;

    [Header("Spawners")]
    public GameObject[] enemySpawners;

    [Header("Voice Clips")]
    public AudioSource voiceSource;
    public AudioClip moveVoice, dashVoice, attackVoice, qVoice, eVoice, pVoice, cameraVoice, doneVoice;

    private PlayerMana playerMana;
    private int step = 0;
    private bool w, a, s, d;
    private int lastCount = -1;
    private Coroutine typingCoroutine;
    private string currentMessage = "";
    private bool isManualActive = false; // Cờ bảo vệ UI

    void Awake()
    {
        playerMana = FindObjectOfType<PlayerMana>();
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
            skipButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        ui.SetActive(true);
        if (skipButton != null) skipButton.gameObject.SetActive(true);
        SetAllSpawners(false);
        ShowText("Chiến trận không chờ kẻ chậm chân… Di chuyển đi!");
        PlayVoice(moveVoice);
    }

    void Update()
    {
        if (ui.activeSelf && playerMana != null)
            playerMana.RestoreMana(playerMana.GetMaxMana());

        // Chỉ xử lý phím Tutorial nếu không có thông báo Thành cổ đè lên
        if (!isManualActive)
        {
            switch (step)
            {
                case 0: MoveStep(); break;
                case 1: DashStep(); break;
                case 2: AttackStep(); break;
                case 3: QStep(); break;
                case 4: EStep(); break;
                case 5: PStep(); break;
                case 6: CameraStep(); break;
            }
        }
    }

    public void MuteMainVoice(bool mute)
    {
        if (voiceSource != null)
        {
            if (mute) voiceSource.Pause(); // Tạm dừng tiếng Tutorial chính
            else voiceSource.UnPause();    // Tiếp tục phát tiếng Tutorial chính
        }
    }

    // --- HÀM CHO THÀNH CỔ GỌI ---
    public void ShowManualText(string message)
    {
        isManualActive = true;
        if (ui != null) ui.SetActive(true);
        ShowText(message);
    }

    public void HideManualText()
    {
        isManualActive = false;
        // Nếu đã xong hết Tutorial thì tắt UI, nếu chưa thì hiện lại bước cũ
        if (step >= 7) ui.SetActive(false);
        else ShowText(GetStepMessage(step));
    }

    string GetStepMessage(int s)
    {
        switch (s)
        {
            case 0: return $"Chiến trận không chờ kẻ chậm chân… Nhấn AWDS để làm chủ bước chân! ({lastCount}/4)";
            case 1: return "Nhấn phím SHIFT để lướt tới áp sát kẻ địch!";
            case 2: return "Đừng mất thời gian nữa, nhấn chuột trái để tấn công!";
            case 3: return "Giải phóng nội lực! Dùng tuyệt kỹ ấn phím Q!";
            case 4: return "Sử dụng kỹ năng phi đao! Ấn Phím E!";
            case 5: return "Đây là đòn quyết định!Kết liễu bằng bẫy cọc Nhấn P!";
            case 6: return "Lăn chuột để Zoom, giữ chuột phải để lia map!";
            default: return "Hãy bảo vệ Thành Bạch Đằng!";
        }
    }
    public bool IsTutorialFinished()
    {
        return step >= 7;
    }

    // --- LOGIC PHỤ ---
    void PlayVoice(AudioClip clip)
    {
        if (clip != null && voiceSource != null) { voiceSource.Stop(); voiceSource.PlayOneShot(clip); }
    }

    void ShowText(string message)
    {
        if (currentMessage == message && ui.activeSelf) return;
        currentMessage = message;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        text.text = "";
        foreach (char c in message) { text.text += c; yield return new WaitForSeconds(0.02f); }
    }

    void MoveStep()
    {
        if (Input.GetKeyDown(KeyCode.W)) w = true; if (Input.GetKeyDown(KeyCode.A)) a = true;
        if (Input.GetKeyDown(KeyCode.S)) s = true; if (Input.GetKeyDown(KeyCode.D)) d = true;
        int count = (w ? 1 : 0) + (a ? 1 : 0) + (s ? 1 : 0) + (d ? 1 : 0);
        if (count != lastCount) { lastCount = count; ShowText($"Chiến trận không chờ kẻ chậm chân… Nhấn phím AWDS để làm chủ bước chân! ({count}/4)"); }
        if (w && a && s && d) { step = 1; ShowText("Nhấn phím SHIFT để lướt tới áp sát kẻ địch!"); PlayVoice(dashVoice); }
    }
    void DashStep() { if (Input.GetKeyDown(KeyCode.LeftShift)) { step = 2; ShowText("Đừng mất thời gian nữa, nhấn chuột trái để tấn công!"); PlayVoice(attackVoice); } }
    void AttackStep() { if (Input.GetMouseButtonDown(0)) { step = 3; ShowText("Giải phóng nội lực! Dùng tuyệt kỹ ấn phím Q!"); PlayVoice(qVoice); } }
    void QStep() { if (Input.GetKeyDown(KeyCode.Q)) { step = 4; ShowText("Sử dụng kỹ năng phi đao! Ấn Phím E!"); PlayVoice(eVoice); } }
    void EStep() { if (Input.GetKeyDown(KeyCode.E)) { step = 5; ShowText("Đây là đòn quyết định!Kết liễu bằng bẫy cọc Nhấn P!"); PlayVoice(pVoice); } }
    void PStep() { if (Input.GetKeyDown(KeyCode.P)) { step = 6; ShowText("Lăn chuột để Zoom, giữ chuột phải để lia map!"); PlayVoice(cameraVoice); } }
    void CameraStep() { if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f || Input.GetMouseButton(1)) { step = 7; EndTutorial(); } }

    void EndTutorial()
    {
        ShowText("Hãy bảo vệ Thành Bạch Đằng!");
        PlayVoice(doneVoice);
        SetAllSpawners(true);
        if (skipButton != null) skipButton.gameObject.SetActive(false);
        Invoke("FinalHide", 3f);
    }
    void FinalHide() { if (!isManualActive) ui.SetActive(false); }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        if (voiceSource != null) voiceSource.Stop();
        step = 7; SetAllSpawners(true);
        if (skipButton != null) skipButton.gameObject.SetActive(false);
        isManualActive = false; ui.SetActive(false);
    }

    void SetAllSpawners(bool state)
    {
        if (enemySpawners == null) return;
        foreach (GameObject spawner in enemySpawners) if (spawner != null) spawner.SetActive(state);
    }
}