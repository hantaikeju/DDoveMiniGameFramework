using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace DDoveFramework.Editor
{
    public readonly struct DDoveEditorPanelEntry
    {
        public DDoveEditorPanelEntry(string id, string title, int order, Type panelType)
        {
            Id = id;
            Title = title;
            Order = order;
            PanelType = panelType;
        }

        public string Id { get; }

        public string Title { get; }

        public int Order { get; }

        public Type PanelType { get; }
    }

    public static class DDoveEditorRegistry
    {
        public static List<DDoveEditorPanelEntry> Collect()
        {
            var list = new List<DDoveEditorPanelEntry>();
            foreach (var type in TypeCache.GetTypesWithAttribute<DDoveEditorPanelAttribute>())
            {
                if (type.IsAbstract || !typeof(IDDoveEditorPanel).IsAssignableFrom(type))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<DDoveEditorPanelAttribute>();
                if (attribute == null || string.IsNullOrEmpty(attribute.Id))
                {
                    continue;
                }

                list.Add(new DDoveEditorPanelEntry(attribute.Id, attribute.Title, attribute.Order, type));
            }

            list.Sort(Compare);
            return list;
        }

        public static IDDoveEditorPanel Create(DDoveEditorPanelEntry entry)
        {
            return (IDDoveEditorPanel)Activator.CreateInstance(entry.PanelType);
        }

        private static int Compare(DDoveEditorPanelEntry left, DDoveEditorPanelEntry right)
        {
            var leftNav = DDoveEditorNav.IndexOf(left.Id);
            var rightNav = DDoveEditorNav.IndexOf(right.Id);
            if (leftNav >= 0 || rightNav >= 0)
            {
                if (leftNav < 0)
                {
                    return 1;
                }

                if (rightNav < 0)
                {
                    return -1;
                }

                return leftNav.CompareTo(rightNav);
            }

            var order = left.Order.CompareTo(right.Order);
            return order != 0 ? order : string.CompareOrdinal(left.Title, right.Title);
        }
    }
}
