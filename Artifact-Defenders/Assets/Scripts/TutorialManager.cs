using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] steps; // Kéo các Panel bước 0, 1, 2... vào đây
    private int index = 0;

    void Start()
    {
        UpdateUI();
    }

    // Hàm để chuyển sang bước tiếp theo
    public void NextStep()
    {
        steps[index].SetActive(false); // Tắt bước hiện tại
        index++;
        if (index < steps.Length)
        {
            steps[index].SetActive(true); // Bật bước tiếp theo
        }
        else
        {
            gameObject.SetActive(false); // Xong hết thì ẩn cả bộ hướng dẫn
        }
    }

    // Hàm để các chiêu thức gọi vào khi người chơi bấm phím đúng
    public void CheckAction(string actionName)
    {
        if (index == 1 && actionName == "P") NextStep();
        if (index == 2 && actionName == "Q") NextStep();
        if (index == 3 && actionName == "E") NextStep();
        if (index == 4 && actionName == "Space") NextStep();
    }

    void UpdateUI()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].SetActive(i == index);
        }
    }
}