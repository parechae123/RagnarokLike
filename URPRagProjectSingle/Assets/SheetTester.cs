using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetTester : MonoBehaviour
{
    public Sprite[] targetSprites;
    public float timer = 0;
    public int currentNumber = 0;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        timer += Time.deltaTime;
        if(timer > 0.1f) 
        {
            timer = 0;
    
            if(currentNumber < targetSprites.Length-1) currentNumber++; 
            else currentNumber = 0;
            sr.sprite = targetSprites[currentNumber];
        }
    }
}
