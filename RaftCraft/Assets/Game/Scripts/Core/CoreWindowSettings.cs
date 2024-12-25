#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class CoreWindowSettings : OdinMenuEditorWindow
    {
        [MenuItem("GTapCore/Core")]
        private static void Open()
        {
            var window = GetWindow<CoreWindowSettings>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
            var objects = Resources.LoadAll<ScriptableObject>("").OfType<IWindowObject>().ToArray();
            foreach (var page in objects)
            {
                tree.Add(page.Patch, page.InstanceObject());
            }
            return tree;
        }
    }
}
#endif
