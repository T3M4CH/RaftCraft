#if  UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Core.SkinMesh.Editor
{
    public class CustomEditorComponentController
    {
        [MenuItem("GameObject/Component/RemoveAll", false, 10)]
        public static void RemoveAllComponents()
        {
            var obj = Selection.activeGameObject;
            if (obj == null)
            {
                return;
            }

            var components = obj.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component is Transform)
                {
                    continue;
                }
                
                Object.DestroyImmediate(component);
            }
        }
        [MenuItem("GameObject/Component/ApplyRigidBody", false, 10)]
        public static void ApplyRigidbody()
        {
            var obj = Selection.activeGameObject;
            if (obj == null)
            {
                return;
            }

            obj.AddComponent<Rigidbody>();
            obj.AddComponent<BoxCollider>();
        }
        
        
    }
}
#endif