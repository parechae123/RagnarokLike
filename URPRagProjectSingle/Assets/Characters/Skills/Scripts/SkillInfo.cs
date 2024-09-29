using DG.Tweening;
using JetBrains.Annotations;
using System;
using UnityEditor.Build.Pipeline;
using UnityEngine;
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
public class SkillInfoInGame : ItemBase
{
    ///�ش� Ŭ������ �÷��̾ ��ų ������ �������־���ϴ� ��ü�� ���� ��ũ���ͺ� ������Ʈ �����ؾ���
    public event Action<Vector3> quickSlotFuncs;
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
    public byte castingSkillLevel;
    public Sprite IconIMG
    {
        get { return skillIcon; }
    }
    public string slotNumberInfo
    {
        get { return castingSkillLevel.ToString(); }
    }
    public bool isSkillLearned
    {
        get { return (nowSkillLevel > 0);}
    }
    public bool isItemUseAble
    {
        get { return isSkillLearned; }
    }

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
        quickSlotFuncs += StartSkillEffect;
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
        quickSlotFuncs?.Invoke(Vector3.zero);
    }
    /// <summary>
    /// ��ų ����Ʈ�� ����մϴ�.
    /// </summary>
    /// <param name="EffectPosition"></param>
    public void StartSkillEffect(Vector3 EffectPosition)
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
                SkillCastInPlace();
                break;
        }
    }
    public void SkillCastInPlace()
    {
        Debug.Log(skillName + "����߾��~~");
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = Player.Instance.transform.position;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        float tempTime = 0;
        
        for (int i = 0; i< tempAnim.runtimeAnimatorController.animationClips.Length; i++)
        {
            tempTime += tempAnim.runtimeAnimatorController.animationClips[i].length;
        }
        DOVirtual.DelayedCall(tempTime, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
    public void SkillCastTargetPlace(Vector3 targetPos)
    {
        Debug.Log(skillName + "����߾��~~");
        Animator tempAnim = GetNonPlayingSkillEffect();
        tempAnim.transform.position = targetPos;
        tempAnim.gameObject.SetActive(true);
        tempAnim.Play(skillName + "Effect");
        float tempTime = 0;

        for (int i = 0; i < tempAnim.runtimeAnimatorController.animationClips.Length; i++)
        {
            tempTime += tempAnim.runtimeAnimatorController.animationClips[i].length;
        }
        DOVirtual.DelayedCall(tempTime, () =>
        {
            if (tempAnim != null) tempAnim.gameObject.SetActive(false);
        });
    }
}