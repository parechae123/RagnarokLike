using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptables;

public class PlayerScriptableTest : MonoBehaviour
{
    public PlayerStat StatOBJ;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StatOBJ.luk++;
            StatOBJ.rawStat.flee++;

            //스크립터블로 저장됨
        }
    }
}
