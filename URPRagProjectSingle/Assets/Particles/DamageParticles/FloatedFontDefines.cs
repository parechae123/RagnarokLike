using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SymbolsTextureData
{
    // 폰트 아틀라스에 대한 참조
    public Texture texture;

    // 왼쪽 상단부터 시작하여 순서대로 정렬된 문자 배열
    public char[] chars;

    // 각 문자의 좌표(행과 열 번호)를 저장하는 Dictionary
    private Dictionary<char, Vector2> charsDict;

    public void Initialize()
    {
        charsDict = new Dictionary<char, Vector2>();
        for (int i = 0; i < chars.Length; i++)
        {
            var c = char.ToLowerInvariant(chars[i]);
            if (charsDict.ContainsKey(c)) continue;
            // 문자의 좌표 계산. 문자 순서 번호를
            // 행과 열 번호로 변환 (한 행의 길이는 10으로 가정).
            var uv = new Vector2(i % 10, 9 - i / 10);
            charsDict.Add(c, uv);
        }
    }

    public Vector2 GetTextureCoordinates(char c)
    {
        c = char.ToLowerInvariant(c);
        if (charsDict == null) Initialize();

        if (charsDict.TryGetValue(c, out Vector2 texCoord))
            return texCoord;
        return Vector2.zero;
    }
}
