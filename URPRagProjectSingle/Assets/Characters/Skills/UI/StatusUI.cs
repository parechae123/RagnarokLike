using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    //0 str 1 agi 2 vit 3 int 4 dex 5 luk
    [SerializeField]private Text[] basicStatusTexts = new Text[6];
    [SerializeField]private Text[] basicStatusRequirePoint = new Text[6];
    [SerializeField]private Button[] statUPButtons = new Button[6];


    [SerializeField]private Text[] otherStats = new Text[11];

    void Start()
    {
        Player.Instance.playerLevelInfo.jobLevelUP += UpdateStatUI;
    }
    
    public void UpdateStatUI()
    {
        
        basicStatusTexts[0].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureStr} + {Player.Instance.playerLevelInfo.stat.basicStatus.Strength - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureStr}";
        basicStatusTexts[1].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureAgi} + {Player.Instance.playerLevelInfo.stat.basicStatus.Agility - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureAgi}";
        basicStatusTexts[2].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureVit} + {Player.Instance.playerLevelInfo.stat.basicStatus.Vitality - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureVit}";
        basicStatusTexts[3].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureInt} + {Player.Instance.playerLevelInfo.stat.basicStatus.Inteligence - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureInt}";
        basicStatusTexts[4].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureDex} + {Player.Instance.playerLevelInfo.stat.basicStatus.Dexterity - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureDex}";
        basicStatusTexts[5].text = $"{Player.Instance.playerLevelInfo.stat.basicStatus.GetPureLuk} + {Player.Instance.playerLevelInfo.stat.basicStatus.Luck - Player.Instance.playerLevelInfo.stat.basicStatus.GetPureLuk}";

        //필요 포인트구하기 + 찍을 수 있는지 여부 체크
        int tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureStr);
        int tempLeftPoint = Player.Instance.playerLevelInfo.LeftSkillPoint;
        statUPButtons[0].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[0].text = $"{tempRequirePoint}";

        tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureAgi);
        statUPButtons[1].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[1].text = $"{tempRequirePoint}";

        tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureVit);
        statUPButtons[2].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[2].text = $"{tempRequirePoint}";

        tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureInt);
        statUPButtons[3].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[3].text = $"{tempRequirePoint}";

        tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureDex);
        statUPButtons[4].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[4].text = $"{tempRequirePoint}";

        tempRequirePoint = Player.Instance.playerLevelInfo.stat.basicStatus.GetRequrePoint(Player.Instance.playerLevelInfo.stat.basicStatus.GetPureLuk);
        statUPButtons[5].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[5].text = $"{tempRequirePoint}";
    }
    
    public void GetTexts()
    {
        for (int i = 0; i < otherStats.Length; i++)
        {
            otherStats[i] = transform.GetChild(i+6).Find("value").GetComponent<Text>();
        }
    }
    public void OnEnable()
    {
        UpdateStatUI();
    }
}
