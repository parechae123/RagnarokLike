using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class DamageDispenser : MonoBehaviour
{
    public SymbolsTextureData textureData;

    private ParticleSystemRenderer particleSystemRenderer;
    private new ParticleSystem particleSystem;
    public Sprite dd;
    [ContextMenu("TestText")]

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
    }

    public void SpawnParticle(Vector3 position, string message, Color color, float? startSize = null)
    {
        var texCords = new Vector2[24]; //¬Þ¬Ñ¬ã¬ã¬Ú¬Ó ¬Ú¬Ù 24 ¬ï¬Ý¬Ö¬Þ¬Ö¬ß¬ä - 23 ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬Ñ + ¬Õ¬Ý¬Ú¬ß¬Ñ ¬ã¬à¬à¬Ò¬ë¬Ö¬ß¬Ú¬ñ
        var messageLenght = Mathf.Min(23, message.Length);
        texCords[texCords.Length - 1] = new Vector2(0, messageLenght);
        for (int i = 0; i < texCords.Length; i++)
        {
            if (i >= messageLenght) break;
            //¬£¬í¬Ù¬í¬Ó¬Ñ¬Ö¬Þ ¬Þ¬Ö¬ä¬à¬Õ GetTextureCoordinates() ¬Ú¬Ù SymbolsTextureData ¬Õ¬Ý¬ñ ¬á¬à¬Ý¬å¬é¬Ö¬ß¬Ú¬ñ ¬á¬à¬Ù¬Ú¬è¬Ú¬Ú ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬Ñ
            texCords[i] = textureData.GetTextureCoordinates(message[i]);
        }

        var custom1Data = CreateCustomData(texCords);
        var custom2Data = CreateCustomData(texCords, 12);
        var custom3Data = CreateCustomData(texCords, 12);

        //¬¬¬ï¬ê¬Ú¬â¬å¬Ö¬Þ ¬ã¬ã¬í¬Ý¬Ü¬å ¬ß¬Ñ ParticleSystem
        if (particleSystem == null) particleSystem = GetComponent<ParticleSystem>();

        if (particleSystemRenderer == null)
        {
            //¬¦¬ã¬Ý¬Ú ¬ã¬ã¬í¬Ý¬Ü¬Ñ ¬ß¬Ñ ParticleSystemRenderer, ¬Ü¬ï¬ê¬Ú¬â¬å¬Ö¬Þ ¬Ú ¬å¬Ò¬Ö¬Ø¬Õ¬Ñ¬Ö¬Þ¬ã¬ñ ¬Ó ¬ß¬Ñ¬Ý¬Ú¬é¬Ú¬Ú ¬ß¬å¬Ø¬ß¬í¬ç ¬á¬à¬ä¬à¬Ü¬à¬Ó
            particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            var streams = new List<ParticleSystemVertexStream>();
            particleSystemRenderer.GetActiveVertexStreams(streams);
            //¬¥¬à¬Ò¬Ñ¬Ó¬Ý¬ñ¬Ö¬Þ ¬Ý¬Ú¬ê¬ß¬Ú¬Û ¬á¬à¬ä¬à¬Ü Vector2(UV2, SizeXY, etc.), ¬é¬ä¬à¬Ò¬í ¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬í ¬Ó ¬ã¬Ü¬â¬Ú¬á¬ä¬Ö ¬ã¬à¬à¬ä¬Ó¬Ö¬ä¬ã¬ä¬Ó¬à¬Ó¬Ñ¬Ý¬Ú ¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬Ñ¬Þ ¬Ó ¬ê¬Ö¬Û¬Õ¬Ö¬â¬Ö
            if (!streams.Contains(ParticleSystemVertexStream.UV2)) streams.Add(ParticleSystemVertexStream.UV2);
            if (!streams.Contains(ParticleSystemVertexStream.Custom1XYZW)) streams.Add(ParticleSystemVertexStream.Custom1XYZW);
            if (!streams.Contains(ParticleSystemVertexStream.Custom2XYZW)) streams.Add(ParticleSystemVertexStream.Custom2XYZW);
            particleSystemRenderer.SetActiveVertexStreams(streams);
        }

        //¬ª¬ß¬Ú¬è¬Ú¬Ñ¬Ý¬Ú¬Ù¬Ú¬â¬å¬Ö¬Þ ¬á¬Ñ¬â¬Ñ¬Þ¬Ö¬ä¬â¬í ¬ï¬Þ¬Þ¬Ú¬ê¬Ö¬ß¬Ñ
        //¬¸¬Ó¬Ö¬ä ¬Ú ¬á¬à¬Ù¬Ú¬è¬Ú¬ð ¬á¬à¬Ý¬å¬é¬Ñ¬Ö¬Þ ¬Ú¬Ù ¬á¬Ñ¬â¬Ñ¬Þ¬Ö¬ä¬â¬à¬Ó ¬Þ¬Ö¬ä¬à¬Õ¬Ñ
        //¬µ¬ã¬ä¬Ñ¬ß¬Ñ¬Ó¬Ý¬Ú¬Ó¬Ñ¬Ö¬Þ startSize3D ¬á¬à X, ¬é¬ä¬à¬Ò¬í ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬í ¬ß¬Ö ¬â¬Ñ¬ã¬ä¬ñ¬Ô¬Ú¬Ó¬Ñ¬Ý¬Ú¬ã¬î ¬Ú ¬ß¬Ö ¬ã¬Ø¬Ú¬Þ¬Ñ¬Ý¬Ú¬ã¬î
        //¬á¬â¬Ú ¬Ú¬Ù¬Þ¬Ö¬ß¬Ö¬ß¬Ú¬Ú ¬Õ¬Ý¬Ú¬ß¬í ¬ã¬à¬à¬Ò¬ë¬Ö¬ß¬Ú¬ñ
        var emitParams = new ParticleSystem.EmitParams
        {
            startColor = color,
            position = position,
            applyShapeToPosition = true,
            startSize3D = new Vector3(messageLenght, 1, 1)
        };
        //¬¦¬ã¬Ý¬Ú ¬Þ¬í ¬ç¬à¬ä¬Ú¬Þ ¬ã¬à¬Ù¬Õ¬Ñ¬Ó¬Ñ¬ä¬î ¬é¬Ñ¬ã¬ä¬Ú¬è¬í ¬â¬Ñ¬Ù¬ß¬à¬Ô¬à ¬â¬Ñ¬Ù¬Þ¬Ö¬â¬Ñ, ¬ä¬à ¬Ó ¬á¬Ñ¬â¬Ñ¬Þ¬Ö¬ä¬â¬Ñ¬ç SpawnParticle ¬ß¬Ö¬à¬ç¬à¬Õ¬Ú¬Þ¬à
        //¬á¬Ö¬â¬Ö¬Õ¬Ñ¬ä¬î ¬ß¬å¬Ø¬ß¬à¬Ö ¬Ù¬ß¬Ñ¬é¬Ö¬ß¬Ú¬Ö startSize
        if (startSize.HasValue) emitParams.startSize3D *= startSize.Value * particleSystem.main.startSizeMultiplier;
        //¬¯¬Ö¬á¬à¬ã¬â¬Ö¬Õ¬ã¬ä¬Ó¬Ö¬ß¬ß¬à ¬ã¬á¬Ñ¬å¬ß ¬é¬Ñ¬ã¬ä¬Ú¬è¬í
        particleSystem.Emit(emitParams, 1);

        //¬±¬Ö¬â¬Ö¬Õ¬Ñ¬Ö¬Þ ¬Ü¬Ñ¬ã¬ä¬à¬Þ¬ß¬í¬Ö ¬Õ¬Ñ¬ß¬ß¬í¬Ö ¬Ó ¬ß¬å¬Ø¬ß¬í¬Ö ¬á¬à¬ä¬à¬Ü¬Ú
        var customData = new List<Vector4>();
        //¬±¬à¬Ý¬å¬é¬Ñ¬Ö¬Þ ¬á¬à¬ä¬à¬Ü ParticleSystemCustomData.Custom1 ¬Ú¬Ù ParticleSystem
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        //¬®¬Ö¬ß¬ñ¬Ö¬Þ ¬Õ¬Ñ¬ß¬ß¬í¬Ö ¬á¬à¬ã¬Ý¬Ö¬Õ¬ß¬Ö¬Ô¬à ¬ï¬Ý¬Ö¬Þ¬Ö¬ß¬ä, ¬ä.¬Ö. ¬ä¬à¬Û ¬é¬Ñ¬ã¬ä¬Ú¬è¬í, ¬Ü¬à¬ä¬à¬â¬å¬ð ¬Þ¬í ¬ä¬à¬Ý¬î¬Ü¬à ¬é¬ä¬à ¬ã¬à¬Ù¬Õ¬Ñ¬Ý¬Ú
        customData[customData.Count - 1] = custom1Data;
        //¬£¬à¬Ù¬Ó¬â¬Ñ¬ë¬Ñ¬Ö¬Þ ¬Õ¬Ñ¬ß¬ß¬í¬Ö ¬Ó ParticleSystem
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        //¬¡¬ß¬Ñ¬Ý¬à¬Ô¬Ú¬é¬ß¬à ¬Õ¬Ý¬ñ ParticleSystemCustomData.Custom2
        particleSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
        customData[customData.Count - 1] = custom2Data;
        particleSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom2);
    }

    //¬¶¬å¬ß¬Ü¬è¬Ú¬ñ ¬å¬á¬Ñ¬Ü¬à¬Ó¬Ü¬Ú ¬Þ¬Ñ¬ã¬ã¬Ú¬Ó¬Ñ Vector2 ¬ã ¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬Ñ¬Þ¬Ú ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬à¬Ó ¬Ó¬à float
    public float PackFloat(Vector2[] vecs)
    {
        if (vecs == null || vecs.Length == 0) return 0;
        //¬±¬à¬â¬Ñ¬Ù¬â¬ñ¬Õ¬ß¬à ¬Õ¬à¬Ò¬Ñ¬Ó¬Ý¬ñ¬Ö¬Þ ¬Ù¬ß¬Ñ¬é¬Ö¬ß¬Ú¬ñ ¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä ¬Ó¬Ö¬Ü¬ä¬à¬â¬à¬Ó ¬Ó float
        var result = vecs[0].y * 10000 + vecs[0].x * 100000;
        if (vecs.Length > 1) result += vecs[1].y * 100 + vecs[1].x * 1000;
        if (vecs.Length > 2) result += vecs[2].y + vecs[2].x * 10;
        return result;
    }

    //¬¶¬å¬ß¬Ü¬è¬Ú¬ñ ¬ã¬à¬Ù¬Õ¬Ñ¬ß¬Ú¬ñ Vector4 ¬Õ¬Ý¬ñ ¬á¬à¬ä¬à¬Ü¬Ñ ¬ã CustomData
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