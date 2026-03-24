using System.Collections;
using UnityEngine;

public class DashSkill : MonoBehaviour
{
    [Header("Tuning")]
    public float dashSpeed = 18f;
    public float dashDur = 0.15f;
    public float cooldown = 1.0f;

    [Header("I-Frame (tùy chọn)")]
    public bool grantIFrame = true;
    public string invincibleLayerName = "PlayerIFrame"; // Đổi tên cho rõ nghĩa

    [Header("Mana Cost")]
    public int manaCost = 10;

    private Rigidbody2D rb;
    private MonoBehaviour moveScript;
    private float lastUseTime = -999f;
    private bool isDashing;
    private Vector2 uiDir = Vector2.zero;
    private PlayerMana playerMana;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMana = GetComponent<PlayerMana>();

        // Tìm script di chuyển (Ưu tiên PlayerMovement nếu có)
        moveScript = GetComponent("PlayerMovement") as MonoBehaviour;
        if (moveScript == null) moveScript = GetComponent<MonoBehaviour>();
    }

    public void TryUse()
    {
        uiDir = Vector2.zero;
        StartDashProcess();
    }

    public void TryUse(Vector2 directionFromUI)
    {
        uiDir = directionFromUI.normalized;
        StartDashProcess();
    }

    void StartDashProcess()
    {
        if (isDashing || Time.time < lastUseTime + cooldown) return;

        if (playerMana != null)
        {
            if (!playerMana.TryUseMana(manaCost)) return;
        }

        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        lastUseTime = Time.time;

        Vector2 dir = GetDashDir();
        int originalLayer = gameObject.layer;

        // 🛑 Tắt script di chuyển để không bị xung đột lực
        if (moveScript != null) moveScript.enabled = false;

        // 🛡️ Xử lý I-Frame (Bất tử)
        if (grantIFrame)
        {
            int targetLayer = LayerMask.NameToLayer(invincibleLayerName);
            // KIỂM TRA: Nếu layer tồn tại (>=0) thì mới gán, tránh lỗi [0...31]
            if (targetLayer >= 0)
            {
                gameObject.layer = targetLayer;
            }
            else
            {
                Debug.LogWarning($"Layer '{invincibleLayerName}' chưa được tạo trong Unity! Hãy vào Tags & Layers để thêm nó.");
            }
        }

        float timer = 0f;
        float origDrag = rb.linearDamping;
        rb.linearDamping = 0f;

        while (timer < dashDur)
        {
            rb.linearVelocity = dir * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        // 🏁 Kết thúc Dash: Trả lại trạng thái cũ
        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = origDrag;
        gameObject.layer = originalLayer; // Trả về layer cũ

        if (moveScript != null) moveScript.enabled = true;
        isDashing = false;
    }

    Vector2 GetDashDir()
    {
        if (uiDir != Vector2.zero) return uiDir;
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f) return rb.linearVelocity.normalized;

        // Dash theo hướng nhân vật đang nhìn (scale X dương là phải, âm là trái)
        return new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);
    }
}