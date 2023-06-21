using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPlaceholderDirector : IMagicObjectDirector
{
    //TODO: Make event chains just in case
    //TODO: See About an Hellper for and object pool
    private IMagicObjectDirector actualDirector;
    private Collider collider;
    private GameObject gObject;
    private string name;
    private Vector3 position;
    private Quaternion rotation;

    public event IMagicObjectDirector.CollisionTrigger OnHit;
    public event GenericTrigger OnObjectDisable;
    public event IMagicObjectDirector.DestroyTrigger OnObjectDestroy;
    public event IMagicObjectDirector.BounceTrigger OnBounce;

    public MagicPlaceholderDirector(IMagicObjectDirector _actualDirector, Collider _collider, GameObject _gObject, string _name, Vector3 _position, Quaternion _rotation)
    {
        actualDirector = _actualDirector;
        collider = _collider;
        gObject = _gObject;
        name = _name;
        position = _position;
        rotation = _rotation;
        //actualDirector.HitTrigger += HitTrigger;
        //actualDirector.OnObjectDestroy += OnObjectDestroy;
        //actualDirector.OnObjectDestroy += OnObjectDisable;
        if(actualDirector != null) 
        {
            actualDirector.OnObjectDisable += RemoveActualDirector;
        }
    }

    public void RemoveActualDirector()
    {
        actualDirector.OnObjectDisable -= RemoveActualDirector;
        actualDirector = null;
    }

    public IMagicObjectDirector GetActualDirector()
    {
        return actualDirector;
    }

    public Collider GetCollider()
    {
        return collider;
    }

    public GameObject GetGameObject()
    {
        return gObject;
    }

    public string GetName()
    {
        return name + " Placeholder";
    }

    public IMagicObjectDirector GetPlaceholder(bool _actualIsNull)
    {
        return this;
    }

    public Vector3 GetProjectilePosition()
    {
        return position;
    }

    public Vector3 GetFlatDir()
    {
        //Note Change so it's avariable dependent on dash so it's the oposite dirrection
        return rotation * Vector3.forward;
    }

    public Vector3 GetTargetDir(Vector3 _from)
    {
        return rotation * Vector3.forward;
    }

    public void DashInDirection(Vector3 _dir, float _distance, float _duration) 
    {
        actualDirector.DashInDirection(_dir, _distance, _duration);
    }

    public void ResetObject(Vector3 _pos, Quaternion _rot)
    {
        position = _pos;
        rotation = _rot;
    }

    public void SetRotation(Quaternion _rot)
    {
        rotation = _rot;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void ModifyHealth(float value)
    {
        actualDirector.ModifyHealth(value);
    }

    public Vector3 GetPropelDir()
    {
        return rotation * Vector3.forward;
    }

    public void TeleportInDirection(Vector3 _dir, float _distance)
    {
        actualDirector.TeleportInDirection(_dir, _distance);
    }

    public void Propel(Vector3 _dir, float _speed)
    {
        actualDirector.Propel(_dir, _speed);
    }

    public void Damage(float _value, IMagicObjectDirector _owner)
    {
        actualDirector.Damage(_value, _owner);
    }

    public void InstaDestory()
    {
        actualDirector.InstaDestory();
    }

    public void DisableObject()
    {
        actualDirector.DisableObject();
    }

    public float GetRadiousFromCenter()
    {
        return 0;
    }

    public void SetBounce(int _no)
    {
        actualDirector.SetBounce(_no);
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}
