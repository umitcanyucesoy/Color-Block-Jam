using System;
using System.Collections.Generic;
using _Game.Enums;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "ColorBlockJam/Sound Data", order = 8)]
    public class SoundData : ScriptableObject
    {
        [Serializable]
        private class Entry
        {
            public SoundId id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
        }

        [SerializeField] private Entry[] sounds;

        private Dictionary<SoundId, Entry> _lookup;

        public bool TryGet(SoundId id, out AudioClip clip, out float volume)
        {
            _lookup ??= Build();

            if (_lookup.TryGetValue(id, out var entry) && entry.clip)
            {
                clip = entry.clip;
                volume = entry.volume;
                return true;
            }

            clip = null;
            volume = 0f;
            return false;
        }

        private Dictionary<SoundId, Entry> Build()
        {
            var map = new Dictionary<SoundId, Entry>(sounds?.Length ?? 0);
            if (sounds != null)
                for (int i = 0; i < sounds.Length; i++)
                    map[sounds[i].id] = sounds[i];

            return map;
        }
    }
}
