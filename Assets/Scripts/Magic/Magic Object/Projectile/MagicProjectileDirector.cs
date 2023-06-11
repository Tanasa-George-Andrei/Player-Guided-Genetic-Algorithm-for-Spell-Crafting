using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MagicProjectileMotor))]
public class MagicProjectileDirector : MonoBehaviour, IMagicObjectDirector
{
    private MagicProjectileMotor motor;

    private float health;
    [SerializeField]
    private float maxHealth = 20;

    public event IMagicObjectDirector.CollisionTrigger HitTrigger;
    public event GenericTrigger OnObjectDisable;
    public event GenericTrigger OnObjectDestroy;

    void OnCollisionEnter(Collision collision)
    {
        HitTrigger?.Invoke(collision);
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

    public IMagicObjectDirector GetPlaceholder()
    {
        return new MagicPlaceholderDirector(this, motor.GetCollider(), gameObject, this.name, transform.position, transform.rotation);
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
        motor.ResetVelocity();
        gameObject.SetActive(true);
        StartCoroutine(ProjectileEnableTimer());
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
        yield return Helpers.GetWait(30f);
        DisableObject();
    }

    private void Awake()
    {
        motor = GetComponent<MagicProjectileMotor>();
        StartCoroutine(ProjectileEnableTimer());
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
}
