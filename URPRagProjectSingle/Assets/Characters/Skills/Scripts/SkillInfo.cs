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
        if (!isSkillLearned) return;
        quickSlotFuncs?.Invoke();
    }
    public void SetSkillObjectToPlayer()
    {
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
    public void SkillCastTargetPlace(Vector3 castingPos,Stats target,Stats caster)
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
                if (target != null) target.HP -= skill[CastingSkillLevel].TotalDamage(caster);
                break;
            case ObjectiveType.Bounded:
                Stats[] tempTargets = GetStats(new Vector2Int((int)castingPos.x, (int)castingPos.z));
                for (int i = 0; i < tempTargets.Length; i++)
                {
                    if (tempTargets[i] == caster) continue;
                    tempTargets[i].HP -= skill[CastingSkillLevel].TotalDamage(caster);
                }
                break;
        }
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