using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillIndicator
{
    public Transform tr;
    MeshFilter meshFilter;

    public SkillIndicator()
    {
        GameObject indicatorOBJ = new GameObject("SkillIndicator");
        meshFilter = indicatorOBJ.AddComponent<MeshFilter>();
        tr = indicatorOBJ.transform;
        ResourceManager.GetInstance().LoadAsync<Material>("HLTest", (mat) => { tr.gameObject.AddComponent<MeshRenderer>().material = mat; });
    }
    public void OnOff(bool isOn)
    {
        tr.gameObject.SetActive(isOn);
    }
    private Vector3[] vertices = new Vector3[]
    {
        // ���鸸 ������ָ� ��
        new Vector3(0.5f , -1.1f , -0.5f),
        new Vector3(-0.5f , -1.1f, -0.5f),
        new Vector3(-0.5f , -1.1f, 0.5f),

        new Vector3(0.5f , -1.1f, 0.5f),
        new Vector3(0.5f , -1.1f, -0.5f),
        new Vector3(-0.5f , -1.1f, 0.5f)
    };
    public void SetBoundary(Vector3[] centerPos)
    {
        OnOff(true);
        Mesh mesh = new Mesh();
        Dictionary<Vector3, int> vertDict = new Dictionary<Vector3, int>(256);
        Queue<int> currTriangles = new Queue<int>();
        //i �� ���簢���� ����
        for (int i = 0; i < centerPos.Length; i++)
        {
            //j vertices �� ������ŭ ������ ��´�
            for (int j = 0; j < vertices.Length; j++)
            {
                //��������
                if (vertDict.ContainsKey(vertices[j] + centerPos[i]))
                {
                    currTriangles.Enqueue(vertDict[vertices[j] + centerPos[i]]);
                }
                else
                {
                    currTriangles.Enqueue(vertDict.Count);
                    vertDict.Add(vertices[j] + centerPos[i], vertDict.Count);
                }


                //�ﰢ�� �ΰ��� �������Ѿ���
            }

        }
        mesh.vertices = vertDict.Keys.ToArray();
        mesh.triangles = currTriangles.ToArray();
        meshFilter.mesh = mesh;
    }
}
