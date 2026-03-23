using UnityEngine;
using System.Collections; // Cần dòng này để dùng Coroutine

public class PlayerMana : MonoBehaviour
{
    [Header("Tuning")]
    public int maxMana = 100;
    public float regenRate = 5f;

    [Header("Direct UI")]
    public GameObject warningTextObject; // Kéo thả Object chữ vào đây

    [Header("Current Stats")]
    [SerializeField] int currentMana;

    void Awake()
    {
        currentMana = maxMana;
        // Ẩn chữ ngay khi bắt đầu game
        if (warningTextObject != null) warningTextObject.SetActive(false);
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            float newMana = currentMana + regenRate * Time.deltaTime;
            currentMana = Mathf.Min(maxMana, Mathf.RoundToInt(newMana));
        }
    }

    public bool TryUseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }

        // HIỆN TRỰC TIẾP Ở ĐÂY
        if (warningTextObject != null)
        {
            StopAllCoroutines(); // Dừng các lần ẩn trước đó nếu bạn bấm liên tục
            StartCoroutine(ShowWarningRoutine());
        }

        Debug.Log("Not enough Mana!");
        return false;
    }

    // Hàm phụ để tự động ẩn chữ sau 1.5 giây
    IEnumerator ShowWarningRoutine()
    {
        warningTextObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningTextObject.SetActive(false);
    }

    public void RestoreMana(int amount) => currentMana = Mathf.Min(maxMana, currentMana + amount);
    public int GetCurrentMana() => currentMana;
    public int GetMaxMana() => maxMana;
}