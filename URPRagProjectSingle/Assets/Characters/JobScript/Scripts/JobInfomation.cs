using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using PlayerDefines.Stat;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "New JobInfomation File", menuName = @"Job/CreateJobInfomationFile")]
[System.Serializable]
public class JobInfoMation : ScriptableObject
{
    public string ClassName;
    public SkillBase[] skills;
    [SerializeField] BaseJobType jobType;
    [SerializeField] JobPhase jobPhase;
    [SerializeField] JobRoot jobRoot;
}