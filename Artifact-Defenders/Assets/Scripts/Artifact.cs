using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    [Header("Stats")]
    public int health;
    public int maxHealth;

    [Header("Cấu hình hướng dẫn")]
    public AudioClip tutorialClip;
    [TextArea(3, 10)]
    public string message = "Đây là thành cổ của bạn...";

    private static bool isArtifactActivated = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameTutorial tutorial = FindObjectOfType<GameTutorial>();

            // --- ĐIỀU KIỆN 0: KIỂM TRA TUTORIAL HỆ THỐNG XONG CHƯA ---
            // Nếu tutorial chính chưa xong step 7 thì không làm gì cả
            if (tutorial != null && !tutorial.IsTutorialFinished())
            {
                Debug.Log("Hãy hoàn thành hướng dẫn cơ bản trước!");
                return;
            }

            // --- PHẦN 1: KÍCH HOẠT HƯỚNG DẪN (CHỈ CHẠY 1 LẦN) ---
            if (!isArtifactActivated)
            {
                if (tutorial != null)
                {
                    tutorial.MuteMainVoice(true);
                    tutorial.ShowManualText(message);

                    if (audioSource != null && tutorialClip != null)
                    {
                        audioSource.clip = tutorialClip;
                        audioSource.Play();
                    }
                    isArtifactActivated = true;
                }
                // Dùng return ở đây để thoát hàm ngay, KHÔNG cho nộp quả ở lần chạm đầu tiên này
                return;
            }

            // --- PHẦN 2: LOGIC NỘP TRÁI CÂY ---
            // Chỉ chạy từ lần chạm thứ 2 trở đi hoặc sau khi đã kích hoạt xong
            if (isArtifactActivated)
            {
                // Nếu bạn muốn GẮT hơn: Bắt phải nghe xong âm thanh mới cho nộp quả
                if (audioSource != null && audioSource.isPlaying)
                {
                    Debug.Log("Đang nghe hướng dẫn, chưa nộp quả được!");
                    return;
                }

                PlayerBackpack backpack = collision.GetComponent<PlayerBackpack>();
                if (backpack != null && backpack.current > 0)
                {
                    // Phát tiếng nộp quả (Ting ting)
                    if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);

                    health += backpack.TakeFruits();
                    if (health > maxHealth) health = maxHealth;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameTutorial tutorial = FindObjectOfType<GameTutorial>();
            if (tutorial != null)
            {
                tutorial.HideManualText();
                tutorial.MuteMainVoice(false);
            }
            // Không Stop âm thanh để nó nói cho hết bài như bạn muốn
        }
    }

    public void TakeDamage(int amount) { health -= amount; if (health < 0) health = 0; }
    public void Damage(int amount) { health -= amount; }
    public void SetMaxHealth(int newMax) { maxHealth = newMax; health = maxHealth; }
}