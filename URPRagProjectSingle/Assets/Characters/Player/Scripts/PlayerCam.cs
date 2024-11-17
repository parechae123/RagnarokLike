using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private static PlayerCam instance;
    public static PlayerCam Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new PlayerCam();
            }
            return instance;
        }
    }
    public float distance;
    float Distance
    {
        get
        {
            return distance;
        }
        set
        {
            if (value >= maxCameraDistance)
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
    [SerializeField]Vector3 cameraNomalizedPos = new Vector3(0, 0, 1).normalized;
    public float cameraRotValue;
    public float rotationSensitivity;
    [SerializeField]private Vector2Int cameraDirrection;
    public event Action rotDirrection;
    public event Action rotations;
    public Vector2Int CameraDirrection
    {
        get 
        {
            if (MathF.Abs(cameraNomalizedPos.x) > MathF.Abs(cameraNomalizedPos.z))
            {
                if (cameraNomalizedPos.x > 0)
                {
                    cameraDirrection = Vector2Int.right;
                }
                else
                {
                    cameraDirrection = Vector2Int.left;

                }
            }
            else
            {
                if (cameraNomalizedPos.z > 0)
                {
                    cameraDirrection = Vector2Int.up;
                }
                else
                {
                    cameraDirrection = Vector2Int.down;

                }
            }
            return cameraDirrection;
        }
    }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed;
        transform.position = ((cameraNomalizedPos) * Distance) + (Vector3.up * height) + playerTarget.position;
        transform.eulerAngles = new Vector3((Mathf.Rad2Deg * Mathf.Atan2(GetXRotAxis(playerTarget.position, transform.position), playerTarget.position.y - transform.position.y)) - 90, Mathf.Rad2Deg * Mathf.Atan2(playerTarget.position.x - transform.position.x, playerTarget.position.z - transform.position.z), 0);
        rotations.Invoke();
        rotDirrection.Invoke();
    }
    // Update is called once per frame
    void Update()
    {
        Distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed;
        transform.position = ((cameraNomalizedPos) * Distance) + (Vector3.up * height) + playerTarget.position;
        transform.eulerAngles = new Vector3((Mathf.Rad2Deg * Mathf.Atan2(GetXRotAxis(playerTarget.position, transform.position), playerTarget.position.y - transform.position.y)) - 90, Mathf.Rad2Deg * Mathf.Atan2(playerTarget.position.x - transform.position.x, playerTarget.position.z - transform.position.z), 0);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            rotations.Invoke();
            cameraRotValue += Input.GetAxis("Mouse X") * rotationSensitivity;
            cameraNomalizedPos = GetCircle(cameraRotValue);

            if (cameraDirrection != CameraDirrection) 
            {
                rotDirrection?.Invoke();
            }
        }

    }

    Vector3 GetCircle(float num)
    {

        return new Vector3(MathF.Sin(num), 0, MathF.Cos(num)).normalized;
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
public enum Dirrections
{
    N, E, S, W
}