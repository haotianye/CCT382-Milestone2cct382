
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Configuration")]
    [SerializeField] private AttackData attackData;
    [SerializeField] private LayerMask targetLayer;

    [Header("Game Event")]
    [SerializeField] private GameEventSO attackEvent;

    [Header("Attack Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string swingTrigger = "Swing";

    [Header("Runtime State")]
    [SerializeField] private float cooldownRemaining = 0f;

    // Keep compatibility with AttackCooldownUI.cs
    public float CooldownProgress
    {
        get
        {
            if (attackData == null || attackData.cooldown <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                cooldownRemaining / attackData.cooldown
            );
        }
    }

    private void Awake()
    {
        // Automatically find Erika Archer's Animator
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        // Update attack cooldown
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownRemaining < 0f)
            {
                cooldownRemaining = 0f;
            }
        }

        // Press F to attack
        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        // Do not attack during cooldown
        if (cooldownRemaining > 0f)
        {
            return;
        }

        if (attackData == null)
        {
            Debug.LogWarning(
                "MeleeAttack: Attack Data is missing!"
            );
            return;
        }

        // Start cooldown
        cooldownRemaining = attackData.cooldown;

        // Play swing animation
        if (animator != null)
        {
            animator.SetTrigger(swingTrigger);
        }
        else
        {
            Debug.LogWarning(
                "MeleeAttack: Animator not found!"
            );
        }

        // Notify listener to play sound and update UI
        if (attackEvent != null)
        {
            attackEvent.Raise();
        }

        // Calculate attack area in front of Player
        Vector3 attackCenter =
            transform.position +
            transform.forward * attackData.attackRange;

        // Detect targets
        Collider[] hits = Physics.OverlapSphere(
            attackCenter,
            attackData.attackRadius,
            targetLayer
        );

        foreach (Collider hit in hits)
        {
            // Find HitFlash on target
            HitFlash flash = hit.GetComponent<HitFlash>();

            if (flash == null)
            {
                flash = hit.GetComponentInParent<HitFlash>();
            }

            // Flash when hit
            if (flash != null)
            {
                flash.TriggerFlash();
            }

            Debug.Log("Melee hit: " + hit.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackData == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Vector3 attackCenter =
            transform.position +
            transform.forward * attackData.attackRange;

        Gizmos.DrawWireSphere(
            attackCenter,
            attackData.attackRadius
        );
    }
}
