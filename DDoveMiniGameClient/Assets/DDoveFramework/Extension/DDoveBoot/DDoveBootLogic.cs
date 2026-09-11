using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveRes;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveBoot
{
    public class DDoveBootLogic : MonoBehaviour
    {
        private CancellationTokenSource _cts;

        private void Start()
        {
            _cts = new CancellationTokenSource();
            BootAsync(_cts.Token).Forget();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid BootAsync(CancellationToken cancellationToken)
        {
            var resReady = await DDoveResKit.InitializeAsync(cancellationToken);
            if (!resReady)
            {
                return;
            }

            var extensionsReady = await InitializeExtensionsAsync(cancellationToken);
            if (!extensionsReady)
            {
                return;
            }

            await OnExtensionsReadyAsync(cancellationToken);
        }

        protected virtual UniTask<bool> InitializeExtensionsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return UniTask.FromResult(true);
        }

        protected virtual async UniTask OnExtensionsReadyAsync(CancellationToken cancellationToken)
        {
            var launchSceneLocation = ResolveLaunchSceneLocation();
            if (string.IsNullOrEmpty(launchSceneLocation))
            {
                return;
            }

            var handle = await DDoveResKit.LoadSceneAsync(
                launchSceneLocation,
                cancellationToken: cancellationToken);
            if (handle == null)
            {
                DDoveDebug.LogError("DDoveBoot", ("location", launchSceneLocation), ("reason", "load scene failed"));
            }
        }

        private static string ResolveLaunchSceneLocation()
        {
            var info = DDoveResInitInfo.Load();
            if (info != null && !string.IsNullOrEmpty(info.LaunchSceneLocation))
            {
                return info.LaunchSceneLocation;
            }

            return DDoveResKit.FallbackLaunchSceneLocation;
        }
    }
}
