using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GTapSoundManager.SoundManager
{
#if UNITY_EDITOR
    public class SoundCreator : MonoBehaviour
    {
        [MenuItem("Assets/Create/Sounds Asset", priority = 1)]
        public static void ButtonCreateSoundsAsset()
        {
            var selectedList = Selection.objects;

            if (selectedList != null && selectedList.Length > 0)
            {
                var list = new List<AudioClip>();
                for (var i = 0; i < selectedList.Length; i++)
                {
                    if (selectedList[i] is AudioClip clip)
                    {
                        list.Add(clip);
                    }
                }
                
                CreateAsset(list.ToArray());
            }
        }

        [MenuItem("Assets/Create/Sound Asset", true)]
        private static bool IsSelectedSound()
        {
            if (Selection.objects != null && Selection.objects.Length > 1)
            {
                return false;
            }

            if (Selection.activeObject == false)
            {
                return false;
            }
            else
            {
                return Selection.activeObject is AudioClip;
            }
        }

        [MenuItem("Assets/Create/Sounds Asset", true)]
        private static bool IsSelectedSounds()
        {
            if (Selection.objects == null)
            {
                return false;
            }
            else
            {
                if (Selection.objects.Length == 1)
                {
                    return false;
                }

                return CheckIsFullClip(Selection.objects);
            }
        }

        [MenuItem("Assets/Create/Sound Asset", priority = 2)]
        public static void ButtonCreateSoundAsset()
        {
            var selected = Selection.objects;

            if (selected.Length > 1)
            {
                return;
            }

            if (selected[0] is AudioClip clip)
            {
                CreateAsset(new AudioClip[]{clip});
            }
        }

        private static bool CheckIsFullClip(Object[] objects)
        {
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] is AudioClip clip)
                {

                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static void CreateAsset(AudioClip[] clip)
        {
            var path = "Assets/Game/Resources/SoundAssets";
            
            var asset = CreateSoundAsset(clip[0].name, path);
            for (var i = 0; i < clip.Length; i++)
            {
                asset.InitClip(clip[i]);
            }
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        
        private static SoundAsset CreateSoundAsset(string nameAssets, string path)
        {
            var asset = ScriptableObject.CreateInstance<SoundAsset>();
            CheckDirectory(path);
            AssetDatabase.CreateAsset(asset, $"{path}/{nameAssets}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        private static bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.Log($"Создал директорию");
                Directory.CreateDirectory(path);
                return true;
            }

            return true;
        }
    }
#endif
}
