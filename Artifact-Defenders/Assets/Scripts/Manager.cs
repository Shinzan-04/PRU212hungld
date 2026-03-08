using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Settings")]
    public float timeToWin = 60f; // Thời gian đếm ngược (giây)
    public Artifact artifact;

    private SceneManager sceneManager;
    private float timer;
    private bool isGameOver = false; // Biến cờ để ngăn gọi chuyển cảnh nhiều lần

    void Awake()
    {
        timer = timeToWin;
        sceneManager = GetComponent<SceneManager>();
    }

    void Update()
    {
        if (isGameOver) return;

        // 1. Cập nhật thời gian trước
        timer -= Time.deltaTime;

        // 2. Kiểm tra điều kiện THUA trước (Ưu tiên cao nhất)
        // Nếu trụ hết máu, game dừng ngay lập tức và xử lý Lose
        if (artifact != null && artifact.health <= 0)
        {
            isGameOver = true;
            Lose();
        }
        // 3. CHỈ KHI trụ vẫn còn máu, mới kiểm tra xem hết giờ chưa để tính THẮNG
        else if (timer <= 0)
        {
            timer = 0;
            isGameOver = true;
            Win();
        }
    }

    void Lose()
    {
        Debug.Log("Game Over!");
        sceneManager.ChangeScene(4); // Đổi từ 3 thành 4 (Scene lose)
    }

    void Win()
    {
        Debug.Log("Victory!");
        sceneManager.ChangeScene(5); // Đổi từ 4 thành 5 (Scene win)
    }

    public float GetTime()
    {
        return timer;
    }
}