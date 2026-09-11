using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using UnityEngine.SceneManagement;
using YooAsset;

namespace DDoveFramework.Extension.DDoveRes
{
    public static class DDoveResKit
    {
        public const string FallbackPackageName = "DefaultPackage";
        public const string FallbackLaunchSceneLocation = "Launch";

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

        public static UniTask<bool> InitializeAsync(CancellationToken cancellationToken = default)
        {
            var info = DDoveResInitInfo.Load();
            var packageName = info != null && !string.IsNullOrEmpty(info.PackageName)
                ? info.PackageName
                : ResolveInitPackageName();
            var playMode = info != null ? info.PlayMode : ResolveInitPlayMode();
            return InitializeAsync(packageName, playMode, cancellationToken);
        }

        public static async UniTask<bool> InitializeAsync(
            string packageName,
            EPlayMode playMode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentException("Package name cannot be null or empty.", nameof(packageName));
            }

            Initialize();

            var options = CreateInitializeOptions(packageName, playMode);
            if (options == null)
            {
                return false;
            }

            return await InitializePackageAsync(packageName, options, cancellationToken);
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

        public static InitializePackageOptions CreateInitializeOptions(string packageName, EPlayMode playMode)
        {
            switch (playMode)
            {
                case EPlayMode.EditorSimulateMode:
#if UNITY_EDITOR
                    var buildResult = EditorSimulateBuildInvoker.Build(packageName, (int)EBundleType.VirtualAssetBundle);
                    return new EditorSimulateModeOptions
                    {
                        EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(
                            buildResult.PackageRootDirectory)
                    };
#else
                    DDoveDebug.LogError("DDoveRes", ("playMode", playMode.ToString()), ("reason", "EditorSimulateMode is editor-only"));
                    return null;
#endif

                case EPlayMode.OfflinePlayMode:
                    return new OfflinePlayModeOptions
                    {
                        BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters()
                    };

                case EPlayMode.HostPlayMode:
                case EPlayMode.WebPlayMode:
                    DDoveDebug.LogError("DDoveRes", ("playMode", playMode.ToString()), ("reason", "patch FSM is not implemented yet"));
                    return null;

                default:
                    DDoveDebug.LogError("DDoveRes", ("playMode", playMode.ToString()), ("reason", "unsupported play mode"));
                    return null;
            }
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

        public static async UniTask<SceneHandle> LoadSceneAsync(
            string location,
            string packageName = null,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            CancellationToken cancellationToken = default)
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

            var handle = package.LoadSceneAsync(location, sceneMode);
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

        public static string ResolveInitPackageName()
        {
#if UNITY_EDITOR
            var fromCollector = TryGetCollectorPackageName();
            if (!string.IsNullOrEmpty(fromCollector))
            {
                return fromCollector;
            }
#endif
            return FallbackPackageName;
        }

        public static EPlayMode ResolveInitPlayMode()
        {
#if UNITY_EDITOR
            return EPlayMode.EditorSimulateMode;
#else
            return EPlayMode.OfflinePlayMode;
#endif
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

#if UNITY_EDITOR
        private static string TryGetCollectorPackageName()
        {
            var dataType = Type.GetType("YooAsset.Editor.BundleCollectorSettingData, YooAsset.Editor");
            var setting = dataType?.GetProperty("Setting", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (setting == null)
            {
                return null;
            }

            if (setting.GetType().GetField("Packages")?.GetValue(setting) is not IEnumerable packages)
            {
                return null;
            }

            string first = null;
            foreach (var package in packages)
            {
                var name = package.GetType().GetField("PackageName")?.GetValue(package) as string;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (name == FallbackPackageName)
                {
                    return name;
                }

                first ??= name;
            }

            return first;
        }
#endif
    }
}
