using _Game.Data;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Audio
{
    public class SoundService : ISoundService
    {
        private const string RootName = "[SoundService]";

        private readonly SoundData _data;
        private readonly AudioSource _source;

        public SoundService(SoundData data)
        {
            _data = data;

            var go = new GameObject(RootName);
            _source = go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        public void Play(SoundId id)
        {
            if (!_data || id == SoundId.None)
                return;

            if (_data.TryGet(id, out var clip, out var volume))
                _source.PlayOneShot(clip, volume);
        }
    }
}
