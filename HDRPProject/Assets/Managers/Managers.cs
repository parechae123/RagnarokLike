using System;
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
        //동적생성 맵을 원한다면 batch Static되어있는 오브젝트로 사용해야함, GPU 인스턴싱 Batch때문임
        GameObject tempGOBJ = GameObject.Instantiate(Resources.Load<GameObject>("WalkAbleTile"));
/*        tempGOBJ.GetComponent<MeshRenderer>().sharedMaterial = isMoveableTile ? GridManager.GetInstance().WalkableMaterial : GridManager.GetInstance().NoneWalkableMaterial;
        tempGOBJ.GetComponent<MeshFilter>().sharedMesh = GridManager.GetInstance().TileMesh;*/
        tempGOBJ.transform.position = new Vector3(nodeCenterPosition.x,floor,nodeCenterPosition.y);
        tempGOBJ.name = nodeCenterPosition.ToString();
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




}
