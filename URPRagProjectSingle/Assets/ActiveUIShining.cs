using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActiveUIShining : MonoBehaviour
{
    [SerializeField] private Material material;
    float timer;

    private void OnEnable()
    {
        material.SetFloat("_Timer", 0);
        ((RectTransform)transform).sizeDelta= new Vector3(600, 100, 1);
        ((RectTransform)transform).DOSizeDelta(new Vector3(600,300,1), 0.3f);
        timer = -0.3f;
    }
    private void Update()
    {
        if (timer > material.GetFloat("_CycleTime")) return;
        timer += Time.deltaTime;
        material.SetFloat("_Timer", timer);
    }
}
