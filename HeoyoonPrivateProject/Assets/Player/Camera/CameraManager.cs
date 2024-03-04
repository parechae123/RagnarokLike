using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager
{
    private Camera _camera;
    public Camera PlayerMainCamer
    {
        get 
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    _camera = new GameObject("PlayerMainCamera").AddComponent<Camera>();
                }
            }
            return _camera;
        }
    }
    List<Transform> RotationTrackingTR = new List<Transform>();
    private static CameraManager instance;
    public static CameraManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = new CameraManager();
            }
            return instance;

        }
    }
    public void RotateCamera(Vector3 CameraRotDirection)
    {

        PlayerMainCamer.transform.eulerAngles += CameraRotDirection;
        SetRotaionSync();
    }
    public void SetRotaionSync()
    {
        for (int i = 0; i < RotationTrackingTR.Count; i++)
        {
            RotationTrackingTR[i].rotation = PlayerMainCamer.transform.rotation;
        }
    }
    public void AddTR(Transform tr)
    {
        RotationTrackingTR.Add(tr);
    }
    public void RemoveTR(Transform tr) 
    { 
        while (RotationTrackingTR.Contains(tr))
        {
            RotationTrackingTR.Remove(tr);
            Debug.Log(RotationTrackingTR.Count);
        }
    }
}
