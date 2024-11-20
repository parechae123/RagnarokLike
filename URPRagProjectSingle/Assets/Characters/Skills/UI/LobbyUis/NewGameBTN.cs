using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameBTN : MonoBehaviour
{
    public string[] labels = new string[0];
    public bool[] loadDone = new bool[0];
    private bool btnUsed = false;
    private bool allDone
    {
        get 
        {
            return loadDone.Min();
        }
    }
    private void Awake()
    {
        
        transform.GetComponent<Button>().interactable = false;
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (btnUsed) return;
            btnUsed = true;

            for (int i = 0; i < labels.Length; i++)
            {
                int currNum = i;
                ResourceManager.GetInstance().LoadAsyncAll<Object>(labels[i], (percent) =>
                {
                    loadDone[currNum] = true;
                    if (allDone) SceneManager.LoadSceneAsync(1);
                });
            }
        });
    }
    private void Start()
    {
        loadDone = new bool[6];
        ResourceManager.GetInstance().SetAtlases((done,arr) =>
        {
            loadDone[arr] = done;
            if (allDone)
            {
                Debug.Log("���̺� ������");
                loadDone = new bool[labels.Length];
                transform.GetComponent<Button>().interactable = true;
            }
            
        });
    }
}
