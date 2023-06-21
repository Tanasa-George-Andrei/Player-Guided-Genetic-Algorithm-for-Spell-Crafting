using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MagicProjectileMotor))]
public class MagicProjectileDirector : MonoBehaviour, IMagicObjectDirector
{
    [SerializeField]
    private bool destroyOnContact = true;

    [SerializeField]
    private MagicProjectileMotor motor;

    private float health;
    [SerializeField]
    private float maxHealth = 20;

    public event IMagicObjectDirector.CollisionTrigger OnHit;
    public event GenericTrigger OnObjectDisable;
    public event IMagicObjectDirector.DestroyTrigger OnObjectDestroy;
    public event IMagicObjectDirector.BounceTrigger OnBounce;

    private int bouncesRemaining = 0;

    private void Awake()
    {
        //StartCoroutine(ProjectileEnableTimer());
        health = maxHealth;
        bouncesRemaining = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        OnHit?.Invoke(collision);
        if(destroyOnContact && bouncesRemaining == 0)
        {
            Damage(maxHealth, null);
        }
        if(bouncesRemaining > 0) 
        {
            motor.ReflectMovementDirection(collision.GetContact(0).normal);
            IMagicObjectDirector tempDirector;
            tempDirector = collision.GetContact(0).otherCollider.gameObject.GetComponent<IMagicObjectDirector>();
            OnBounce?.Invoke(new MagicPlaceholderDirector(
            tempDirector,
            collision.GetContact(0).otherCollider,
            collision.GetContact(0).otherCollider.gameObject,
            collision.GetContact(0).otherCollider.gameObject.name,
            collision.GetContact(0).point,
            Quaternion.FromToRotation(Vector3.forward, collision.GetContact(0).normal
                )), bouncesRemaining == 1);
            bouncesRemaining--;
        }
        
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

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public IMagicObjectDirector GetPlaceholder(bool _actualIsNull)
    {
        return new MagicPlaceholderDirector(_actualIsNull ? null : this, motor.GetCollider(), gameObject, this.name, transform.position, transform.rotation);
    }

    public Vector3 GetProjectilePosition()
    {
        return transform.position + transform.rotation * Vector3.forward;
    }

    public Vector3 GetFlatDir()
    {
        return transform.forward;
    }

    public Vector3 GetTargetDir(Vector3 _from)
    {
        return transform.forward;
    }

    public void DashInDirection(Vector3 _dir, float _distance, float _duration)
    {
        //Change to dash behaviour and make a propel mechanism
        motor.Dash(_dir, _distance, _duration);
    }

    public void ResetObject(Vector3 _pos, Quaternion _rot)
    {
        transform.position = _pos;
        transform.rotation = _rot;
        bouncesRemaining = 0;
        motor.ResetVelocity();
        gameObject.SetActive(true);
        //StartCoroutine(ProjectileEnableTimer());
        //foreach (Delegate d in OnHit.GetInvocationList())
        //{
        //    OnHit -= (IMagicObjectDirector.CollisionTrigger)d;
        //}
        //foreach (Delegate d in OnBounce.GetInvocationList())
        //{
        //    OnBounce -= (IMagicObjectDirector.BounceTrigger)d;
        //}
    }

    void OnEnable()
    {
        health = maxHealth;
    }

    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private IEnumerator ProjectileEnableTimer()
    {
        yield return Helpers.GetWait(300f);
        Damage(maxHealth,null);
        //DisableObject();
    }

    public void ModifyHealth(float value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);
        if (health <= 0)
        {
            OnObjectDestroy?.Invoke(GetPlaceholder(true));
            DisableObject();
        }
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
        Damage(maxHealth,null);
    }

    public float GetRadiousFromCenter()
    {
        return motor.GetCollider().radius;
    }

    public void SetBounce(int _no)
    {
        bouncesRemaining += _no;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
}
