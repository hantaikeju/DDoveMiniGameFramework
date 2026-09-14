using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveRes;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;

namespace DDoveFramework.Extension.DDoveAtlas
{
    public static class DDoveAtlasKit
    {
        public const string LogTitle = DDoveAtlasInitInfo.LogTitle;

        private static readonly Dictionary<string, AtlasEntry> Atlases = new Dictionary<string, AtlasEntry>();
        private static readonly Dictionary<int, HashSet<string>> OwnerAtlases = new Dictionary<int, HashSet<string>>();

        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            SpriteAtlasManager.atlasRequested += OnAtlasRequested;
            _initialized = true;
        }

        public static async UniTask<Sprite> LoadSpriteAsync(
            string url,
            int ownerId,
            CancellationToken cancellationToken = default)
        {
            if (!TryParseUrl(url, out var atlasName, out var spriteName))
            {
                return null;
            }

            var entry = await LoadAtlasAsync(atlasName, false, ownerId, cancellationToken);
            if (entry?.Atlas == null)
            {
                return null;
            }

            return GetOrCreateSprite(entry, spriteName);
        }

        public static void ReleaseScope(int ownerId)
        {
            if (!OwnerAtlases.TryGetValue(ownerId, out var names))
            {
                return;
            }

            OwnerAtlases.Remove(ownerId);
            foreach (var atlasName in names)
            {
                if (!Atlases.TryGetValue(atlasName, out var entry))
                {
                    continue;
                }

                entry.Owners.Remove(ownerId);
                if (entry.Owners.Count == 0 && !entry.LateBound)
                {
                    ReleaseEntry(atlasName, entry);
                }
            }
        }

        private static void OnAtlasRequested(string atlasName, Action<SpriteAtlas> callback)
        {
            BindAtlasAsync(atlasName, callback).Forget();
        }

        private static async UniTaskVoid BindAtlasAsync(string atlasName, Action<SpriteAtlas> callback)
        {
            try
            {
                var entry = await LoadAtlasAsync(atlasName, true, null, CancellationToken.None);
                callback?.Invoke(entry?.Atlas);
            }
            catch (Exception e)
            {
                DDoveDebug.LogError(LogTitle, ("atlas", atlasName), ("lateBind", e.Message));
                callback?.Invoke(null);
            }
        }

        private static async UniTask<AtlasEntry> LoadAtlasAsync(
            string atlasName,
            bool lateBound,
            int? ownerId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(atlasName))
            {
                return null;
            }

            if (Atlases.TryGetValue(atlasName, out var existing))
            {
                if (existing.Loading != null)
                {
                    await existing.Loading.Task;
                }

                if (existing.Atlas == null)
                {
                    return existing;
                }

                Attach(existing, atlasName, lateBound, ownerId);
                return existing;
            }

            var entry = new AtlasEntry();
            entry.Loading = new UniTaskCompletionSource<SpriteAtlas>();
            Atlases[atlasName] = entry;

            AssetHandle handle;
            try
            {
                handle = await DDoveResKit.LoadAssetAsync<SpriteAtlas>(atlasName, cancellationToken: cancellationToken);
            }
            catch
            {
                entry.Loading.TrySetResult(null);
                entry.Loading = null;
                Atlases.Remove(atlasName);
                throw;
            }

            if (handle == null)
            {
                entry.Loading.TrySetResult(null);
                entry.Loading = null;
                Atlases.Remove(atlasName);
                return null;
            }

            entry.Handle = handle;
            entry.Atlas = handle.GetAssetObject<SpriteAtlas>();
            if (entry.Atlas == null)
            {
                handle.Release();
                entry.Loading.TrySetResult(null);
                entry.Loading = null;
                Atlases.Remove(atlasName);
                DDoveDebug.LogError(LogTitle, ("atlas", atlasName), ("reason", "atlas asset null"));
                return null;
            }

            Attach(entry, atlasName, lateBound, ownerId);
            entry.Loading.TrySetResult(entry.Atlas);
            entry.Loading = null;
            return entry;
        }

        private static void Attach(AtlasEntry entry, string atlasName, bool lateBound, int? ownerId)
        {
            if (lateBound)
            {
                entry.LateBound = true;
            }

            if (!ownerId.HasValue)
            {
                return;
            }

            var id = ownerId.Value;
            if (!entry.Owners.Add(id))
            {
                return;
            }

            if (!OwnerAtlases.TryGetValue(id, out var names))
            {
                names = new HashSet<string>();
                OwnerAtlases[id] = names;
            }

            names.Add(atlasName);
        }

        private static Sprite GetOrCreateSprite(AtlasEntry entry, string spriteName)
        {
            if (entry.Sprites.TryGetValue(spriteName, out var cached) && cached != null)
            {
                return cached;
            }

            var sprite = entry.Atlas.GetSprite(spriteName);
            if (sprite == null)
            {
                DDoveDebug.LogError(LogTitle, ("sprite", spriteName), ("reason", "not in atlas"));
                return null;
            }

            entry.Sprites[spriteName] = sprite;
            return sprite;
        }

        private static void ReleaseEntry(string atlasName, AtlasEntry entry)
        {
            foreach (var sprite in entry.Sprites.Values)
            {
                if (sprite != null)
                {
                    UnityEngine.Object.Destroy(sprite);
                }
            }

            entry.Sprites.Clear();
            entry.Handle?.Release();
            Atlases.Remove(atlasName);
        }

        private static bool TryParseUrl(string url, out string atlasName, out string spriteName)
        {
            atlasName = null;
            spriteName = null;
            if (string.IsNullOrEmpty(url))
            {
                DDoveDebug.LogError(LogTitle, ("url", url ?? ""), ("reason", "empty"));
                return false;
            }

            var split = url.IndexOf('/');
            if (split <= 0 || split == url.Length - 1)
            {
                DDoveDebug.LogError(LogTitle, ("url", url), ("reason", "expected atlas/sprite"));
                return false;
            }

            atlasName = url.Substring(0, split);
            spriteName = url.Substring(split + 1);
            return true;
        }

        private sealed class AtlasEntry
        {
            public AssetHandle Handle;
            public SpriteAtlas Atlas;
            public bool LateBound;
            public UniTaskCompletionSource<SpriteAtlas> Loading;
            public readonly HashSet<int> Owners = new HashSet<int>();
            public readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
        }
    }
}
