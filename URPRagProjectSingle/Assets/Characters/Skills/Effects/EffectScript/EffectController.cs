using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public GameObject prefab;
    public Effects[] effect= new Effects[1];
    public void playOrder(Vector3 position)
    {
        //QueueStack형태로 
    }
    
}
[System.Serializable]
public class Effects
{
    public ParticleSystem particle;
    public GameObject decal;
    public bool IsPlaying()
    {
        if (particle.isPlaying) return true;
        if (decal == null) return false;
        else if (decal != null && decal.activeSelf) return true;
        return false;
    }
    /// <summary>
    /// 이미 재생중이라 실패 시 false를 리턴
    /// </summary>
    /// <param name="positions">쎾쓰</param>
    /// <returns></returns>
    public void Play(Vector3 positions)
    {
        particle.transform.position = positions;
        decal.transform.position = positions;
        particle.Play();
        decal.SetActive(true);
    }
}
