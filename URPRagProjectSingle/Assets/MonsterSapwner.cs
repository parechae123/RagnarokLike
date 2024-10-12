using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSapwner : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        MonsterManager.GetInstance().UpdateRespawnTime(Time.deltaTime);
    }
}
