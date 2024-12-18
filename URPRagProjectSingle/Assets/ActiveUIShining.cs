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
    bool isOrderdFadeOut = false;

    private void OnEnable()
    {
        material.SetFloat("_Timer", 0);
        transform.DOKill();
        isOrderdFadeOut = false;
        RectTransform rectTR = (RectTransform)transform;
        rectTR.sizeDelta= new Vector3(600, 100, 1);
        rectTR.DOSizeDelta(new Vector3(600, 300, 1), 0.3f);
        timer = -0.3f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer> 5f&& !isOrderdFadeOut) OnFadeOut();
        if (timer > material.GetFloat("_CycleTime")) return;
        material.SetFloat("_Timer", timer);
    }
    private void OnFadeOut()
    {
        isOrderdFadeOut = true;
        RectTransform rectTR = (RectTransform)transform;
        rectTR.DOSizeDelta(new Vector3(600, 50, 1), 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
