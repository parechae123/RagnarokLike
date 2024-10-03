using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlayerDefines.Stat;
using System;
using UnityEditor;
#region 1Â÷ Á÷¾÷
[CreateAssetMenu(fileName = "DefaultJobs", menuName = @"Job/FirstJobFile")]
public class FirstJobBase : DefaultJobBase
{
    public SkillInfo[] firstJobSkills;
    public virtual void ChangeJob(ref DefaultJobBase jobInstance)
    {
        this.noviceSkills = jobInstance.noviceSkills;
    }
}
#endregion
