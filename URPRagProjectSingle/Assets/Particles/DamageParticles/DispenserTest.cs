using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenserTest : MonoBehaviour
{
    // Start is called before the first frame update
    public DamageDispenser temp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.GetInstance().SpawnFloatText(transform.position, "asdf123", Color.red, 1);
        }
    }
}
