using System.IO;
using DDoveFramework.Editor;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace DDoveFramework.Extension.DDoveAtlas.Editor
{
    public static class DDoveAtlasSpriteEditor
    {
        private const string InitInfoAssetPath =
            "Assets/DDoveFramework/Extension/DDoveAtlas/Resources/DDoveAtlasInitInfo.asset";

        [DDoveHotboxEntry("生成图集", "Atlas", "从选中的模块散图文件夹生成 SpriteAtlas")]
        public static void GenerateAtlasFromFolder()
        {
            var info = GetOrCreateInitInfo();
            var folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                EditorUtility.DisplayDialog("DDoveAtlas", "请先在 Project 里选中散图文件夹。", "确定");
                return;
            }

            if (!TryResolveOutput(info, folderPath, out var atlasName, out var saveDir))
            {
                EditorUtility.DisplayDialog("DDoveAtlas", $"当前：{folderPath}", "确定");
                return;
            }

            try
            {
                ProcessTexturesInFolder(folderPath, info);
                AssetDatabase.Refresh();

                var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
                if (folder == null)
                {
                    EditorUtility.DisplayDialog("DDoveAtlas", $"读不到文件夹：{folderPath}", "确定");
                    return;
                }

                EnsureFolder(saveDir);
                var atlasPath = ResolveAtlasPath(saveDir, atlasName);
                if (IsSpriteAtlasV2Path(atlasPath))
                {
                    WriteOrUpdateAtlasV2(atlasPath, folder, info);
                }
                else
                {
                    WriteOrUpdateAtlasV1(atlasPath, folder, info);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath)
                    ?? AssetDatabase.LoadMainAssetAtPath(atlasPath);
                EditorUtility.DisplayDialog(
                    "DDoveAtlas",
                    $"当前：{folderPath}\n产物：{atlasPath}",
                    "确定");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DDoveAtlas] {e.Message}\n{e.StackTrace}");
            }
        }

        private static bool TryResolveOutput(
            DDoveAtlasInitInfo info,
            string folderPath,
            out string atlasName,
            out string saveDir)
        {
            atlasName = null;
            saveDir = null;
            var source = info.SourceRoot.TrimEnd('/');
            var normalized = folderPath.Replace('\\', '/').TrimEnd('/');
            if (!normalized.StartsWith(source + "/", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var relative = normalized.Substring(source.Length + 1);
            if (string.IsNullOrEmpty(relative) || relative.IndexOf('/') >= 0)
            {
                return false;
            }

            atlasName = relative;
            saveDir = info.OutputRoot.TrimEnd('/');
            return true;
        }

        private static string ResolveAtlasPath(string saveDir, string atlasName)
        {
            var v2 = $"{saveDir}/{atlasName}.spriteatlasv2";
            var v1 = $"{saveDir}/{atlasName}.spriteatlas";
            if (File.Exists(v2))
            {
                return v2;
            }

            if (File.Exists(v1))
            {
                return v1;
            }

            return UsesSpriteAtlasV2() ? v2 : v1;
        }

        private static bool IsSpriteAtlasV2Path(string path)
        {
            return path.EndsWith(".spriteatlasv2", System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool UsesSpriteAtlasV2()
        {
            var mode = EditorSettings.spritePackerMode;
            return mode == SpritePackerMode.SpriteAtlasV2 || mode == SpritePackerMode.SpriteAtlasV2Build;
        }

        private static DDoveAtlasInitInfo GetOrCreateInitInfo()
        {
            var info = AssetDatabase.LoadAssetAtPath<DDoveAtlasInitInfo>(InitInfoAssetPath);
            if (info != null)
            {
                return info;
            }

            var folder = "Assets/DDoveFramework/Extension/DDoveAtlas/Resources";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets/DDoveFramework/Extension/DDoveAtlas", "Resources");
            }

            info = ScriptableObject.CreateInstance<DDoveAtlasInitInfo>();
            AssetDatabase.CreateAsset(info, InitInfoAssetPath);
            AssetDatabase.SaveAssets();
            return info;
        }

        private static bool WriteOrUpdateAtlasV2(string atlasPath, DefaultAsset folder, DDoveAtlasInitInfo info)
        {
            var existing = File.Exists(atlasPath) ? TryLoadAtlasAsset(atlasPath) : null;
            var isNew = existing == null;
            if (File.Exists(atlasPath) && existing == null)
            {
                AssetDatabase.DeleteAsset(atlasPath);
            }

            var asset = new SpriteAtlasAsset();
            asset.Add(new Object[] { folder });
            SpriteAtlasAsset.Save(asset, atlasPath);
            AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceSynchronousImport);

            if (!isNew)
            {
                return false;
            }

            var importer = AssetImporter.GetAtPath(atlasPath) as SpriteAtlasImporter;
            if (importer == null)
            {
                throw new System.InvalidOperationException($"没有 SpriteAtlasImporter：{atlasPath}");
            }

            importer.includeInBuild = false;
            importer.packingSettings = CreatePackingSettings(info);
            importer.textureSettings = CreateTextureSettings(info);
            ApplyImporterTextureSize(importer, 2048);
            importer.SaveAndReimport();
            return true;
        }

        private static SpriteAtlasAsset TryLoadAtlasAsset(string atlasPath)
        {
            try
            {
                return SpriteAtlasAsset.Load(atlasPath);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static bool WriteOrUpdateAtlasV1(string atlasPath, DefaultAsset folder, DDoveAtlasInitInfo info)
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            var isNew = atlas == null;
            if (isNew)
            {
                atlas = new SpriteAtlas();
                AssetDatabase.CreateAsset(atlas, atlasPath);
                atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                SpriteAtlasExtensions.SetIncludeInBuild(atlas, false);
                atlas.SetPackingSettings(CreatePackingSettings(info));
                atlas.SetTextureSettings(CreateTextureSettings(info));
            }
            else
            {
                var old = SpriteAtlasExtensions.GetPackables(atlas);
                if (old != null && old.Length > 0)
                {
                    SpriteAtlasExtensions.Remove(atlas, old);
                }
            }

            SpriteAtlasExtensions.Add(atlas, new Object[] { folder });
            EditorUtility.SetDirty(atlas);
            return isNew;
        }

        private static SpriteAtlasPackingSettings CreatePackingSettings(DDoveAtlasInitInfo info)
        {
            return new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = info.Padding
            };
        }

        private static SpriteAtlasTextureSettings CreateTextureSettings(DDoveAtlasInitInfo info)
        {
            return new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = info.FilterMode,
                anisoLevel = 1
            };
        }

        private static void ApplyImporterTextureSize(SpriteAtlasImporter importer, int maxTextureSize)
        {
            var so = new SerializedObject(importer);
            var maxSize = so.FindProperty("m_TextureSettings.maxTextureSize")
                ?? so.FindProperty("textureSettings.maxTextureSize");
            if (maxSize == null)
            {
                return;
            }

            maxSize.intValue = maxTextureSize;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ProcessTexturesInFolder(string folderPath, DDoveAtlasInitInfo info)
        {
            var guids = AssetDatabase.FindAssets("t:Texture", new[] { folderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                var dirty = importer.textureType != TextureImporterType.Sprite || importer.mipmapEnabled;
                importer.textureType = TextureImporterType.Sprite;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteExtrude = info.PixelArtPreset ? 2u : 0u;
                importer.SetTextureSettings(settings);
                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        private static void EnsureFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        private static string GetSelectedFolderPath()
        {
            var objects = Selection.GetFiltered<Object>(SelectionMode.Assets);
            foreach (var obj in objects)
            {
                var path = AssetDatabase.GetAssetPath(obj).Replace('\\', '/');
                if (AssetDatabase.IsValidFolder(path))
                {
                    return path;
                }

                if (File.Exists(path))
                {
                    var dir = Path.GetDirectoryName(path)?.Replace('\\', '/');
                    if (!string.IsNullOrEmpty(dir) && AssetDatabase.IsValidFolder(dir))
                    {
                        return dir;
                    }
                }
            }

            return null;
        }
    }
}
