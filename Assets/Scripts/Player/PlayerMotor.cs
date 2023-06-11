using System;
using UnityEngine;
using System.Collections;

public enum PlayerMovementType
{
    Ground,
    Air,
    Slope,
    Dash
}

//Change this to a scriptable object enum List
public enum PlayerMovementMod
{
    Walking,
    Sprinting
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private PlayerMotorData data;

    public PlayerMovementType movementType = PlayerMovementType.Ground;
    public PlayerMovementMod movementMod = PlayerMovementMod.Walking;

    private Rigidbody rb;

    private CapsuleCollider capCol;
    private BoxCollider boxCol;
    private float height;

    private Quaternion finalRotation;
    private Quaternion baseRotation;
    private float YRotation = 0;
    private float yRotationDelta;
    private float sensitivity;

    private Vector3 inputDir;
    private Vector3 XZVelocity;
    private Vector3 LastXZDir;
    private Vector3 LastPosition;
    private Vector3 dashVelocity;
    private Vector3 propelVelocity;
    private Vector3 YVelocity;
    private Vector3 finalVelocity;
    private Vector3 gravityDirection = new Vector3(0, -1, 0);
    private float expectedSpeed = 1f;
    private float bonusSpeed = 0;
    private int jumpsRemaining = 1;
    private bool isDashing;
    private Coroutine dashCoroutine;

    private Vector3 groundNormal;
    private float groundAngle;
    //Calculated from center
    private float distanceToGround;
    private bool isGrounded;
    private float groundRayLength;

    private void GetGroundData()
    {
        RaycastHit hit;
        //add more stringent conditions for being grounded
        isGrounded = Physics.Raycast(transform.position, gravityDirection, out hit, groundRayLength, data.environmentLayerMask);
        distanceToGround = hit.distance;
        Debug.DrawRay(transform.position, gravityDirection * groundRayLength, Color.green);
        if (isGrounded)
        {
            groundNormal = hit.normal;
            groundAngle = Vector3.Angle(-gravityDirection, groundNormal);
        }
        else
        {
            groundNormal = -gravityDirection;
            groundAngle = -1;
        }
    }

    public CapsuleCollider GetCollider()
    {
        return capCol;
    }
    public void SetMovementInput(Vector3 _inputDir, bool _sprinting)
    {
        inputDir = _inputDir;
        if (_sprinting)
        {
            movementMod = PlayerMovementMod.Sprinting;
            expectedSpeed = data.sprintingSpeed;
        }
        else
        {
            movementMod = PlayerMovementMod.Walking;
            expectedSpeed = data.walkingSpeed;
        }

    }

    private void HandleBonusSpeed()
    {

        if (isGrounded && movementMod == PlayerMovementMod.Sprinting && bonusSpeed <= data.maxBonusSpeed && GetMoveDir() != Vector3.zero && Vector3.Angle(LastXZDir, GetMoveDir()) <= data.MaxAngleForBonusSpeed && Vector3.Distance(transform.position, LastPosition) > data.minMoveDistance)
        {
            bonusSpeed = Mathf.Clamp(bonusSpeed + data.speedUpConstant * GetSpeed() * Time.fixedDeltaTime, 0, data.maxBonusSpeed);
        }
        else if (!isGrounded && bonusSpeed > 0 && MathF.Abs(yRotationDelta) < data.MaxAngleForBonusSpeed)
        {
            bonusSpeed = Mathf.Clamp(bonusSpeed - data.bonsusDragConstant * GetSpeed() * Time.fixedDeltaTime, 0, data.maxBonusSpeed);
        }
        else
        {
            bonusSpeed = 0;
        }
    }

    private float GetSpeed()
    {
        return expectedSpeed + bonusSpeed;
    }

    public void SetRotationInput(float _inputRotationDelta, float _sensitivity)
    {
        yRotationDelta = _inputRotationDelta;
        sensitivity = _sensitivity;
    }

    public void ResetRotation()
    {
        YRotation = 0;
    }

    public void ResetRotation(float _rot)
    {
        YRotation = _rot;
    }

