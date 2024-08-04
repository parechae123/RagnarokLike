using DG.Tweening;
using System;
using UnityEngine;
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
public class SkillInfoInGame : SlotItem
{
    ///해당 클래스는 플레이어가 스킬 정보로 가지고있어야하는 객체로 추후 스크립터블 오브젝트 해제해야함
    public string skillName;
    public string jobName;
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
    public byte castingSkillLevel;
    public Sprite IconIMG
    {
        get { return skillIcon; }
    }
    public string slotNumberInfo
    {
        get { return castingSkillLevel.ToString(); }
    }
    public void SlotFunction(Vector3 effectPosition)
    {
        StartSkillEffect(effectPosition);
    }
    public bool isSkillLearned
    {
        get { return (nowSkillLevel > 0);}
    }

    public SkillInfoInGame(SkillInfo data)
    {
        skillName = data.skillName;
        jobName = data.jobName;
        skillType = data.skillType;
        objectiveType = data.objectiveType;
        skillPosition = data.skillPosition;
        skill = convertInGameData(data.skill);
        effectOBJPrefab = data.effectOBJPrefab;
        skillIcon = data.skillIcon;
        maxSkillLevel = data.maxSkillLevel;
    }
    private SkillBaseInGameData[] convertInGameData(SkillBase[] skills)
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
    /// <summary>
    /// 스킬 이펙트를 재생합니다.
    /// </summary>
    /// <param name="EffectPosition"></param>
    public void StartSkillEffect(Vector3 EffectPosition)
    {
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = EffectPosition;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        DOVirtual.DelayedCall(tempAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
}