using DG.Tweening;
using JetBrains.Annotations;
using PlayerDefines.Stat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
[System.Serializable]
public class SkillInfo : ScriptableObject
{
    ///해당 클래스는 플레이어가 스킬 정보로 가지고있어야하는 객체로 추후 스크립터블 오브젝트 해제해야함
    public string skillName;
    public string jobName;

    [Header("스킬 유형")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBase[] skill;

    [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
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
    /// 해당 스크립트에 스킬배열에 스킬을 레벨순서로 넣어줌
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
            Debug.LogError("해당 파일의 아래 항목을 찾을 수 없습니다 : " + skillName);
            Debug.LogError("아이콘 : "+(Image == null? "비정상": "정상"));
            Debug.LogError("이팩트 : "+ (prefab == null? "비정상": "정상"));
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
    ///해당 클래스는 플레이어가 스킬 정보로 가지고있어야하는 객체로 추후 스크립터블 오브젝트 해제해야함
    public event Action quickSlotFuncs;
    [Header("스킬 상태")]
    public string skillName;
    public string jobName;
    public SkillStatus skillStatus;

    [Header("스킬 유형")]
    public SkillType skillType;
    [SerializeField] public ObjectiveType objectiveType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBaseInGameData[] skill;
    [SerializeField] public Sprite skillIcon;

    [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
    private Animator[] effectOBJs = new Animator[0];//씬 내의 이펙트 오브젝트
    [SerializeField] private GameObject effectOBJPrefab;
    public byte maxSkillLevel;
    public byte nowSkillLevel;
    private byte castingSkillLevel =1;
    public int leftTick;
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
    public int originTick;

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
    /// 비 사용중인 이펙트 오브젝트의 애니메이션을 반환해줍니다.
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
        if (leftTick != 0) return;
        if (!isSkillLearned) return;
        quickSlotFuncs?.Invoke();
    }
    public virtual void SetSkillObjectToPlayer()
    {
        if (Player.Instance.StateMachine.CurrentState.stateName == "castingState") return;
        Player.Instance.SkillObj = this;
    }
    
    /// <summary>
    /// 스킬 이펙트를 재생합니다.
    /// </summary>
    /// <param name="EffectPosition"></param>
/*    public void StartSkillEffect(Vector3 EffectPosition)
    {
        //castingState로 바꿉니다
        Player.Instance.CastingOrder(skill[castingSkillLevel].defaultCastingTime);
        //캐스팅 시간이 정상적으로 끝났을때 아래 함수가 실행되도록 추가 작업 필요

        switch (skillPosition)
        {
            case SkillPosition.cursor:
                //마우스 클릭 이벤트가 발생할때 아래 함수를 실행시켜주는 구문을 작성해야함
                SkillCastTargetPlace(EffectPosition);
                break;
            case SkillPosition.self:
                SkillCastInPlace(EffectPosition);
                break;
        }
    }*/
    public virtual void SkillCastTargetPlace(Vector3 castingPos,Stats target,Stats caster)
    {
        Debug.Log(skillName + "사용했어용~~");
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = castingPos;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        float tempTime = 0;
        Color tempColor = Color.black;
        switch (skill[CastingSkillLevel].damageType)
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
                if (target != null) UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos+Vector3.up, target.GetDamage(skill[CastingSkillLevel].TotalDamage(caster), skill[CastingSkillLevel].damageType).ToString("N0"),tempColor,1);
                break;
            case ObjectiveType.Bounded:
                Stats[] tempTargets = GetStats(new Vector2Int((int)castingPos.x, (int)castingPos.z));
                for (int i = 0; i < tempTargets.Length; i++)
                {
                    if (tempTargets[i] == caster) continue;
                    UIManager.GetInstance().SpawnFloatText(tempTargets[i].standingNode.worldPos + Vector3.up, tempTargets[i].GetDamage(skill[CastingSkillLevel].TotalDamage(caster), skill[CastingSkillLevel].damageType).ToString("N0"), tempColor, 1);
                }
                break;
        }
        SkillManager.GetInstance().SetSkillCoolTime(skillName, skill[CastingSkillLevel].coolTimeTick);
        Debug.Log("카운팅 시작");

        for (int i = 0; i < tempAnim.runtimeAnimatorController.animationClips.Length; i++)
        {
            tempTime += tempAnim.runtimeAnimatorController.animationClips[i].length;
        }
        DOVirtual.DelayedCall(tempTime, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
    public virtual Stats[] GetStats(Vector2Int nodePos)
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
    public void ResetAction()
    {
        quickSlotFuncs = null;
    }
}
[System.Serializable]
public class BuffSkillInfoInGame : SkillInfoInGame
{
    [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
    private Animator[] effectOBJs = new Animator[0];//씬 내의 이펙트 오브젝트
    [SerializeField] private GameObject effectOBJPrefab;
    private byte castingSkillLevel =1;
    public new BuffSkillBaseInGameData[] skill;


    public BuffSkillInfoInGame(SkillInfo data) : base(data)
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
        base.ResetAction();
        quickSlotFuncs += SetSkillObjectToPlayer;
    }
    public BuffSkillInfoInGame(BuffSkillInfoInGame data) : base(data)
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
        base.ResetAction();
        quickSlotFuncs += SetSkillObjectToPlayer;
    }
    private BuffSkillBaseInGameData[] ConvertInGameData(SkillBase[] skills)
    {
        BuffSkillBaseInGameData[] tempInGameData = new BuffSkillBaseInGameData[skills.Length];
        for (int i = 0; i < tempInGameData.Length; i++)
        {
            tempInGameData[i] = new BuffSkillBaseInGameData(skills[i]);
        }
        return tempInGameData;
    }
    public override void SkillCastTargetPlace(Vector3 castingPos,Stats target,Stats caster)
    {
        Debug.Log(skillName + "사용했어용~~");
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = castingPos;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        float tempTime = 0;
        switch (objectiveType)
        {
            case ObjectiveType.None:
                break;
            case ObjectiveType.OnlyTarget:
                if (target != null) 
                { 
                    UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos+Vector3.up, $"{skillName}!!",Color.cyan,1.5f);
                    target.buffs.AcceptBuff(new BuffOBJ() {buffName = skillName,buffLevel = CastingSkillLevel, buffs= skill[CastingSkillLevel].buffSet,buffTargets = target,leftTick = skill[CastingSkillLevel].skillDuration });
                }
                break;
            case ObjectiveType.Bounded:
                Stats[] tempTargets = GetStats(new Vector2Int((int)castingPos.x, (int)castingPos.z));
                for (int i = 0; i < tempTargets.Length; i++)
                {
                    if (tempTargets[i] == caster) continue;
                    target.buffs.AcceptBuff(new BuffOBJ() { buffName = skillName, buffLevel = CastingSkillLevel, buffs = skill[CastingSkillLevel].buffSet, buffTargets = target, leftTick = skill[CastingSkillLevel].skillDuration });
                    UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos + Vector3.up, $"{skillName}!!", Color.cyan, 1.5f);
                    
                }
                break;
        }
        SkillManager.GetInstance().SetSkillCoolTime(skillName, skill[CastingSkillLevel].coolTimeTick);
        Debug.Log("카운팅 시작");

