using System;
using System.Collections.Generic;
using System.IO;
using DDoveFramework.Core;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveSave
{
    public static class DDoveSaveKit
    {
        public const string LogTitle = "DDoveSave";

        private static Dictionary<string, byte[]> _data = new Dictionary<string, byte[]>();
        private static readonly MessagePackSerializerOptions Options =
            ContractlessStandardResolverAllowPrivate.Options;

        public static string CurrentFileName { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _data = new Dictionary<string, byte[]>();
            CurrentFileName = null;
        }

        public static void LoadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                DDoveDebug.LogError(LogTitle, ("reason", "LoadFile name empty"));
                return;
            }

            CurrentFileName = fileName;
            var path = GetPath(fileName);
            if (TryReadFile(path, out var data))
            {
                _data = data;
                try
                {
                    File.Copy(path, path + ".bak", true);
                }
                catch (Exception e)
                {
                    DDoveDebug.LogError(LogTitle, ("reason", "bak copy failed"), ("error", e.Message));
                }

                return;
            }

            DDoveDebug.LogWarning(LogTitle, ("reason", "primary missing or invalid"), ("file", fileName));
            if (TryReadFile(path + ".bak", out var backup))
            {
                _data = backup;
                return;
            }

            DDoveDebug.LogWarning(LogTitle, ("reason", "no backup, empty store"), ("file", fileName));
            _data = new Dictionary<string, byte[]>();
        }

        public static void SaveFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                DDoveDebug.LogError(LogTitle, ("reason", "SaveFile name empty"));
                return;
            }

            var path = GetPath(fileName);
            var tempPath = path + ".tmp";

            try
            {
                File.WriteAllBytes(tempPath, Serialize(_data));
                if (!TryReadFile(tempPath, out _))
                {
                    throw new InvalidOperationException("Temporary save file validation failed.");
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.Move(tempPath, path);
            }
            catch (Exception e)
            {
                DDoveDebug.LogError(LogTitle, ("reason", "SaveFile failed"), ("file", fileName), ("error", e.Message));
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        public static void Save<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
            {
                DDoveDebug.LogError(LogTitle, ("reason", "Save key empty"));
                return;
            }

            _data[key] = Serialize(data);
        }

        public static bool TryLoad<T>(string key, out T data)
        {
            if (!string.IsNullOrEmpty(key) && _data.TryGetValue(key, out var bytes))
            {
                data = Deserialize<T>(bytes);
                return true;
            }

            data = default;
            return false;
        }

        public static bool HasKey(string key)
        {
            return !string.IsNullOrEmpty(key) && _data.ContainsKey(key);
        }

        public static void DeleteKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _data.Remove(key);
        }

        public static void Clear()
        {
            _data.Clear();
        }

        private static bool TryReadFile(string path, out Dictionary<string, byte[]> data)
        {
            try
            {
                if (File.Exists(path))
                {
                    var bytes = File.ReadAllBytes(path);
                    if (bytes.Length > 0)
                    {
                        data = Deserialize<Dictionary<string, byte[]>>(bytes);
                        return data != null;
                    }
                }
            }
            catch (Exception e)
            {
                DDoveDebug.LogError(LogTitle, ("reason", "read failed"), ("path", path), ("error", e.Message));
            }

            data = null;
            return false;
        }

        private static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        private static byte[] Serialize<T>(T data)
        {
            return MessagePackSerializer.Serialize(data, Options);
        }

        private static T Deserialize<T>(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes, Options);
        }
    }
}
