using System.Collections.Generic;
using System.Reflection;
using DDoveFramework.Core;
using TMPro;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI
{
    public static partial class DDoveUIKit
    {
        public const string RebuiltDynamicFontSessionKey = "DDoveUI.RebuiltDynamicFont";

        private static readonly HashSet<int> FontSeen = new HashSet<int>();
        private static readonly List<Texture2D> OwnedAtlases = new List<Texture2D>();
        private static readonly FieldInfo AtlasIndexField = typeof(TMP_FontAsset).GetField(
            "m_AtlasTextureIndex",
            BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo ClearTablesMethod = typeof(TMP_FontAsset).GetMethod(
            "ClearFontAssetTables",
            BindingFlags.Instance | BindingFlags.NonPublic);

        private static void EnsureDynamicFontAtlases()
        {
            FontSeen.Clear();
            EnsureFont(TMP_Settings.defaultFontAsset);
            var fallbacks = TMP_Settings.fallbackFontAssets;
            if (fallbacks == null)
            {
                return;
            }

            for (var i = 0; i < fallbacks.Count; i++)
            {
                EnsureFont(fallbacks[i]);
            }

            var loaded = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            for (var i = 0; i < loaded.Length; i++)
            {
                EnsureFont(loaded[i]);
            }
        }

        private static void EnsureFont(TMP_FontAsset font)
        {
            if (font == null || !FontSeen.Add(font.GetInstanceID()))
            {
                return;
            }

            if (font.atlasPopulationMode == AtlasPopulationMode.Dynamic)
            {
                EnsureDynamicAtlas(font);
            }

            var table = font.fallbackFontAssetTable;
            if (table == null)
            {
                return;
            }

            for (var i = 0; i < table.Count; i++)
            {
                EnsureFont(table[i]);
            }
        }

        private static void EnsureDynamicAtlas(TMP_FontAsset font)
        {
            if (TryOwnPages(font))
            {
                return;
            }

            var recovered = FindLiveAtlas(font);
            if (recovered != null && !OwnsAtlas(recovered))
            {
                recovered = CloneAtlas(recovered);
            }

            if (recovered == null)
            {
                var width = font.atlasWidth > 0 ? font.atlasWidth : 1024;
                var height = font.atlasHeight > 0 ? font.atlasHeight : 1024;
                recovered = new Texture2D(width, height, TextureFormat.Alpha8, false)
                {
                    name = font.name + " Atlas",
                    hideFlags = HideFlags.HideAndDontSave
                };
                OwnedAtlases.Add(recovered);
            }

            AssignPrimary(font, recovered, true);
            MarkRebuilt(font);
            DDoveDebug.LogWarning(
                DDoveUIInitInfo.LogTitle,
                ("reason", "dynamic font atlas missing, rebuilt"),
                ("font", font.name));
        }

        private static bool TryOwnPages(TMP_FontAsset font)
        {
            var textures = font.atlasTextures;
            if (textures == null || textures.Length == 0)
            {
                return false;
            }

            var used = font.atlasTextureCount;
            var chars = font.characterTable;
            if (chars != null)
            {
                for (var i = 0; i < chars.Count; i++)
                {
                    var glyph = chars[i] != null ? chars[i].glyph : null;
                    if (glyph != null && glyph.atlasIndex + 1 > used)
                    {
                        used = glyph.atlasIndex + 1;
                    }
                }
            }

            if (used <= 0 || used > textures.Length)
            {
                return false;
            }

            var copies = new Texture2D[textures.Length];
            var changed = false;
            for (var i = 0; i < used; i++)
            {
                var tex = textures[i];
                if (tex == null)
                {
                    return false;
                }

                if (OwnsAtlas(tex))
                {
                    copies[i] = tex;
                    continue;
                }

                var copy = CloneAtlas(tex);
                if (copy == null)
                {
                    return false;
                }

                copies[i] = copy;
                changed = true;
            }

            if (!changed)
            {
                return true;
            }

            font.atlasTextures = copies;
            if (font.material != null && copies[0] != null)
            {
                font.material.SetTexture("_MainTex", copies[0]);
            }

            MarkRebuilt(font);
            return true;
        }

        private static void AssignPrimary(TMP_FontAsset font, Texture2D texture, bool resetTables)
        {
            var textures = font.atlasTextures;
            if (textures == null || textures.Length == 0)
            {
                font.atlasTextures = new[] { texture };
            }
            else
            {
                textures[0] = texture;
            }

            AtlasIndexField?.SetValue(font, 0);
            Pin(texture);
            if (font.material != null)
            {
                font.material.SetTexture("_MainTex", texture);
            }

            if (!resetTables)
            {
                return;
            }

            if (ClearTablesMethod != null)
            {
                ClearTablesMethod.Invoke(font, null);
                font.ReadFontAssetDefinition();
                return;
            }

            font.ClearFontAssetData(false);
        }

        private static Texture2D FindLiveAtlas(TMP_FontAsset font)
        {
            if (font.material != null && font.material.GetTexture("_MainTex") is Texture2D fromMaterial && fromMaterial != null)
            {
                return fromMaterial;
            }

#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GetAssetPath(font);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            Texture2D best = null;
            for (var i = 0; i < assets.Length; i++)
            {
                if (assets[i] is Texture2D tex && tex != null && (best == null || tex.width > best.width))
                {
                    best = tex;
                }
            }

            return best;
#else
            return null;
#endif
        }

        private static bool OwnsAtlas(Texture2D texture)
        {
            return texture != null && (texture.hideFlags & HideFlags.DontUnloadUnusedAsset) != 0;
        }

        private static Texture2D CloneAtlas(Texture2D source)
        {
            if (source == null || !source.isReadable)
            {
                return null;
            }

            var copy = new Texture2D(source.width, source.height, source.format, false)
            {
                name = source.name,
                filterMode = source.filterMode,
                wrapMode = source.wrapMode,
                hideFlags = HideFlags.HideAndDontSave
            };
            copy.LoadRawTextureData(source.GetRawTextureData());
            copy.Apply(false, false);
            OwnedAtlases.Add(copy);
            return copy;
        }

        private static void Pin(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        }

        private static void MarkRebuilt(TMP_FontAsset font)
        {
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GetAssetPath(font);
            if (!string.IsNullOrEmpty(path))
            {
                UnityEditor.SessionState.SetString(RebuiltDynamicFontSessionKey, path);
            }
#endif
        }
    }
}
