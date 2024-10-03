using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Manager<ResourceManager>
{
    public Dictionary<string, UnityEngine.Object> resourceDict;
    private uint loadTaskNum = 0;
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