        for (int i = 0; i < tempAnim.runtimeAnimatorController.animationClips.Length; i++)
        {
            tempTime += tempAnim.runtimeAnimatorController.animationClips[i].length;
        }
        DOVirtual.DelayedCall(tempTime, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
    public override Stats[] GetStats(Vector2Int nodePos)
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

public struct BuffOBJ
{
    public string buffName;
    public IBuffs[] buffs;
    public int leftTick;
    public byte buffLevel;
    public Stats buffTargets;
    public void ApplyBuffs()
    {
        for (sbyte i = 0; i < buffs.Length; i++)
        {
            buffs[i].SetTarget(buffTargets);
            buffs[i].ApplyBuff();
        }
    }
    public void RemoveBuffs()
    {
        for (sbyte i = 0; i < buffs.Length; i++)
        {
            buffs[i].RemoveBuff();
        }
    }
}
public interface IBuffs
{
    float buffValue
    {
        get;
        set;
    }
    bool SetTarget(Stats aa);
    void ApplyBuff();
    void RemoveBuff();
}



public class OffensiveBuff : IBuffs
{
    public OffensiveBuff(float buffValue,WeaponApixType type)
    {
        this.buffValue = buffValue;
        this.buffType = type;
    }
    public Stats target;

    public float buffValue
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
    public bool SetTarget(Stats target)
    {
        this.target = target;
        return true;
    }

    public void ApplyBuff()
    {
        GetTargetInstance += buffValue;
    }
    public void RemoveBuff()
    {
        GetTargetInstance -= buffValue;
    }
}

public class DeffensiveBuff : IBuffs
{
    public DeffensiveBuff(float buffValue, ArmorApixType type)
    {
        this.buffValue = buffValue;
        this.buffType = type;
    }
    Stats target;
    public float buffValue
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

    public bool SetTarget(Stats target)
    {
        this.target = target;
        return true;
    }
    public void ApplyBuff()
    {
        GetTargetInstance += buffValue;
    }
    public void RemoveBuff()
    {
        GetTargetInstance -= buffValue;
    }
}
/// <summary>
/// 플레이어에게만 적용 가능
/// </summary>
public class StatBuff : IBuffs
{
    public StatBuff(float buffValue,BasicStatTypes type)
    {
        this.buffValue = buffValue;
        buffType = type;
    }
    PlayerStat target;
    public float buffValue
    {
        get;
        set;
    }
    public BasicStatTypes buffType;
    public bool SetTarget(Stats target)
    {
        if (target.GetType() != typeof(PlayerStat)) return false;
        this.target = (PlayerStat)target;
        return true;
    }
    public void ApplyBuff()
    {
        target.BasicStatus.SetChangeAbleStatus(buffType, (int)buffValue);
    }
    public void RemoveBuff()
    {
        target.BasicStatus.SetChangeAbleStatus(buffType, (int)-buffValue);
    }
}