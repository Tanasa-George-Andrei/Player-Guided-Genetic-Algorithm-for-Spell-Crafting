using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileMovementType
{
    None,
    Normal,
    Dash
}

[RequireComponent(typeof(Rigidbody))]
public class MagicProjectileMotor : MonoBehaviour
{
    private ProjectileMovementType movementType = ProjectileMovementType.Normal;

    private SphereCollider col;
    private Rigidbody rb;

    private Vector3 gravityDirection = new Vector3(0, -1, 0);
    public float gravityAcceleration = 0;
    private bool isGrounded;

    private Vector3 dashVelocity;
    private Vector3 propelVelocity;
    private Vector3 YVelocity;
    private Vector3 finalVelocity;
    private bool isDashing;
    private Coroutine dashCoroutine;

    public float drag = 0;

    private void GetGroundData()
    {
        RaycastHit hit;
        //add more stringent conditions for being grounded
        isGrounded = Physics.Raycast(transform.position, gravityDirection, out hit, col.radius*2, 1<<8);
    }

    public void ResetVelocity()
    {
        dashVelocity = Vector3.zero;
    }

    public SphereCollider GetCollider()
    {
        return col;
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
        dashVelocity = (_distance / _duration) * _dir;
        yield return Helpers.GetWait(_duration);
        dashVelocity = Vector3.zero;
        isDashing = false;
    }

    private void DashMove()
    {
        rb.AddForce(dashVelocity - rb.velocity, ForceMode.VelocityChange);
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
            if (!Physics.CheckSphere(newPos, col.radius))
            {
                transform.position = newPos;
                break;
            }
            else
            {
                newPos.y += col.radius;
            }
        }
    }

    public void Propel(Vector3 _dir, float _speed)
    {
        propelVelocity += _speed * _dir;
    }

    private ProjectileMovementType GetMovementType()
    {
        if (isDashing)
        {
            return ProjectileMovementType.Dash;
        }
        else if(propelVelocity == Vector3.zero && YVelocity == Vector3.zero)
        {
            return ProjectileMovementType.None;
        }
        
        return ProjectileMovementType.Normal;
    }

    private void ApplyDrag(float _coeficient)
    {
        propelVelocity *= (1 - _coeficient * Time.fixedDeltaTime);
        if (propelVelocity.magnitude < 0.1f)
        {
            propelVelocity = Vector3.zero;
        }
    }

    private void ApplyGravity(float _force)
    {
        //Change this so it works in later versions with custom gravity
        if (propelVelocity.y > 0)
        {
            propelVelocity += _force * Time.fixedDeltaTime * gravityDirection;
        }
        else
        {
            YVelocity += _force * Time.fixedDeltaTime * gravityDirection;
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

    private void NormalMove()
    {
        HandleGravityReset();
        finalVelocity = YVelocity + propelVelocity;
        HandleGravityReset();
        rb.AddForce(finalVelocity - rb.velocity, ForceMode.VelocityChange);
    }

    private void Move()
    {
        switch (movementType)
        {
            case ProjectileMovementType.Normal:
                NormalMove();
                break;
            case ProjectileMovementType.Dash:
                DashMove();
                break;
            case ProjectileMovementType.None:
                
                break;
            default:
                break;
        }
        if(propelVelocity != Vector3.zero)
        {
            ApplyDrag(drag);
        }
    }

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        dashVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        movementType = GetMovementType();
        Move();
    }
}
