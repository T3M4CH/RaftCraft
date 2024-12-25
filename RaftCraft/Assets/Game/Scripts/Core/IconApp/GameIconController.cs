#if UNITY_EDITOR
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

namespace Game.Scripts.Core.IconApp
{
    [CreateAssetMenu(menuName = "GTapUtils/Android/IconController", fileName = "IconController")]
    public class GameIconController 
    {
        [MenuItem("Gtap/Android/IconApp")]
        private static async void OpenWindowSelectIcons()
        {
            CreateFolder();
            await WaitCopyFile();
        }

        private static async UniTask WaitCopyFile()
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder res", "", "");
            var pathProject = Application.dataPath + "/~Resources";
            UnityEngine.Debug.Log(pathProject);
            if (!File.Exists($"{path}/appiconMain.png"))
            {
                UnityEngine.Debug.LogError($"AppIconMain.png not found from folder: {path}");
                return;
            }

            var fileInfo = new FileInfo($"{path}/appiconMain.png");
            if (fileInfo.Exists)
            {
                fileInfo.CopyTo($"{pathProject}/appiconMain.png", true);
            }
            
            if (!Directory.Exists($"{path}/res/mipmap-xxxhdpi"))
            {
                UnityEngine.Debug.LogError("Folder mipmap-xxxhdpi not found!");
                return;
            }

            foreach (var pathFile in Directory.GetFiles($"{path}/res/mipmap-xxxhdpi","*.*", SearchOption.AllDirectories))
            {
                var fileName = pathFile.Replace($"{path}/res/mipmap-xxxhdpi", "")[1..];
                var fileIconInfo = new FileInfo($"{path}/res/mipmap-xxxhdpi/{fileName}");
                if (fileIconInfo.Exists)
                {
                    fileIconInfo.CopyTo($"{pathProject}/android/{fileName}", true);
                }
            }
            
            EditorUtility.DisplayProgressBar("Wait copy file icons...", "Copying is in progress", 0f);
            while (AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/appiconMain.png") == null)
            {
                await UniTask.NextFrame();
            }
            EditorUtility.ClearProgressBar();
            
            var appIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/appiconMain.png");
            
            var adaptiveBackground = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/android/ic_launcher_background.png");
            var adaptiveForeground = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/android/ic_launcher_foreground.png");
            var roundIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/android/ic_launcher_round.png");
            var legacyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/~Resources/android/ic_launcher.png");
            SetIcons(appIcon, adaptiveBackground, adaptiveForeground, roundIcon, legacyIcon);
        }
        
        private static void SetIcons(Texture2D appIcon, Texture2D adaptiveBackground, Texture2D adaptiveForeground, Texture2D roundIcon, Texture2D legacyIcon)
        {
            var defaultIcon = new Texture2D[1];
            defaultIcon[0] = appIcon;
            
            var adaptive = new Texture2D[6][];
            for (var i = 0; i < adaptive.Length; i++)
            {
                adaptive[i] = new[] { adaptiveBackground, adaptiveForeground };
            }

            var round = new Texture2D[6][];
            for (var i = 0; i < round.Length; i++)
            {
                round[i] = new[] { roundIcon };
            }

            var legacy = new Texture2D[6][];
            for (var i = 0; i < legacy.Length; i++)
            {
                legacy[i] = new[] { legacyIcon };
            }
            
            ImportTexture("Assets/~Resources/appiconMain.png");
            ImportTexture("Assets/~Resources/android/ic_launcher_background.png");
            ImportTexture("Assets/~Resources/android/ic_launcher_foreground.png");
            ImportTexture("Assets/~Resources/android/ic_launcher_round.png");
            ImportTexture("Assets/~Resources/android/ic_launcher.png");
            
            SetIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Round, round);
            
            SetIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Adaptive, adaptive);
            
            SetIcons(NamedBuildTarget.Android, AndroidPlatformIconKind.Legacy, legacy);
            
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, defaultIcon);
        }

        private static void ImportTexture(string path)
        {
            var textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            textureImporter.compressionQuality = 0;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.SaveAndReimport();
        }      
        private static void SetIcons(NamedBuildTarget platform, PlatformIconKind kind,
            Texture2D[][] textures)
        {
            PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, kind);
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetTextures(textures[i]);
            }
            
            PlayerSettings.SetPlatformIcons(platform, kind, icons);
        }
        
        private static void CreateFolder()
        {
            if (AssetDatabase.IsValidFolder("Assets/~Resources") == false)
            {
                AssetDatabase.CreateFolder("Assets", "~Resources");
            }
            if (AssetDatabase.IsValidFolder("Assets/~Resources/android") == false)
            {
                AssetDatabase.CreateFolder("Assets/~Resources", "android");
            }
        }
    }
}
#endif
