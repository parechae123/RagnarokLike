using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

public class Node
{
    public Vector2Int nodeCenterPosition;
    public bool isMoveableTile;
    public Node connectedNode { get; private set; }
    //���� ���κ��� Ÿ�ٳ������� �Ÿ�
    float H;
    //���� ���κ��� ���۳������� �Ÿ�
    float G;
    //�� �Ÿ��� ��ģ ��
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
        // MeshFilter ������Ʈ�� ���� ���ο� ���� ������Ʈ ����
        //�������� ���� ���Ѵٸ� batch Static�Ǿ��ִ� ������Ʈ�� ����ؾ���, GPU �ν��Ͻ� Batch������
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
