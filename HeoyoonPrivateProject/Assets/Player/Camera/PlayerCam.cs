using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float distance;
    float Distance
    {
        get 
        { 
            return distance;
        }
        set 
        { 
            if (value >=maxCameraDistance)
            {
                distance = maxCameraDistance;
                return;
            }
            else if (value < minCameraDistance)
            {
                distance = minCameraDistance;
                return;
            }
            distance = value;
        }
    }
    public float maxCameraDistance;
    public float minCameraDistance;
    public float wheelSpeed;
    public float height;
    public Transform playerTarget;
    Vector3 cameraNomalizedPos = new Vector3(0,0,1).normalized;
    public float cameraRotValue;
    public float rotationSensitivity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Distance -= Input.GetAxis("Mouse ScrollWheel")*wheelSpeed;
        transform.position = ((cameraNomalizedPos)*Distance) + (Vector3.up*height)+playerTarget.position;
        transform.eulerAngles =  new Vector3((Mathf.Rad2Deg * Mathf.Atan2(GetXRotAxis(playerTarget.position, transform.position), playerTarget.position.y-transform.position.y)) - 90, Mathf.Rad2Deg * Mathf.Atan2(playerTarget.position.x-transform.position.x, playerTarget.position.z-transform.position.z), 0);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            cameraRotValue += Input.GetAxis("Mouse X")*rotationSensitivity;
            cameraNomalizedPos = GetCircle(cameraRotValue);
        }

    }
    Vector3 GetCircle(float num)
    {
        
        return new Vector3(MathF.Sin(num),0, MathF.Cos(num)).normalized;
    }
    float GetXRotAxis(Vector3 target, Vector3 cam)
    {
        float output = Vector2.Distance(new Vector2(target.x, target.z), new Vector2(cam.x, cam.z));
        return output;
    }
/*    float OneDimensionDIstance(float a,float b)
    {
        float tempNum = a-b;
        return Mathf.Sqrt(tempNum*tempNum);
    }*/
}
