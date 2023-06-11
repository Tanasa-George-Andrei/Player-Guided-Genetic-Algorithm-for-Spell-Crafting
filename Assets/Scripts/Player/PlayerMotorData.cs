using UnityEngine;

[CreateAssetMenu]
public class PlayerMotorData : ScriptableObject
{
    public float normalHeight = 2f;
    public float hoverHeight = 0.15f;
    public float walkingSpeed = 10f;
    public float sprintingSpeed = 20f;
    public float jumpHeight = 2f;
    public int maxJumps = 1;
    public float groundDrag = 0.9f;
    public float airDrag = 0.25f;
    public float minForce = 0.5f;
    public float normalGravity = 10f;
    public float dropGravity = 15f;
    public float maxSlopeAngle = 45f;
    public LayerMask environmentLayerMask = (1 << 8);
    public float MaxAngleForBonusSpeed = 30;
    public float maxBonusSpeed = 5;
    public float bonsusDragConstant = 0.1f;
    public float speedUpConstant = 0.02f;
    public float minMoveDistance = 0.1f;
}
