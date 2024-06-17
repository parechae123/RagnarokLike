using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

public class Node
{
    public Vector2Int nodeCenterPosition;
    public bool isMoveableTile;
    public Node connectedNode { get; private set; }
    //현재 노드로부터 타겟노드까지의 거리
    float H;
    //현재 노드로부터 시작노드까지의 거리
    float G;
    //두 거리를 합친 값
    float F { get { return H + G; } }
    public void SetConnection(Node node) => connectedNode = node;
    public void SetG(float g) => G = g;
    public void SetH(float h) => H = h;
    public void init(byte floor , Vector2Int vec,bool isMoveable)
    {
        SetNodePos(vec,isMoveable);
        CreateTile(floor);
    }
    private void SetNodePos(Vector2Int nodePos, bool isMoveable)
    {
        nodeCenterPosition = nodePos;
        isMoveableTile = isMoveable;
    }
    private void CreateTile(byte floor)
    {
        // MeshFilter 컴포넌트를 가진 새로운 게임 오브젝트 생성
        MeshFilter thisTile = new GameObject().AddComponent<MeshFilter>();
        thisTile.AddComponent<MeshRenderer>().material = isMoveableTile ? Resources.Load<Material>("WalkAbleTileMaterial") : Resources.Load<Material>("TileMaterial");
        // 새로 생성한 게임 오브젝트의 위치 설정
        thisTile.transform.position = new Vector3(nodeCenterPosition.x, floor, nodeCenterPosition.y);

        // Mesh 객체 생성
        Mesh mesh = new Mesh();
        // Mesh의 정점(vertex) 설정
        mesh.vertices = new Vector3[4]
        {
        new Vector3(-0.5f, floor, -0.5f),   // 왼쪽 아래
        new Vector3(-0.5f, floor, 0.5f),    // 왼쪽 위
        new Vector3(0.5f, floor, -0.5f),    // 오른쪽 아래
        new Vector3(0.5f, floor, 0.5f),      // 오른쪽 위
        };
        // UV 좌표 설정
        mesh.uv = new Vector2[4]
        {
            new Vector2(0, 0),   // 왼쪽 아래
            new Vector2(0, 1),   // 왼쪽 위
            new Vector2(1, 0),   // 오른쪽 아래
            new Vector2(1, 1)   // 오른쪽 위
        };

        // Mesh의 삼각형(triangles) 설정
        mesh.triangles = new int[6]
        {
            1, 3, 2,    // 첫 번째 삼각형 (왼쪽 아래 -> 왼쪽 위 -> 오른쪽 아래)
             1, 2, 0     // 두 번째 삼각형 (오른쪽 아래 -> 왼쪽 위 -> 오른쪽 위)
        };

        // 생성한 Mesh를 MeshFilter에 할당하여 오브젝트에 그림을 입힘
        thisTile.mesh = mesh;
    } 
}
public class GridManager : Manager<GridManager>
{
    public Dictionary<Vector2Int, Node> grids = new Dictionary<Vector2Int, Node>();


    
}
