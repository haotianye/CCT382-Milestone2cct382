
using UnityEngine;
using UnityEngine.UI;

public class AttackCooldownUI : MonoBehaviour
{
    public MeleeAttack meleeAttack;
    public Slider cooldownSlider;

    void Update()
    {
        if (meleeAttack == null || cooldownSlider == null)
            return;

        cooldownSlider.value = meleeAttack.CooldownProgress;
    }
}
