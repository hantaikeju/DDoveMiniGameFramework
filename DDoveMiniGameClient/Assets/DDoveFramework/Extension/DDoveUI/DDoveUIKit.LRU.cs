using System.Collections.Generic;
using DDoveFramework.Core;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI
{
    public static partial class DDoveUIKit
    {
        private static readonly Dictionary<string, IDDoveUIPanel> LruCache = new Dictionary<string, IDDoveUIPanel>();
        private static readonly LinkedList<string> LruOrder = new LinkedList<string>();
        private static readonly Dictionary<string, LinkedListNode<string>> LruNodes = new Dictionary<string, LinkedListNode<string>>();

        public static bool IsPanelCached<T>() where T : DDoveUIPanelBase<T>
        {
            return LruCache.ContainsKey(typeof(T).Name);
        }

        public static int GetCachedPanelCount()
        {
            return LruCache.Count;
        }

        private static bool TryCachePanel(string panelName, IDDoveUIPanel panel)
        {
            if (PanelsWithChildren.Contains(panelName))
            {
                return false;
            }

            var capacity = Config.PanelCacheCapacity;
            if (capacity <= 0 || !(panel is MonoBehaviour behaviour))
            {
                return false;
            }

            if (LruNodes.TryGetValue(panelName, out var existing))
            {
                LruOrder.Remove(existing);
                LruNodes[panelName] = LruOrder.AddFirst(panelName);
                return true;
            }

            if (LruCache.Count >= capacity)
            {
                EvictLruTail();
            }

            behaviour.transform.SetParent(_cacheRoot.transform, false);
            LruCache[panelName] = panel;
            LruNodes[panelName] = LruOrder.AddFirst(panelName);
            return true;
        }

        private static bool TryPopFromCache<T>(string panelName, out T panel) where T : DDoveUIPanelBase<T>
        {
            panel = null;
            if (!LruCache.TryGetValue(panelName, out var cached))
            {
                return false;
            }

            if (LruNodes.TryGetValue(panelName, out var node))
            {
                LruOrder.Remove(node);
                LruNodes.Remove(panelName);
            }

            LruCache.Remove(panelName);
            panel = cached as T;
            return panel != null;
        }

        private static void RemoveFromCache(string panelName)
        {
            if (!LruCache.TryGetValue(panelName, out var panel))
            {
                return;
            }

            if (LruNodes.TryGetValue(panelName, out var node))
            {
                LruOrder.Remove(node);
                LruNodes.Remove(panelName);
            }

            LruCache.Remove(panelName);
            CloseRegisteredChildren(panelName);
            ReleasePanelInstance(panelName, panel);
        }

        private static void ClearLRUCache()
        {
            foreach (var pair in LruCache)
            {
                CloseRegisteredChildren(pair.Key);
                ReleasePanelInstance(pair.Key, pair.Value);
            }

            LruCache.Clear();
            LruOrder.Clear();
            LruNodes.Clear();
        }

        private static void EvictLruTail()
        {
            if (LruOrder.Count == 0)
            {
                return;
            }

            var tailName = LruOrder.Last.Value;
            LruOrder.RemoveLast();
            LruNodes.Remove(tailName);
            if (!LruCache.TryGetValue(tailName, out var evicted))
            {
                return;
            }

            LruCache.Remove(tailName);
            CloseRegisteredChildren(tailName);
            ReleasePanelInstance(tailName, evicted);
            DDoveDebug.Log(DDoveUIInitInfo.LogTitle, ("lru", tailName));
        }
    }
}
