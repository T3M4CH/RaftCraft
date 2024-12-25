using System;
using Game.Scripts.ResourceController.Enums;
using UnityEngine;

namespace Game.Scripts.ResourceController.UI
{
    [Serializable]
    public struct BubbleStruct
    {
        public EResourceType[] ResourceTypes;
        public int[] RecourcesCount;
        public float X;
        public float Y;
        public float Z;
    }
}