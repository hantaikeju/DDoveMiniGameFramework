using System;
using System.Collections.Generic;
using System.Linq;
using DDoveFramework.Core;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDovePlayingSoundPoolModel : AbstractModel
    {
        internal readonly Dictionary<string, List<DDoveAudioPlayer>> SoundPlayerInPlaying =
            new Dictionary<string, List<DDoveAudioPlayer>>(30);

        protected override void OnInit()
        {
        }

        internal void ClearAllPlayingSound() => SoundPlayerInPlaying.Clear();

        internal void RemoveSoundPlayerFromPool(string nameForPool, DDoveAudioPlayer audioPlayer)
        {
            if (SoundPlayerInPlaying.TryGetValue(nameForPool, out var list))
            {
                list.Remove(audioPlayer);
            }
        }

        internal void AddSoundPlayer2Pool(string nameForPool, DDoveAudioPlayer audioPlayer)
        {
            if (SoundPlayerInPlaying.TryGetValue(nameForPool, out var list))
            {
                list.Add(audioPlayer);
                return;
            }

            SoundPlayerInPlaying.Add(nameForPool, new List<DDoveAudioPlayer> { audioPlayer });
        }

        public void ForEachAllSound(Action<DDoveAudioPlayer> operation)
        {
            var allPlayers = SoundPlayerInPlaying.SelectMany(kv => kv.Value).ToList();
            foreach (var player in allPlayers)
            {
                operation(player);
            }
        }
    }
}
