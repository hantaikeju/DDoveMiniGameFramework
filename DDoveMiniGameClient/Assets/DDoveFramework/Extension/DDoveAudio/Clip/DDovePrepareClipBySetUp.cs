using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDovePrepareClipBySetUp : IDDoveClipPrepareMode
    {
        private AudioClip _clip;

        internal void SetClipForPrepare(AudioClip clip)
        {
            _clip = clip;
        }

        public void PrepareClip(DDoveAbstractAudioPlayer audioPlayer, GameObject root, string name, bool loop)
        {
            audioPlayer.AudioSourceProxy.CreateOrUpdateAudioSource(root, name);
            audioPlayer.ClearDataAndStop();
            audioPlayer.AudioName = name;
            audioPlayer.AudioSourceProxy.SetLoop(loop);
            audioPlayer.AudioSourceProxy.SetClip(_clip);
            audioPlayer.OnClipPrepareFinished(true, _clip);
            _clip = null;
        }

        public void UnPrepareClip()
        {
        }
    }
}
