using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class EasySavePreset : SavePreset
{
    public override bool HaveKey(string key)
    {
        return Data.ContainsKey(key);
    }

    public override T GetData<T>(string key, T defaultValue)
    {
        if (Data.ContainsKey(key) == false)
        {
            return defaultValue;
        }
        else
        {
            return JsonConvert.DeserializeObject<T>(Data[key]);
        }
    }

    public override void SetKey<T>(string key, T value)
    {
        if (Data.ContainsKey(key) == false)
        {
            Data.Add(key, JsonConvert.SerializeObject(value));
        }
        else
        {
            Data[key] = JsonConvert.SerializeObject(value);
        }
    }

    public override void Save()
    {
        ES3.Save("GameSaveData", Data);
    }

    public override void Load()
    {
        Data = ES3.Load("GameSaveData", new Dictionary<string, string>());
    }
    
}