using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(MapReader))]
public class LoadMapBTN : Editor
{

    public override void OnInspectorGUI()           //유니티의 인스펙터 함수를 재정의
    {
        base.OnInspectorGUI();
        MapReader gameSystem = (MapReader)target;//유니티 인스펙터 함수 동작을 같이 한다.(Base)

        if (GUILayout.Button("GetMapData"))
        {
            gameSystem.GetBlockPositions();
        }
    }

}

#endif
public class MapReader : MonoBehaviour
{
    //각각 해당 배열의 층, 타일의 위치, 맵 타일 묶음
    public sbyte[] sameArrayFloor = new sbyte[0];
    public Vector2Int[] blockPositions= new Vector2Int[0];
    public Transform[] targetMaps = new Transform[0];
    void Awake()
    {
        for (int i = 0; i < blockPositions.Length; i++)
        {
            GridManager.GetInstance().grids.Add(blockPositions[i], new Node(sameArrayFloor[i], blockPositions[i], true));
            Debug.Log(GridManager.GetInstance().grids[blockPositions[i]].nodeCenterPosition);
        }
        Destroy(gameObject);
    }
#if UNITY_EDITOR
    public void GetBlockPositions()
    {
        Array.Resize(ref blockPositions, 0);
        Array.Resize(ref sameArrayFloor, 0);
        for (byte i = 0; i < targetMaps.Length; i++)
        {
            for (ushort s = 0; s < targetMaps[i].transform.childCount; s++)
            {
                Array.Resize<Vector2Int>(ref blockPositions, blockPositions.Length + 1);
                blockPositions[blockPositions.Length-1] = new Vector2Int((int)targetMaps[i].transform.GetChild(s).position.x,(int)targetMaps[i].transform.GetChild(s).position.z);
                Array.Resize<sbyte>(ref sameArrayFloor, sameArrayFloor.Length + 1);
                sameArrayFloor[sameArrayFloor.Length - 1] = (sbyte)targetMaps[i].transform.GetChild(s).position.y;
            }
        }
    }
#endif
}
