using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveRes;
using UnityEngine;
using YooAsset;

namespace DDoveFramework.Extension.DDoveCfg
{
    public static class DDoveCfgKit
    {
        public const string LogTitle = "DDoveCfg";
        public const string AssetTag = "cfg";

        private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();

        public static bool IsLoaded { get; private set; }

        public static async UniTask<bool> LoadAsync(CancellationToken cancellationToken = default)
        {
            Texts.Clear();
            IsLoaded = false;

            var package = DDoveResKit.GetPackage();
            if (package == null)
            {
                DDoveDebug.LogError(LogTitle, ("reason", "package not ready"));
                return false;
            }

            var infos = package.GetAssetInfos(AssetTag);
            if (infos == null || infos.Length == 0)
            {
                DDoveDebug.LogError(LogTitle, ("tag", AssetTag), ("reason", "no cfg assets"));
                return false;
            }

            for (var i = 0; i < infos.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var address = infos[i].Address;
                var handle = await DDoveResKit.LoadAssetAsync<TextAsset>(address, cancellationToken: cancellationToken);
                if (handle == null)
                {
                    Texts.Clear();
                    return false;
                }

                var asset = handle.GetAssetObject<TextAsset>();
                if (asset == null)
                {
                    handle.Release();
                    DDoveDebug.LogError(LogTitle, ("location", address), ("reason", "text asset null"));
                    Texts.Clear();
                    return false;
                }

                Cache(address, asset.text);
                handle.Release();
            }

            IsLoaded = true;
            return true;
        }

        public static string RequireText(string file)
        {
            if (TryGetText(file, out var text))
            {
                return text;
            }

            DDoveDebug.LogError(LogTitle, ("file", file), ("reason", "cfg text missing"));
            return null;
        }

        public static bool TryGetText(string file, out string text)
        {
            if (string.IsNullOrEmpty(file))
            {
                text = null;
                return false;
            }

            if (Texts.TryGetValue(file, out text))
            {
                return true;
            }

            var slash = file.LastIndexOf('/');
            var name = slash >= 0 ? file.Substring(slash + 1) : file;
            if (Texts.TryGetValue(name, out text))
            {
                return true;
            }

            var dot = name.LastIndexOf('.');
            if (dot > 0 && Texts.TryGetValue(name.Substring(0, dot), out text))
            {
                return true;
            }

            text = null;
            return false;
        }

        private static void Cache(string address, string text)
        {
            Texts[address] = text;
            var slash = address.LastIndexOf('/');
            var name = slash >= 0 ? address.Substring(slash + 1) : address;
            Texts[name] = text;
            var dot = name.LastIndexOf('.');
            if (dot > 0)
            {
                Texts[name.Substring(0, dot)] = text;
            }
        }
    }
}
