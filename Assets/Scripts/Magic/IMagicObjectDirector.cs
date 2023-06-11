using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GenericTrigger();
public interface IMagicObjectDirector
{
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
    public IMagicObjectDirector GetPlaceholder();

    public delegate void CollisionTrigger(Collision _collision);
    public event CollisionTrigger HitTrigger;

    public event GenericTrigger OnObjectDisable;
    public event GenericTrigger OnObjectDestroy;

}
