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
        MeshFilter thisTile = new GameObject().AddComponent<MeshFilter>();
        thisTile.AddComponent<MeshRenderer>().material = isMoveableTile ? Resources.Load<Material>("WalkAbleTileMaterial") : Resources.Load<Material>("TileMaterial");
        // ���� ������ ���� ������Ʈ�� ��ġ ����
        thisTile.transform.position = new Vector3(nodeCenterPosition.x, floor, nodeCenterPosition.y);

        // Mesh ��ü ����
        Mesh mesh = new Mesh();
        // Mesh�� ����(vertex) ����
        mesh.vertices = new Vector3[4]
        {
        new Vector3(-0.5f, floor, -0.5f),   // ���� �Ʒ�
        new Vector3(-0.5f, floor, 0.5f),    // ���� ��
        new Vector3(0.5f, floor, -0.5f),    // ������ �Ʒ�
        new Vector3(0.5f, floor, 0.5f),      // ������ ��
        };
        // UV ��ǥ ����
        mesh.uv = new Vector2[4]
        {
            new Vector2(0, 0),   // ���� �Ʒ�
            new Vector2(0, 1),   // ���� ��
            new Vector2(1, 0),   // ������ �Ʒ�
            new Vector2(1, 1)   // ������ ��
        };

        // Mesh�� �ﰢ��(triangles) ����
        mesh.triangles = new int[6]
        {
            1, 3, 2,    // ù ��° �ﰢ�� (���� �Ʒ� -> ���� �� -> ������ �Ʒ�)
             1, 2, 0     // �� ��° �ﰢ�� (������ �Ʒ� -> ���� �� -> ������ ��)
        };

        // ������ Mesh�� MeshFilter�� �Ҵ��Ͽ� ������Ʈ�� �׸��� ����
        thisTile.mesh = mesh;
    } 
}
public class GridManager : Manager<GridManager>
{
    public Dictionary<Vector2Int, Node> grids = new Dictionary<Vector2Int, Node>();


    
}
