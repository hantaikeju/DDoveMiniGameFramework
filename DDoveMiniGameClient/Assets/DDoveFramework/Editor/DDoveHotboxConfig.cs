using System;
using System.Collections.Generic;
using UnityEngine;

namespace DDoveFramework.Editor
{
    [CreateAssetMenu(fileName = "DDoveHotboxConfig", menuName = "DDove/Editor/Hotbox Config", order = 20)]
    public sealed class DDoveHotboxConfig : ScriptableObject
    {
        public List<DDoveHotboxRing> Rings = new List<DDoveHotboxRing>();
    }

    [Serializable]
    public sealed class DDoveHotboxRing
    {
        public string Name = "热盒";
        public List<DDoveHotboxSlot> Slots = new List<DDoveHotboxSlot>();
    }

    [Serializable]
    public sealed class DDoveHotboxSlot
    {
        public string EntryId;
        public bool Placed;
        public float X;
        public float Y;
    }
}
