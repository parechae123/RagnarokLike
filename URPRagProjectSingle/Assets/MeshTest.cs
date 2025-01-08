using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    MeshFilter meshFilter;
    public Vector3[] vertices = new Vector3[]
    {
        // ���鸸 ������ָ� ��
        new Vector3(-0.5f , 0 , -0.5f),
        new Vector3(0.5f , 0 , -0.5f),
        new Vector3(-0.5f , 0 , 0.5f),
        new Vector3(-0.5f , 0 , -0.5f),

        new Vector3(0.5f , 0 , 0.5f),
        new Vector3(0.5f , 0 , -0.5f),
        new Vector3(-0.5f , 0 , 0.5f),
        new Vector3(0.5f , 0 , 0.5f),
    };
    public Vector3[] centerPositions;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Mesh mesh = new Mesh();
            HashSet<Vector3> boundaryVert = new HashSet<Vector3>();
            Queue<int> currTriangles = new Queue<int>();
            //i �� ���簢���� ����
            for (int i = 0; i < centerPositions.Length; i++)
            {
                //j vertices �� ������ŭ ������ ��´�
                for (int j = 0; j < vertices.Length; j++)
                {
                    //��������
                    currTriangles.Enqueue(boundaryVert.Count);
                    boundaryVert.Add(vertices[j] + centerPositions[i]);
                    //�ﰢ�� �ΰ��� �������Ѿ���
                }
                //�������鼭 �����غ���

            }
            mesh.vertices = boundaryVert.ToArray<Vector3>();
            mesh.triangles = currTriangles.ToArray();
            meshFilter.mesh = mesh;
            
        }
    }
}
