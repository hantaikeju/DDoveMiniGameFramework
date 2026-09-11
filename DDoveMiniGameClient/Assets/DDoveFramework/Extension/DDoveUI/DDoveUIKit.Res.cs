using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveRes;
using UnityEngine;
using YooAsset;

namespace DDoveFramework.Extension.DDoveUI
{
    public static partial class DDoveUIKit
    {
        private static readonly Dictionary<string, AssetHandle> AssetHandles = new Dictionary<string, AssetHandle>();

        private static async UniTask<GameObject> LoadPanelPrefabAsync(string panelName)
        {
            if (AssetHandles.TryGetValue(panelName, out var cached) && cached.IsValid)
            {
                return cached.AssetObject as GameObject;
            }

            if (cached != null)
            {
                AssetHandles.Remove(panelName);
            }

            var handle = await DDoveResKit.LoadAssetAsync<GameObject>(panelName);
            if (handle == null)
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", panelName), ("reason", "load prefab failed"));
                return null;
            }

            AssetHandles[panelName] = handle;
            return handle.AssetObject as GameObject;
        }

        private static void OnPanelClosed(string panelName)
        {
            if (!AssetHandles.TryGetValue(panelName, out var handle))
            {
                return;
            }

            handle.Release();
            AssetHandles.Remove(panelName);
        }
    }
}
