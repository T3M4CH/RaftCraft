using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SavePreset
{
    protected Dictionary<string, string> Data = new Dictionary<string, string>();

    public virtual bool HaveLoad()
    {
        return true;
    }
        
    public abstract bool HaveKey(string key);

    public abstract T GetData<T>(string key, T defaultValue);
        
    public abstract void SetKey<T>(string key, T value);

    public abstract void Save();
    public abstract void Load();
}