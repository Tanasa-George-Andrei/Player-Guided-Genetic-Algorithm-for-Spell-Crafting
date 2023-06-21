using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Policy;
using UnityEngine;

public delegate void GenericTrigger();
public interface IMagicObjectDirector
{
    public void SetBounce(int _no);

    public float GetRadiousFromCenter();

    public void ModifyHealth(float _value);

    public void Damage(float _value, IMagicObjectDirector _owner);

    public void InstaDestory();

    public Vector3 GetPosition();
    public Vector3 GetProjectilePosition();
    public Vector3 GetTargetDir(Vector3 _from);
    public Vector3 GetFlatDir();
    public Vector3 GetPropelDir();
    public void ResetObject(Vector3 _pos, Quaternion _rot);
    public void DisableObject();
    public GameObject GetGameObject();
    public Collider GetCollider();
    public IMagicObjectDirector GetActualDirector();
    public void SetRotation(Quaternion _rot);
    public void DashInDirection(Vector3 _dir, float _distance, float _duration);
    public void TeleportInDirection(Vector3 _dir, float _distance);
    public void Propel(Vector3 _dir, float _speed);
    public string GetName();

    public Quaternion GetRotation();
    public IMagicObjectDirector GetPlaceholder(bool _actualIsNull);

    public delegate void CollisionTrigger(Collision _collision);
    public delegate void BounceTrigger(IMagicObjectDirector _director, bool _isLastBounce);
    public delegate void DestroyTrigger(IMagicObjectDirector _director);
    public event CollisionTrigger OnHit;
    public event BounceTrigger OnBounce;

    public event GenericTrigger OnObjectDisable;
    public event DestroyTrigger OnObjectDestroy;

}
