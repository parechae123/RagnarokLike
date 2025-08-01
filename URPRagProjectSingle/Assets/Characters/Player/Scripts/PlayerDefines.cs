using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static SkillManager;
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
        public class DieState : PlayerStates
        {
            public DieState(float coolTime, float durationTime, string targetStateName, string nextStateName, bool isCancelableState) : base(coolTime, durationTime, targetStateName, nextStateName, isCancelableState)
            {
                isCancelableState = false;
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
                skillTimer = 0;
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;

                if (Player.Instance.playerLevelInfo.stat.target.isCharacterDie || Player.Instance.isMotionBookCancel)
                {
                    Player.Instance.isMotionBookCancel = true;
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                    Player.Instance.isMotionBookCancel = false;
                    skillTimer = 0;
                    return;
                }
                if (Player.Instance.playerLevelInfo.stat.TotalAttackSpeed <= skillTimer)
                {
                    if (Player.Instance.playerLevelInfo.stat.target == null)
                    {
                        Player.Instance.StateMachine.ChangeState(nextStateName);
                        skillTimer = 0;
                        return;
                    }

                    if (Player.Instance.playerLevelInfo.stat.target.isCharacterDie|| Player.Instance.isMotionBookCancel)
                    {
                        Player.Instance.isMotionBookCancel = true;
                        Player.Instance.StateMachine.ChangeState(nextStateName);
                        Player.Instance.isMotionBookCancel = false;
                        skillTimer = 0;
                        return;
                    }
                    else
                    {
                        Player.Instance.playerLevelInfo.stat.AttackTarget();
                    }
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
                Player.Instance.transform.DOKill(false);
                Player.Instance.skillIndicator.OnOff(true, false);
            }
            public override void Execute()
            {
                skillTimer += Time.deltaTime;
                UIManager.GetInstance().SetCastingBarValue(durationTime, skillTimer);
                if (durationTime <= skillTimer)
                {
                    UIManager.GetInstance().CastingBarOnOff(false);
                    Player.Instance.StateMachine.ChangeState(nextStateName);
                    casting.Invoke(castPos,targetStat,Player.Instance.playerLevelInfo.stat);
                    skillTimer = 0;
                }

            }
            public override void Exit()
            {
                UIManager.GetInstance().CastingBarOnOff(false);
                Player.Instance.skillIndicator.OnOff(false,false);
                Player.Instance.skillIndicator.OnOff(true,false);
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
                get { return defaultMaxHP >= 0 ? HP <= 0 : false; }
            }
            public virtual bool isCharacterDamaged
            {
                get { return defaultMaxHP > hp; }
            }
            public Action<Vector3, Action, int> moveFunction;
            public Action dieFunctions;//TODO : 사망 연출 등록필요


            public Stats(Node initializeNode, float hp, float sp, float moveSpeed, float attackSpeed, float attackDamage, byte attackRange, float evasion)
            {
                standingNode = initializeNode;
                standingNode.CharacterOnNode = this;
                defaultMaxHP = hp;
                defaultSP = sp;
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
            #region buffList
            protected List<buffTime> buffs = new List<buffTime>();

            protected HashSet<string> buffHash = new HashSet<string>();
            protected virtual void HashAction(string name)
            {
                buffHash.Remove(name);
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (buffs[i].buffName == name)
                    {
                        buffs.RemoveAt(i);
                        return;
                    }
                }
            }
            public virtual void AcceptBuff(BuffOBJ target)
            {
                if (buffHash.Contains(target.buffName))
                {
                    if (SearchBuff(target.buffLevel, target.buffName))
                    {
                        target.ApplyBuffs();
                        Action<string> nameAction = new Action<string>(HashAction);
                        Action removeAction = new Action(target.RemoveBuffs);

                        buffTime tempTimer = new buffTime(target.buffName, target.leftTick, target.buffLevel, removeAction, nameAction, false);
                        SkillManager.GetInstance().RegistBuffTimer(tempTimer);
                        buffs.Add(tempTimer);
                        buffHash.Add(target.buffName);

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    target.ApplyBuffs();
                    Action<string> nameAction = new Action<string>(HashAction);
                    Action removeAction = new Action(target.RemoveBuffs);

                    buffTime tempTimer = new buffTime(target.buffName, target.leftTick, target.buffLevel, removeAction, nameAction, false);
                    SkillManager.GetInstance().RegistBuffTimer(tempTimer);
                    buffs.Add(tempTimer);
                    buffHash.Add(target.buffName);
                }
            }
            protected bool SearchBuff(byte buffLevel, string buffName)
            {
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (buffs[i].buffName == buffName)
                    {
                        if (buffs[i].buffLevel > buffLevel)
                        {
                            return false;
                        }
                        else
                        {
                            buffs[i].End();
                            return true;
                        }
                    }
                    else continue;
                }
                return true;
            }
            #endregion
            public virtual float TotalAD
            {
                get { return attackDamage; }
            }
            public virtual float TotalAP
            {
                //현재 AP와 int값에만 영향받음, 추후 장비영향 추가하여야함
                get { return abilityPower; }
                set { if (value >= 0) abilityPower = value; }
            }
            //적중률
            public virtual float TotalAccuracy
            {
                get { return accuracy; }
            }

            public float defaultCasting = 1;
            public float defaultEvasion;
            public virtual float Evasion
            {
                get { return defaultEvasion; }
            }

            public float defaultMaxHP;
            protected float hp;
            public virtual float HP
            {
                get
                {
                    return hp;
                }
                set
                {
                    Debug.Log((hp - value) + "몬스터 데미지");
                    if (isCharacterDie) return;

                    hp = value;
                    if (isCharacterDie)
                    {
                        hp = 0;
                        dieFunctions?.Invoke();
                    }
                }
            }
            public float defaultSP;
            protected float sp;
            public virtual float SP
            {
                get { return sp; }
                set
                {
                    if (value > defaultSP) sp = defaultSP;
                    else sp = value;

                }
            }
            public float moveSpeed;
            public virtual float MoveSpeed
            {
                get { return moveSpeed; }
                set { moveSpeed = value; }
            } //초당 이동하는 타일 수

            public float attackDamage;//공격력
            public float abilityPower;
            public float accuracy = 50; //명중률

            public float deff;
            public virtual float Deff
            {
                get
                {
                    return deff < 0 ? 0 : deff;
                }
            }
            public float magicDeff;
            public virtual float MagicDeff
            {
                get
                {
                    return magicDeff < 0 ? 0 : magicDeff;
                }
            }

            public float attackSpeed;
            public float statTimer;
            [SerializeField] private byte charactorAttackRange;
            public byte pureAttackRange
            {
                get { return charactorAttackRange; }
            }
            public int CharactorAttackRange
            {
                get { return charactorAttackRange * 10; }
            }


            public Stats target;
            //방어력계산
            public virtual float GetDamage(float value, ValueType valueType)
            {
                switch (valueType)
                {
                    case ValueType.Physical:
                        value -= value * (Deff * 0.001f);
                        break;
                    case ValueType.Magic:
                        value -= value * (MagicDeff * 0.001f);
                        break;
                    case ValueType.Heal:
                        //TODO : 받는 치유량 apix가 생기면 여기에
                        if (value > 0) value = -value;
                        break;
                    case ValueType.PhysicalRange:
                        value -= value * (Deff * 0.001f);
                        break;
                    case ValueType.TrueDamage:
                        break;
                }

                HP -= value;
                return value;
            }
            public virtual void AttackTarget(Stats target)
            {
                this.target = target;
                if (target == null) return;

                //타겟 회피율 계산
                if (accuracy < target.Evasion)
                {
                    if (UnityEngine.Random.Range(1, 101) > target.Evasion - accuracy)
                    {
                        UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos + (Vector3.up * 2), "MISS!", new Color(0.47058f, 0.18039f, 0.45098f, 1), 1);
                        return;
                    }
                }
                UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos + (Vector3.up * 2), target.GetDamage(attackDamage, ValueType.Physical).ToString("N0"), Color.red, 1);
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
            public virtual void OnClick()
            {

            }
        }

        public class PlayerStat : Stats
        {
            //직업관련
            public BaseJobType jobType;
            public JobPhase jobPhase;
            public JobRoot jobRoot;
            public float RegenTime = 5f;

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
            public override bool isCharacterDamaged
            {
                get { return MaxHP > hp; }
            }
            public override float MoveSpeed { get => base.moveSpeed+GetArmorApixValue(ArmorApixType.MoveSpeed); }

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
                            BasicStatus.SetChangeAbleStatus(armors[0].apixList.statLine.Item1, -armors[0].apixList.statLine.Item2);
                            armors[0] = value;
                            UIManager.GetInstance().equipWindowArmors[0].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Chest:
                            armors[1].Amount = 1;
                            armors[1].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[1]);
                            BasicStatus.SetChangeAbleStatus(armors[1].apixList.statLine.Item1, -armors[1].apixList.statLine.Item2);
                            armors[1] = value;
                            UIManager.GetInstance().equipWindowArmors[1].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Pants:
                            armors[2].Amount = 1;
                            armors[2].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[2]);
                            BasicStatus.SetChangeAbleStatus(armors[2].apixList.statLine.Item1, -armors[2].apixList.statLine.Item2);
                            armors[2] = value;
                            UIManager.GetInstance().equipWindowArmors[2].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Boots:
                            armors[3].Amount = 1;
                            armors[3].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[3]);
                            BasicStatus.SetChangeAbleStatus(armors[3].apixList.statLine.Item1, -armors[3].apixList.statLine.Item2);
                            armors[3] = value;
                            UIManager.GetInstance().equipWindowArmors[3].SlotItem = value;
                            successChange = true;
                            break;
                        case EquipPart.Gauntlet:
                            armors[4].Amount = 1;
                            armors[4].isEquiped = false;
                            UIManager.GetInstance().equipInven.GetItems(armors[4]);
                            BasicStatus.SetChangeAbleStatus(armors[4].apixList.statLine.Item1, -armors[4].apixList.statLine.Item2);
                            armors[4] = value;
                            UIManager.GetInstance().equipWindowArmors[4].SlotItem = value;
                            successChange = true;
                            break;
                    }
                    if (successChange)
                    {
                        UIManager.GetInstance().equipInven.RemoveItem(value);
                        BasicStatus.SetChangeAbleStatus(value.apixList.statLine.Item1, value.apixList.statLine.Item2);
                        UIManager.GetInstance().PlayerMaxCurrHP = (MaxHP, HP);
                        UIManager.GetInstance().PlayerMaxCurrSP = (MaxSP, SP);
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
                            BasicStatus.SetChangeAbleStatus(weapons[0].apixList.statLine.Item1, -weapons[0].apixList.statLine.Item2);
                            weapons[0] = value;
                            UIManager.GetInstance().equipWindowWeapons[0].SlotItem = value;
                            UIManager.GetInstance().equipInven.RemoveItem(value);
                            successChange = true;
                            break;
                        case EquipPart.RightHand:
                            weapons[1].isEquiped = false;
                            weapons[1].Amount = 1;
                            UIManager.GetInstance().equipInven.GetItems(weapons[1]);
                            BasicStatus.SetChangeAbleStatus(weapons[1].apixList.statLine.Item1, -weapons[1].apixList.statLine.Item2);
                            weapons[1] = value;
                            UIManager.GetInstance().equipWindowWeapons[1].SlotItem = value;
                            UIManager.GetInstance().equipInven.RemoveItem(value);
                            successChange = true;
                            if (weapons[0].GetPart == EquipPart.TwoHanded)
                            {
                                weapons[0].Amount = 1;
                                weapons[0].isEquiped = false;
                                UIManager.GetInstance().equipInven.GetItems(weapons[0]);
                                BasicStatus.SetChangeAbleStatus(weapons[0].apixList.statLine.Item1, -weapons[0].apixList.statLine.Item2);
                                weapons[0] = new Weapons(EquipPart.LeftHand);
                                UIManager.GetInstance().equipWindowWeapons[0].SlotItem = null;

                            }
                            break;
                        case EquipPart.TwoHanded:
                            weapons[0].isEquiped = false;
                            weapons[0].Amount = 1;
                            UIManager.GetInstance().equipInven.GetItems(weapons[0]);
                            BasicStatus.SetChangeAbleStatus(weapons[0].apixList.statLine.Item1, -weapons[0].apixList.statLine.Item2);
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
                                BasicStatus.SetChangeAbleStatus(weapons[1].apixList.statLine.Item1, -weapons[1].apixList.statLine.Item2);
                                weapons[1] = new Weapons(EquipPart.RightHand);
                                UIManager.GetInstance().equipWindowWeapons[1].SlotItem = null;
                            }
                            break;
                    }
                    if (successChange)
                    {
                        UIManager.GetInstance().equipInven.RemoveItem(value);
                        BasicStatus.SetChangeAbleStatus(value.apixList.statLine.Item1, value.apixList.statLine.Item2);

                        UIManager.GetInstance().PlayerMaxCurrHP = (MaxHP, HP);
                        UIManager.GetInstance().PlayerMaxCurrSP = (MaxSP, SP);
                    }
                }
            }
            #endregion
            #region 버프 관련
            public override void AcceptBuff(BuffOBJ target)
            {
                if (buffHash.Contains(target.buffName))
                {
                    if (SearchBuff(target.buffLevel, target.buffName))
                    {
                        target.ApplyBuffs();
                        Action<string> nameAction = new Action<string>(HashAction);
                        Action removeAction = new Action(target.RemoveBuffs);

                        buffTime tempTimer = new buffTime(target.buffName, target.leftTick, target.buffLevel, removeAction, nameAction, true);
                        SkillManager.GetInstance().RegistBuffTimer(tempTimer);
                        buffs.Add(tempTimer);
                        buffHash.Add(target.buffName);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    target.ApplyBuffs();
                    Action<string> nameAction = new Action<string>(HashAction);
                    Action removeAction = new Action(target.RemoveBuffs);

                    buffTime tempTimer = new buffTime(target.buffName, target.leftTick, target.buffLevel, removeAction, nameAction, true);
                    SkillManager.GetInstance().RegistBuffTimer(tempTimer);
                    buffs.Add(tempTimer);
                    buffHash.Add(target.buffName);
                }
            } 
            #endregion
            public PlayerStat(Node initializeNode, float hp,float sp, float moveSpeed, float attackSpeed, float attackDamage,byte attackRange,float evasion) : base(initializeNode, hp,sp, moveSpeed, attackSpeed, attackDamage,attackRange,evasion)
            {
                HP = hp;
                SP = sp;
            }
            //어택데미지
            public override float TotalAD
            {
                get { return attackDamage + (BasicStatus.Strength * 3) + (BasicStatus.Luck * 0.6f) + GetWeaponValue(false)+GetWeaponApixValue(WeaponApixType.ATK); ; }
            }
            public override float TotalAP
            {
                //현재 AP와 int값에만 영향받음, 추후 장비영향 추가하여야함
                get { return abilityPower + (BasicStatus.Inteligence*2) + (BasicStatus.Luck * 0.4f) + GetWeaponValue(true) + GetWeaponApixValue(WeaponApixType.MATK); }
                set { if(value>=0) abilityPower = value; }
            }
            //적중률
            public override float TotalAccuracy
            {
                get { return (accuracy + (BasicStatus.Dexterity * 1.5f) + (BasicStatus.Luck * 0.3f) + GetWeaponApixValue(WeaponApixType.Accuracy));}
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
                    float tempCT = defaultCasting - ( (BasicStatus.Dexterity * 0.006f) + (BasicStatus.Inteligence * 0.003f) + GetWeaponApixValue(WeaponApixType.CastingSpeed) + GetArmorMatValue(WeaponApixType.CastingSpeed));
                    return tempCT <0? 0:tempCT;
                }
            }
            //글로벌 쿨타임 배율
            public float GlobalCooltimePercent
            {
                get 
                {
                    float tempGC = (1 - (BasicStatus.Agility * 0.006f));
                    return tempGC < 0.01? 0.01f: tempGC; 
                }
            }
            public override float Evasion
            {
                get
                {
                    return defaultEvasion + ((BasicStatus.Agility / 3) * 2) + (BasicStatus.Luck  * 0.4f)+GetArmorApixValue(ArmorApixType.Evasion);
                }
            }
            public int defaultCriChance = 0;
            public int CriChance
            {
                get { return defaultCriChance+(int)(BasicStatus.Luck * 0.8f) + (int)GetWeaponApixValue(WeaponApixType.CriticalChance); } 
            }
            public float defaultCriDamage = 2;
            public float CriDamage
            {
                
                get { return defaultCriDamage+ GetArmorMatValue(WeaponApixType.CriticalDMG); }
            }

            #region 방어기제
            public override float Deff
            {
                get 
                {
                    float temp = deff + GetArmorValue(false) + GetArmorApixValue(ArmorApixType.deff) + (basicStatus.Vitality * 0.1f);
                    return temp < 750f? temp : 750f;        
                }
            }
            public override float MagicDeff
            {
                get 
                {
                    float temp = magicDeff + GetArmorValue(true) + GetArmorApixValue(ArmorApixType.magicDeff) + (basicStatus.Inteligence * 0.1f);
                    return temp < 750f ? temp : 750f;
                }
            }
            public float defaultHPRegen = 0;
            public float HPRegen 
            {
                get { return (MaxHP / 100f) + (GetArmorApixValue(ArmorApixType.HpRegen))+ defaultHPRegen; }
            }
            public float MaxHP 
            {
                get { return defaultMaxHP + (this.BasicStatus.Vitality * 10f)*(1f+GetArmorMatValue(WeaponApixType.MaxHp)); }
            }

            public override float HP
            {
                get
                {
                    return base.hp;
                }
                set
                {
                    if (hp > value)
                    {
                        Player.Instance.stateMachine.ChangeState(Player.Instance.stateMachine.SearchState("damagedState"));
                    }

                    if (MaxHP < value) 
                    {
                        if (hp < MaxHP)
                        {
                            UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 1.5f), $"+" + (MaxHP - hp).ToString("N0"), Color.green, 1f);
                        }
                        base.hp = MaxHP;
                    }
                    else
                    {
                        if (value > hp) UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 1.5f), $"+"+(value - hp).ToString("N0"), Color.green, 1f);
                        base.hp = value;
                    }

                    if (isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                    }
                    UIManager.GetInstance().PlayerMaxCurrHP = (MaxHP, hp);
                }
            }
            public float defaultSPRegen = 0;
            public float MaxSP
            {
                get { return base.defaultSP + GetArmorApixValue(ArmorApixType.MaxMana) + (this.BasicStatus.Inteligence * 7f); }
            }
            public override float SP
            {
                get { return base.sp; }
                set 
                {
                    if (value > MaxSP) 
                    {
                        if (sp < MaxSP)
                        {
                            UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 2f), "+"+(MaxSP - sp).ToString("N0"), Color.blue, 1f);
                        }
                        sp = MaxSP;
                    }
                    else 
                    {
                        if(value > sp) UIManager.GetInstance().SpawnFloatText(Player.Instance.transform.position + (Vector3.up * 2f), "+"+(value - sp).ToString("N0"), Color.blue, 1f);
                        sp = value;
                    } 
                    
                    
                    UIManager.GetInstance().PlayerMaxCurrSP = (MaxSP, sp);
                }
            }
            public float SPRegen
            {
                get { return (MaxSP / 100f) + (GetArmorApixValue(ArmorApixType.ManaRegen))+ defaultSPRegen; }
            }
            #endregion
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
                        UIManager.GetInstance().SpawnFloatText(target.standingNode.worldPos + (Vector3.up * 2), "MISS!", new Color(120, 46, 115, 1), 1);
                        return;
                    }
                }


                if (UnityEngine.Random.Range(1, 101) <= CriChance) 
                {
                    
                    UIManager.GetInstance().SpawnFloatText(this.target.standingNode.worldPos + Vector3.up, this.target.GetDamage(TotalAD * CriDamage, ValueType.Physical).ToString("N0"),Color.yellow,1f);
                }
                else
                {
                    UIManager.GetInstance().SpawnFloatText(this.target.standingNode.worldPos + Vector3.up, this.target.GetDamage(TotalAD, ValueType.Physical).ToString("N0"), Color.red, 1f);
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
                        if (weapons[i].apixList.abilityApixes[j].Item1 == targetType) temp += weapons[i].apixList.abilityApixes[j].Item2;
                    }
                }
                return temp;
            }
            public float GetWeaponValue(bool callOnMATK)
            {
                float temp = 0f;
                for (byte i = 0; i < weapons.Length; i++)
                {
                    temp += weapons[i].IsMATKWeapon == callOnMATK ? weapons[i].ValueOne : 0;

                }
                return temp;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public float GetArmorValue(bool isMDeff)
            {
                float temp = 0f;
                for (byte i = 0; i < armors.Length; i++)
                {
                    temp += armors[i].magicDeff == isMDeff? armors[i].ValueOne : 0;
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
                        if (armors[i].apixList.abilityApixes[j].Item1 == targetType) temp += armors[i].apixList.abilityApixes[j].Item2;
                    }
                }
                return temp;
            }
        }
        [System.Serializable]
        public class MonsterStat : Stats
        {
            public byte monsterLevel = 0;
            public MonsterStat(byte monsterLevel,Node initializeNode, float hp, float sp, float moveSpeed, float attackSpeed, float attackDamage, byte attackRange, float evasion) : base(initializeNode, hp, sp, moveSpeed, attackSpeed, attackDamage, attackRange,evasion)
            {
                this.monsterLevel=monsterLevel;
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
                    if (defaultMaxHP <= value) base.hp = defaultMaxHP;
                    else base.hp = value;

                    if (isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                        return;
                    }
                    if (HPBar == null) HPBar = UIManager.GetInstance().HPBarDequeue();
                    HPBar.value = hp;
                    HPBar.maxValue = defaultMaxHP;
                }
            }
            public override float SP
            {
                get { return base.sp; }
                set
                {
                    if (value > defaultMaxHP) sp = base.defaultSP;
                    else sp = value;
                }
            }
        }

        [System.Serializable]
        public class NPCStat : Stats
        {
            public override float HP 
            {
                get
                {
                    return base.hp;
                }
                set
                {
                    if (defaultMaxHP <= value) base.hp = defaultMaxHP;
                    else base.hp = value;

                    if (isCharacterDie)
                    {
                        dieFunctions?.Invoke();
                        return;
                    }
                }
            }
            public override float SP
            {
                get { return base.sp; }
                set
                {
                    if (value > defaultMaxHP) sp = base.defaultSP;
                    else sp = value;
                }
            }
            [SerializeField] public DialogStateMachine dialogStateMachine;


            public NPCStat(Node initializeNode, float hp, float sp, float moveSpeed, float attackSpeed, float attackDamage, byte attackRange, float evasion) : base(initializeNode, hp, sp, moveSpeed, attackSpeed, attackDamage, attackRange, evasion)
            {
                SP = sp;
                HP = hp;
            }
            public override void OnClick()
            {
                dialogStateMachine.ChangeDialog(DialogType.greeting);
                QuestManager.GetInstance().conversationEvent?.Invoke(dialogStateMachine.npcName);
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

