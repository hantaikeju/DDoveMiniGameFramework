using System;

namespace DDoveFramework.Editor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DDoveEditorPanelAttribute : Attribute
    {
        public DDoveEditorPanelAttribute(string id, string title, int order = 0)
        {
            Id = id;
            Title = title;
            Order = order;
        }

        public string Id { get; }

        public string Title { get; }

        public int Order { get; }
    }
}
