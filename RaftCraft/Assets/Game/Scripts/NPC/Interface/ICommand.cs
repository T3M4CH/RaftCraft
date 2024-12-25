using UnityEngine;

namespace Game.Scripts.NPC
{
    public interface ICommand
    {
    }

    public struct MoveRope: ICommand
    {
        public Vector3 Target;
    }
    
    public struct Move: ICommand { }
    public struct Idle: ICommand { }
}