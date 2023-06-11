using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(CameraMotor))]
[RequireComponent(typeof(PlayerAction))]
public class PlayerDirector : MonoBehaviour, IMagicObjectDirector
{
    private PlayerMotor motor;
    private CameraMotor camMotor;
    private PlayerAction actions;
    [SerializeField]
    private PlayerSettings settings;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private ClampedFloatVariable health;


    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        camMotor = GetComponent<CameraMotor>();
        actions = GetComponent<PlayerAction>();
        health.OnValueChange += OnHealthChange;

    }

    //Do the triggers;
    public event IMagicObjectDirector.CollisionTrigger HitTrigger;
    public event GenericTrigger OnObjectDisable;
    public event GenericTrigger OnObjectDestroy;

    void OnCollisionEnter(Collision collision)
    {
        HitTrigger?.Invoke(collision);
    }

    public float GetRemainingHealth()
    {
        return health.Value;
    }

    public string GetName()
    {
        return this.name;
    }
    public IMagicObjectDirector GetPlaceholder()
    {
        return new MagicPlaceholderDirector(this, motor.GetCollider(), gameObject, this.name, transform.position, camMotor.GetCameraRotation());
    }
    public Vector3 GetProjectilePosition()
    {
        return camMotor.GetCameraPosition() + camMotor.GetCameraRotation() * new Vector3(0, -0.25f, 0.75f);
    }

    public Vector3 GetTargetDir(Vector3 _from)
    {
        RaycastHit hit;
        if(Physics.Raycast(camMotor.GetCameraPosition() + camMotor.GetCameraDirection() * 0.5f, camMotor.GetCameraDirection(), out hit, 1000f))
        {
            return (hit.point - _from).normalized;
        }
        else
        {
            return camMotor.GetCameraDirection();
        }
    }
    public void ResetObject(Vector3 _pos, Quaternion _rot)
    {
        health.Value = health.maxValue;
        motor.ResetVelocity();
        motor.Teleport(_pos);
        motor.ResetRotation(_rot.eulerAngles.y);
        camMotor.ResetRotation();
        //gameObject.SetActive(true);
    }

    public void ResetObject()
    {
        health.Value = health.maxValue;
        motor.ResetVelocity();
        motor.Teleport(spawnPoint.position);
        motor.ResetRotation(spawnPoint.rotation.eulerAngles.y);
        camMotor.ResetRotation();
        //gameObject.SetActive(true);
    }

    public Collider GetCollider()
    {
        return motor.GetCollider();
    }

    public IMagicObjectDirector GetActualDirector()
    {
        return this;
    }
    public void DisableObject()
    {
        OnObjectDisable?.Invoke();
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public Vector3 GetFlatDir()
    {
        //return camMotor.GetCameraDirection();
        //NOTE Modify so it's done on motor level to take into account for custom gravity
        return (motor.GetMoveDir() != Vector3.zero) ? motor.GetMoveDir() : transform.forward;
    }

    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    public void DashInDirection(Vector3 _dir, float _distance, float _duration)
    {
        motor.Dash(_dir, _distance, _duration);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    private List<bool> actionList = new List<bool>() {false, false, false, false};
    public void ProcessInput(InputWrapper _input)
    {
        if(PlayerUIManager.Instance.IsPaused)
        { return; }
        motor.SetMovementInput(new Vector3(_input.inputMovement.x, 0, _input.inputMovement.y), _input.sprinting);
        motor.SetRotationInput(_input.mouseDelta.x, settings.mouseSensitivity.x);
        camMotor.FPSRotate(_input.mouseDelta.y, settings.mouseSensitivity.y);
        actionList[0] = _input.action1Start;
        actionList[1] = _input.action2Start;
        actionList[2] = _input.action3Start;
        actionList[3] = _input.action4Start;
        actions.OnAction(actionList, _input.specialAction);
        if (_input.jumped){ motor.Jump(); }
        if(_input.spellCreationStart) { actions.HandleSpellMaker(); }
    }

    public void ModifyHealth(float _value)
    {
        health.Value = Mathf.Clamp(health.Value + _value, health.minValue, health.maxValue);
    }

    public void Respawn()
    {
        ResetObject();
    }

    public void OnHealthChange()
    {
        if(health.Value <= health.minValue)
        {
            Respawn();
            OnObjectDestroy?.Invoke();
        }
    }

    public Vector3 GetPropelDir()
    {
        return camMotor.GetCameraDirection();
    }

    public void Propel(Vector3 _dir, float _speed)
    {
        motor.Propel(_dir, _speed);
    }

    public void TeleportInDirection(Vector3 _dir, float _distance)
    {
        motor.Teleport(_dir,_distance);
    }

    public void Damage(float _value, IMagicObjectDirector _owner)
    {
        ModifyHealth(-_value);
    }

    public bool HasEnoughHealth(float _value)
    {
        if(health.Value < _value)
        {
            return false;
        }
        return true;
    }

    public void InstaDestory()
    {
        Damage(health.maxValue,null);
    }

    public float GetRadiousFromCenter()
    {
        return motor.GetCollider().radius;
    }
}
