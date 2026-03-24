using UnityEngine;
using System.Collections;

public class PlayerMana : MonoBehaviour
{
    [Header("Tuning")]
    public int maxMana = 100;
    public float regenRate = 5f;

    [Header("Direct UI")]
    public GameObject warningTextObject; // Kéo thả Object chữ báo thiếu mana vào đây

    [Header("Current Stats")]
    [SerializeField] int currentMana;

    void Awake()
    {
        currentMana = maxMana;
        // Ẩn chữ cảnh báo ngay khi bắt đầu game
        if (warningTextObject != null) warningTextObject.SetActive(false);
    }

    void Update()
    {
        // Tự động hồi mana theo thời gian
        if (currentMana < maxMana)
        {
            float newMana = currentMana + regenRate * Time.deltaTime;
            currentMana = Mathf.Min(maxMana, Mathf.RoundToInt(newMana));
        }
    }

    public bool TryUseMana(int amount)
    {
        // Nếu đủ mana thì trừ và cho phép dùng skill
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }

        // Nếu không đủ mana thì hiện chữ cảnh báo
        if (warningTextObject != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowWarningRoutine());
        }

        Debug.Log("Not enough Mana!");
        return false;
    }

    // Coroutine để tự động ẩn chữ sau 1.5 giây
    IEnumerator ShowWarningRoutine()
    {
        warningTextObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        warningTextObject.SetActive(false);
    }

    // Các hàm bổ trợ
    public void RestoreMana(int amount) => currentMana = Mathf.Min(maxMana, currentMana + amount);
    public int GetCurrentMana() => currentMana;
    public int GetMaxMana() => maxMana;
}