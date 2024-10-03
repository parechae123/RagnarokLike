using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlayerDefines.Stat;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "DefaultJobs", menuName = @"Job/SecondJobFile")]
public class SecondJobBase : FirstJobBase
{
    public SkillInfo[] secondJobSkills;
    public void ChangeJob(ref FirstJobBase jobInstance)
    {
        this.noviceSkills = jobInstance.noviceSkills;
        this.firstJobSkills = jobInstance.firstJobSkills;
    }
}