    private void Rotate()
    {
        YRotation = (YRotation + (yRotationDelta * sensitivity * Time.deltaTime)) % 360;
        finalRotation = baseRotation * Quaternion.Euler(0, YRotation, 0);
        rb.MoveRotation(finalRotation);
        //rb.rotation = finalRotation;
    }

    private Vector3 hoverVelocity = Vector3.zero;
    private void MaintainMinHoverDistance()
    {
        //When jumping over the 2m cube there is a bumb if movemetn continues
        //TODO: Test other collider types
        if (YVelocity == Vector3.zero)
        {
            rb.MovePosition(Vector3.SmoothDamp(transform.position, transform.position + gravityDirection * (distanceToGround - height / 2), ref hoverVelocity, Time.fixedDeltaTime * 2, Mathf.Infinity, Time.fixedDeltaTime));
        }
    }

    public void Dash(Vector3 _dir, float _distance, float _duration)
    {
        if (isDashing && dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
        dashCoroutine = StartCoroutine(ApplyDashVelocity(_dir, _distance, _duration));
    }

    private IEnumerator ApplyDashVelocity(Vector3 _dir, float _distance, float _duration)
    {
        isDashing = true;
        propelVelocity = Vector3.zero;
        YVelocity = Vector3.zero;
        dashVelocity = (_distance / _duration) * _dir;
        yield return Helpers.GetWait(_duration);
        dashVelocity = Vector3.zero;
        //It causes jitters/snap back
        //Note posible visual effect
        //movementType = GetMovementType();
        //Move();
        isDashing = false;
    }

    public Vector3 GetMoveDir()
    {
        return transform.rotation * Vector3.Normalize(inputDir);
    }

    //private void ApplyMagicDrag()
    //{
    //    //NOTE Formalise this method for better results and eliminate the *10 in the apply magical velocity
    //    //Find better drag. Quadratic works preety well for the first few seconds then it's fells slippery
    //    dashVelocity -= magicDrag * dashVelocity;
    //    magicDrag *= MathF.Log(1/magicDrag);
    //    //NOTE: Very Unstable
    //    //magicVelocity -= magicDrag * magicVelocity.normalized * magicVelocity.sqrMagnitude;

    //    if (dashVelocity.sqrMagnitude < 1)
    //    {
    //        dashVelocity = Vector3.zero;
    //        magicDrag = 0.15f;
    //    }
    //}

    public void Propel(Vector3 _dir, float _speed)
    {
        propelVelocity += _speed * _dir;
        if(isGrounded && propelVelocity.y < 0)
        {
            propelVelocity.y = 0;
        }
    }

    public void Teleport(Vector3 _dir, float _distance)
    {
        Teleport(transform.position + _dir * _distance);
    }

    public void Teleport(Vector3 _newPos)
    {
        Vector3 newPos = _newPos;
        for (int i = 0; i < 10; i++)
        {
            if (!Physics.CheckBox(newPos, boxCol.size / 2.1f, transform.rotation))
            {
                transform.position = newPos;
                break;
            }
            else
            {
                newPos.y += boxCol.size.y;
            }
        }
    }

    private void ApplyFinalForce()
    {
        finalVelocity = XZVelocity + YVelocity + propelVelocity;
        rb.AddForce(finalVelocity - rb.velocity, ForceMode.VelocityChange);
    }

    private void GroundMove()
    {
        XZVelocity = GetSpeed() * GetMoveDir();
        //XZVelocity.y = rb.velocity.y;
        //rb.velocity = XZVelocity;
        ApplyFinalForce();
        MaintainMinHoverDistance();
    }
    private void DashMove()
    {
        rb.AddForce(dashVelocity - rb.velocity, ForceMode.VelocityChange);
        if(isGrounded)
        { 
            MaintainMinHoverDistance();
        }
    }

    private void ApplyDrag(float _coeficient)
    {
        propelVelocity *= (1 - _coeficient * Time.fixedDeltaTime);
        if(propelVelocity.magnitude < data.minForce)
        {
            propelVelocity = Vector3.zero;
        }
    }

    private void HandleDrag()
    {
        if(isGrounded)
        {
            ApplyDrag(data.groundDrag);
        }
        else
        {
            ApplyDrag(data.airDrag);
        }
    }

    private void ApplyGravity(float _force)
    {
        //Change this so it works in later versions with custom gravity
        if(propelVelocity.y > 0) 
        {
            propelVelocity += _force * Time.fixedDeltaTime* gravityDirection;
        }
        else
        {
            YVelocity += _force * Time.fixedDeltaTime * gravityDirection;
        }
    }

    private void HandleGravity()
    {
        if (YVelocity.normalized == gravityDirection)
        {
            ApplyGravity(data.dropGravity);
        }
        else
        {
            ApplyGravity(data.normalGravity);
        }
    }

    private void AirMove()
    {
        XZVelocity = GetSpeed() * GetMoveDir();
        HandleGravity();
        ApplyFinalForce();
    }

    private void SlopeMove()
    {
        XZVelocity = GetSpeed() * Vector3.ProjectOnPlane(GetMoveDir(), groundNormal).normalized;
        ApplyFinalForce();
        MaintainMinHoverDistance();
    }

    public void ResetVelocity()
    {
        XZVelocity = Vector3.zero;
        YVelocity = Vector3.zero;
        dashVelocity = Vector3.zero;
        isDashing = false;
        propelVelocity = Vector3.zero;
        LastXZDir = Vector3.zero;
        finalVelocity = Vector3.zero;
        LastPosition = Vector3.zero;
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
    }

    private void ResetJumps()
    {
        //NOTE: IT would be a fun thing to change it to reset only when jumpsRemaingin is smaller than max
        if(jumpsRemaining != data.maxJumps)
        {
            jumpsRemaining = data.maxJumps;
        }
    }

    private bool CanJump()
    {
        //Modify this for cayote time
        return jumpsRemaining > 0 && isGrounded;
    }

    public void Jump()
    {
        //Change is grounded to 
        if(CanJump())
        { 
            YVelocity -= gravityDirection * Mathf.Sqrt(2f * data.normalGravity * data.jumpHeight);
            jumpsRemaining--;
        }
    }

    public void HandleGravityReset()
    {
        if (YVelocity.normalized == gravityDirection)
        {
            YVelocity = Vector3.zero;
            propelVelocity.y = 0;
        }
    }

    private void Move()
    {
        switch (movementType)
        {
            case PlayerMovementType.Ground:
                HandleGravityReset();
                ResetJumps();
                GroundMove();
                break;
            case PlayerMovementType.Air:
                AirMove();
                break;
            case PlayerMovementType.Slope:
                HandleGravityReset();
                ResetJumps();
                SlopeMove();
                break;
            case PlayerMovementType.Dash:
                DashMove();
                break;
            default:
                break;
        }
        HandleBonusSpeed();
        HandleDrag();
        LastXZDir = GetMoveDir();
        LastPosition = transform.position;
    }


    private PlayerMovementType GetMovementType() 
    {
        if(isDashing)
        {
            return PlayerMovementType.Dash;
        }
        else if(!isGrounded || YVelocity.normalized == -gravityDirection || groundAngle > data.maxSlopeAngle)
        {
            return PlayerMovementType.Air;
        }
        else if (isGrounded && !(groundAngle <= 0f)) 
        {
            return PlayerMovementType.Slope;
        }

        return PlayerMovementType.Ground;
    }

    private void SetColliderProperties()
    {
        capCol.center = new Vector3(0, data.hoverHeight, 0);
        groundRayLength = data.hoverHeight * 2;
        if (movementMod == PlayerMovementMod.Walking || movementMod == PlayerMovementMod.Sprinting)
        {
            height = data.normalHeight;
            capCol.height = data.normalHeight - data.hoverHeight * 2;
            groundRayLength += data.normalHeight / 2;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        boxCol = GetComponent<BoxCollider>();
        SetColliderProperties();
        baseRotation = transform.rotation;
        ResetVelocity();
    }

    private void FixedUpdate()
    {
        movementType = GetMovementType();
        GetGroundData();
        Move();
        //Reminder in the function deltaTime is used comapred to fixedDeltaTime which gives a higher sensitivity on Y roatation
        Rotate();
        //Debug.Log(rb.velocity);
    }
}
