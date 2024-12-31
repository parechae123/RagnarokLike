using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    MeshFilter meshFilter;
    public Vector3[] vertices = new Vector3[]
    {
        // �ո�
        new Vector3(0, 0, 0), // 0
        new Vector3(1, 0, 0), // 1
        new Vector3(1, 1, 0), // 2
        new Vector3(0, 1, 0), // 3

        // �޸�
        new Vector3(0, 0, 1), // 4
        new Vector3(1, 0, 1), // 5
        new Vector3(1, 1, 1), // 6
        new Vector3(0, 1, 1)  // 7
    };

    public int[] triangles = new int[]
    {
        // �ո�
        0, 2, 1,
        0, 3, 2,

        // �޸�
        4, 5, 6,
        4, 6, 7,

        // ����
        3, 7, 6,
        3, 6, 2,

        // �Ʒ���
        0, 1, 5,
        0, 5, 4,

        // ���ʸ�
        0, 4, 7,
        0, 7, 3,

        // �����ʸ�
        1, 2, 6,
        1, 6, 5
    };
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
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;
            
        }
    }
}
