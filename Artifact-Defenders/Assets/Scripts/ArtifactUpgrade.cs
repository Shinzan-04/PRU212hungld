using UnityEngine;

/// <summary>
/// Quản lý việc nâng cấp Artifact (trụ) bao gồm level, máu tối đa và kích hoạt các họng súng bắn tên.
/// </summary>
public class ArtifactUpgrade : MonoBehaviour
{
    [Header("Animator & Artifact")]
    public Animator animator;
    private Artifact artifact;

    [Header("Point Launchers (Họng súng)")]
    // Kéo 4 Object Point (A, B, C, D) vào đây theo thứ tự
    [SerializeField] private GameObject[] shootPoints;

    [Header("Player Inventory")]
    public PlayerUpgradeInventory playerInv;

    [Header("Upgrade Settings")]
    [SerializeField] private int[] upgradeCosts = { 0, 20, 30 };
    [SerializeField] private int[] maxHealthByLevel = { 0, 100, 200, 300 };

    [Header("Upgrade UI")]
    public UpgradeUI upgradeUI;

    [Header("Upgrade Effect")]
    public GameObject upgradeEffectPrefab;

    private int level = 1;

    void Start()
    {
        level = Mathf.Clamp(level, 1, 3);
        artifact = GetComponent<Artifact>();

        // Cập nhật trạng thái ban đầu
        UpdateLevelVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryUpgrade();
        }
    }

    void TryUpgrade()
    {
        if (level >= 3)
        {
            upgradeUI?.ShowTowerMaxLevel();
            return;
        }

        int cost = upgradeCosts[Mathf.Min(level, upgradeCosts.Length - 1)];

        if (!playerInv.UseStones(cost))
        {
            upgradeUI?.ShowNotEnoughStones();
            return;
        }

        // Nâng cấp level
        level = Mathf.Min(level + 1, 3);

        // Cập nhật animation, máu, họng súng
        UpdateLevelVisual();

        if (upgradeEffectPrefab != null)
        {
            Vector3 effectPos = transform.position;
            GameObject effect = Instantiate(upgradeEffectPrefab, effectPos, Quaternion.identity);
            Destroy(effect, 3f);
        }

        upgradeUI?.ShowUpgradeSuccessful();
    }

    private void UpdateLevelVisual()
    {
        // 1. Cập nhật Animation
        if (animator != null)
            animator.SetInteger("level", level);

        // 2. Cập nhật Máu tối đa
        if (artifact != null)
            artifact.SetMaxHealth(maxHealthByLevel[level]);

        // 3. Cập nhật kích hoạt họng súng (Point Launchers)
        UpdatePointsByLevel();
    }

    private void UpdatePointsByLevel()
    {
        if (shootPoints == null || shootPoints.Length == 0) return;

        // Tắt tất cả các điểm trước
        for (int i = 0; i < shootPoints.Length; i++)
        {
            if (shootPoints[i] != null) shootPoints[i].SetActive(false);
        }

        // Kích hoạt theo Level
        if (level == 1)
        {
            // Level 1: Bật 1 điểm đầu tiên
            if (shootPoints.Length >= 1) shootPoints[0].SetActive(true);
        }
        else if (level == 2)
        {
            // Level 2: Bật 2 điểm đầu tiên (ví dụ Trái - Phải)
            if (shootPoints.Length >= 2)
            {
                shootPoints[0].SetActive(true);
                shootPoints[1].SetActive(true);
            }
        }
        else if (level >= 3)
        {
            // Level 3: Bật tất cả các điểm (tối đa 4 điểm)
            for (int i = 0; i < shootPoints.Length && i < 4; i++)
            {
                if (shootPoints[i] != null) shootPoints[i].SetActive(true);
            }
        }
    }

    public void OnUpgradeButtonPressed()
    {
        TryUpgrade();
    }
}