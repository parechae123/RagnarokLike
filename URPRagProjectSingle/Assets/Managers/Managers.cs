using PlayerDefines;
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
/// 메니저 템플릿화
/// </summary>
/// <typeparam name="T">해당 메니저 클래스명</typeparam>
public class Manager<T> where T : new()
{
    protected static T instance;
    /// <summary>
    /// 인스턴스를 가져옵니다.
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
    /// 매니저를 기본값으로 설정하여 설정을 해제합니다.
    /// </summary>
    public static void Release()
    {

        instance = default(T);
    }
}
[System.Serializable]
public class Node
{
    public Vector2Int nodeCenterPosition;       //노드의 중앙 좌표값
    public bool isMoveableTile;                 //이동가능 타일인지
    public sbyte nodeFloor;                     //노드의 층(1층 = y1,-1층 = y-1
    private Stats characterOnNode;
    public bool isEmptyNode(Stats stat)
    {
        if (characterOnNode == null)
        {
            return true;
        }
        return false;
    }
    public void nodeCharacterUpdate(Stats stat)
    {
        characterOnNode = stat;
    }
    public Node(sbyte floor, Vector2Int vec, bool isMoveable)
    {
        nodeFloor = floor;
        nodeCenterPosition = vec;
        isMoveableTile = isMoveable;
    }
    public Node connectedNode;
    //현재 노드로부터 타겟노드까지의 거리
    public int H { get; private set; }
    //현재 노드로부터 시작노드까지의 거리
    public int G { get; private set; }
    //두 거리를 합친 값
    /// <summary>
    /// 토탈코스트
    /// </summary>
    public int F { get { return H + G; } }

    public void SetGH(Vector2Int startPos, Vector2Int endPos)
    {
        G = GetDistance(startPos);
        H = GetDistance(endPos);
    }


