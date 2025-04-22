using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatSO", menuName = "Scriptable Objects/PlayerStatSO")]
public class PlayerStatSO : ScriptableObject
{
    [Header("Movement")]
    public float MoveSpeed = 10.0f;
    public float JumpPower = 10.0f;

    [Header("Stamina")]
    [Range(0f, 1f)]
    public float Stamina = 0.1f;
    public float StaminaChangeRate = 1.0f;

    [Header("Rolling")]
    public float RollSpeed = 15.0f;
    public float RollDuration = 0.5f;
}
