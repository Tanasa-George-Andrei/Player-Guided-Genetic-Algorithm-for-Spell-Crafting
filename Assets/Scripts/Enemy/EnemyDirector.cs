using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class EnemyDirector : MonoBehaviour, IMagicObjectDirector
{
    public event IMagicObjectDirector.CollisionTrigger HitTrigger;
    public event GenericTrigger OnObjectDestroy;
    public event GenericTrigger OnObjectDisable;

    private float health;
    [SerializeField]
    private float maxHealth = 100;

    private PlayerMotor motor;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // cache tranasform

    public void DashInDirection(Vector3 _dir, float _distance, float _duration)
    {
        motor.Dash(_dir, _distance, _duration);
    }

    public void DisableObject()
    {
        OnObjectDisable?.Invoke();
        gameObject.SetActive(false);
    }

    public IMagicObjectDirector GetActualDirector()
    {
        return this;
    }

    public Collider GetCollider()
    {
        return motor.GetCollider();
    }

    public Vector3 GetFlatDir()
    {
        return transform.forward;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public string GetName()
    {
        return this.gameObject.name;
    }

    public IMagicObjectDirector GetPlaceholder()
    {
        return new MagicPlaceholderDirector(this, motor.GetCollider(), gameObject, this.name, transform.position, transform.rotation);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetProjectilePosition()
    {
        return transform.position + 0.75f * transform.forward;
    }

    public Vector3 GetTargetDir(Vector3 _from)
    {
        return transform.forward;
    }
    void OnEnable()
    {
        health = maxHealth;
    }

    public void ModifyHealth(float value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);
        if (health <= 0)
        {
            OnObjectDestroy?.Invoke();
            DisableObject();
        }
    }

    public void ResetObject(Vector3 _pos, Quaternion _rot)
    {
        transform.position = _pos;
        transform.rotation = _rot;
        //motor.ResetVelocity();
        gameObject.SetActive(true);
    }

    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    public Vector3 GetPropelDir()
    {
        return transform.forward;
    }

    public void TeleportInDirection(Vector3 _dir, float _distance)
    {
        motor.Teleport(_dir, _distance);
    }

    public void Propel(Vector3 _dir, float _speed)
    {
        motor.Propel(_dir, _speed);
    }

    public void Damage(float _value, IMagicObjectDirector _owner)
    {
        ModifyHealth(-_value);
    }

    public void InstaDestory()
    {
        Damage(maxHealth, null);
    }

    private bool canAttack = true;
    private Coroutine canAttackCoroutine;

    private IEnumerator WaitToAttack()
    {
        canAttack = false;
        yield return Helpers.GetWait(2.5f);
        canAttack = true;
    }

    private IMagicObjectDirector closestTarget;
    private void Update()
    {
        //if(Vector3.Distance(GameManager.Instance.Player.GetPosition(), transform.position) >= Vector3.Distance(GameManager.Instance.Base.GetPosition(), transform.position))
        //{
        //    closestTarget = GameManager.Instance.Base;
        //}
        //else
        //{
        //    closestTarget = GameManager.Instance.Player;
        //}

        //transform.LookAt(closestTarget.GetPosition(), Vector3.up);
        //if (Vector3.Distance(closestTarget.GetPosition(), transform.position) > (GetRadiousFromCenter() + closestTarget.GetRadiousFromCenter()) * 1.5f)
        //{
        //    motor.SetMovementInput(new Vector2(0, 1), true);
        //}
        //else 
        //{
        //    motor.SetMovementInput(new Vector2(0, 0), true);
        //    if(canAttack) 
        //    {
        //        closestTarget.Damage(5f, this);
        //        canAttackCoroutine = StartCoroutine(WaitToAttack());
        //    }
            
        //}
    }

    public float GetRadiousFromCenter()
    {
        return motor.GetCollider().radius; 
    }
}
