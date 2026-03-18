using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public float followSmoothTime = 0.2f;

    [Header("Map Boundaries")]
    public float mapMinX = -51f;
    public float mapMaxX = 18f;
    public float mapMinY = -40.6f;
    public float mapMaxY = 5.8f;

    [Header("Mouse Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 15f;

    private Camera cam;
    private Vector3 dragOrigin;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 offset;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Ghi nhớ độ lệch bạn đã setup trong Editor
        if (player != null)
        {
            offset = transform.position - player.position;
        }

        // Fix Z = -10 để tránh lỗi hiển thị và kéo chuột
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    void LateUpdate()
    {
        HandleZoom();

        // LOGIC MỚI: Nếu đang giữ chuột phải thì cho phép kéo Cam
        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            Vector3 currentMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentMousePos;
            transform.position += difference;
        }
        // Nếu thả chuột phải ra thì lập tức bám theo Player
        else if (player != null)
        {
            Vector3 targetPosition = player.position + offset;
            targetPosition.z = -10f;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, followSmoothTime);
        }

        ClampCamera();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    void ClampCamera()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        // Chống khựng khi Zoom quá xa
        float finalX = (minX >= maxX) ? (mapMinX + mapMaxX) / 2f : Mathf.Clamp(transform.position.x, minX, maxX);
        float finalY = (minY >= maxY) ? (mapMinY + mapMaxY) / 2f : Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(finalX, finalY, -10f);
    }
}