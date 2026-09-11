using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using DDoveFramework.Core;
using UnityEditor;
using UnityEditor.Compilation;

namespace DDoveFramework.Editor
{
    internal readonly struct DDoveArchitectureBindEntry : IEquatable<DDoveArchitectureBindEntry>
    {
        public DDoveArchitectureBindEntry(string role, string keyFullName, string implFullName)
        {
            Role = role;
            KeyFullName = keyFullName;
            ImplFullName = implFullName;
        }

        public string Role { get; }

        public string KeyFullName { get; }

        public string ImplFullName { get; }

        public bool Equals(DDoveArchitectureBindEntry other)
        {
            return Role == other.Role && KeyFullName == other.KeyFullName && ImplFullName == other.ImplFullName;
        }

        public override bool Equals(object obj)
        {
            return obj is DDoveArchitectureBindEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Role != null ? Role.GetHashCode() : 0;
                hash = (hash * 397) ^ (KeyFullName != null ? KeyFullName.GetHashCode() : 0);
                hash = (hash * 397) ^ (ImplFullName != null ? ImplFullName.GetHashCode() : 0);
                return hash;
            }
        }
    }

    internal static class DDoveArchitectureBindGenerator
    {
        public const string GeneratedPath = "Assets/Game/Generate/Core/GameArchitecture.Generated.cs";

        private const string BindPrefix = "// BIND:";
        private const string LogTitle = "Architecture";

        public const string GenerateAfterCompileKey = "DDove.ArchitectureBind.GenerateAfterCompile";

        [MenuItem("DDove/Generate Architecture Bind")]
        [DDoveHotboxEntry("生成根类", DDoveHotboxEntryAttribute.ArchitectureGroup, "按特性集合重写 GameArchitecture")]
        public static void MenuGenerate()
        {
            GenerateAndRefresh();
        }

        public static void GenerateAndRefresh()
        {
            if (TryGenerate(out var wroteFile) && wroteFile)
            {
                AssetDatabase.Refresh();
            }
        }

        public static void QueueGenerateAfterCompile()
        {
            SessionState.SetBool(GenerateAfterCompileKey, true);
        }

        public static bool TryCollect(out List<DDoveArchitectureBindEntry> entries)
        {
            entries = new List<DDoveArchitectureBindEntry>();
            var errors = new List<string>();
            var keys = new HashSet<string>();

            CollectRole(
                TypeCache.GetTypesWithAttribute<DDoveBindModelAttribute>(),
                "Model",
                typeof(IModel),
                type => type,
                entries,
                keys,
                errors);
            CollectRole(
                TypeCache.GetTypesWithAttribute<DDoveBindSystemAttribute>(),
                "System",
                typeof(ISystem),
                type => type,
                entries,
                keys,
                errors);
            CollectRole(
                TypeCache.GetTypesWithAttribute<DDoveBindUtilityAttribute>(),
                "Utility",
                typeof(IUtility),
                ResolveUtilityKey,
                entries,
                keys,
                errors);

            if (errors.Count == 0)
            {
                entries.Sort(Compare);
                return true;
            }

            for (var i = 0; i < errors.Count; i++)
            {
                DDoveDebug.LogError(LogTitle, ("reason", errors[i]));
            }

            entries = null;
            return false;
        }

        public static bool IsGeneratedCurrent(IReadOnlyList<DDoveArchitectureBindEntry> entries)
        {
            if (!File.Exists(GeneratedPath))
            {
                return false;
            }

            var actual = ParseBindLines(File.ReadAllText(GeneratedPath));
            if (actual.Count != entries.Count)
            {
                return false;
            }

            var expected = new HashSet<DDoveArchitectureBindEntry>(entries);
            return expected.SetEquals(actual);
        }

        public static bool TryGenerate(out bool wroteFile)
        {
            wroteFile = false;
            if (!TryCollect(out var entries))
            {
                return false;
            }

            var text = Render(entries);
            if (File.Exists(GeneratedPath) && File.ReadAllText(GeneratedPath) == text)
            {
                return true;
            }

            var directory = Path.GetDirectoryName(GeneratedPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(GeneratedPath, text);
            wroteFile = true;
            return true;
        }

        private static void CollectRole(
            TypeCache.TypeCollection types,
            string role,
            Type required,
            Func<Type, Type> resolveKey,
            List<DDoveArchitectureBindEntry> entries,
            HashSet<string> keys,
            List<string> errors)
        {
            foreach (var type in types)
            {
                if (IsEditorAssembly(type))
                {
                    continue;
                }

                if (!IsPublicConcrete(type))
                {
                    errors.Add($"{type.FullName} is not a public concrete class");
                    continue;
                }

                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    errors.Add($"{type.FullName} has no public parameterless constructor");
                    continue;
                }

                if (!required.IsAssignableFrom(type))
                {
                    errors.Add($"{type.FullName} does not implement {required.Name}");
                    continue;
                }

                Type keyType;
                try
                {
                    keyType = resolveKey(type);
                }
                catch (InvalidOperationException e)
                {
                    errors.Add(e.Message);
                    continue;
                }

                if (keyType == null)
                {
                    errors.Add($"{type.FullName} bind key is missing");
                    continue;
                }

                var keyName = CSharpName(keyType);
                if (!keys.Add(keyName))
                {
                    errors.Add($"duplicate bind key {keyName}");
                    continue;
                }

                entries.Add(new DDoveArchitectureBindEntry(role, keyName, CSharpName(type)));
            }
        }

        private static Type ResolveUtilityKey(Type type)
        {
            var attribute = type.GetCustomAttribute<DDoveBindUtilityAttribute>();
            if (attribute?.As == null)
            {
                return type;
            }

            if (attribute.As.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"{type.FullName} As must not be an open generic");
            }

            if (!typeof(IUtility).IsAssignableFrom(attribute.As))
            {
                throw new InvalidOperationException($"{type.FullName} As {attribute.As.FullName} is not IUtility");
            }

            if (!attribute.As.IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"{type.FullName} is not assignable to As {attribute.As.FullName}");
            }

            return attribute.As;
        }

        private static bool IsPublicConcrete(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                return false;
            }

            return type.IsPublic || type.IsNestedPublic;
        }

        private static bool IsEditorAssembly(Type type)
        {
            var assemblyName = type.Assembly.GetName().Name;
            var assemblies = CompilationPipeline.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].name == assemblyName)
                {
                    return (assemblies[i].flags & AssemblyFlags.EditorAssembly) != 0;
                }
            }

            return false;
        }

        private static string CSharpName(Type type)
        {
            if (string.IsNullOrEmpty(type.FullName))
            {
                return type.Name;
            }

            return "global::" + type.FullName.Replace('+', '.');
        }

        private static int Compare(DDoveArchitectureBindEntry left, DDoveArchitectureBindEntry right)
        {
            var role = RoleOrder(left.Role).CompareTo(RoleOrder(right.Role));
            if (role != 0)
            {
                return role;
            }

            var key = string.CompareOrdinal(left.KeyFullName, right.KeyFullName);
            return key != 0 ? key : string.CompareOrdinal(left.ImplFullName, right.ImplFullName);
        }

        private static int RoleOrder(string role)
        {
            switch (role)
            {
                case "Utility":
                    return 0;
                case "Model":
                    return 1;
                default:
                    return 2;
            }
        }

        private static HashSet<DDoveArchitectureBindEntry> ParseBindLines(string text)
        {
            var set = new HashSet<DDoveArchitectureBindEntry>();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith(BindPrefix, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var body = line.Substring(BindPrefix.Length);
                    var parts = body.Split(':');
                    if (parts.Length != 3)
                    {
                        continue;
                    }

                    set.Add(new DDoveArchitectureBindEntry(parts[0], parts[1], parts[2]));
                }
            }

            return set;
        }

        private static string Render(IReadOnlyList<DDoveArchitectureBindEntry> entries)
        {
            var sb = new StringBuilder(512);
            sb.AppendLine("//------------------------------------------------------------------------------");
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("//     Generated by DDove Architecture bind. Do not edit.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine("//------------------------------------------------------------------------------");
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                sb.Append(BindPrefix)
                    .Append(entry.Role)
                    .Append(':')
                    .Append(entry.KeyFullName)
                    .Append(':')
                    .Append(entry.ImplFullName)
                    .AppendLine();
            }

            sb.AppendLine("using DDoveFramework.Core;");
            sb.AppendLine();
            sb.AppendLine("namespace Game");
            sb.AppendLine("{");
            sb.AppendLine("    public sealed class GameArchitecture : Architecture<GameArchitecture>");
            sb.AppendLine("    {");
            sb.AppendLine("        protected override void Init()");
            sb.AppendLine("        {");
            for (var i = 0; i < entries.Count; i++)
            {
                AppendRegister(sb, entries[i]);
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void AppendRegister(StringBuilder sb, DDoveArchitectureBindEntry entry)
        {
            sb.Append("            ");
            switch (entry.Role)
            {
                case "Model":
                    sb.Append("RegisterModel(new ").Append(entry.ImplFullName).AppendLine("());");
                    break;
                case "System":
                    sb.Append("RegisterSystem(new ").Append(entry.ImplFullName).AppendLine("());");
                    break;
                default:
                    if (entry.KeyFullName == entry.ImplFullName)
                    {
                        sb.Append("RegisterUtility(new ").Append(entry.ImplFullName).AppendLine("());");
                    }
                    else
                    {
                        sb.Append("RegisterUtility<")
                            .Append(entry.KeyFullName)
                            .Append(">(new ")
                            .Append(entry.ImplFullName)
                            .AppendLine("());");
                    }

                    break;
            }
        }
    }
}
