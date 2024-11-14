using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 pos;
    public float effectDuration;
    public Queue<Effects> readyQueue = new Queue<Effects>();
#if UNITY_EDITOR
    public void SetDurationTime()
    {
        Transform decalTR = prefab.transform.Find("Decal");

        
        if(decalTR != null)
        {
            NewMaterialChange tempDecal = decalTR.GetComponent<NewMaterialChange>();
            if(tempDecal != null )effectDuration = tempDecal.m_timeToReduce > prefab.GetComponent<ParticleSystem>().main.duration ? tempDecal.m_timeToReduce: prefab.GetComponent<ParticleSystem>().main.duration;
        }
        else
        {
            effectDuration = prefab.GetComponent<ParticleSystem>().main.duration;
        }
    }
#endif
    public void PlayOrder(Vector3 position,Vector3 size,float rot)
    {
        StartCoroutine(SkillTimer(position,size,rot));
    }
    public void CreateEffect()
    {
        GameObject newEffect = GameObject.Instantiate(prefab);
        newEffect.transform.parent = transform;
        readyQueue.Enqueue(new Effects() { particle = newEffect.GetComponent<ParticleSystem>(), decal = newEffect.transform.Find("Decal")?.gameObject});
    }
    public IEnumerator SkillTimer(Vector3 position,Vector3 size,float rot)
    {
        if (readyQueue.Count > 0 )
        {
            //여기서 기다리고
            readyQueue.TryDequeue(out Effects result);
            if(result.particle == null) 
            {
                CreateEffect();
                StartCoroutine(SkillTimer(position, size, rot));
                yield break;
            }
            result.particle.gameObject.transform.localScale = size;
            result.particle.transform.eulerAngles = rot*Vector3.up;
            result.Play(position);
            yield return new WaitForSeconds(effectDuration);
            result.particle.gameObject.SetActive(false);
            readyQueue.Enqueue(result);
            yield break;
        }
        else
        {
            CreateEffect();
            StartCoroutine(SkillTimer(position, size, rot));
            yield break;
        }
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(EffectController))]
public class EffectControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EffectController controller = (EffectController)target;
        if (GUILayout.Button("SetDurationTime"))
        {
            controller.SetDurationTime();
        }
    }
}
#endif
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
        particle.gameObject.SetActive(true);
        particle.Play();
        decal?.SetActive(true);
    }
}
