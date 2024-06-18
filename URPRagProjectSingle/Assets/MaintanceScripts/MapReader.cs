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

    public override void OnInspectorGUI()           //����Ƽ�� �ν����� �Լ��� ������
    {
        base.OnInspectorGUI();
        MapReader gameSystem = (MapReader)target;//����Ƽ �ν����� �Լ� ������ ���� �Ѵ�.(Base)

        if (GUILayout.Button("GetMapData"))
        {
            gameSystem.GetBlockPositions();
        }
    }

}

#endif
public class MapReader : MonoBehaviour
{
    //���� �ش� �迭�� ��, Ÿ���� ��ġ, �� Ÿ�� ����
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
