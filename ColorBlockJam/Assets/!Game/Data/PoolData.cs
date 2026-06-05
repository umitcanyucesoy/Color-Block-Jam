using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "PoolData", menuName = "ColorBlockJam/Pool Data", order = 5)]
    public class PoolData : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public GameObject prefab;
            public int prewarmCount = 10;
        }

        [SerializeField] private List<Entry> entries = new();

        public IReadOnlyList<Entry> Entries => entries;
    }
}
