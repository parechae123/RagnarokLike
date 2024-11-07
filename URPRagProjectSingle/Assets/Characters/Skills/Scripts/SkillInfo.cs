using DG.Tweening;
using JetBrains.Annotations;
using PlayerDefines.Stat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
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
    public Image iconRenderer;
    public bool isSkillLearned
    {
        get { return (nowSkillLevel > 0);}
    }
    public bool IsItemUseAble
    {
        get { return isSkillLearned; }
    }

    public float goalCool;
    public float originCool;

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
        if (SkillManager.GetInstance().activatedCDTimer || goalCool != 0) return;
        if (!isSkillLearned) return;
        quickSlotFuncs?.Invoke();
    }
    public void SetSkillObjectToPlayer()
    {
        if (SkillManager.GetInstance().activatedCDTimer|| Player.Instance.StateMachine.CurrentState.stateName == "castingState") return;
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
public interface IBuffs
{
    string buffName
    {
        get;
        set;
    }
    float buffValue
    {
        get;
        set;
    }
    float duration
    {
        get;
        set;
    }
    byte buffLevel
    {
        get;
        set;
    }
    void ApplyBuff();
    void RemoveBuff();
}

public class Buff
{
    IBuffs[] buffs = new IBuffs[0];

}


public class PlayerOffensiveBuff : IBuffs
{
    PlayerOffensiveBuff(string buffName,float buffValue,float time, byte buffLevel,WeaponApixType type ,Stats target)
    {
        this.target = target;
        this.buffValue = buffValue;
        this.buffName = buffName;
        duration = time;
        this.buffLevel = buffLevel;
        this.buffType = type;
    }
    Stats target;
    public string buffName
    {
        get;
        set;
    }
    public float buffValue
    {
        get;
        set;
    }
    public float duration
    {
        get;
        set;
    }
    public byte buffLevel
    {
        get;
        set;
    }
    private WeaponApixType buffType;



    public float GetTargetInstance
    {
        get
        {
            switch (buffType)
            {
                case WeaponApixType.CriticalDMG:
                    if (target.GetType() != typeof(PlayerStat)) return 0;
                    return ((PlayerStat)target).defaultCriDamage;
                case WeaponApixType.CriticalChance:
                    if (target.GetType() != typeof(PlayerStat)) return 0;
                    return ((PlayerStat)target).defaultCriChance;
                case WeaponApixType.ATK:
                    return target.attackDamage;
                case WeaponApixType.MATK:
                    return target.abilityPower;
                case WeaponApixType.AttackSpeed:
                    return target.attackSpeed;
                case WeaponApixType.CastingSpeed:
                    return target.defaultCasting;
                case WeaponApixType.MaxHP:
                    return target.defaultMaxHP;
                case WeaponApixType.Accuracy:
                    return target.accuracy;
                default:
                    return 0;
            }
        }
        set
        {
            switch (buffType)
            {
                case WeaponApixType.CriticalDMG:
                    if (target.GetType() != typeof(PlayerStat)) break;
                    ((PlayerStat)target).defaultCriDamage += value;
                    break;
                case WeaponApixType.CriticalChance:
                    if (target.GetType() != typeof(PlayerStat)) break;
                    ((PlayerStat)target).defaultCriChance += (int)value;
                    break;
                case WeaponApixType.ATK:
                    target.attackDamage += value;
                    break;
                case WeaponApixType.MATK:
                    target.abilityPower += value;
                    break;
                case WeaponApixType.AttackSpeed:
                    target.attackSpeed -= value;
                    break;
                case WeaponApixType.CastingSpeed:
                    target.defaultCasting -= value;
                    break;
                case WeaponApixType.MaxHP:
                    target.defaultMaxHP += value;
                    break;
                case WeaponApixType.Accuracy:
                    target.accuracy += value;
                    break;
                default:
                    break;
            }
        }
    }


    public void ApplyBuff()
    {
        SkillManager.GetInstance().RegistBuffTimer(duration, RemoveBuff);
        GetTargetInstance += buffValue;
    }
    public void RemoveBuff()
    {
        GetTargetInstance -= buffValue;
    }
}

public class PlayerDeffensiveBuff : IBuffs
{
    PlayerDeffensiveBuff(string buffName,float buffValue, float time, byte buffLevel, ArmorApixType type,Stats stat)
    {
        this.buffName = buffName;
        this.buffValue = buffValue;
        duration = time;
        this.buffLevel = buffLevel;
        this.buffType = type;
        target = stat;
    }
    Stats target;
    public string buffName
    {
        get;
        set;
    }
    public float buffValue
    {
        get;
        set;
    }
    public float duration
    {
        get;
        set;
    }
    public byte buffLevel
    {
        get;
        set;
    }
    public float GetTargetInstance
    {
        get 
        {
            switch (buffType)
            {
                case ArmorApixType.MaxMana:
                    return target.defaultSP;
                case ArmorApixType.ManaRegen:
                    if (target.GetType() != typeof(PlayerStat)) return 0;
                    return ((PlayerStat)target).defaultSPRegen;
                case ArmorApixType.MaxHp:
                    return target.defaultMaxHP;
                case ArmorApixType.HpRegen:
                    if (target.GetType() != typeof(PlayerStat)) return 0;
                    return ((PlayerStat)target).defaultHPRegen;
                case ArmorApixType.MoveSpeed:
                    return target.moveSpeed;
                case ArmorApixType.deff:
                    return target.deff;
                case ArmorApixType.Evasion:
                    return target.defaultEvasion;
                case ArmorApixType.magicDeff:
                    return target.magicDeff;
                default:
                    return 0;
            }
        }
        set
        {
            switch (buffType)
            {
                case ArmorApixType.MaxMana:
                    target.defaultSP = value;
                    return;
                case ArmorApixType.ManaRegen:
                    if (target.GetType() != typeof(PlayerStat)) return;
                    ((PlayerStat)target).defaultSPRegen = value;
                    return;
                case ArmorApixType.MaxHp:
                    target.defaultMaxHP = value;
                    return;
                case ArmorApixType.HpRegen:
                    if (target.GetType() != typeof(PlayerStat)) return;
                    ((PlayerStat)target).defaultHPRegen = value;
                    return;
                case ArmorApixType.MoveSpeed:
                    target.moveSpeed = value;
                    return;
                case ArmorApixType.deff:
                    target.deff = value;
                    return;
                case ArmorApixType.Evasion:
                    target.defaultEvasion = value;
                    return;
                case ArmorApixType.magicDeff:
                    target.magicDeff = value;
                    return;
            }
        }
    }
    private ArmorApixType buffType;
    public void ApplyBuff()
    {
        SkillManager.GetInstance().RegistBuffTimer(duration, RemoveBuff);
        GetTargetInstance += buffValue;
    }
    public void RemoveBuff()
    {
        GetTargetInstance -= buffValue;
    }
}
/// <summary>
/// �÷��̾�Ը� ���� ����
/// </summary>
public class PlayerStatBuff : IBuffs
{
    PlayerStatBuff(string buffName,float buffValue, float time, byte buffLevel, BasicStatTypes type, PlayerStat stat)
    {
        this.buffName = buffName;
        this.buffValue = buffValue;
        this.duration = time;
        this.buffLevel = buffLevel;
        buffType = type;
        target = stat;
    }
    PlayerStat target;
    public string buffName
    {
        get;
        set;
    }
    public float buffValue
    {
        get;
        set;
    }
    public float duration
    {
        get;
        set;
    }
    public byte buffLevel
    {
        get;
        set;
    }
    public BasicStatTypes buffType;
    public void ApplyBuff()
    {
        target.BasicStatus.SetChangeAbleStatus(buffType, (int)buffValue);
        SkillManager.GetInstance().RegistBuffTimer(duration, RemoveBuff);
    }
    public void RemoveBuff()
    {
        target.BasicStatus.SetChangeAbleStatus(buffType, (int)-buffValue);
    }
}