    private int GetDistance(Vector2Int targetPos)
    {
        // 두 점 사이의 x와 y 좌표 차이 계산
        Vector2Int dist = new Vector2Int(Mathf.Abs(nodeCenterPosition.x - targetPos.x), Mathf.Abs(nodeCenterPosition.y - targetPos.y));

        // 맨해튼 거리 계산 (두 점 사이의 수평 및 수직 거리 합)
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
    /// vector3를 vector2int로 형변환(반올림)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Node PositionToNode(Vector3 position)
    {
        Vector2Int tempIntPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        return grids.ContainsKey(tempIntPos) ? grids[new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z))] : null;
    }

    /// <summary>
    /// 코스트와 휴리스틱을 비교하여 최적의 경로를 찾음
    /// </summary>
    /// <param name="startPos">시작노드 위치</param>
    /// <param name="endPos">이동목표 노드 위치</param>
    /// <returns></returns>
    /// 
    public LinkedList<Node> PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        //열린노드 : 길이 될 수 있는(닫힌노드의 주변)
        //닫힌노드 : 열린 노드 중 가장 낮은 f비용의 노드,닫힌노드로 포함 될 시 열린노드에서 제함
        LinkedList<Node> closedNodes = new LinkedList<Node>();
        if (!grids.ContainsKey(endPos))
        {
            closedNodes.Clear();
            return closedNodes;
        }

        if (startPos == endPos) 
        {
            closedNodes.Clear();
            return closedNodes;
        } 

        LinkedList<Node> openNodes = new LinkedList<Node>();
        bool isSearchDone = false;


        closedNodes.AddLast(grids[startPos]);
        grids[startPos].SetGH(startPos, endPos);
        short whileCounterMax = (short)1000;
        while (!isSearchDone)
        {
            GetNearOpenNodes(ref openNodes, closedNodes.Last().nodeCenterPosition, closedNodes);
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
            if (whileCounterMax <= 0) isSearchDone = true;
        }
        if (closedNodes.Last() != grids[endPos] || closedNodes.First() != grids[startPos])
        {
            return new LinkedList<Node>();
        }

        isSearchDone = false;
        whileCounterMax = (short)1000;
        LinkedList<Node> doubleCheckedNodes = new LinkedList<Node>();
        doubleCheckedNodes.AddFirst(closedNodes.Last());
        while (!isSearchDone)
        {
            GetNearClosedNodes(ref doubleCheckedNodes, doubleCheckedNodes.First().nodeCenterPosition, closedNodes);
            if (doubleCheckedNodes.First() == grids[startPos] || whileCounterMax <=0)
            {
                isSearchDone =true;
            }
            whileCounterMax--;
        }
        if (doubleCheckedNodes.Last() != grids[endPos]|| doubleCheckedNodes.First() != grids[startPos])
        {
            return new LinkedList<Node>();
        }
        return doubleCheckedNodes;

    }
    /*    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
        {
            //열린노드 : 길이 될 수 있는(닫힌노드의 주변)
            //닫힌노드 : 열린 노드 중 가장 낮은 f비용의 노드,닫힌노드로 포함 될 시 열린노드에서 제함
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
            //예외처리
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
    /// 가까운 Node를 Add해주는 함수 (좌,우,하,상 순서)
    /// </summary>
    /// <param name="nearNodes">add할 linkedList<Node>    </param>
    /// <param name="position">기준점</param>
    /// <param name="closedNodes">닫힌노드,비교를 위해 필요</param>
    public void GetNearOpenNodes(ref LinkedList<Node> nearNodes, Vector2Int position/*,ref LinkedList<Vector2Int> noneWalkAbleNodes*/, LinkedList<Node> closedNodes)
    {
        //해당 가까운 노드를 등록
        Vector2Int[] tempN = NearPositions(position);
        for (sbyte i = 0; i < tempN.Length; i++)
        {
            if (grids.ContainsKey(tempN[i]))
            {
                if (grids[tempN[i]].isMoveableTile && !closedNodes.Contains(grids[tempN[i]])) nearNodes.AddLast(grids[tempN[i]]);
            }
        }
    }
    public void GetNearClosedNodes(ref LinkedList<Node> reversedClosedNode, Vector2Int position, LinkedList<Node> closedNodes)
    {
        //해당 좌표에 노드가 없거나 wallkable이 아니면 해당 배열은 null로 바꿔줌
        int lowerstGValue = int.MaxValue;
        Node tempNode = null;
        Vector2Int[] nearPositions = NearPositions(position);
        for (sbyte i = 0; i < nearPositions.Length; i++)
        {
            if (closedNodes.Contains(grids[nearPositions[i]]))
            {
                if (lowerstGValue > grids[nearPositions[i]].G&&!reversedClosedNode.Contains(grids[nearPositions[i]]))
                {
                    lowerstGValue = grids[nearPositions[i]].G;
                    tempNode = grids[nearPositions[i]];
                }
            }
        }
        if (tempNode != null)
        {
            grids[position].connectedNode = tempNode;
            reversedClosedNode.AddFirst(tempNode);
        }

    }
    /// <summary>
    /// 가까운 노드의 존재여부를 판단
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2Int[] NearPositions(Vector2Int position)
    {
        Vector2Int[] tempVec = new Vector2Int[0];

        if (grids.ContainsKey(position + Vector2Int.left))
        {
            if (grids[position + Vector2Int.left].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.left;
            }
        }

        if (grids.ContainsKey(position + Vector2Int.right))
        {
            if (grids[position + Vector2Int.right].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.right;
            }

        }

        if (grids.ContainsKey(position + Vector2Int.down))
        {
            if (grids[position + Vector2Int.down].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.down;
            }

        }

        if (grids.ContainsKey(position + Vector2Int.up))
        {
            if (grids[position+Vector2Int.up].isMoveableTile)
            {
                Array.Resize(ref tempVec, tempVec.Length + 1);
                tempVec[tempVec.Length - 1] = position + Vector2Int.up;
            }
        }
        return tempVec;
    }
}
