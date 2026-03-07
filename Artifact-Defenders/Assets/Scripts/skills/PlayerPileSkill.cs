using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerPileSkill : MonoBehaviour
{
    [Header("Setup")]
    public GameObject pilePrefab;
    public float minDistance = 1.2f;
    public float cooldown = 2f;
    public Text cooldownText;

    private Tilemap waterTilemap;
    private GameObject preview;
    private Camera cam;

    private float lastPlaceTime;
    private bool isPlacing;
    private int pileLayer;

    void Awake()
    {
        cam = Camera.main;

        pileLayer = LayerMask.NameToLayer("Pile");

        GameObject water = GameObject.FindWithTag("Water");

        if (water != null)
            waterTilemap = water.GetComponent<Tilemap>();

        if (waterTilemap == null)
            Debug.LogError("Tilemap nước cần Tag = Water");

        if (pilePrefab == null)
            Debug.LogError("Chưa gán pilePrefab!");
    }

    void Update()
    {
        if (isPlacing)
        {
            UpdatePreview();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlace();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacing();
            }
        }

        if (cooldownText != null)
        {
            float remain = Mathf.Max(0, lastPlaceTime + cooldown - Time.time);
            cooldownText.text = remain > 0 ? remain.ToString("F1") + "s" : "Ready";
        }
    }

    public void TryUse()
    {
        if (isPlacing)
        {
            CancelPlacing();
            return;
        }

        if (Time.time < lastPlaceTime + cooldown)
        {
            Debug.Log("Skill đang cooldown!");
            return;
        }

        StartPlacing();
    }

    void StartPlacing()
    {
        preview = Instantiate(pilePrefab);
        preview.GetComponent<Pile>().SetPreview(false);

        isPlacing = true;

        Debug.Log("Bắt đầu đặt cọc");
    }

    void UpdatePreview()
    {
        Vector3 pointerPos = Input.mousePosition;

        Vector3 worldPos = cam.ScreenToWorldPoint(pointerPos);
        worldPos.z = 0;

        Vector3Int cell = waterTilemap.WorldToCell(worldPos);
        worldPos = waterTilemap.GetCellCenterWorld(cell);

        preview.transform.position = worldPos;

        bool valid = IsValidPosition(worldPos);

        preview.GetComponent<Pile>().SetPreview(valid);
    }

    bool IsValidPosition(Vector3 pos)
    {
        if (waterTilemap == null)
            return false;

        Vector3Int cell = waterTilemap.WorldToCell(pos);

        // phải ở trên nước
        bool onWater = waterTilemap.HasTile(cell);

        // kiểm tra có cọc gần đó không
        Collider2D hit = Physics2D.OverlapCircle(pos, minDistance);

        bool noOverlap = true;

        if (hit != null)
        {
            if (hit.GetComponent<Pile>() != null)
            {
                noOverlap = false;
            }
        }

        return onWater && noOverlap;
    }

    void TryPlace()
    {
        if (preview == null)
            return;

        Vector3 pos = preview.transform.position;

        if (IsValidPosition(pos))
        {
            GameObject placed = Instantiate(pilePrefab);
            placed.transform.position = pos;

            placed.GetComponent<Pile>().Place();

            lastPlaceTime = Time.time;

            Debug.Log("Cọc đã đặt!");
        }
        else
        {
            Debug.Log("Vị trí không hợp lệ!");
        }

        CancelPlacing();
    }

    void CancelPlacing()
    {
        if (preview != null)
            Destroy(preview);

        preview = null;
        isPlacing = false;
    }
}