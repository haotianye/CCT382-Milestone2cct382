
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewAttackData",
    menuName = "CCT382/Attack Data"
)]
public class AttackData : ScriptableObject
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackRadius = 1.5f;
    public float cooldown = 0.8f;

    [Header("Audio Settings")]
    public AudioClip attackSound;
    public float volume = 0.8f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
}
