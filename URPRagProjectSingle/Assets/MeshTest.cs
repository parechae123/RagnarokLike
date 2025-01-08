using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    MeshFilter meshFilter;
    public Vector3[] vertices = new Vector3[]
    {
        // 윗면만 만들어주면 됨
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
            //i 는 정사각형의 갯수
            for (int i = 0; i < centerPositions.Length; i++)
            {
                //j vertices 의 갯수만큼 정점을 찍는다
                for (int j = 0; j < vertices.Length; j++)
                {
                    //정점수집
                    currTriangles.Enqueue(boundaryVert.Count);
                    boundaryVert.Add(vertices[j] + centerPositions[i]);
                    //삼각형 두개를 수립시켜야함
                }
                //내려가면서 생각해보자

            }
            mesh.vertices = boundaryVert.ToArray<Vector3>();
            mesh.triangles = currTriangles.ToArray();
            meshFilter.mesh = mesh;
            
        }
    }
}
