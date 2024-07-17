using DG.Tweening;
using PlayerDefines.Stat;
using PlayerDefines.States;
using System;
using UnityEditor;
using UnityEngine;

namespace NeutralDefines
{
    namespace State
    {

        using PlayerDefines.States;
        using System.Collections;
        using Unity.VisualScripting;
        using UnityEngine;

        [System.Serializable]
        public class PlayerStateMachine
        {

            private PlayerStates[] allStates = new PlayerStates[0];
            public PlayerStates[] AllStates
            {
                get { return allStates; }
            }
            private PlayerStates currentState;
            public PlayerStates CurrentState
            {
                get { return currentState; }
            }
            public PlayerStateMachine(PlayerStates[] defaultStates, Animator anim)
            {
                allStates = defaultStates;
                this.anim = anim;
            }
            [SerializeField] public Animator anim;
            private Dirrections animationDirrection
            {
                get
                {
                    Vector2Int tempVecInt = Player.Instance.PlayerLookDir - PlayerCam.Instance.CameraDirrection;
                    Debug.Log(tempVecInt);
                    sbyte maxValue = (sbyte)Mathf.Max(tempVecInt.x, tempVecInt.y);
                    sbyte minValue = (sbyte)Mathf.Min(tempVecInt.x, tempVecInt.y);
                    if (maxValue == default(sbyte) && minValue == default(sbyte)) return Dirrections.N;
                    else if (maxValue == (sbyte)2 || minValue == (sbyte)-2) return Dirrections.S;
                    else
                    {
                        if (PlayerCam.Instance.CameraDirrection.x == 0)
                        {
                            if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte))) return Dirrections.W;
                            else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte))) return Dirrections.E;
                        }
                        else
                        {
                            if ((maxValue < default(sbyte) && minValue < default(sbyte)) || (maxValue > default(sbyte) && minValue > default(sbyte))) return Dirrections.E;
                            else if ((maxValue > default(sbyte) && minValue < default(sbyte)) || (maxValue < default(sbyte) && minValue < default(sbyte))) return Dirrections.W;
                        }

                    }
                    return Dirrections.E;
                }
            }
            public void SetDirrection(ref Vector2Int targetInstance,Vector3 startPos, Vector3 endPos)
            {
                Vector3 tempPos = startPos - endPos;
                if (tempPos.x != 0)
                {
                    if (tempPos.x > 0)
                    {
                        targetInstance = Vector2Int.right;
                    }
                    else if (tempPos.x < 0)
                    {
                        targetInstance = Vector2Int.left;
                    }
                }
                else
                {
                    if (tempPos.z > 0)
                    {
                        targetInstance = Vector2Int.up;
                    }
                    else if (tempPos.z < 0)
                    {
                        targetInstance = Vector2Int.down;
                    }
                }
            }
            public void SetDirrection(ref Vector2Int targetInstance,Vector2Int startPos, Vector2Int endPos)
            {
                Vector2Int tempPos = startPos - endPos;
                if (tempPos.x != 0)
                {
                    if (tempPos.x > 0)
                    {
                        targetInstance = Vector2Int.right;
                    }
                    else if (tempPos.x < 0)
                    {
                        targetInstance = Vector2Int.left;
                    }
                }
                else
                {
                    if (tempPos.y > 0)
                    {
                        targetInstance = Vector2Int.up;
                    }
                    else if (tempPos.y < 0)
                    {
                        targetInstance = Vector2Int.down;
                    }
                }
            }
            public void ChangeState(string newStateName)
            {
                if (currentState == null)
                {
                    currentState = SearchState("idleState");
                    currentState.Enter();
                    return;
                }
                currentState?.Exit();                   //이전 상태값을 빠져나간다
                currentState = SearchState(newStateName);               //인수로 받아온 상태값을 입력
                currentState?.Enter();                  //다음 상태값
                AnimationChange();
            }
            public void AnimationChange()
            {
                currentState?.SetAnimationSpeed(anim);
                anim.Play(currentState.stateName + animationDirrection);
            }

            public PlayerStates SearchState(string stateName)
            {
                sbyte i = 0;
                for (; i < allStates.Length; i++)
                {
                    if (allStates[i].stateName == stateName)
                    {
                        break;
                    }
                }
                return allStates[i];
            }
        }
        [System.Serializable]
        public class CursorStates
        {
            [SerializeField] private Texture2D defaultCursorIMG;
            [SerializeField] private Texture2D noneClickAbleIMG;
            [SerializeField] private Texture2D grabCursorIMG;
            [SerializeField] private Texture2D attackAbleCursorIMG;

            public void SetDefaultCursor()
            {
                Cursor.SetCursor(defaultCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
            }
            public cursorState CurrentCursorState
            {
                get;
                private set;
            }
            public void changeState(cursorState nextCursorState)
            {
                if (nextCursorState == CurrentCursorState) return;
                CurrentCursorState = nextCursorState;
                switch (CurrentCursorState)
                {
                    case cursorState.defaultCurser:
                        Cursor.SetCursor(defaultCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.noneClickAbleState:
                        Cursor.SetCursor(noneClickAbleIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.grabCursor:
                        Cursor.SetCursor(grabCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;
                    case cursorState.attackAble:
                        Cursor.SetCursor(attackAbleCursorIMG, Vector2.left + Vector2.up, CursorMode.Auto);
                        break;

                }
            }
        }
        public enum cursorState
        {
            defaultCurser, noneClickAbleState, grabCursor, attackAble
        }

    }
    namespace skills
    {
        [CreateAssetMenu(fileName = "new Skill",menuName = "Job/Skill")]
        [System.Serializable]

        public class SkillInfo
        {
            public string skillName;
            public SkillBase[] skill;
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
            public SkillInfo(string skillName,SkillBase[] skill,byte maxSkillLevel)
            {
                this.maxSkillLevel = maxSkillLevel;
                for (int i = 0; i < skill.Length; i++)
                {
                    if (skill[i].skillName != skillName )
                    {
                        Debug.LogError("스킬을 불러오는 중 문제가 발생하였습니다, 테이블과 입력하려는 데이터가 상이합니다.");
                    }
                    this.skill[i] = skill[i];
                }
            }
        }
        public class SkillBase : ScriptableObject
        {
            [Header("스킬 이름")]
            public string skillName;
            public string koreanSkillName;
            [Header("데미지,데미지 타입 및 계수타입,스킬타입")]
            public float defaultValue;
            public ValueType damageType;
            public float coefficient;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="caster">시전자</param>
            /// <returns></returns>
            public float totalDamage(Stats caster)
            {
                switch (damageType)
                {
                    case ValueType.Physical:
                        return defaultValue +(caster.attackDamage*coefficient);
                    case ValueType.Magic:
                        return defaultValue +(caster.abilityPower*coefficient);
                    case ValueType.heal:
                        return -1*(defaultValue + (caster.abilityPower * coefficient));
                    default:
                        return 0;
                }

            }
            public ValueType coefficientType;
            [Header("스킬 유형")]
            [HideInInspector][SerializeField] public ObjectiveType objectiveType;
            [HideInInspector][SerializeField]public SkillPosition skillPosition;
            public SkillType skillType;
            [Header("스킬 범위")]
            [HideInInspector] [SerializeField] private float skillBound;

            [Header("스킬 레벨,최대치,마나 소모값 및 캐스팅 시간")]
            public byte skillLevelInfo;
            [HideInInspector][SerializeField] public float spCost;

            [HideInInspector][SerializeField] private float defaultCastingTime;
            
            private Animator[] effectOBJs = new Animator[0];//씬 내의 이펙트 오브젝트
            [Header("스킬 이펙트 오브젝트 프리팹, 아이콘")]
            [SerializeField] private GameObject effectOBJPrefab;
            [SerializeField] private Sprite skillIcon;
            [Header("스킬 사거리")]
            [HideInInspector][SerializeField]public byte skillRange;
            [Header("스킬 지속시간")]
            [HideInInspector][SerializeField] public float skillDuration;
            public Sprite SkillIcon
            {
                get { return skillIcon; }
            }
            

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
                Array.Resize(ref effectOBJs, tempNum+1);
                effectOBJs[tempNum] = GameObject.Instantiate(effectOBJPrefab).GetComponent<Animator>();
                return effectOBJs[tempNum];
                
            } 
            public void StartSkillEffect(Vector3 EffectPosition)
            {
                Animator tempAnim = GetNonPlayingSkillEffect();
                tempAnim.transform.position = EffectPosition;
                tempAnim.gameObject.SetActive(true);
                tempAnim.Play(skillName+"Effect");
                DOVirtual.DelayedCall(tempAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length, () =>
                {
                    if(tempAnim != null)tempAnim.gameObject.SetActive(false);
                });
            }
        }
        #region 스킬 커스텀 인스펙터

#if UNITY_EDITOR
        [CustomEditor(typeof(SkillBase))]
        public class SkillBaseEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                SkillBase skill = (SkillBase)target;
                base.OnInspectorGUI();

                serializedObject.Update();
                ObjectiveType objectiveType = (ObjectiveType)serializedObject.FindProperty("objectiveType").intValue;
                SkillPosition skillPosition = (SkillPosition)serializedObject.FindProperty("skillPosition").intValue;
                SkillType skillType = (SkillType)serializedObject.FindProperty("skillType").intValue;
                switch (skillType)
                {
                    case SkillType.None:
                        skill.objectiveType = ObjectiveType.None;
                        break;
                    case SkillType.Passive:
                        skill.objectiveType = ObjectiveType.None;
                        break;
                    case SkillType.Active:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("spCost"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCastingTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillRange"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectiveType"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillPosition"));


                        break;
                    case SkillType.buff:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillDuration"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("spCost"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCastingTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillRange"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectiveType"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillPosition"));
                        break;
                    default:
                        break;
                }
                switch (objectiveType) 
                {
                    case ObjectiveType.None:
                        break;
                    case ObjectiveType.OnlyTarget:
                        
                        break;
                    case ObjectiveType.Bounded:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillBound"));
                        break;
                }
                switch (skillPosition)
                {
                    case SkillPosition.self:
                        break;
                    case SkillPosition.cursor:
                        
                        break;
                    default:
                        break;
                }


                serializedObject.ApplyModifiedProperties();
            }
        }  
#endif
        #endregion
        public enum SkillType
        {
            None,Passive,Active,buff
        }
        public enum SkillPosition
        {
            self,cursor
        }
        [Serializable]
        public enum ObjectiveType
        {
            None,OnlyTarget,Bounded
        }
        public enum ValueType
        {
            Physical, Magic, heal
        }
    }
}
