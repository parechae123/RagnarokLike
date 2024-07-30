using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResourceManager : Manager<ResourceManager>
{
    private uint loadTaskNum;
    public uint LoadTaskNum
    {
        get { return loadTaskNum; }
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
                    if (dataCount == infoAsyncOP.Result.Count)
                    {
                        callback?.Invoke(tempT);
                        Addressables.Release(infoAsyncOP);
                    }
                    tempT[dataCount].Item1 = OBJ.PrimaryKey;
                    tempT[dataCount].Item2 = result;
                });

            }

        };
    }
}
