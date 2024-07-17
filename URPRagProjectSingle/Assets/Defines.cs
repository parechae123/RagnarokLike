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
                currentState?.Exit();                   //���� ���°��� ����������
                currentState = SearchState(newStateName);               //�μ��� �޾ƿ� ���°��� �Է�
                currentState?.Enter();                  //���� ���°�
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
            /// ��ų �ҷ�����
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
                        Debug.LogError("��ų�� �ҷ����� �� ������ �߻��Ͽ����ϴ�, ���̺�� �Է��Ϸ��� �����Ͱ� �����մϴ�.");
                    }
                    this.skill[i] = skill[i];
                }
            }
        }
        public class SkillBase : ScriptableObject
        {
            [Header("��ų �̸�")]
            public string skillName;
            public string koreanSkillName;
            [Header("������,������ Ÿ�� �� ���Ÿ��,��ųŸ��")]
            public float defaultValue;
            public ValueType damageType;
            public float coefficient;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="caster">������</param>
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
            [Header("��ų ����")]
            [HideInInspector][SerializeField] public ObjectiveType objectiveType;
            [HideInInspector][SerializeField]public SkillPosition skillPosition;
            public SkillType skillType;
            [Header("��ų ����")]
            [HideInInspector] [SerializeField] private float skillBound;

            [Header("��ų ����,�ִ�ġ,���� �Ҹ� �� ĳ���� �ð�")]
            public byte skillLevelInfo;
            [HideInInspector][SerializeField] public float spCost;

            [HideInInspector][SerializeField] private float defaultCastingTime;
            
            private Animator[] effectOBJs = new Animator[0];//�� ���� ����Ʈ ������Ʈ
            [Header("��ų ����Ʈ ������Ʈ ������, ������")]
            [SerializeField] private GameObject effectOBJPrefab;
            [SerializeField] private Sprite skillIcon;
            [Header("��ų ��Ÿ�")]
            [HideInInspector][SerializeField]public byte skillRange;
            [Header("��ų ���ӽð�")]
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
        #region ��ų Ŀ���� �ν�����

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
