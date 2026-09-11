using System;

namespace DDoveFramework.Editor
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DDoveHotboxEntryAttribute : Attribute
    {
        public const string ArchitectureGroup = "Architecture";

        public DDoveHotboxEntryAttribute(string label, string group = "通用", string tooltip = "")
        {
            Label = label;
            Group = string.IsNullOrEmpty(group) ? "通用" : group;
            Tooltip = tooltip ?? string.Empty;
        }

        public string Label { get; }

        public string Group { get; }

        public string Tooltip { get; }
    }
}
