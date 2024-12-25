using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.WindowManager
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UIWindow[] _windows;
        [SerializeField] private TextMeshProUGUI _textMission;
        
        private Dictionary<Type, UIWindow> _instanceWindow = new();
        
        private void Awake()
        {
            foreach (var window in _windows)
            {
                var type = window.GetType();
                if (_instanceWindow.ContainsKey(type) == false)
                {
                    var instanceWindow = window;
                    instanceWindow.Initialize(this);
                    instanceWindow.Hide();
                    _instanceWindow.Add(type, instanceWindow);
                }
            }
        }

        public void SetNameTutorial(string nameTutorial)
        {
            _textMission.text = nameTutorial;
        }
        
        public T OpenWindow<T>() where T : UIWindow
        {
            var window = GetWindow<T>();
            if (window == null)
            {
                throw new Exception("Not found");
            }

            window.Show();
            return window;
        }

        public T CloseWindow<T>() where T : UIWindow
        {
            var window = GetWindow<T>();
            if (window == null)
            {
                throw new Exception("Not found");
            }

            window.Hide();
            return window;
        }

        public T GetWindow<T>() where T : UIWindow
        {
            if (_instanceWindow.ContainsKey(typeof(T)) == false)
            {
                Debug.LogWarning("Window not found");
                return null;
            }

            return _instanceWindow[typeof(T)] as T;
        }

#if UNITY_EDITOR
        [Button]
        public void InitializeWindows()
        {
            _windows = GetComponentsInChildren<UIWindow>();
        }
#endif
    }
}