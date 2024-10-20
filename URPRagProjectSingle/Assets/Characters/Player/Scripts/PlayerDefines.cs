using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
namespace PlayerDefines
{
    namespace States
    {
        using PlayerDefines.Stat;
        public interface IState
        {
            void Enter();
            void Execute();
            void Exit();
        }
        [System.Serializable]
        public class PlayerStates : IState
        {
            public string stateName;
            public string nextStateName;
            //state 도중 끊을 수 없는지 가능시 false 불가능시 true
            public bool isCancelableState;
            //TODO : 방향 지정필요

            protected float skillCoolTime;
            protected float skillTimer;
            public float GetTimer { get { return skillTimer; } }
            public float durationTime;
            /// <summary>
            /// 스테이트 생성자
            /// </summary>
            /// <param name="keyCode">입력 트리거 키</param>
            /// <param name="coolTime">해당 행동의 쿨타임</param>
            /// <param name="targetStateName">스테이트 이름</param>
            /// <param name="isCancelableState">상태 중 다른 상태를 받을 것인지</param>
            public PlayerStates(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState)
            {
                skillCoolTime = coolTime;
                skillTimer = coolTime;
                this.stateName = targetStateName;
                this.nextStateName = nextStateName;
                this.isCancelableState = isCancelableState;
                this.durationTime = durationTime;
            }

            public virtual void SetAnimationSpeed(Animator anim)
            {
                anim.speed = 1;
            }
            public virtual void Enter()
            {
                skillTimer = 0;
            }
            public virtual void Execute()
            {
                skillTimer += Time.deltaTime;
                if (skillCoolTime < skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }
            }

            public virtual void Exit()
            {
                skillTimer = skillCoolTime;
                
            }


        }




        public class MoveState : PlayerStates
        {
            public MoveState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void Enter()
            {
                base.Enter();
                durationTime = Player.Instance.arriveTime;
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                if (durationTime <= skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }

            }
            public override void Exit()
            {
                base.Exit();
            }
        }
        public class DamagedState : PlayerStates
        {
            public DamagedState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public void SetDurationTime(float time)
            {
                durationTime = time;
            }
            public override void Enter()
            {
                base.Enter();
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                if (durationTime <= skillTimer)
                {
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                }

            }
            public override void Exit()
            {
                base.Exit();
            }
        }




