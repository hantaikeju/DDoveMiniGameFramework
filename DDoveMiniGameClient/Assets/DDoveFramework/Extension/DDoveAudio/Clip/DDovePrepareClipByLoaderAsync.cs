using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDovePrepareClipByLoaderAsync : IDDoveClipPrepareMode
    {
        private IDDoveAudioLoader _loader;

        public void PrepareClip(DDoveAbstractAudioPlayer audioPlayer, GameObject root, string name, bool loop)
        {
            PrepareClipAsync(audioPlayer, root, name, loop).Forget();
        }

        private async UniTask PrepareClipAsync(DDoveAbstractAudioPlayer audioPlayer, GameObject root, string name,
            bool loop)
        {
            if (DDoveAudioKit.GetLoader == null)
            {
                DDoveDebug.LogError(DDoveAudioKit.LogTitle, ("reason", "loader not registered"));
                return;
            }

            audioPlayer.AudioSourceProxy.CreateOrUpdateAudioSource(root, name);

            var preLoader = _loader;
            _loader = null;

            audioPlayer.ClearDataAndStop();
            _loader = DDoveAudioKit.GetLoader.Invoke();
            audioPlayer.AudioName = name;
            audioPlayer.AudioSourceProxy.SetLoop(loop);

            var clip = await _loader.LoadClipAsync(name);
            audioPlayer.OnClipPrepareFinished(clip != null, clip);

            if (preLoader != null)
            {
                DDoveAudioKit.ReturnLoader(preLoader);
            }
        }

        public void UnPrepareClip()
        {
            if (_loader == null)
            {
                return;
            }

            DDoveAudioKit.ReturnLoader(_loader);
            _loader = null;
        }
    }
}
