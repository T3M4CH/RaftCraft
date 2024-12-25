using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SerializableScene
{
    [SerializeField, ValidateInput("ValidateScene")] private Object _assetScene;

    [SerializeField, HideInInspector]
    private string _nameScene;

    [ShowInInspector, ShowIf("ValidateScene"), ReadOnly]
    public string NameScene
    {
        get => _nameScene;
        private set => _nameScene = value;
    }

    public bool HaveScene => _assetScene != null;

#if UNITY_EDITOR
    public bool ValidateScene
    {
        get
        {
            if (_assetScene == null)
            {
                return false;
            }

            if (_assetScene is SceneAsset != false)
            {
                _nameScene = _assetScene.name;
                return _assetScene is SceneAsset;
            }

            _assetScene = null;
            return false;
        }
    }
#endif
}