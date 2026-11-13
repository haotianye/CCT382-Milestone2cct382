
using UnityEngine;
using UnityEngine.UI;

public class AttackEventListener : MonoBehaviour
{
    [Header("Game Event")]
    public GameEventSO attackEvent;

    [Header("Cooldown Data")]
    public FloatVariableSO cooldownValue;
    public AttackData attackData;

    [Header("UI")]
    public Slider cooldownSlider;

    [Header("Audio")]
    public AudioSource audioSource;

    void OnEnable()
    {
        if (attackEvent != null)
        {
            attackEvent.OnEventRaised += OnAttack;
        }
    }

    void OnDisable()
    {
        if (attackEvent != null)
        {
            attackEvent.OnEventRaised -= OnAttack;
        }
    }

    void Start()
    {
        if (cooldownValue != null)
        {
            cooldownValue.SetValue(0f);
        }

        UpdateSlider();
    }

    void Update()
    {
        if (cooldownValue == null)
            return;

        if (cooldownValue.value > 0f)
        {
            cooldownValue.SetValue(
                Mathf.Max(
                    0f,
                    cooldownValue.value - Time.deltaTime
                )
            );

            UpdateSlider();
        }
    }

    void OnAttack()
    {
        if (attackData == null)
            return;

        // Reset cooldown when attack event fires
        if (cooldownValue != null)
        {
            cooldownValue.SetValue(attackData.cooldown);
        }

        UpdateSlider();

        // Play audio exactly when event fires
        if (audioSource != null &&
            attackData.attackSound != null)
        {
            audioSource.pitch = Random.Range(
                attackData.minPitch,
                attackData.maxPitch
            );

            audioSource.PlayOneShot(
                attackData.attackSound,
                attackData.volume
            );
        }

        Debug.Log("Attack Event Received!");
    }

    void UpdateSlider()
    {
        if (cooldownSlider == null ||
            cooldownValue == null ||
            attackData == null)
            return;

        if (attackData.cooldown <= 0f)
        {
            cooldownSlider.value = 0f;
            return;
        }

        cooldownSlider.value = Mathf.Clamp01(
            cooldownValue.value / attackData.cooldown
        );
    }
}
