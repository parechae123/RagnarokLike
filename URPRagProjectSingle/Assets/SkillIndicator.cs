using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillIndicators
{
    public SkillProjector boundProjector = new SkillProjector(Color.cyan);
    public SkillProjector rangeProjector = new SkillProjector(Color.green);
    /// <summary>
    /// rangeProjector인지 boundProjector인지 판별
    /// </summary>
    /// <param name="isRangeProjector"></param>
    public void SetBoundary(bool isRangeProjector,Vector3[] boundary)
    {
        switch (isRangeProjector)
        {
            case true:
                rangeProjector.SetBoundary(boundary);
                break;
            case false:
                boundProjector.SetBoundary(boundary);
                break;
        }
    }
    public void SetPosition(bool isRangeProjector, Vector3 pos)
    {
        switch (isRangeProjector)
        {
            case true:
                rangeProjector.tr.position = pos;
                break;
            case false:
                boundProjector.tr.position = pos;
                break;
        }
    }
    public void OnOff(bool isRangeProjector, bool onOff)
    {
        switch (isRangeProjector)
        {
            case true:
                rangeProjector.tr.gameObject.SetActive(onOff);
                break;
            case false:
                boundProjector.tr.gameObject.SetActive(onOff);
                break;
        }
    }

}
public class SkillProjector
{
    public Transform tr;
    MeshFilter meshFilter;

    public SkillProjector(Color effColor)
    {
        GameObject indicatorOBJ = new GameObject("SkillIndicator");
        meshFilter = indicatorOBJ.AddComponent<MeshFilter>();
        tr = indicatorOBJ.transform;
        tr.localPosition = Vector3.zero;
        effColor.a = 0.2f;
        ResourceManager.GetInstance().LoadAsync<Material>("HLTest", (mat) => 
        {
            MeshRenderer tempMR = tr.gameObject.AddComponent<MeshRenderer>();
            tempMR.material = mat;
            tempMR.material.SetColor("_Color", effColor);
            
        }
        );
    }
    private void OnOff(bool isOn)
    {
        tr.gameObject.SetActive(isOn);
    }
    private Vector3[] vertices = new Vector3[]
    {
        // 윗면만 만들어주면 됨
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
        //i 는 정사각형의 갯수
        for (int i = 0; i < centerPos.Length; i++)
        {
            //j 미리 선언해둔 vertices 의 갯수만큼 정점을 찍는다
            for (int j = 0; j < vertices.Length; j++)
            {
                //정점수집
                if (vertDict.ContainsKey(vertices[j] + centerPos[i]))
                {
                    currTriangles.Enqueue(vertDict[vertices[j] + centerPos[i]]);
                }
                else
                {
                    currTriangles.Enqueue(vertDict.Count);
                    vertDict.Add(vertices[j] + centerPos[i], vertDict.Count);
                }
            }

        }
        mesh.vertices = vertDict.Keys.ToArray();
        mesh.triangles = currTriangles.ToArray();
        meshFilter.mesh = mesh;
    }
}
