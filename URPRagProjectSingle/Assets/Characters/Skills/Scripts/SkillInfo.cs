using DG.Tweening;
using JetBrains.Annotations;
using PlayerDefines.Stat;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
[System.Serializable]
public class SkillInfo : ScriptableObject
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public string skillName;
    public string jobName;

    [Header("��ų ����")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBase[] skill;

    [Header("��ų ����Ʈ ������Ʈ ������, ������")]
    [SerializeField] public GameObject effectOBJPrefab;
    [SerializeField] public Sprite skillIcon;
    public byte maxSkillLevel;
    public byte nowSkillLeve;
    public byte castingSkillLevel;

    public bool isSkillLearned
    {
        get;
        private set;
    }

#if UNITY_EDITOR

    public void ObjectToScriptableOBJ(SkillInfoObjectOnly basedObject)
    {
        skillName = basedObject.skillName;
        maxSkillLevel = basedObject.maxSkillLevel;
        skill = new SkillBase[maxSkillLevel];
        jobName = basedObject.jobName;
        skillType = basedObject.skillType;
        objectiveType = basedObject.objectiveType;
        skillPosition = basedObject.skillPosition;
    }
    /// <summary>
    /// �ش� ��ũ��Ʈ�� ��ų�迭�� ��ų�� ���������� �־���
    /// </summary>
    /// <param name="skillData"></param>
    public void AddSkillDetailData(SkillBase skillData)
    {
        skill[skillData.skillLevel - (byte)1] = skillData; 
    }
    public void UpdateInfomation(SkillInfo lastestInfo)
    {
        skillName = lastestInfo.skillName;
        maxSkillLevel = lastestInfo.maxSkillLevel;
        skill = lastestInfo.skill;
        jobName = lastestInfo.jobName;
        skillType = lastestInfo.skillType;
        objectiveType = lastestInfo.objectiveType;
        skillPosition = lastestInfo.skillPosition;
        SetSkillAsset(lastestInfo.skillIcon, lastestInfo.effectOBJPrefab);
    }
    public void SetSkillAsset(Sprite Image,GameObject prefab)
    {
        if (Image == null || prefab == null)
        {
            Debug.LogError("�ش� ������ �Ʒ� �׸��� ã�� �� �����ϴ� : " + skillName);
            Debug.LogError("������ : "+(Image == null? "������": "����"));
            Debug.LogError("����Ʈ : "+ (prefab == null? "������": "����"));
            return;

        }
        skillIcon = Image;
        effectOBJPrefab = prefab;
    }

#endif
}
[System.Serializable]
public class SkillInfoInGame : IItemBase
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public event Action quickSlotFuncs;
    [Header("��ų ����")]
    public string skillName;
    public string jobName;
    public SkillStatus skillStatus;

    [Header("��ų ����")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBaseInGameData[] skill;
    [SerializeField] public Sprite skillIcon;

    [Header("��ų ����Ʈ ������Ʈ ������, ������")]
    private Animator[] effectOBJs = new Animator[0];//�� ���� ����Ʈ ������Ʈ
    [SerializeField] private GameObject effectOBJPrefab;
    public byte maxSkillLevel;
    public byte nowSkillLevel;
    private byte castingSkillLevel =1;
    public byte CastingSkillLevel
    {
        get
        {
            if (castingSkillLevel == 0) return 0;
            return (byte)(castingSkillLevel-1);
        }
        set
        {
            if (nowSkillLevel < value||value <= 0) return;
            castingSkillLevel = value;
        }
    }
    public Sprite IconIMG
    {
        get { return skillIcon; }
    }
    public string slotNumberInfo
    {
        get 
        {
            return CastingSkillLevel == 0 ? (isSkillLearned ? 1.ToString() : string.Empty  ) : (CastingSkillLevel+1).ToString();
        }
    }
    public bool isSkillLearned
    {
        get { return (nowSkillLevel > 0);}
    }
    public bool IsItemUseAble
    {
        get { return isSkillLearned; }
    }

    public float goalCool;

    public SlotType slotType { get { return SlotType.Skills; } }


    public SkillInfoInGame(SkillInfo data)
    {
        skillName = data.skillName;
        jobName = data.jobName;
        skillType = data.skillType;
        objectiveType = data.objectiveType;
        skillPosition = data.skillPosition;
        skill = ConvertInGameData(data.skill);
        effectOBJPrefab = data.effectOBJPrefab;
        skillIcon = data.skillIcon;
        maxSkillLevel = data.maxSkillLevel;
        quickSlotFuncs = null;
        quickSlotFuncs += SetSkillObjectToPlayer;
    }
    public SkillInfoInGame(SkillInfoInGame data)
    {
        skillName = data.skillName;
        jobName = data.jobName;
        skillType = data.skillType;
        objectiveType = data.objectiveType;
        skillPosition = data.skillPosition;
        skill = data.skill;
        effectOBJPrefab = data.effectOBJPrefab;
        skillIcon = data.skillIcon;
        nowSkillLevel = data.nowSkillLevel;
        castingSkillLevel = data.castingSkillLevel;
        maxSkillLevel = data.maxSkillLevel;
        quickSlotFuncs = null;
        quickSlotFuncs += SetSkillObjectToPlayer;
    }
    private SkillBaseInGameData[] ConvertInGameData(SkillBase[] skills)
    {
        SkillBaseInGameData[] tempInGameData = new SkillBaseInGameData[skills.Length];
        for (int i = 0; i < tempInGameData.Length; i++)
        {
            tempInGameData[i] = new SkillBaseInGameData(skills[i]);
        }
        return tempInGameData;
    }
    /// <summary>
    /// �� ������� ����Ʈ ������Ʈ�� �ִϸ��̼��� ��ȯ���ݴϴ�.
    /// </summary>
    /// <returns></returns>
    public Animator GetNonPlayingSkillEffect()
    {
        for (int i = 0; i < effectOBJs.Length; i++)
        {
            if (effectOBJs[i].gameObject.activeSelf)
            {
                continue;
            }
            else
            {
                return effectOBJs[i];
            }
        }
        int tempNum = effectOBJs.Length;
        Array.Resize(ref effectOBJs, tempNum + 1);
        effectOBJs[tempNum] = GameObject.Instantiate(effectOBJPrefab).GetComponent<Animator>();
        return effectOBJs[tempNum];
    }
    public void UseItem()
    {
        if (!isSkillLearned) return;
        quickSlotFuncs?.Invoke();
    }
    public void SetSkillObjectToPlayer()
    {
        Player.Instance.SkillObj = this;
    }
    
    /// <summary>
    /// ��ų ����Ʈ�� ����մϴ�.
    /// </summary>
    /// <param name="EffectPosition"></param>
/*    public void StartSkillEffect(Vector3 EffectPosition)
    {
        //castingState�� �ٲߴϴ�
        Player.Instance.CastingOrder(skill[castingSkillLevel].defaultCastingTime);
        //ĳ���� �ð��� ���������� �������� �Ʒ� �Լ��� ����ǵ��� �߰� �۾� �ʿ�

        switch (skillPosition)
        {
            case SkillPosition.cursor:
                //���콺 Ŭ�� �̺�Ʈ�� �߻��Ҷ� �Ʒ� �Լ��� ��������ִ� ������ �ۼ��ؾ���
                SkillCastTargetPlace(EffectPosition);
                break;
            case SkillPosition.self:
                SkillCastInPlace(EffectPosition);
                break;
        }
    }*/
    public void SkillCastTargetPlace(Vector3 castingPos,Stats target,Stats caster)
    {
        Debug.Log(skillName + "����߾��~~");
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = castingPos;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        float tempTime = 0;
        Color tempColor = Color.black;
        switch (skill[castingSkillLevel].damageType)
        {
            case ValueType.Physical:
                tempColor = Color.red;
                break;
            case ValueType.Magic:
                tempColor = Color.cyan;
                break;
            case ValueType.Heal:
                tempColor = Color.green;
                break;
            case ValueType.PhysicalRange:
                tempColor = Color.magenta;
                break;
            case ValueType.TrueDamage:
                tempColor = Color.white;
                break;
        }
        switch (objectiveType)
        {
            case ObjectiveType.None:
                break;
            case ObjectiveType.OnlyTarget:
                if (target != null) UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos+Vector3.up, target.GetDamage(skill[CastingSkillLevel].TotalDamage(caster), skill[castingSkillLevel].damageType).ToString("N0"),tempColor,1);
                break;
            case ObjectiveType.Bounded:
                Stats[] tempTargets = GetStats(new Vector2Int((int)castingPos.x, (int)castingPos.z));
                for (int i = 0; i < tempTargets.Length; i++)
                {
                    if (tempTargets[i] == caster) continue;
                    UIManager.GetInstance().SpawnFloatText(tempTargets[i].standingNode.worldPos + Vector3.up, tempTargets[i].GetDamage(skill[CastingSkillLevel].TotalDamage(caster), skill[castingSkillLevel].damageType).ToString("N0"), tempColor, 1);
                }
                break;
        }
        SkillManager.GetInstance().SetSkillCoolTime(skillName, skill[CastingSkillLevel].coolTime);
        Debug.Log("ī���� ����");
        SkillManager.GetInstance().activatedCDTimer = true;
        for (int i = 0; i < tempAnim.runtimeAnimatorController.animationClips.Length; i++)
        {
            tempTime += tempAnim.runtimeAnimatorController.animationClips[i].length;
        }
        DOVirtual.DelayedCall(tempTime, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
    public Stats[] GetStats(Vector2Int nodePos)
    {
        Stats[] outPutStats = new Stats[0];
        int boundMax = skill[CastingSkillLevel].SkillBound;
        for (int i = -boundMax; i <= boundMax; i++)
        {
            for (int j = -boundMax; j <= boundMax; j++)
            {
                if ((i*i) + (j*j) <= boundMax * boundMax)
                {
                    Vector2Int tempVec = new Vector2Int(i, j) + nodePos;
                    if (GridManager.GetInstance().grids.ContainsKey(tempVec))
                    {
                        if (GridManager.GetInstance().grids[tempVec].CharacterOnNode != null)
                        {
                            Array.Resize(ref outPutStats, outPutStats.Length+1);
                            outPutStats[outPutStats.Length - 1] = GridManager.GetInstance().grids[tempVec].CharacterOnNode;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else continue;

            }
        }
        return outPutStats;
    }
}