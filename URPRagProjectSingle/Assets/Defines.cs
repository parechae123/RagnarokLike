using DG.Tweening;
using System;
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
        public abstract class skillBase
        {
            public string skillName;
            public bool isSkillLearned
            {
                get;
                private set;
            }
            private float defaultCastingTime;
            private Animator[] effectOBJs = new Animator[0];
            private GameObject effectOBJPrefab;
            private Sprite skillIcon;
            public Sprite SkillIcon
            {
                get { return skillIcon; }
            }
            /// <summary>
            /// ��ų ��ũ��Ʈ ������
            /// </summary>
            /// <param name="skillName">��ų �̸��� ������� �Ͽ� Resources�������� �̸� �����ص� prefab�� �ҷ���</param>
            public skillBase(string skillName,float defaultCastingTime)
            { 
                this.skillName = skillName;
                //TODO : ���� ���̺��Ľ����� ���̺��� ������ ���� �ʿ�
                this.defaultCastingTime = defaultCastingTime;
                effectOBJPrefab = Resources.Load<GameObject>(skillName+"SkillPrefab");
                Array.Resize(ref effectOBJs, 1);
                effectOBJs[0] = GameObject.Instantiate(effectOBJPrefab).GetComponent<Animator>();
                effectOBJs[0].gameObject.SetActive(false);

                skillIcon = skillIcon = Resources.Load<Sprite>(skillName + "Skill_Icon");
            }
            
            public void LearnSkill()
            {
                isSkillLearned = true;
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
    }
}
