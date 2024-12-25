using UnityEngine;

namespace Game.Scripts.Extension
{
    public static class Extension
    {
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

        public static bool RandomBool(this int chance, int options = 101)                          
        {
            int result = Random.Range(0, options);
            return (result < chance);
        }

        public static Vector3 Scale(this Transform transform)
        {
            if (transform.parent == null)
            {
                return transform.localScale;
            }

            return transform.localScale - transform.parent.localScale;
        }
    }
}
