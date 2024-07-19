using DG.Tweening;
using System;
using UnityEngine;
[CreateAssetMenu(fileName = "new SkillInfo", menuName = "Skill/SkillInfomations")]
[System.Serializable]
public class SkillInfo/* : ScriptableObject*/
{
    ///해당 클래스는 플레이어가 스킬 정보로 가지고있어야하는 객체로 추후 스크립터블 오브젝트 해제해야함
    public string skillName;
    [Header("스킬 유형")]
    [SerializeField] public ObjectiveType objectiveType;
    public SkillType skillType;
    [SerializeField] public SkillPosition skillPosition;
    public SkillBase[] skill;
    [SerializeField] private Sprite skillIcon;
    [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
    private Animator[] effectOBJs = new Animator[0];//씬 내의 이펙트 오브젝트
    [SerializeField] private GameObject effectOBJPrefab;
    public byte maxSkillLevel;
    public byte nowSkillLeve;
    public byte castingSkillLevel;
    public bool isSkillLearned
    {
        get;
        private set;
    }

    /// <summary>
    /// 스킬 불러오기
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="skill"></param>
    /// <param name="maxSkillLevel"></param>
/*    public SkillInfo(string skillName, SkillBase[] skill, byte maxSkillLevel)
    {
        this.maxSkillLevel = maxSkillLevel;
        for (int i = 0; i < skill.Length; i++)
        {
            if (skill[i].skillName != skillName)
            {
                Debug.LogError("스킬을 불러오는 중 문제가 발생하였습니다, 테이블과 입력하려는 데이터가 상이합니다.");
            }
            this.skill[i] = skill[i];
        }
    }*/


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