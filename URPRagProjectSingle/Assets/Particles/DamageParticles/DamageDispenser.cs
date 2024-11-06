using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class DamageDispenser : MonoBehaviour
{
    public SymbolsTextureData textureData;

    private ParticleSystemRenderer particleSystemRenderer;
    private new ParticleSystem particleSystem;
    private void Start()
    {
        UIManager.GetInstance().FontParticleRegist(SpawnParticle);
    }
    //미사용 함수
    /*    [ContextMenu("TestText")]

        public void TestText()
        {
            SpawnParticle(transform.position, "Hello world!", Color.red);
        }

        public void SpawnParticle(Vector3 position, float amount, Color color, float? startSize = null)
        {
            var amountInt = Mathf.RoundToInt(amount);
            if (amountInt == 0) return;
            var str = amountInt.ToString();
            if (amountInt > 0) str = "+" + str;
            SpawnParticle(position, str, color, startSize);
        }*/

    public void SpawnParticle(Vector3 position, string message, Color color, float? startSize = null)
    {
        var texCords = new Vector2[24]; // 24개의 요소를 가진 배열 - 23개의 문자 + 메시지 길이
        var messageLenght = Mathf.Min(23, message.Length);
        texCords[texCords.Length - 1] = new Vector2(0, messageLenght);
        for (int i = 0; i < texCords.Length; i++)
        {
            if (i >= messageLenght) break;
            // SymbolsTextureData에서 GetTextureCoordinates() 메서드를 호출하여 문자 위치를 가져옴
            texCords[i] = textureData.GetTextureCoordinates(message[i]);
        }

        var custom1Data = CreateCustomData(texCords);
        var custom2Data = CreateCustomData(texCords, 12);

        // ParticleSystem에 대한 참조를 캐시
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();

        if (particleSystemRenderer == null)
        {
            // ParticleSystemRenderer에 대한 참조를 캐시하고 필요한 스트림이 있는지 확인
            particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            var streams = new List<ParticleSystemVertexStream>();
            particleSystemRenderer.GetActiveVertexStreams(streams);
            // UV2, SizeXY 등의 추가 스트림을 추가하여 스크립트의 좌표가 셰이더의 좌표와 일치하도록 함
            if (!streams.Contains(ParticleSystemVertexStream.UV2)) streams.Add(ParticleSystemVertexStream.UV2);
            if (!streams.Contains(ParticleSystemVertexStream.Custom1XYZW)) streams.Add(ParticleSystemVertexStream.Custom1XYZW);
            if (!streams.Contains(ParticleSystemVertexStream.Custom2XYZW)) streams.Add(ParticleSystemVertexStream.Custom2XYZW);
            particleSystemRenderer.SetActiveVertexStreams(streams);
        }

        // 이미션 매개변수 초기화
        // 색상과 위치는 메서드의 매개변수에서 가져옴
        // 메시지 길이가 변할 때 문자가 늘어나거나 줄어들지 않도록 startSize3D의 X를 설정
        var emitParams = new ParticleSystem.EmitParams
        {
            startColor = color,
            position = position,
            applyShapeToPosition = true,
            startSize3D = new Vector3(messageLenght, 1, 1)
        };
        // 서로 다른 크기의 파티클을 생성하려면 SpawnParticle의 매개변수에서 startSize 값을 전달해야 함
        if (startSize.HasValue) emitParams.startSize3D *= startSize.Value * particleSystem.main.startSizeMultiplier;
        // 파티클 스폰
        particleSystem.Emit(emitParams, 1);

        // 커스텀 데이터를 원하는 스트림에 전달
        var customData = new List<Vector4>();
        // ParticleSystem에서 ParticleSystemCustomData.Custom1 스트림을 가져옴
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        // 방금 생성한 파티클의 데이터를 변경
        customData[customData.Count - 1] = custom1Data;
        // ParticleSystem에 데이터 반환
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        // ParticleSystemCustomData.Custom2에 대해 동일하게 처리
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        customData[customData.Count - 1] = custom2Data;
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
    }

    // 문자 좌표를 float로 패킹하는 함수
    public float PackFloat(Vector2[] vecs)
    {
        if (vecs == null || vecs.Length == 0) return 0;
        // 벡터 좌표 값을 비트별로 float에 추가
        var result = vecs[0].y * 10000 + vecs[0].x * 100000;
        if (vecs.Length > 1) result += vecs[1].y * 100 + vecs[1].x * 1000;
        if (vecs.Length > 2) result += vecs[2].y + vecs[2].x * 10;
        return result;
    }

    // CustomData 스트림용 Vector4 생성 함수
    private Vector4 CreateCustomData(Vector2[] texCoords, int offset = 0)
    {
        var data = Vector4.zero;
        for (int i = 0; i < 4; i++)
        {
            var vecs = new Vector2[3];
            for (int j = 0; j < 3; j++)
            {
                var ind = i * 3 + j + offset;
                if (texCoords.Length > ind)
                {
                    vecs[j] = texCoords[ind];
                }
                else
                {
                    data[i] = PackFloat(vecs);
                    i = 5;
                    break;
                }
            }
            if (i < 4) data[i] = PackFloat(vecs);
        }
        return data;
    }
    private Vector2 CreateCustomSprite()
    {
        return Vector2.zero;
    }
    
}
