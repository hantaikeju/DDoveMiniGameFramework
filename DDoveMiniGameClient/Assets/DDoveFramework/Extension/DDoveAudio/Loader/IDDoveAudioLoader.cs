using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal interface IDDoveAudioLoader
    {
        AudioClip Clip { get; }
        AudioClip LoadClip(string assetName);
        UniTask<AudioClip> LoadClipAsync(string assetName);
        void Unload();
    }
}
