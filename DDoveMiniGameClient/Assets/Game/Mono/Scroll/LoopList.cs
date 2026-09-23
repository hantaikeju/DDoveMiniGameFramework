using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    [RequireComponent(typeof(ScrollRect))]
    public sealed partial class LoopList : MonoBehaviour
    {
        const int BufferHead = 1;
        const int BufferTail = 1;
        const string NodeBindTypeName = "DDoveUINodeBind";

        [SerializeField] RectTransform itemTemplate;
        [SerializeField] LoopDirection direction = LoopDirection.Vertical;
        [SerializeField] float itemSize = 100f;
        [SerializeField] float spacing;
        [SerializeField] RectOffset padding = new RectOffset();
        [SerializeField] int previewCount = 6;

        readonly List<Cell> _cells = new List<Cell>();
        ScrollRect _scroll;
        DataSource _source;
        int _boundCount;
        int _startIndex = int.MinValue;
        bool _subscribed;

        public bool HorizontalMotion => direction == LoopDirection.Horizontal;

        public float MotionItemSize => itemSize;

        public float MotionStride => Mathf.Max(0.01f, itemSize + spacing);

        public void Create<T>(IList<T> data)
        {
            BindHierarchy();
            if (Application.isPlaying)
            {
                RecycleAll();
            }

            _source = data != null ? new DataSource<T>(data) : null;
            CaptureCount();
            if (!Application.isPlaying || !isActiveAndEnabled)
            {
                return;
            }

            ApplyAndRefresh();
        }

        public T GetData<T>(int index)
        {
            var source = _source as DataSource<T>;
            if (source == null || source.List == null)
            {
                return default;
            }

            if (index < 0 || index >= source.List.Count)
            {
                return default;
            }

            return source.List[index];
        }

        public RectTransform GetShown(int index)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                var cell = _cells[i];
                if (cell.Index == index && cell.Rect != null)
                {
                    return cell.Rect;
                }
            }

            return null;
        }

        public ILoopItem<T> GetShown<T>(int index)
        {
            var rect = GetShown(index);
            return rect != null ? rect.GetComponent<ILoopItem<T>>() : null;
        }

        public void ScrollTo(int index, LoopAlign align)
        {
            CacheRefs();
            int count = DataCount();
            if (count <= 0)
            {
                return;
            }

            index = Mathf.Clamp(index, 0, count - 1);
            var content = Content();
            var viewport = Viewport();
            if (content == null)
            {
                return;
            }

            ApplyContentSize(count);
            LockAxis();
            float view = ViewMain(viewport);
            float itemStart = MainPadStart() + index * Stride();
            float offset = AlignOffset(EffectiveAlign(align), itemStart, view);
            float maxOffset = Mathf.Max(0f, ContentMain(count) - view);
            offset = Mathf.Clamp(offset, 0f, maxOffset);
            var pos = content.anchoredPosition;
            if (IsHorizontal())
            {
                pos.x = -offset;
            }
            else
            {
                pos.y = offset;
            }

            content.anchoredPosition = pos;
            if (_scroll != null)
            {
                _scroll.StopMovement();
            }

            EnsurePool();
            RefreshVisible(true);
        }

        public void Refresh()
        {
            CacheRefs();
            if (!Application.isPlaying)
            {
                return;
            }

            CaptureCount();
            ApplyAndRefresh();
        }

        void Awake()
        {
            CacheRefs();
        }

        void OnEnable()
        {
            CacheRefs();
            if (Application.isPlaying)
            {
#if UNITY_EDITOR
                ClearPreview();
#endif
                Subscribe();
                if (_source != null)
                {
                    ApplyAndRefresh();
                }

                return;
            }

#if UNITY_EDITOR
            _previewDirty = true;
#endif
        }

        void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            CacheRefs();
            if (_source != null)
            {
                ApplyAndRefresh();
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                Unsubscribe();
                StopCellMotion();
                ClearExitGhosts();
                RecycleAll();
            }
        }

        void OnDestroy()
        {
            Unsubscribe();
            if (Application.isPlaying)
            {
                StopCellMotion();
                ClearExitGhosts();
                RecycleAll();
                DropSource();
            }

#if UNITY_EDITOR
            ClearPreview();
#endif
        }

        public void BindHierarchy()
        {
            CacheRefs();
            if (_scroll == null)
            {
                return;
            }

            var viewport = transform.Find("Viewport") as RectTransform;
            if (viewport != null && _scroll.viewport == null)
            {
                _scroll.viewport = viewport;
            }

            var contentParent = _scroll.viewport != null ? _scroll.viewport : viewport;
            var content = contentParent != null ? contentParent.Find("Content") as RectTransform : null;
            if (content != null && _scroll.content == null)
            {
                _scroll.content = content;
            }

            LockAxis();

            if (itemTemplate == null && content != null)
            {
                var template = content.Find("itemTemplate") as RectTransform;
                if (template != null)
                {
                    itemTemplate = template;
                }
            }
        }

        void CacheRefs()
        {
            if (_scroll == null)
            {
                _scroll = GetComponent<ScrollRect>();
            }
        }

        void Subscribe()
        {
            if (_subscribed || _scroll == null)
            {
                return;
            }

            _scroll.onValueChanged.AddListener(OnScroll);
            _subscribed = true;
        }

        void Unsubscribe()
        {
            if (!_subscribed || _scroll == null)
            {
                return;
            }

            _scroll.onValueChanged.RemoveListener(OnScroll);
            _subscribed = false;
        }

        void OnScroll(Vector2 _)
        {
            bool poolChanged = EnsurePool();
            RefreshVisible(poolChanged);
        }

        void ApplyAndRefresh()
        {
            LockAxis();
            ApplyContentSize(DataCount());
            EnsurePool();
            RefreshVisible(true);
        }

        void DropSource()
        {
            _source = null;
            _boundCount = 0;
        }

        void CaptureCount()
        {
            _boundCount = _source != null ? _source.Count : 0;
        }

        int DataCount()
        {
            return _boundCount;
        }

        RectTransform ResolveTemplate()
        {
            if (itemTemplate != null)
            {
                return itemTemplate;
            }

            var content = Content();
            if (content == null)
            {
                return null;
            }

            for (int i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i) as RectTransform;
                if (child == null || child.gameObject.activeSelf)
                {
                    continue;
                }

                if ((child.gameObject.hideFlags & HideFlags.DontSave) != 0)
                {
                    continue;
                }

                return child;
            }

            return null;
        }

        RectTransform Content()
        {
            return _scroll != null ? _scroll.content : null;
        }

        RectTransform Viewport()
        {
            if (_scroll == null)
            {
                return null;
            }

            if (_scroll.viewport != null)
            {
                return _scroll.viewport;
            }

            return transform as RectTransform;
        }

        static void ApplyNavigationNone(GameObject go)
        {
            var selectables = go.GetComponentsInChildren<Selectable>(true);
            for (int i = 0; i < selectables.Length; i++)
            {
                var nav = selectables[i].navigation;
                nav.mode = Navigation.Mode.None;
                selectables[i].navigation = nav;
            }
        }

        static void StripNodeBind(GameObject go)
        {
            var behaviours = go.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                if (behaviour == null)
                {
                    continue;
                }

                if (behaviour.GetType().Name != NodeBindTypeName)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(behaviour);
                }
                else
                {
                    DestroyImmediate(behaviour);
                }
            }
        }

        abstract class DataSource
        {
            public abstract int Count { get; }

            public abstract void Bind(GameObject go, int index);

            public abstract void Recycle(GameObject go);
        }

        sealed class DataSource<T> : DataSource
        {
            public readonly IList<T> List;

            public DataSource(IList<T> list)
            {
                List = list;
            }

            public override int Count
            {
                get { return List != null ? List.Count : 0; }
            }

            public override void Bind(GameObject go, int index)
            {
                if (List == null || index < 0 || index >= List.Count)
                {
                    return;
                }

                var item = go.GetComponent<ILoopItem<T>>();
                if (item != null)
                {
                    item.Bind(List[index], index);
                }
            }

            public override void Recycle(GameObject go)
            {
                var item = go.GetComponent<ILoopItem<T>>();
                if (item != null)
                {
                    item.Recycle();
                }
            }
        }
    }
}
