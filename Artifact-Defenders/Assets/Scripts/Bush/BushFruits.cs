using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushFruits : MonoBehaviour
{
    [SerializeField] int[] amountPerType;
    [SerializeField] float[] respawnTime;

    // Biến này giúp tất cả bụi cây biết là đã hiện hướng dẫn 1 lần chưa
    private static bool hasShownBushTutorial = false;

    BushVisual bushVisual;
    bool ready;
    float timer;

    void Start()
    {
        bushVisual = GetComponent<BushVisual>();
        if (Random.Range(0, 2) == 0)
        {
            ready = false;
            timer = Time.time + respawnTime[(int)bushVisual.GetVariant()];
        }
        else
        {
            ready = true;
            bushVisual.ShowFruits();
        }
    }

    void Update()
    {
        if (!ready && Time.time > timer)
        {
            ready = true;
            bushVisual.ShowFruits();
        }
    }

    // 1. KHI NGƯỜI CHƠI BƯỚC VÀO VÙNG BỤI CÂY
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ready)
        {
            GameTutorial tutorial = FindObjectOfType<GameTutorial>();
            if (tutorial != null)
            {
                if (!hasShownBushTutorial)
                {
                    tutorial.ShowManualText("Ấn SPACE để thu thập. Bạn có thể thu hoạch những trái cây từ bụi cây để hồi máu cho thành cổ.");
                    hasShownBushTutorial = true;
                }
                else
                {
                    tutorial.ShowManualText("Ấn SPACE để thu thập");
                }
            }
        }
    }

    // 2. KHI NGƯỜI CHƠI ĐI RA XA
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Đã chạm vào bụi cây!");
        if (collision.CompareTag("Player"))
        {
            GameTutorial tutorial = FindObjectOfType<GameTutorial>();
            if (tutorial != null && tutorial.ui != null)
            {
                tutorial.ui.SetActive(false); // Tắt bảng hướng dẫn đi
            }
        }
    }

    public bool HasFruits() => ready;

    public int HarvestFruit()
    {
        if (ready)
        {
            ready = false;
            bushVisual.HideFruits();
            timer = Time.time + respawnTime[(int)bushVisual.GetVariant()];

            // Hái xong thì ẩn chữ luôn cho đỡ vướng
            GameTutorial tutorial = FindObjectOfType<GameTutorial>();
            if (tutorial != null && tutorial.ui != null) tutorial.ui.SetActive(false);

            return amountPerType[(int)bushVisual.GetVariant()];
        }
        return 0;
    }

    public void EatBush()
    {
        enabled = false;
        bushVisual.SetToDry();
    }
  
}