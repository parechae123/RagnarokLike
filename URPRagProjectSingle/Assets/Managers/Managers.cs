using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    public static T GetInstance()
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
    public Node connectedNode;
    //���� ���κ��� Ÿ�ٳ������� �Ÿ�
    public int H { get; private set; }
    //���� ���κ��� ���۳������� �Ÿ�
    public int G { get; private set; }
    //�� �Ÿ��� ��ģ ��
    /// <summary>
    /// ��Ż�ڽ�Ʈ
    /// </summary>
    public int F { get { return H + G; } }

    public void SetGH(Vector2Int startPos, Vector2Int endPos)
    {
        G = GetDistance(startPos);
        H = GetDistance(endPos);
    }


    private int GetDistance(Vector2Int targetPos)
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
        Debug.Log(Mathf.RoundToInt(position.x) + "," + Mathf.RoundToInt(position.z));
        Vector2Int tempIntPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        return grids.ContainsKey(tempIntPos) ? grids[new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z))] : null;
    }

    /// <summary>
    /// �ڽ�Ʈ�� �޸���ƽ�� ���Ͽ� ������ ��θ� ã��
    /// </summary>
    /// <param name="startPos">���۳�� ��ġ</param>
    /// <param name="endPos">�̵���ǥ ��� ��ġ</param>
    /// <returns></returns>
    /// 
    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        //������� : ���� �� �� �ִ�(��������� �ֺ�)
        //������� : ���� ��� �� ���� ���� f����� ���,�������� ���� �� �� ������忡�� ����
        if (!grids.ContainsKey(endPos)) return null;
        LinkedList<Node> openNodes = new LinkedList<Node>();
        LinkedList<Node> closedNodes = new LinkedList<Node>();
        bool isSearchDone = false;


        closedNodes.AddLast(grids[startPos]);
        grids[startPos].SetGH(startPos, endPos);
        short whileCounterMax = (short)1000;
        while (!isSearchDone)
        {
            GetNearNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);
            Vector2Int lowerstHNodePos = Vector2Int.zero;
            int lowerstHValue = int.MaxValue;
            while (openNodes.Count > 0)
            {
                Node node = openNodes.First();
                node.SetGH(startPos, endPos);
                openNodes.RemoveFirst();
                if (lowerstHValue > node.H)
                {
                    lowerstHValue = node.H;
                    lowerstHNodePos = node.nodeCenterPosition;
                }


                if (endPos == node.nodeCenterPosition)
                {
                    isSearchDone = true;
                    openNodes.Clear();
                }
            }
            openNodes.Clear();
            grids[lowerstHNodePos].connectedNode = closedNodes.Last();
            closedNodes.AddLast(grids[lowerstHNodePos]);
            --whileCounterMax;
            if (whileCounterMax <= 0) return closedNodes.ToArray();
        }

        return closedNodes.ToArray();

    }
    /*    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
        {
            //������� : ���� �� �� �ִ�(��������� �ֺ�)
            //������� : ���� ��� �� ���� ���� f����� ���,�������� ���� �� �� ������忡�� ����
            if (!grids.ContainsKey(endPos)) return null;
            LinkedList<Node> openNodes = new LinkedList<Node>();
            LinkedList<Node> closedNodes = new LinkedList<Node>();
            bool isSearchDone = false;


            closedNodes.AddLast(grids[startPos]);
            grids[startPos].SetGH(startPos, endPos);
            short whileCounterMax = (short)1000;
            while (!isSearchDone)
            {
                GetNearNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);
                Vector2Int lowerstHNodePos = Vector2Int.zero;
                int lowerstHValue = int.MaxValue;
                while (openNodes.Count > 0)
                {
                    Node node = openNodes.First();
                    node.SetGH(startPos, endPos);
                    openNodes.RemoveFirst();
                    if (lowerstHValue > node.H)
                    {
                        lowerstHValue = node.H;
                        lowerstHNodePos = node.nodeCenterPosition;
                    }


                    if (endPos == node.nodeCenterPosition)
                    {
                        isSearchDone = true;
                        openNodes.Clear();
                    }
                }
                openNodes.Clear();

                if (grids[lowerstHNodePos].G > 10) grids[lowerstHNodePos].connectedNode = closedNodes.Last();
                else grids[lowerstHNodePos].connectedNode = grids[startPos];
                closedNodes.AddLast(grids[lowerstHNodePos]);
                --whileCounterMax;
                if (whileCounterMax <= 0) return closedNodes.ToArray();
            }
            //����ó��
            Node[] checkedShortestNodes = new Node[0];
            ushort tempCount = (ushort)closedNodes.Count;
            for (ushort i = 0; i < tempCount; i++)
            {
                Array.Resize(ref  checkedShortestNodes, i+1);
                checkedShortestNodes[i] = closedNodes.Last();
                closedNodes.RemoveLast();
                if (grids[startPos] == checkedShortestNodes[i].connectedNode)
                {
                    Array.Resize(ref checkedShortestNodes, i + 2);
                    checkedShortestNodes[i + 1] = grids[startPos];
                    break;
                }
            }
            return checkedShortestNodes;

        }*/
    /// <summary>
    /// ����� Node�� Add���ִ� �Լ� (��,��,��,�� ����)
    /// </summary>
    /// <param name="nearNodes">add�� linkedList<Node>    </param>
    /// <param name="position">������</param>
    /// <param name="closedNodes">�������,�񱳸� ���� �ʿ�</param>
    public void GetNearNodes(ref LinkedList<Node> nearNodes, Vector2Int position/*,ref LinkedList<Vector2Int> noneWalkAbleNodes*/, LinkedList<Node> closedNodes)
    {
        //�ش� ��ǥ�� ��尡 ���ų� wallkable�� �ƴϸ� �ش� �迭�� null�� �ٲ���

        if (grids.ContainsKey(position + Vector2Int.left))
        {
            if (grids[position + Vector2Int.left].isMoveableTile && !closedNodes.Contains(grids[position + Vector2Int.left])) nearNodes.AddLast(grids[position + Vector2Int.left]);
        }

        if (grids.ContainsKey(position + Vector2Int.right))
        {
            if (grids[position + Vector2Int.right].isMoveableTile && !closedNodes.Contains(grids[position + Vector2Int.right])) nearNodes.AddLast(grids[position + Vector2Int.right]);
        }

        if (grids.ContainsKey(position + Vector2Int.down))
        {
            if (grids[position + Vector2Int.down].isMoveableTile && !closedNodes.Contains(grids[position + Vector2Int.down])) nearNodes.AddLast(grids[position + Vector2Int.down]);
        }

        if (grids.ContainsKey(position + Vector2Int.up))
        {
            if (grids[position + Vector2Int.up].isMoveableTile && !closedNodes.Contains(grids[position + Vector2Int.up])) nearNodes.AddLast(grids[position + Vector2Int.up]);
        }
        /*        //��尡 ����ְ� list�� ���� ���� ���� ��
                if (nearNodes[0] == null && !noneWalkAbleNodes.Contains(position + Vector2Int.left)) noneWalkAbleNodes.AddLast(position + Vector2Int.left);
                if (nearNodes[1] == null && !noneWalkAbleNodes.Contains(position + Vector2Int.right)) noneWalkAbleNodes.AddLast(position + Vector2Int.right);
                if (nearNodes[2] == null && !noneWalkAbleNodes.Contains(position + Vector2Int.down)) noneWalkAbleNodes.AddLast(position + Vector2Int.down);
                if (nearNodes[3] == null && !noneWalkAbleNodes.Contains(position + Vector2Int.up)) noneWalkAbleNodes.AddLast(position + Vector2Int.up);*/
    }
}
