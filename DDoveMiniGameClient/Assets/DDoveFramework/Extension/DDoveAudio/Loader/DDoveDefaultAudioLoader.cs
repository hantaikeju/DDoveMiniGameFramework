using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDovePool;
using DDoveFramework.Extension.DDoveRes;
using UnityEngine;
using YooAsset;

namespace DDoveFramework.Extension.DDoveAudio
{
    public class DDoveDefaultAudioLoader : IDDoveAudioLoader, IDDovePoolObject
    {
        public AudioClip Clip { get; private set; }

        private string _currentAssetName;
        private AssetHandle _resHandle;

        public AudioClip LoadClip(string assetName)
        {
            if (IsSameResource(assetName) && _resHandle != null)
            {
                if (Clip != null)
                {
                    return Clip;
                }

                Clip = _resHandle.AssetObject as AudioClip;
                return Clip;
            }

            ReleaseHandle();
            _currentAssetName = assetName;

            var package = DDoveResKit.GetPackage();
            if (package == null)
            {
                DDoveDebug.LogError(DDoveAudioKit.LogTitle, ("reason", "package not ready"), ("location", assetName));
                return null;
            }

            _resHandle = package.LoadAssetSync<AudioClip>(assetName);
            Clip = _resHandle.AssetObject as AudioClip;
            return Clip;
        }

        public async UniTask<AudioClip> LoadClipAsync(string assetName)
        {
            if (IsSameResource(assetName) && _resHandle != null)
            {
                if (Clip != null)
                {
                    return Clip;
                }

                if (!_resHandle.IsDone)
                {
                    await _resHandle;
                }

                Clip = _resHandle.AssetObject as AudioClip;
                return Clip;
            }

            ReleaseHandle();
            _currentAssetName = assetName;

            _resHandle = await DDoveResKit.LoadAssetAsync<AudioClip>(assetName);
            if (_resHandle == null)
            {
                DDoveDebug.LogError(DDoveAudioKit.LogTitle, ("reason", "clip load failed"), ("location", assetName));
                return null;
            }

            Clip = _resHandle.AssetObject as AudioClip;
            if (Clip == null)
            {
                DDoveDebug.LogError(DDoveAudioKit.LogTitle, ("reason", "clip load failed"), ("location", assetName));
            }

            return Clip;
        }

        public void Unload()
        {
            ReleaseHandle();
            _currentAssetName = null;
            Clip = null;
        }

        public void OnAcquire()
        {
            Clip = null;
            _currentAssetName = null;
            _resHandle = null;
        }

        public void OnRelease()
        {
            Unload();
        }

        private bool IsSameResource(string assetName)
            => !string.IsNullOrEmpty(_currentAssetName) && _currentAssetName == assetName;

        private void ReleaseHandle()
        {
            _resHandle?.Release();
            _resHandle = null;
        }
    }
}
