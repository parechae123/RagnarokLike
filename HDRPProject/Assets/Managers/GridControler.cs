using UnityEngine;
using UnityEngine.UIElements;

public class GridControler : MonoBehaviour
{
    public Texture2D noiseTexture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int textureWidth = 100;
        int textureHeight = 100;
        noiseTexture = new Texture2D(textureWidth, textureHeight);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth;
                float yCoord = (float)y / textureHeight;

                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(noiseValue, noiseValue, noiseValue); // 그레이스케일 텍스처
                GridManager.GetInstance().grids.Add(new Vector2Int(x, y), new Node());
                GridManager.GetInstance().grids[new Vector2Int(x, y)].init(0, new Vector2Int(x, y), noiseValue < 0.5f ? true:false);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
