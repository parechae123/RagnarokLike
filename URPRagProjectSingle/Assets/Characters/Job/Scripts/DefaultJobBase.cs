using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using DG.Tweening;
using PlayerDefines.Stat;
using System;
using UnityEditor;
[CreateAssetMenu(fileName = "DefaultJobs", menuName = @"Job/DefaultJobFile")]
public class DefaultJobBase : ScriptableObject
{
    public string ClassName;
    public byte MaxJobLevel;
    public byte jobLevel
    {
        get;
        protected set;
    }
    public byte defaultJobSkillPoint
    {
        get;
        protected set;
    }
    public Sprite basicCharacterSprite;
    public AnimatorController animContoller;
    public SkillInfo[] noviceSkills;
    public void JobLevelUp()
    {
        if (jobLevel == MaxJobLevel) return;
        jobLevel++;
        defaultJobSkillPoint++;
    }
}
enum BaseJobType
{
    Novice, SwordMan, Mage, Archer
}
enum JobPhase
{
    defaultJob, first, second, third
}
enum JobRoot
{
    None, _1, _2
}