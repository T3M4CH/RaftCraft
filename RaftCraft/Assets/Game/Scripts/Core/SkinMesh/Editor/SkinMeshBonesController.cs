#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Core.SkinMesh.Editor
{
    public class SkinMeshBonesController : EditorWindow
    {
        [MenuItem("Window/Update Skinned Mesh Bones")]
        public static void OpenWindow()
        {
            var window = GetWindow<SkinMeshBonesController>();
            window.titleContent = new GUIContent("Skin Updater");
        }
        private SkinnedMeshRenderer targetSkin;
        private Transform rootBone;
        private void OnGUI()
        {
            targetSkin = EditorGUILayout.ObjectField("Target", targetSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            rootBone = EditorGUILayout.ObjectField("RootBone", rootBone, typeof(Transform), true) as Transform;
            GUI.enabled = (targetSkin != null && rootBone != null);
            if (GUILayout.Button("Update Skinned Mesh Renderer"))
            {
                Transform[] newBones = new Transform[targetSkin.bones.Length];
                for (int i = 0; i < targetSkin.bones.Length; i++)
                {
                    foreach (var newBone in rootBone.GetComponentsInChildren<Transform>())
                    {
                        if (newBone.name == targetSkin.bones[i].name)
                        {
                            newBones[i] = newBone;
                            continue;
                        }
                    }
                }
                targetSkin.bones = newBones;
            }
        }
    }
}

#endif
