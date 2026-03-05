using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public int health;
    public int maxHealth;

    // Bạn có thể giữ biến này hoặc xóa nếu không dùng tới nữa
    public int bleed;
    AudioSource audioSource;
    float timer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        health = maxHealth;
        // timer = Time.time + 1; // Không cần khởi tạo timer nữa
    }

    void Update()
    {
        /* --- ĐÃ TẮT TÍNH NĂNG MẤT MÁU THEO THỜI GIAN ---
        if(Time.time > timer)
        {
            health -= bleed;
            timer = Time.time + 1;
        }
        ----------------------------------------------- */

        // Vẫn giữ kiểm tra để máu không bị âm
        if (health <= 0)
        {
            health = 0;
            // Ở đây bạn có thể thêm logic Game Over nếu cần
        }
    }

    public void Damage(int amount)
    {
        health -= amount;
    }

    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        health = maxHealth;
        Debug.Log($"💪 Trụ được tăng máu tối đa lên {maxHealth}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerBackpack>() != null)
        {
            int fruits = collision.GetComponent<PlayerBackpack>().current;
            if (fruits != 0)
            {
                audioSource.Play();
            }

            health += collision.GetComponent<PlayerBackpack>().TakeFruits();

            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }
}