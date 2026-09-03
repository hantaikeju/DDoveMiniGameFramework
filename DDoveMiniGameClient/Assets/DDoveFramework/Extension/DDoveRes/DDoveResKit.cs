using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using YooAsset;

namespace DDoveFramework.Extension.DDoveRes
{
    public static class DDoveResKit
    {
        private static string s_defaultPackageName;

        public static string DefaultPackageName => s_defaultPackageName;

        public static void Initialize()
        {
            if (YooAssets.IsInitialized)
            {
                return;
            }

            YooAssets.Initialize();
        }

        public static ResourcePackage CreatePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentException("Package name cannot be null or empty.", nameof(packageName));
            }

            Initialize();

            if (YooAssets.TryGetPackage(packageName, out var existing))
            {
                return existing;
            }

            return YooAssets.CreatePackage(packageName);
        }

        public static void SetDefaultPackage(string packageName)
        {
            if (!YooAssets.TryGetPackage(packageName, out _))
            {
                DDoveDebug.LogError("DDoveRes", ("package", packageName), ("reason", "is not created"));
                return;
            }

            s_defaultPackageName = packageName;
        }

        public static ResourcePackage GetPackage(string packageName = null)
        {
            var name = ResolvePackageName(packageName);
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!YooAssets.TryGetPackage(name, out var package))
            {
                DDoveDebug.LogError("DDoveRes", ("package", name), ("reason", "is not created"));
                return null;
            }

            return package;
        }

        public static async UniTask<bool> InitializePackageAsync(
            string packageName,
            InitializePackageOptions options,
            CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var package = CreatePackage(packageName);
            if (package.InitializeStatus == EOperationStatus.Succeeded)
            {
                return true;
            }

            var operation = package.InitializePackageAsync(options);
            await operation;
            cancellationToken.ThrowIfCancellationRequested();

            if (operation.Status != EOperationStatus.Succeeded)
            {
                DDoveDebug.LogError("DDoveRes", ("package", packageName), ("error", operation.Error));
                return false;
            }

            if (string.IsNullOrEmpty(s_defaultPackageName))
            {
                s_defaultPackageName = packageName;
            }

            return true;
        }

        public static async UniTask<AssetHandle> LoadAssetAsync<TObject>(
            string location,
            string packageName = null,
            uint priority = 0,
            CancellationToken cancellationToken = default)
            where TObject : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentException("Location cannot be null or empty.", nameof(location));
            }

            var package = GetPackage(packageName);
            if (package == null)
            {
                return null;
            }

            var handle = package.LoadAssetAsync<TObject>(location, priority);
            await handle;
            cancellationToken.ThrowIfCancellationRequested();

            if (handle.Status != EOperationStatus.Succeeded)
            {
                handle.Release();
                DDoveDebug.LogError("DDoveRes", ("location", location), ("error", handle.Error));
                return null;
            }

            return handle;
        }

        private static string ResolvePackageName(string packageName)
        {
            if (!string.IsNullOrEmpty(packageName))
            {
                return packageName;
            }

            if (string.IsNullOrEmpty(s_defaultPackageName))
            {
                DDoveDebug.LogError("DDoveRes", ("reason", "default package is not set"));
                return null;
            }

            return s_defaultPackageName;
        }
    }
}
