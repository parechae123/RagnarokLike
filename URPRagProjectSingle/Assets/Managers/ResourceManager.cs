using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class ResourceManager : Manager<ResourceManager>
{
    private SpriteAtlas itemIconAtlas;
    public SpriteAtlas ItemIconAtlas
    {
        get
        {
            if (itemIconAtlas == null) LoadAsync<SpriteAtlas>("ItemIconAtlas", (atlas) => { itemIconAtlas = atlas; });
            return itemIconAtlas;
        }
    }
    private SpriteAtlas skillIconAtlas;
    public SpriteAtlas SkillIconAtlas
    {
        get
        {
            if (skillIconAtlas == null) LoadAsync<SpriteAtlas>("SkillIconAtlas", (atlas) => { skillIconAtlas = atlas; });
            return skillIconAtlas;
        }
    }
    private ApixDatas apixDatas;
    public ApixDatas ApixDatas
    {
        get 
        { 
            if (apixDatas == null) LoadAsync<ApixDatas>("Apixes", (Apixes) => { apixDatas = Apixes; });
            return apixDatas;
        }
    }
    private NameDatas nameSheet;
    public NameDatas NameSheet
    {
        get 
        { 
            if (nameSheet == null) LoadAsync<NameDatas>("NameData", (NameData) => { nameSheet = NameData; });
            return nameSheet;
        }
    }
    private PosionDatas posionDatas;
    public PosionDatas PosionDatas
    {
        get 
        { 
            if (posionDatas == null) LoadAsync<PosionDatas>("Posions", (PosionDatas) => { posionDatas = PosionDatas; });
            return posionDatas;
        }
    }
    private MiscDatas miscDatas;
    public MiscDatas MiscDatas
    {
        get 
        { 
            if (miscDatas == null) LoadAsync<MiscDatas>("Miscs", (MiscDatas) => { miscDatas = MiscDatas; });
            return miscDatas;
        }
    }

    public Dictionary<string, UnityEngine.Object> resourceDict;
    private uint loadTaskNum = 0;
    public uint LoadTaskNum
    {
        get { return loadTaskNum; }
    }
    public void SetAtlases(Action<bool,int> allDone)
    {
        LoadAsync<SpriteAtlas>("SkillIconAtlas", (atlas) => { skillIconAtlas = atlas; allDone.Invoke(true, 0); });
        LoadAsync<SpriteAtlas>("ItemIconAtlas", (atlas) => { itemIconAtlas = atlas; allDone.Invoke(true, 1); });
        LoadAsync<ApixDatas>("Apixes", (data) => { apixDatas = data; allDone.Invoke(true, 2); });
        LoadAsync<NameDatas>("NameData", (data) => { nameSheet= data; allDone.Invoke(true, 3); });
        LoadAsync<PosionDatas>("Posions", (data) => { posionDatas = data; allDone.Invoke(true, 4); });
        LoadAsync<MiscDatas>("Miscs", (data) => { miscDatas = data; allDone.Invoke(true, 5); });

    }

    public void LoadAsyncSkillInGameInfo(string key,Action<SkillInfoInGame> callback)
    {
        AsyncOperationHandle<SkillInfo> infoAsyncOP = Addressables.LoadAssetAsync<SkillInfo>(key);
        loadTaskNum++;

        infoAsyncOP.Completed += (op) =>
        {
            loadTaskNum--;
            
            callback?.Invoke(new SkillInfoInGame(infoAsyncOP.Result));
            Addressables.Release(infoAsyncOP);
        };
    }
    public void LoadAsync<T>(string key, Action<T> callback)
    {
        if (key.Contains(".sprite"))
        {
            key = $"{key}[{key.Replace(".sprite", "")}]";
        }
        AsyncOperationHandle<T> infoAsyncOP = Addressables.LoadAssetAsync<T>(key);
        loadTaskNum++;
        infoAsyncOP.Completed += (op) =>
        {
            loadTaskNum--;

            callback?.Invoke(infoAsyncOP.Result);
            Addressables.Release(infoAsyncOP);
        };
    }
    public void LoadAsyncAll<T>(string label, Action<(string,T)[]> callback)
    {
        var infoAsyncOP = Addressables.LoadResourceLocationsAsync(label,typeof(T));
        infoAsyncOP.WaitForCompletion();
        Debug.Log(infoAsyncOP.Result);
        if (infoAsyncOP.Result.Count == 0) callback.Invoke(null);
        infoAsyncOP.Completed += (op) =>
        {
            loadTaskNum++;
            int dataCount = 0;
            (string, T)[] tempT = new (string, T)[dataCount];
            foreach (var OBJ in infoAsyncOP.Result)
            {
                LoadAsync<T>(OBJ.PrimaryKey, (result) =>
                {
                    loadTaskNum--;
                    dataCount++;
                    Array.Resize(ref tempT, dataCount);
                    tempT[dataCount - 1].Item1 = OBJ.PrimaryKey;
                    tempT[dataCount - 1].Item2 = result;
                    if (dataCount == infoAsyncOP.Result.Count)
                    {
                        callback?.Invoke(tempT);
                        Addressables.Release(infoAsyncOP);
                    }

                });

            }

        };
    }
}