        public class AttackState : PlayerStates
        {
            //해당 스테이트를 가지고있는 캐릭터의 스텟
            public AttackState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState, Stats characterStat) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                
            }
            public override void SetAnimationSpeed(Animator anim)
            {
                // TODO : 중간메모
                anim.speed = 1f/Player.Instance.playerLevelInfo.stat.TotalAttackSpeed;
            }
            public override void Enter()
            {
                durationTime = Player.Instance.playerLevelInfo.stat.TotalAttackSpeed;
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                

                if (Player.Instance.playerLevelInfo.stat.TotalAttackSpeed < skillTimer)
                {

                    if (Player.Instance.playerLevelInfo.stat.target.isCharacterDie|| Player.Instance.isMotionBookCancel)
                    {
                        Player.Instance.StateMachine.ChangeState(nextStateName);
                        Player.Instance.isMotionBookCancel = false;
                    }
                    else
                    {
                        Player.Instance.playerLevelInfo.stat.AttackTarget();
                    }
                    if (Player.Instance.playerLevelInfo.stat.target.isCharacterDie) Player.Instance.StateMachine.ChangeState(nextStateName);
                    skillTimer = 0;
                }

            }
            public override void Exit()
            {
                skillTimer = 0;
            }
        }
        public class CastingState : PlayerStates
        {
            public Action<Vector3,Stats,Stats> casting;
            public Vector3 castPos;
            public Stats targetStat;
            public CastingState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                
            }
            public override void SetAnimationSpeed(Animator anim)
            {
                anim.speed = 1+(1* Player.Instance.playerLevelInfo.stat.CastTimePercent);
            }
            public override void Enter()
            {
                skillTimer = 0;
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                UIManager.GetInstance().SetCastingBarValue(durationTime, skillTimer);
                if (durationTime <= skillTimer)
                {
                    UIManager.GetInstance().CastingBarOnOff(false);
                    casting.Invoke(castPos,targetStat,Player.Instance.playerLevelInfo.stat);
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                    skillTimer = 0;
                }

            }
            public override void Exit()
            {
                UIManager.GetInstance().CastingBarOnOff(false);
                skillTimer = 0;
            }
        }
        public class IdleState : PlayerStates
        {
            public IdleState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {

            }
            public override void Enter()
            {
                base.Enter();
            }
            public override void Execute()
            {

                base.Execute();


            }
            public override void Exit()
            {
                skillTimer = 0;
            }
        }
    }
    namespace Stat
    {
        [System.Serializable]
        public class Stats
        {

            public bool isCharacterDie
            {
                get { return HP <= 0; }
            }
            public bool isCharacterDamaged
            {
                get { return maxHP > hp; }
            }
            public Action<Vector3, bool> moveFunction;
            public Action dieFunctions;//TODO : 사망 연출 등록필요

            
            public Stats(Node initializeNode, float hp,float sp, float moveSpeed, float attackSpeed, float attackDamage,byte attackRange,float evasion)
            {
                standingNode = initializeNode;
                standingNode.CharacterOnNode = this;
                maxHP = hp;
                maxSP = sp;
                this.HP = hp;
                this.sp = sp;
                this.charactorAttackRange = attackRange;
                this.moveSpeed = moveSpeed;
                this.attackSpeed = attackSpeed;
                this.attackDamage = attackDamage;
                defaultEvasion = evasion;
            }
            public Node standingNode
            {
                get;
                set;
            }
            protected float defaultEvasion;
            public virtual float Evasion
            {
                get { return defaultEvasion; }
            }

            protected float maxHP;
            protected float hp;
            public virtual float HP
            {
                get
                {
                    return hp;
                }
                set
                {
                    Debug.Log((hp- value) +"몬스터 데미지");
                    hp = value;
                    if(isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                    }
                }
            }
            protected float maxSP;
            protected float sp;
            protected virtual float SP
            {
                get { return sp; }
                set 
                { 
                    if(value>maxSP) sp = maxSP;
                    else sp = value;

                }
            }
            private float moveSpeed;
            public float MoveSpeed 
            {
                get { return Player.Instance.playerLevelInfo.stat.moveSpeed; }
                set { }
            } //초당 이동하는 타일 수

            public float attackDamage;//공격력
            public float abilityPower;
            public float accuracy = 50; //명중률

            public float attackSpeed;
            public float basicAttackTimer;
            private byte charactorAttackRange;
            public int CharactorAttackRange
            {
                get { return charactorAttackRange*10; }
            }

            

            public Stats target;
            public virtual void AttackTarget(Stats target)
            {
                this.target = target;
                if (target == null) return;

                //타겟 회피율 계산
                if (accuracy < target.Evasion)
                {
                    if (UnityEngine.Random.Range(1, 101) >  target.Evasion- accuracy)
                    {
                        //TODO : MISS! DamageText 추가요망
                        return;
                    }
                }
                target.HP -= attackDamage;
            }

            public virtual bool IsEnoughSP(float spCost)
            {
                if (sp >= spCost && !isCharacterDie)
                {
                    sp -= spCost;
                    return true;
                }
                return false;
            }
        }

        public class PlayerStat : Stats
        {
            //직업관련
            public BaseJobType jobType;
            public JobPhase jobPhase;
            public JobRoot jobRoot;
            

            private BasicStatus basicStatus;
            public BasicStatus BasicStatus
            {
                get
                {
                    if (basicStatus == null)
                    {
                        basicStatus = new BasicStatus();
                    }
                    return basicStatus;
                }
            }



            #region 장비 관련
            private Weapons[] weapons = new Weapons[2] {new Weapons(EquipPart.LeftHand), new Weapons(EquipPart.RightHand) };
            private Armors[] armors = new Armors[5] { new Armors(EquipPart.Head), new Armors(EquipPart.Chest),new Armors(EquipPart.Pants),new Armors(EquipPart.Boots), new Armors(EquipPart.Gauntlet) };
            public Armors GetArmorSlot
            {
                set 
                {
                    bool successChange = false;
                    switch (value.GetPart)
                    {
                        case EquipPart.Head:
                            armors[0].Amount = 1;
                            armors[0].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[0]);
                            BasicStatus.SetChangeAbleStatus(armors[0].apixList.firstLine.Item1, -armors[0].apixList.firstLine.Item2);
                            armors[0] = value;
                            UIManager.GetInstance().equipWindowArmors[0].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Chest:
                            armors[1].Amount = 1;
                            armors[1].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[1]);
                            BasicStatus.SetChangeAbleStatus(armors[1].apixList.firstLine.Item1, -armors[1].apixList.firstLine.Item2);
                            armors[1] = value;
                            UIManager.GetInstance().equipWindowArmors[1].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Pants:
                            armors[2].Amount = 1;
                            armors[2].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[2]);
                            BasicStatus.SetChangeAbleStatus(armors[2].apixList.firstLine.Item1, -armors[2].apixList.firstLine.Item2);
                            armors[2] = value;
                            UIManager.GetInstance().equipWindowArmors[2].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Boots:
                            armors[3].Amount = 1;
                            armors[3].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[3]);
                            BasicStatus.SetChangeAbleStatus(armors[3].apixList.firstLine.Item1, -armors[3].apixList.firstLine.Item2);
                            armors[3] = value;
                            UIManager.GetInstance().equipWindowArmors[3].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Gauntlet:
                            armors[4].Amount = 1;
                            armors[4].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[4]);
                            BasicStatus.SetChangeAbleStatus(armors[4].apixList.firstLine.Item1, -armors[4].apixList.firstLine.Item2);
                            armors[4] = value;
                            UIManager.GetInstance().equipWindowArmors[4].SlotItem = value;
                            successChange = true;
                            break;
                    }
                    if (successChange)
                    {
                        UIManager.GetInstance().equipInven.RemoveItem(value);
                        BasicStatus.SetChangeAbleStatus(value.apixList.firstLine.Item1, value.apixList.firstLine.Item2);
                    }
                }
            }
            public Weapons GetWeaponSlot
            {
                set
                {
                    bool successChange = false;

                    switch (value.GetPart)
                    {
                        case EquipPart.LeftHand:
                            weapons[0].isEquiped = false;
                            weapons[0].Amount = 1;
                            UIManager.GetInstance().equipInven.GetItems(weapons[0]);
                            BasicStatus.SetChangeAbleStatus(weapons[0].apixList.firstLine.Item1, -weapons[0].apixList.firstLine.Item2);
                            weapons[0] = value;
                            UIManager.GetInstance().equipWindowWeapons[0].SlotItem = value;
                            UIManager.GetInstance().equipInven.RemoveItem(value);
                            successChange = true;
                            break;
                        case EquipPart.RightHand:
                            weapons[1].isEquiped = false;
                            weapons[1].Amount = 1;
                            UIManager.GetInstance().equipInven.GetItems(weapons[1]);
                            BasicStatus.SetChangeAbleStatus(weapons[1].apixList.firstLine.Item1, -weapons[1].apixList.firstLine.Item2);
                            weapons[1] = value;
                            UIManager.GetInstance().equipWindowWeapons[1].SlotItem = value;
                            UIManager.GetInstance().equipInven.RemoveItem(value);
                            successChange = true;
                            if (weapons[0].GetPart == EquipPart.TwoHanded)
                            {
                                weapons[0].Amount = 1;
                                weapons[0].isEquiped = false;
                                UIManager.GetInstance().equipInven.GetItems(weapons[0]);
                                BasicStatus.SetChangeAbleStatus(weapons[0].apixList.firstLine.Item1, -weapons[0].apixList.firstLine.Item2);
                                weapons[0] = new Weapons(EquipPart.LeftHand);
                                UIManager.GetInstance().equipWindowWeapons[0].SlotItem = null;

                            }
                            break;
                        case EquipPart.TwoHanded:
                            weapons[0].isEquiped = false;
                            weapons[0].Amount = 1;
                            UIManager.GetInstance().equipInven.GetItems(weapons[0]);
                            BasicStatus.SetChangeAbleStatus(weapons[0].apixList.firstLine.Item1, -weapons[0].apixList.firstLine.Item2);
                            weapons[0] = value;
                            UIManager.GetInstance().equipWindowWeapons[0].SlotItem = value;
                            UIManager.GetInstance().equipInven.RemoveItem(value);
                            successChange = true;
                            if (weapons[1].isEquiped)
                            {
                                weapons[1].isEquiped = false;
                                weapons[1].Amount = 1;
                                UIManager.GetInstance().equipInven.GetItems(weapons[1]);
                                weapons[1].isEquiped = false;
                                BasicStatus.SetChangeAbleStatus(weapons[1].apixList.firstLine.Item1, -weapons[1].apixList.firstLine.Item2);
                                weapons[1] = new Weapons(EquipPart.RightHand);
                                UIManager.GetInstance().equipWindowWeapons[1].SlotItem = null;
                            }
                            break;
                    }
                    if (successChange)
                    {
                        UIManager.GetInstance().equipInven.RemoveItem(value);
                        BasicStatus.SetChangeAbleStatus(value.apixList.firstLine.Item1, value.apixList.firstLine.Item2);
                    }
                }
            }
            #endregion

            public PlayerStat(Node initializeNode, float hp,float sp, float moveSpeed, float attackSpeed, float attackDamage,byte attackRange,float evasion) : base(initializeNode, hp,sp, moveSpeed, attackSpeed, attackDamage,attackRange,evasion)
            {
                HP = hp;
                SP = sp;
            }
            //어택데미지
            public float TotalAD
            {
                get { return attackDamage + (BasicStatus.Strength * 3) + (BasicStatus.Luck * 0.6f) + GetWeaponValue(false)+GetWeaponApixValue(WeaponApixType.ATK); ; }
            }
            public float TotalAP
            {
                //현재 AP와 int값에만 영향받음, 추후 장비영향 추가하여야함
                get { return abilityPower + (BasicStatus.Inteligence*2) + (BasicStatus.Luck * 0.4f) + GetWeaponValue(true) + GetWeaponApixValue(WeaponApixType.MATK); }
                set { if(value>=0) abilityPower = value; }
            }
            //적중률
            public int TotalAccuracy
            {
                get { return (int)(accuracy + (BasicStatus.Dexterity * 1.5f) + (BasicStatus.Luck * 0.3f) + GetWeaponApixValue(WeaponApixType.Accuracy));}
            }

            //공격속도
            public float TotalAttackSpeed
            {
                get 
                {
                    float tempAS = attackSpeed - (BasicStatus.Agility * 0.06f)- GetWeaponApixValue(WeaponApixType.AttackSpeed);
                    if (tempAS < 0.1f) return 0.1f;
                    return tempAS;
                }
            }
            //캐스팅 타임 배율
            public float CastTimePercent
            {
                get 
                {
                    float tempCT = (1 - (BasicStatus.Dexterity * 0.006f)) - (BasicStatus.Inteligence * 0.003f)- GetWeaponApixValue(WeaponApixType.CastingSpeed)-GetArmorMatValue(WeaponApixType.CastingSpeed);
                    return tempCT <0? 0:tempCT;
                }
            }
            //글로벌 쿨타임 배율
            public float GlobalCooltimePercent
            {
                get 
                {
                    float tempGC = (1 - (BasicStatus.Agility * 0.006f));
                    return tempGC < 0? 0: tempGC; 
                }
            }
            public override float Evasion
            {
                get
                {
                    return defaultEvasion + ((BasicStatus.Agility / 3) * 2) + (BasicStatus.Luck  * 0.4f)+GetArmorApixValue(ArmorApixType.Evasion);
                }
            }

            public int CriChance
            {
                get { return (int)(BasicStatus.Luck * 0.8f) + (int)GetWeaponApixValue(WeaponApixType.CriticalChance); } 
            }
            public float defaultCriDamage = 2;
            public float CriDamage
            {
                
                get { return defaultCriDamage+ GetArmorMatValue(WeaponApixType.CriticalDMG); }
            }

            public override float HP
            {
                get
                {
                    return base.hp;
                }
                set
                {
                    if(maxHP< value) base.hp = value;
                    else base.hp = value;
                    
                    if (isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                    }
                    UIManager.GetInstance().PlayerMaxCurrHP = (maxHP, base.hp);
                }
            }
            protected override float SP
            {
                get { return base.sp; }
                set 
                {
                    if (value > maxSP) sp = base.maxSP;
                    else sp = value;
                    UIManager.GetInstance().PlayerMaxCurrSP = (base.maxSP, base.sp);
                }
            }
            public override bool IsEnoughSP(float spCost)
            {
                if (SP >= spCost && !isCharacterDie)
                {
                    SP -= spCost;
                    return true;
                }
                return false;
            }
            public override void AttackTarget(Stats target = null)
            {
                if (target != null) { this.target = target; }
                if (this.target == null) return;
                if(TotalAccuracy < this.target.Evasion)
                {
                    if (UnityEngine.Random.Range(1, 101) > this.target.Evasion - TotalAccuracy)
                    {
                        //TODO : MISS! DamageText 추가요망
                        return;
                    }
                }


                if (UnityEngine.Random.Range(1, 101) <= CriChance) 
                {
                    this.target.HP -= (TotalAD*CriDamage);
                    //TODO : 크리티컬 전용 데미지 텍스트 추가요망
                }
                else
                {
                    this.target.HP -= TotalAD;
                    //TODO : 데미지 텍스트 추가요망
                }

            }

            public float GetWeaponApixValue(WeaponApixType targetType)
            {
                float temp = 0f;
                for (byte i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i].apixList.abilityApixes == null) continue;
                    for (byte j = 0; j < weapons[i].apixList.abilityApixes.Length; j++)
                    {
                        if (weapons[i].apixList.abilityApixes[i].Item1 == targetType) temp += weapons[i].apixList.abilityApixes[i].Item2;
                    }
                }
                return temp;
            }
            public float GetWeaponValue(bool callOnMATK)
            {
                float temp = 0f;
                for (byte i = 0; i < weapons.Length; i++)
                {
                    if (callOnMATK && weapons[i].IsMATKWeapon) temp += weapons[i].ValueOne;
                    else if (!callOnMATK && !weapons[i].IsMATKWeapon) temp += weapons[i].ValueOne;

                }
                return temp;
            }
            /// <summary>
            /// TODO : 방어도 미구현으로 추후 사용 필요
            /// </summary>
            /// <returns></returns>
            public float GetArmorValue()
            {
                float temp = 0f;
                for (byte i = 0; i < armors.Length; i++)
                {
                    temp += armors[i].ValueOne;
                }
                return temp;
            }
            public float GetArmorMatValue(WeaponApixType targetType)
            {
                float temp = 0f;
                for (byte i = 0; i < armors.Length; i++)
                {
                    if (armors[i].GetValueType == targetType) temp += armors[i].GetMatValue;
                }
                return temp;
            }
            public float GetArmorApixValue(ArmorApixType targetType)
            {
                float temp = 0f;
                for (byte i = 0; i < armors.Length; i++)
                {
                    if (armors[i].apixList.abilityApixes == null) continue;
                    for (byte j = 0; j < armors[i].apixList.abilityApixes.Length; j++)
                    {
                        if (armors[i].apixList.abilityApixes[i].Item1 == targetType) temp += armors[i].apixList.abilityApixes[i].Item2;
                    }
                }
                return temp;
            }
        }
        public class MonsterStat : Stats
        {
            public MonsterStat(Node initializeNode, float hp, float sp, float moveSpeed, float attackSpeed, float attackDamage, byte attackRange, float evasion) : base(initializeNode, hp, sp, moveSpeed, attackSpeed, attackDamage, attackRange,evasion)
            {
                HP = hp;
                SP = sp;
            }

            private Slider hpBar;
            public Slider HPBar
            {
                get
                {
                    return hpBar;
                }
                set
                {
                    if (value == null)
                    {
                        if (hpBar != null)
                        {
                            UIManager.GetInstance().HPBarEnqueue(hpBar);
                        }
                    }
                    hpBar = value;
                }
            }
            public override float HP
            {
                get
                {
                    return base.hp;
                }
                set
                {
                    if (maxHP <= value) base.hp = maxHP;
                    else base.hp = value;

                    if (isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                        return;
                    }
                    if (HPBar == null) HPBar = UIManager.GetInstance().HPBarDequeue();
                    HPBar.value = hp;
                    HPBar.maxValue = maxHP;
                }
            }
            protected override float SP
            {
                get { return base.sp; }
                set
                {
                    if (value > maxSP) sp = base.maxSP;
                    else sp = value;
                }
            }
        }
    }
    public class PlayerUI
    {
        private Canvas mainCanvas;
        public RectTransform skillTreeUI;
        public RectTransform statusUI;
        public RectTransform equipUI;
        public RectTransform inventoryUI;
        public RectTransform escUI;
        public RectTransform quickSlotUI;
        public Stack<RectTransform> uiStack;
        public PlayerUI() 
        {
            ResetUI();
        }
        public void ResetUI()
        {
            ResourceManager.GetInstance().LoadAsync<GameObject>("InGameCanvas", (UIs) =>
            {
                uiStack = uiStack ?? new Stack<RectTransform>();
                uiStack.Clear();
                if (mainCanvas != null) GameObject.Destroy(mainCanvas);

                mainCanvas = GameObject.Instantiate(UIs).GetComponent<Canvas>();

                skillTreeUI = mainCanvas.transform.Find("skillTreeUI").transform as RectTransform;
                skillTreeUI.gameObject.SetActive(false);

                statusUI = mainCanvas.transform.Find("statusUI").transform as RectTransform;
                statusUI.gameObject.SetActive(false);

                equipUI = mainCanvas.transform.Find("equipUI").transform as RectTransform;
                equipUI.gameObject.SetActive(false);

                inventoryUI = mainCanvas.transform.Find("inventoryUI").transform as RectTransform;
                inventoryUI.gameObject.SetActive(false);

                escUI = mainCanvas.transform.Find("escUI").transform as RectTransform;
                escUI.gameObject.SetActive(false);

                quickSlotUI = mainCanvas.transform.Find("quickSlotUI").transform as RectTransform;
                quickSlotUI.gameObject.SetActive(false);
            });
        }
        /// <summary>
        /// 마지막으로 켠 UI를 ActiveFalse해주고 uiStack에서 제거
        /// </summary>
        /// <param name="targetUI">해당 매개변수 등록 시 대상 UI만 ActiveFalse해줌</param>
        public void PopUI(RectTransform targetUI = null)
        {
            if (uiStack.Count <= 0) return;

            if (targetUI != null)
            {
                if (uiStack.Contains(targetUI))
                {
                    Stack<RectTransform> tempStack = new Stack<RectTransform>();
                    RectTransform tempRect;
                    for (int i = 0; i < uiStack.Count; i++)
                    {
                        tempRect = tempStack.Pop();
                        if (tempRect != targetUI)
                        {
                            tempStack.Push(tempRect);
                        }
                        else
                        {
                            targetUI.gameObject.SetActive(false);
                        }
                    }
                }

            }
            else
            {
                uiStack.Pop().gameObject.SetActive(false);
            }


        }
        /// <summary>
        /// uiStack인스턴스에 타겟 UI의 RectTransform을 등록,gameobject의 active를 true로 바꿔줌
        /// </summary>
        /// <param name="targetUI">활성화 시킬 UI</param>
        public void PushUI(RectTransform targetUI)
        {
            uiStack.Push(targetUI);
            targetUI.gameObject.SetActive(true);
        }
    }
}

