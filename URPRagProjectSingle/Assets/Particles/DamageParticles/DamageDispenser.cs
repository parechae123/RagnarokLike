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
    //�̻�� �Լ�
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
        var texCords = new Vector2[24]; // 24���� ��Ҹ� ���� �迭 - 23���� ���� + �޽��� ����
        var messageLenght = Mathf.Min(23, message.Length);
        texCords[texCords.Length - 1] = new Vector2(0, messageLenght);
        for (int i = 0; i < texCords.Length; i++)
        {
            if (i >= messageLenght) break;
            // SymbolsTextureData���� GetTextureCoordinates() �޼��带 ȣ���Ͽ� ���� ��ġ�� ������
            texCords[i] = textureData.GetTextureCoordinates(message[i]);
        }

        var custom1Data = CreateCustomData(texCords);
        var custom2Data = CreateCustomData(texCords, 12);

        // ParticleSystem�� ���� ������ ĳ��
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();

        if (particleSystemRenderer == null)
        {
            // ParticleSystemRenderer�� ���� ������ ĳ���ϰ� �ʿ��� ��Ʈ���� �ִ��� Ȯ��
            particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            var streams = new List<ParticleSystemVertexStream>();
            particleSystemRenderer.GetActiveVertexStreams(streams);
            // UV2, SizeXY ���� �߰� ��Ʈ���� �߰��Ͽ� ��ũ��Ʈ�� ��ǥ�� ���̴��� ��ǥ�� ��ġ�ϵ��� ��
            if (!streams.Contains(ParticleSystemVertexStream.UV2)) streams.Add(ParticleSystemVertexStream.UV2);
            if (!streams.Contains(ParticleSystemVertexStream.Custom1XYZW)) streams.Add(ParticleSystemVertexStream.Custom1XYZW);
            if (!streams.Contains(ParticleSystemVertexStream.Custom2XYZW)) streams.Add(ParticleSystemVertexStream.Custom2XYZW);
            particleSystemRenderer.SetActiveVertexStreams(streams);
        }

        // �̹̼� �Ű����� �ʱ�ȭ
        // ����� ��ġ�� �޼����� �Ű��������� ������
        // �޽��� ���̰� ���� �� ���ڰ� �þ�ų� �پ���� �ʵ��� startSize3D�� X�� ����
        var emitParams = new ParticleSystem.EmitParams
        {
            startColor = color,
            position = position,
            applyShapeToPosition = true,
            startSize3D = new Vector3(messageLenght, 1, 1)
        };
        // ���� �ٸ� ũ���� ��ƼŬ�� �����Ϸ��� SpawnParticle�� �Ű��������� startSize ���� �����ؾ� ��
        if (startSize.HasValue) emitParams.startSize3D *= startSize.Value * particleSystem.main.startSizeMultiplier;
        // ��ƼŬ ����
        particleSystem.Emit(emitParams, 1);

        // Ŀ���� �����͸� ���ϴ� ��Ʈ���� ����
        var customData = new List<Vector4>();
        // ParticleSystem���� ParticleSystemCustomData.Custom1 ��Ʈ���� ������
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        // ��� ������ ��ƼŬ�� �����͸� ����
        customData[customData.Count - 1] = custom1Data;
        // ParticleSystem�� ������ ��ȯ
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        // ParticleSystemCustomData.Custom2�� ���� �����ϰ� ó��
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        customData[customData.Count - 1] = custom2Data;
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
    }

    // ���� ��ǥ�� float�� ��ŷ�ϴ� �Լ�
    public float PackFloat(Vector2[] vecs)
    {
        if (vecs == null || vecs.Length == 0) return 0;
        // ���� ��ǥ ���� ��Ʈ���� float�� �߰�
        var result = vecs[0].y * 10000 + vecs[0].x * 100000;
        if (vecs.Length > 1) result += vecs[1].y * 100 + vecs[1].x * 1000;
        if (vecs.Length > 2) result += vecs[2].y + vecs[2].x * 10;
        return result;
    }

    // CustomData ��Ʈ���� Vector4 ���� �Լ�
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
