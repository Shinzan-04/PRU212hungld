using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public DashSkill dash;
    public SpinAttackSkill spin;
    public BoomerangSkill boomerang;
    public PlayerPileSkill pile;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) dash?.TryUse();
        if (Input.GetKeyDown(KeyCode.Q)) spin?.TryUse();
        if (Input.GetKeyDown(KeyCode.E)) boomerang?.TryUse();
        if (Input.GetKeyDown(KeyCode.L)) pile?.TryUse();
    }

    // 3 hàm này để UI Button gọi
    public void UseDash() => dash?.TryUse();
    public void UseSpin() => spin?.TryUse();
    public void UseBoomerang() => boomerang?.TryUse();
    public void UsePile() => pile?.TryUse();
}
