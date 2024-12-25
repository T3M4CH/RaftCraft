using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core.DataParser
{
    [CreateAssetMenu(fileName = "DataParser", menuName = "Core/DataParser")]
    public abstract class ScriptableObjectParser<T> : ScriptableObject
    {
        [SerializeField] private TextAsset _assetLoaded;
        
        public abstract void LoadParse(T result);
        
        [Button]
        private void Parse()
        {
            if (_assetLoaded == null)
            {
                UnityEngine.Debug.LogError($"Not found asset loaded!");
                return;
            }

            var lines = _assetLoaded.text.Split("\n");
            var lineOne = lines[0].Split(',', '\n', '\r');
            var lineTwo = lines[1].Split(',', '\n', '\r');
            var dRows = new Dictionary<string, string>();
            for (var i = 0; i < lineTwo.Length; i++)
            {
                dRows.TryAdd(lineTwo[i], lineOne[i]);
            }

            var arrayData = "[ \n";
            for (var i = 2; i < lines.Length; i++)
            {
                var data = lines[i].Split(',', '\n', '\r');
                arrayData += $"{ToJson(GetOne(GetDataParse(dRows, data)))}";
                arrayData += i == lines.Length - 1 ? "\n" : ", \n";
            }
            arrayData += "]";

            try
            {
                LoadParse(JsonConvert.DeserializeObject<T>(arrayData));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Fail parse data!");
            }
        }
        
        private string ToJson(List<PresetField> data)
        {
            var result = "{ \n";

            foreach (var item in data)
            {
                result += '"';
                result += item.Key;
                result += '"';
                result += $" : {item}, \n";
            }

            result += "}";
            return result;
        }

        private Dictionary<string, (string, string)> GetDataParse(Dictionary<string, string> keys, string[] data)
        {
            var result = new Dictionary<string, (string, string)>();
            
            for (var i = 0; i < data.Length; i++)
            {
                var element = keys.ElementAt(i);
                result.Add(element.Key, (element.Value, data[i]));
            }
            
            
            return result;
        }

        private List<PresetField> GetOne(Dictionary<string, (string, string)> keysAndTypes)
        {
            var result = new List<PresetField>();

            foreach (var valueName in keysAndTypes)
            {
                switch (valueName.Value.Item1)
                {
                    case "int":
                        var cellInt = new PresetInt(valueName.Key, int.Parse(valueName.Value.Item2));
                        result.Add(cellInt);
                        break;
                    case "float":
                        var cellFloat = new PresetFloat(valueName.Key, float.Parse(valueName.Value.Item2, CultureInfo.InvariantCulture.NumberFormat));
                        result.Add(cellFloat);
                        break;
                    case "string":
                        var cellString = new PresetString(valueName.Key, valueName.Value.Item2);
                        result.Add(cellString);
                        break;
                }
            }
            
            return result;
        }
    }

    [System.Serializable]
    public class PresetField
    {
        public string Key { get; private set; }
        public object ValuePreset { get; private set; }

        public PresetField(string key, object value)
        {
            Key = key;
            ValuePreset = value;
        }
    }

    public class PresetInt : PresetField
    {
        public PresetInt(string key, int value) : base(key, value)
        {
        }

        public override string ToString()
        {
            return $"{ValuePreset}";
        }
    }

    public class PresetFloat : PresetField
    {
        public PresetFloat(string key, float value) : base(key, value)
        {
        }

        public override string ToString()
        {
            return $"{JsonConvert.SerializeObject(ValuePreset)}";
        }
    }

    public class PresetString : PresetField
    {
        public PresetString(string key, string value) : base(key, value)
        {
        }

        public override string ToString()
        {
            return '"' + $"{ValuePreset}" + '"';
        }
    }
}
