using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameTutorial : MonoBehaviour
{
    [Header("UI & Spawner")]
    public GameObject ui;
    public TextMeshProUGUI text;
    public GameObject enemySpawner;
    public Button skipButton; // Nút Skip trên màn hình

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float panSpeed = 15f;
    public float zoomSpeed = 8f;
    public float minZoom = 3f;
    public float maxZoom = 15f;

    [Header("Voice Clips")]
    public AudioSource voiceSource;
    public AudioClip moveVoice, dashVoice, attackVoice, qVoice, eVoice, pVoice, cameraVoice, doneVoice;

    private PlayerMana playerMana;
    private int step = 0;
    private bool w, a, s, d;
    private int lastCount = -1;
    private Coroutine typingCoroutine;
    private string currentMessage = "";
    private Vector3 lastMousePosition;

    void Awake()
    {
        playerMana = FindObjectOfType<PlayerMana>();
        if (mainCamera == null) mainCamera = Camera.main;

        // Tự động gán sự kiện nhấn chuột cho nút Skip
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
            // Mặc định ẩn nút đi khi vừa load Scene
            skipButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // --- KHI VÀO TUTORIAL MỚI HIỆN ---
        ui.SetActive(true);
        if (skipButton != null) skipButton.gameObject.SetActive(true);

        if (enemySpawner != null) enemySpawner.SetActive(false);

        ShowText("Chiến trận không chờ kẻ chậm chân… Di chuyển đi!");
        PlayVoice(moveVoice);
    }

    void Update()
    {
        // Liên tục hồi Mana khi đang hiện hướng dẫn
        if (ui.activeSelf && playerMana != null)
            playerMana.RestoreMana(playerMana.GetMaxMana());

        HandleCameraControls();

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

    // ================= CHỨC NĂNG SKIP =================
    public void SkipTutorial()
    {
        StopAllCoroutines();
        if (voiceSource != null) voiceSource.Stop();

        step = 7;
        if (enemySpawner != null) enemySpawner.SetActive(true);
        if (playerMana != null) playerMana.RestoreMana(playerMana.GetMaxMana());

        // Ẩn nút Skip ngay khi nhấn
        if (skipButton != null) skipButton.gameObject.SetActive(false);

        HideUI();
        Debug.Log("Đã bỏ qua hướng dẫn!");
    }

    // ================= ĐIỀU KHIỂN CAMERA =================
    void HandleCameraControls()
    {
        if (mainCamera == null) return;

        // 1. Kéo chuột phải để lia map
        if (Input.GetMouseButtonDown(1)) lastMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            mainCamera.transform.Translate(-delta.x * panSpeed * 0.005f, -delta.y * panSpeed * 0.005f, 0);
            lastMousePosition = Input.mousePosition;
        }

        // 2. Lăn chuột để Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f && mainCamera.orthographic)
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }

    // ================= CÁC BƯỚC HƯỚNG DẪN =================
    void MoveStep()
    {
        if (Input.GetKeyDown(KeyCode.W)) w = true; if (Input.GetKeyDown(KeyCode.A)) a = true;
        if (Input.GetKeyDown(KeyCode.S)) s = true; if (Input.GetKeyDown(KeyCode.D)) d = true;
        int count = (w ? 1 : 0) + (a ? 1 : 0) + (s ? 1 : 0) + (d ? 1 : 0);
        if (count != lastCount) { lastCount = count; ShowText($"Chiến trận không chờ kẻ chậm chân… Nhấn phím AWDS để làm chủ bước chân! ({count}/4)"); }
        if (w && a && s && d) { step = 1; ShowText("Nhấn SHIFT để lướt đi thần tốc, áp sát kẻ địch!"); PlayVoice(dashVoice); }
    }
    void DashStep() { if (Input.GetKeyDown(KeyCode.LeftShift)) { step = 2; ShowText("Đừng mất thời gian nữa! Nhấn chuột trái để Tấn công!"); PlayVoice(attackVoice); } }
    void AttackStep() { if (Input.GetMouseButtonDown(0)) { step = 3; ShowText("Giải phóng nội lực! Dùng tuyệt kỹ ấn phím Q!"); PlayVoice(qVoice); } }
    void QStep() { if (Input.GetKeyDown(KeyCode.Q)) { step = 4; ShowText("Sử dụng kỹ năng phi đao! Ấn phím E!"); PlayVoice(eVoice); } }
    void EStep() { if (Input.GetKeyDown(KeyCode.E)) { step = 5; ShowText("Đây là đòn quyết định! Giăng bẫy cọc Nhấn P để để tiêu diệt thuyền chiến quân Nam Hán!"); PlayVoice(pVoice); } }
    void PStep() { if (Input.GetKeyDown(KeyCode.P)) { step = 6; ShowText("Lăn chuột để nhìn toàn bản đồ, giữ chuột phải để theo dõi chiến trường!"); PlayVoice(cameraVoice); } }
    void CameraStep() { if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f || Input.GetMouseButton(1)) { step = 7; EndTutorial(); } }

    // ================= HỆ THỐNG PHỤ =================
    void ShowText(string message) { if (currentMessage == message) return; currentMessage = message; if (typingCoroutine != null) StopCoroutine(typingCoroutine); typingCoroutine = StartCoroutine(TypeText(message)); }
    IEnumerator TypeText(string message) { text.text = ""; foreach (char c in message) { text.text += c; yield return new WaitForSeconds(0.02f); } }
    void PlayVoice(AudioClip clip) { if (clip == null || voiceSource == null || !voiceSource.gameObject.activeInHierarchy) return; voiceSource.Stop(); voiceSource.PlayOneShot(clip); }

    void EndTutorial()
    {
        ShowText("Hãy cùng ta bảo vệ Thành Bạch Đằng, tiêu diệt toàn bộ quân địch!");
        PlayVoice(doneVoice);
        if (enemySpawner != null) enemySpawner.SetActive(true);
        if (skipButton != null) skipButton.gameObject.SetActive(false); // Hoàn thành cũng ẩn nút
        Invoke("HideUI", 3f);
    }
    void HideUI() => ui.SetActive(false);

    // Hàm này để các script khác (như bụi cây) gọi hiện chữ bất cứ lúc nào
    public void ShowManualText(string message)
    {
        if (ui != null)
        {
            ui.SetActive(true); // Bật khung UI lên
            ShowText(message);  // Chạy hiệu ứng chữ đánh máy
        }
    }
}