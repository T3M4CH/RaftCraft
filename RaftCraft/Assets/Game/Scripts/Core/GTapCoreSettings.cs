using Game.Scripts.Core.Debug;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core
{
    [CreateAssetMenu(menuName = "GTapUtils/Core/ProjectSettings", fileName = "ProjectSettings")]
    public class GTapCoreSettings : ScriptableObject, IWindowObject
    {
        private static GTapCoreSettings _instance;

        public static GTapCoreSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load("ProjectSettings") as GTapCoreSettings;
                }

                return _instance;
            }
        }
        
        [SerializeField, FoldoutGroup("Debug")]
        private DebugMode _debug;
        
        [field:SerializeField, FoldoutGroup("Debug")]
        public HaveSaveCondition SaveCondition { get; private set; }

        public DebugMode Debug => _debug;
        public string Patch => "Core/Settings";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }
    }
}
