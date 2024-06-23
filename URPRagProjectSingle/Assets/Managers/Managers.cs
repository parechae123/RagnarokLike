using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    public static T GetInstance ()
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
    public Node(sbyte floor, Vector2Int vec, bool isMoveable)
    {
        nodeFloor = floor;
        nodeCenterPosition = vec;
        isMoveableTile = isMoveable;
    }
    public Node connectedNode { get; private set; }
    //현재 노드로부터 타겟노드까지의 거리
    int H;
    //현재 노드로부터 시작노드까지의 거리
    int G;
    //두 거리를 합친 값
    /// <summary>
    /// 토탈코스트
    /// </summary>
    int F { get { return H + G; } }

    public void SetGH(Vector2Int startPos,Vector2Int endPos)
    {
        G = GetDistance(startPos);
        H = GetDistance(endPos);
    }


    public int GetDistance(Vector2Int targetPos)
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
        Debug.Log(Mathf.RoundToInt(position.x)+","+ Mathf.RoundToInt(position.z));
        Vector2Int tempIntPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        return grids.ContainsKey(tempIntPos)? grids[new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z))]: null;
    }

    /// <summary>
    /// 코스트와 휴리스틱을 비교하여 최적의 경로를 찾음
    /// </summary>
    /// <param name="startPos">시작노드 위치</param>
    /// <param name="endPos">이동목표 노드 위치</param>
    /// <returns></returns>
    public Node[] PathFinding(Vector2Int startPos, Vector2Int endPos)
    {
        Node[] tempNode = new Node[0];
        HashSet<Vector2Int> closedNodePosition = new HashSet<Vector2Int>();
        

        return tempNode;
    }
    /// <summary>
    /// 가까운 Node를 반환받는 함수
    /// </summary>
    /// <param name="position">기준점</param>
    /// <returns>좌,우,하,상 순서</returns>
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
