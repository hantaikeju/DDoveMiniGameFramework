using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal interface IDDoveClipPrepareMode
    {
        void PrepareClip(DDoveAbstractAudioPlayer audioPlayer, GameObject root, string name, bool loop);
        void UnPrepareClip();
    }
}
