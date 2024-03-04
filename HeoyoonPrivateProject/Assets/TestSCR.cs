using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSCR : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform targetSTR;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            CameraManager.Instance.RemoveTR(targetSTR);
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) 
        {
            CameraManager.Instance.AddTR(targetSTR);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            CameraManager.Instance.RotateCamera(new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")));
        }
    }
}
