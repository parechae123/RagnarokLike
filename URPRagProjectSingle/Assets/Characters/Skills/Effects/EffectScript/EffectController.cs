using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public GameObject prefab;
    public Queue<Effects> readyQueue = new Queue<Effects>();
    public Effects[] effect= new Effects[1];
    public void PlayOrder(Vector3 position)
    {
        //QueueStack형태로 
        
    }
    public void CreateEffect()
    {
        
    }
    public IEnumerator SkillTimer(Vector3 position)
    {
        if (readyQueue.Count > 0 )
        {
            //여기서 기다리고
            readyQueue.TryDequeue(out Effects result);
            yield return new WaitForSeconds(result.particle.main.duration);
            result.particle.gameObject.SetActive(false);
            readyQueue.Enqueue(result);
            yield break;
        }
        else
        {
            CreateEffect();
            StartCoroutine(SkillTimer(position));
            yield break;
        }
    }
}
[System.Serializable]
public struct Effects
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
    /// <param name="positions"></param>
    /// <returns></returns>
    public void Play(Vector3 positions)
    {
        particle.transform.position = positions;
        particle.Play();
        decal.SetActive(true);
    }
}
