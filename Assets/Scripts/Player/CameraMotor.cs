using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Camera camera;

    private float xRotation = 0;
    private float xRotationDelta;
    private float xSesitivity;

    public void ResetRotation()
    {
        xRotation = 0;
    }

    public void FPSRotate(float _inputRoationDelta, float _xSensitivity)
    {
        xRotationDelta = _inputRoationDelta;
        xSesitivity = _xSensitivity;
    }

    private void FPS()
    {
        xRotation -= xRotationDelta * xSesitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    public Vector3 GetCameraDirection()
    {
        return camera.transform.forward;
    }

    public Vector3 GetCameraPosition()
    {
        return camera.transform.position;
    }

    public Quaternion GetCameraRotation()
    {
        return camera.transform.rotation;
    }

    private void Update()
    {
        FPS();
    }
}
