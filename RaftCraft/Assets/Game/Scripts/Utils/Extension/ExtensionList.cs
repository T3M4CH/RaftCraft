using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Extension
{
    public static class ExtensionList
    {
        public static T Last <T> (this List<T> list)
        {
            return list[list.Count-1];
        } 
    
        public static T Last <T> (this T[] array)
        {
            return array[array.Length-1];
        } 

        public static T Peek <T> (this List<T> list)
        {
            return list[0];
        }
    
        public static T Peek <T> (this T[] array)
        {
            return array[0];
        }

        public static T Dequeue <T> (this List<T> list)
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T Trim <T> (this List<T> list)
        {
            T item = list.Last();
            list.Remove(item);
            return item;
        }

        public static T GetRandomItem <T> (this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandomItem <T> (this T[] list)
        {
            return list[Random.Range(0, list.Length)];
        }

        public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dict)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }
}
