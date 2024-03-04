using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    // Start is called before the first frame update
    Camera tem;
    public float distance;
    public Vector3 angle;
    void Start()
    {
        tem = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        angle = new Vector3 ((Mathf.Rad2Deg * Mathf.Atan2(GetXRotAxis(transform.position,tem.transform.position), transform.position.y - tem.transform.position.y))-90
            , Mathf.Rad2Deg * Mathf.Atan2(transform.position.x - tem.transform.position.x, transform.position.z - tem.transform.position.z)
            ,0);
        tem.transform.eulerAngles = angle;
    }
    float GetXRotAxis(Vector3 target,Vector3 cam)
    {
        float output = Vector2.Distance(new Vector2(target.x,target.z), new Vector2(cam.x, cam.z));
        return output;
    }
}
