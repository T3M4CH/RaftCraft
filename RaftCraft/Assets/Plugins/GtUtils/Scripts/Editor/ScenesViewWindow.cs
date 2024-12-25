using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.GtUtils.Scripts.Editor
{
    public class ScenesViewWindow : OdinEditorWindow
    {
        [ValueDropdown("Scenes"), HideLabel, OnValueChanged("OnSelectScene")]
        public string OpenScene;

        private Dictionary<string, string> _pathsScenes = new Dictionary<string, string>();
        
        private IEnumerable<string> Scenes => GetScenes();

        private void OnBecameVisible()
        {
            OpenScene = SceneManager.GetActiveScene().name;
        }

        private void OnSelectScene()
        {
            if (SceneManager.GetActiveScene().name == OpenScene)
            {
                return;
            }
            if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[]
                {
                    SceneManager.GetActiveScene()
                }))
            {
                return;
            }
            EditorSceneManager.OpenScene(_pathsScenes[OpenScene]);
        }

        private List<string> GetScenes()
        {
            var result = new List<string>();
            _pathsScenes.Clear();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                var split = scene.path.Split('/');
                _pathsScenes.TryAdd(split[^1].Split('.')[0], scene.path);
                result.Add(split[^1].Split('.')[0]);
            }

            return result;
        }
        
        [MenuItem("GTools/SceneView")]
        private static void Show()
        {
            ((EditorWindow)GetWindow<ScenesViewWindow>()).Show();
        }

        
    }
}
