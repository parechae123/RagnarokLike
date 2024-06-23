using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

/// <summary>
/// �޴��� ���ø�ȭ
/// </summary>
/// <typeparam name="T">�ش� �޴��� Ŭ������</typeparam>
public class Manager<T> where T : new()
{
    protected static T instance;
    /// <summary>
    /// �ν��Ͻ��� �����ɴϴ�.
    /// </summary>
    /// <returns></returns>
    public static T GetInstance ()
    {
        if (instance == null)
        {
            instance = new T();
        }
        return instance;
    }
    /// <summary>
    /// �Ŵ����� �⺻������ �����Ͽ� ������ �����մϴ�.
    /// </summary>
    public static void Release()
    {

        instance = default(T);
    }
}
[System.Serializable]
public class Node
{
    public Vector2Int nodeCenterPosition;       //����� �߾� ��ǥ��
    public bool isMoveableTile;                 //�̵����� Ÿ������
    public sbyte nodeFloor;                     //����� ��(1�� = y1,-1�� = y-1
    public Node(sbyte floor, Vector2Int vec, bool isMoveable)
    {
        nodeFloor = floor;
        nodeCenterPosition = vec;
        isMoveableTile = isMoveable;
    }
    public Node connectedNode { get; private set; }
    //���� ���κ��� Ÿ�ٳ������� �Ÿ�
    int H;
    //���� ���κ��� ���۳������� �Ÿ�
    int G;
    //�� �Ÿ��� ��ģ ��
    /// <summary>
    /// ��Ż�ڽ�Ʈ
    /// </summary>
    int F { get { return H + G; } }

    public void SetGH(Vector2Int startPos,Vector2Int endPos)
    {
        G = GetDistance(startPos);
        H = GetDistance(endPos);
    }


    public int GetDistance(Vector2Int targetPos)
    {
        // �� �� ������ x�� y ��ǥ ���� ���
        Vector2Int dist = new Vector2Int(Mathf.Abs(nodeCenterPosition.x - targetPos.x), Mathf.Abs(nodeCenterPosition.y - targetPos.y));

        // ����ư �Ÿ� ��� (�� �� ������ ���� �� ���� �Ÿ� ��)
        return (dist.x + dist.y) * 10;
    }



}
public class GridManager : Manager<GridManager>
{
    public Dictionary<Vector2Int, Node> grids = new Dictionary<Vector2Int, Node>();
    private Mesh tileMesh;
    public Mesh TileMesh
    {
        get 
        {
            if (tileMesh == null)
            {
                tileMesh = Resources.Load<GameObject>("WalkAbleTile").GetComponent<MeshFilter>().sharedMesh;

            }
            return tileMesh;
        }
    }
    private Material walkableMaterial;
    public Material WalkableMaterial
    {
        get 
        {
            if (walkableMaterial == null)
            {
                walkableMaterial = Resources.Load<Material>("WalkAbleTileMaterial");
            }
            return walkableMaterial;
        }
    }
    private Material noneWalkableMaterial;
    public Material NoneWalkableMaterial
    {
        get 
        {
            if (noneWalkableMaterial == null)
            {
                noneWalkableMaterial = Resources.Load<Material>("NoneWalkAbleTileMaterial");
            }
            return noneWalkableMaterial;
        }
    }
    /// <summary>
    /// vector3�� vector2int�� ����ȯ(�ݿø�)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Node PositionToNode(Vector3 position)
    {
        Debug.Log(Mathf.RoundToInt(position.x)+","+ Mathf.RoundToInt(position.z));
        Vector2Int tempIntPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        return grids.ContainsKey(tempIntPos)? grids[new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z))]: null;
    }

    /// <summary>
    /// �ڽ�Ʈ�� �޸���ƽ�� ���Ͽ� ������ ��θ� ã��
    /// </summary>
    /// <param name="startPos">���۳�� ��ġ</param>
    /// <param name="endPos">�̵���ǥ ��� ��ġ</param>
    /// <returns></returns>
    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        Node[] tempNode = new Node[0];
        HashSet<Vector2Int> closedNodePosition = new HashSet<Vector2Int>();
        

        return tempNode;
    }
    /// <summary>
    /// ����� Node�� ��ȯ�޴� �Լ�
    /// </summary>
    /// <param name="position">������</param>
    /// <returns>��,��,��,�� ����</returns>
    public Node[] GetNearNodes(Vector2Int position,ref HashSet<Vector2Int> closedHash)
    {
        Node[] nearNodes = new Node[4];;
        nearNodes[0] = grids.ContainsKey(position + Vector2Int.left) ? grids[position + Vector2Int.left].isMoveableTile? grids[position + Vector2Int.left]: null : null ;
        nearNodes[1] = grids.ContainsKey(position + Vector2Int.right) ? grids[position + Vector2Int.right].isMoveableTile? grids[position + Vector2Int.right] : null : null ;
        nearNodes[2] = grids.ContainsKey(position + Vector2Int.down) ? grids[position + Vector2Int.down].isMoveableTile ? grids[position + Vector2Int.down] : null :null ;
        nearNodes[3] = grids.ContainsKey(position + Vector2Int.up) ? grids[position + Vector2Int.up].isMoveableTile ? grids[position + Vector2Int.up] : null :null ;

        if (nearNodes[0] == null && closedHash.Contains(position + Vector2Int.left)) closedHash.Add(position + Vector2Int.left);
        if (nearNodes[1] == null && closedHash.Contains(position + Vector2Int.right)) closedHash.Add(position + Vector2Int.right);
        if (nearNodes[2] == null && closedHash.Contains(position + Vector2Int.down)) closedHash.Add(position + Vector2Int.down);
        if (nearNodes[3] == null && closedHash.Contains(position + Vector2Int.up)) closedHash.Add(position + Vector2Int.up);
        return nearNodes;
    }
}
