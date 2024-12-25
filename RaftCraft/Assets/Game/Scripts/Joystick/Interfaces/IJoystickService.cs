using System;
using UnityEngine;

namespace Game.Scripts.Joystick.Interfaces
{
    public interface IJoystickService
    {
        public event Action<DirectionJoystick> OnChangeDirection; 
        /// <summary>
        /// Can be use to turn off interface while action 
        /// </summary>
        void HideGUI(bool isPersistent = true);
        
        /// <summary>
        /// Can be use to turn on interface 
        /// </summary>
        void ReturnGUI(bool isPersistent = true);
        
        bool IsDragging { get; }
        
        /// <summary>
        /// Return normalized joystick value
        /// </summary>
        Vector3 Direction { get;}

        void SetStateDefault();
        void SetStateWater();
    }
}