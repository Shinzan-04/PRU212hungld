using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Transform target; // Thuyền cần theo dõi
    public GameObject indicatorPrefab;
    private GameObject indicator;
    private RectTransform rectTransform;
    private Image indicatorImage;

    void Start()
    {
        indicator = Instantiate(indicatorPrefab, GameObject.Find("Canvas").transform);
        rectTransform = indicator.GetComponent<RectTransform>();
        indicatorImage = indicator.GetComponent<Image>();
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(indicator);
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);

        // Kiểm tra xem mục tiêu có nằm ngoài màn hình không
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            indicatorImage.enabled = false; // Đang ở trong màn hình thì ẩn đi
        }
        else
        {
            indicatorImage.enabled = true;
            if (screenPos.z < 0) screenPos *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2f;
            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            float slope = Mathf.Tan(angle);

            // Tính toán vị trí ở mép màn hình
            if (screenPos.x > 0) screenPos = new Vector3(screenCenter.x, screenCenter.x * slope, 0);
            else screenPos = new Vector3(-screenCenter.x, -screenCenter.x * slope, 0);

            if (screenPos.y > screenCenter.y) screenPos = new Vector3(screenCenter.y / slope, screenCenter.y, 0);
            else if (screenPos.y < -screenCenter.y) screenPos = new Vector3(-screenCenter.y / slope, -screenCenter.y, 0);

            rectTransform.localPosition = screenPos;
            rectTransform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }
}