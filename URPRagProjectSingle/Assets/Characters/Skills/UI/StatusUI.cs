using PlayerDefines.Stat;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        RegistGameOBJ();
        Player.Instance.playerLevelInfo.baseLevelUP += UpdateStatUI;
        Player.Instance.playerLevelInfo.stat.BasicStatus.updateStat += UpdateStatUI;
        statUPButtons[0].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.Str);
            UpdateStatUI();
        });
        statUPButtons[1].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.AGI);
            UpdateStatUI();
        });
        statUPButtons[2].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.Vit);
            UIManager.GetInstance().PlayerMaxCurrHP = (Player.Instance.playerLevelInfo.stat.MaxHP,
            Player.Instance.playerLevelInfo.stat.HP);
            UpdateStatUI();
        });
        statUPButtons[3].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.Int);
            UIManager.GetInstance().PlayerMaxCurrSP = (Player.Instance.playerLevelInfo.stat.MaxSP,
            Player.Instance.playerLevelInfo.stat.SP);
            UpdateStatUI();
        });
        statUPButtons[4].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.Dex);
            UpdateStatUI();
        });
        statUPButtons[5].onClick.AddListener(() =>
        {
            Player.Instance.playerLevelInfo.stat.BasicStatus.PureStatUP(BasicStatTypes.Luk);
            UpdateStatUI();
        });
        
    }
    /// <summary>
    /// 레벨업시,장비 장착시, 능력치 변동시 발동필요
    /// </summary>
    public void UpdateStatUI()
    {
        PlayerStat tempStat = Player.Instance.playerLevelInfo.stat;
        basicStatusTexts[0].text = $"{tempStat.BasicStatus.GetPureStr} "+
         (tempStat.BasicStatus.Strength - tempStat.BasicStatus.GetPureStr == 0 ?
         string.Empty: $"+ {tempStat.BasicStatus.Strength - tempStat.BasicStatus.GetPureStr}");

        basicStatusTexts[1].text = $"{tempStat.BasicStatus.GetPureAgi}" +
            (tempStat.BasicStatus.Agility - tempStat.BasicStatus.GetPureAgi == 0? 
             string.Empty : $" + {tempStat.BasicStatus.Agility - tempStat.BasicStatus.GetPureAgi}");

        basicStatusTexts[2].text = $"{tempStat.BasicStatus.GetPureVit}" + 
            (tempStat.BasicStatus.Vitality - tempStat.BasicStatus.GetPureVit == 0? 
            string.Empty:$" + {tempStat.BasicStatus.Vitality - tempStat.BasicStatus.GetPureVit}");

        basicStatusTexts[3].text = $"{tempStat.BasicStatus.GetPureInt}"+
            (tempStat.BasicStatus.Inteligence - tempStat.BasicStatus.GetPureInt== 0?
            string.Empty: $" + {tempStat.BasicStatus.Inteligence - tempStat.BasicStatus.GetPureInt}");

        basicStatusTexts[4].text = $"{tempStat.BasicStatus.GetPureDex}" +
            (tempStat.BasicStatus.Dexterity - tempStat.BasicStatus.GetPureDex== 0 ?
            string.Empty : $" + {tempStat.BasicStatus.Dexterity - tempStat.BasicStatus.GetPureDex}");

        basicStatusTexts[5].text = $"{tempStat.BasicStatus.GetPureLuk}" +
            (tempStat.BasicStatus.Luck - tempStat.BasicStatus.GetPureLuk == 0 ?
            string.Empty : $" + {tempStat.BasicStatus.Luck - tempStat.BasicStatus.GetPureLuk}");

        //필요 포인트구하기 + 찍을 수 있는지 여부 체크
        int tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureStr);
        int tempLeftPoint = Player.Instance.playerLevelInfo.LeftStatusPoint;
        statUPButtons[0].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[0].text = $"{tempRequirePoint}";

        tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureAgi);
        statUPButtons[1].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[1].text = $"{tempRequirePoint}";

        tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureVit);
        statUPButtons[2].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[2].text = $"{tempRequirePoint}";

        tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureInt);
        statUPButtons[3].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[3].text = $"{tempRequirePoint}";

        tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureDex);
        statUPButtons[4].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[4].text = $"{tempRequirePoint}";

        tempRequirePoint = tempStat.BasicStatus.GetRequrePoint(tempStat.BasicStatus.GetPureLuk);
        statUPButtons[5].gameObject.SetActive(tempRequirePoint <= tempLeftPoint ? true : false);
        basicStatusRequirePoint[5].text = $"{tempRequirePoint}";

        otherStats[0].text = tempStat.TotalAD.ToString();
        otherStats[1].text = tempStat.TotalAP.ToString();
        otherStats[2].text = tempStat.TotalAccuracy.ToString();
        otherStats[3].text = tempStat.CriChance.ToString();
        otherStats[4].text = Player.Instance.playerLevelInfo.LeftStatusPoint.ToString();
        otherStats[5].text = tempStat.HPRegen.ToString("n1");
        otherStats[6].text = tempStat.CastTimePercent.ToString("n3");
        otherStats[7].text = tempStat.SPRegen.ToString("n1");
        otherStats[8].text = tempStat.Evasion.ToString();
        otherStats[9].text = tempStat.TotalAttackSpeed.ToString("n3");
        otherStats[10].text = tempStat.GlobalCooltimePercent.ToString("n3");


    }
    
    public void OnEnable()
    {
        UpdateStatUI();
    }

    public void RegistGameOBJ()
    {
        foreach (KeyCode item in KeyMapManager.GetInstance().keyMaps.Keys)
        {
            if (KeyMapManager.GetInstance().keyMaps[item].UIType == UITypes.StatusWindow)
            {
                ShortCutOBJ temp = KeyMapManager.GetInstance().keyMaps[item];


                temp.subScribFuncs = null;
                temp.subScribFuncs += OnOff;
                KeyMapManager.GetInstance().keyMaps[item] = temp;
                gameObject.SetActive(false);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    public void OnOff() {gameObject.SetActive(!gameObject.activeSelf); }
}
