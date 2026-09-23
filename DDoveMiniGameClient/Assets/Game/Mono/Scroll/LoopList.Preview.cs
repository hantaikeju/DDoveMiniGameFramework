using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
    public sealed partial class LoopList
    {
        const HideFlags PreviewHideFlags = HideFlags.HideAndDontSave;

#if UNITY_EDITOR
        readonly List<RectTransform> _preview = new List<RectTransform>();
        bool _previewDirty = true;
        float _previewItemSize;
        float _previewSpacing;
        int _previewPadL;
        int _previewPadR;
        int _previewPadT;
        int _previewPadB;
        int _previewCountSeen;
        int _previewTemplateId;
        LoopDirection _previewDirection;

        void OnValidate()
        {
            if (itemSize < 1f)
            {
                itemSize = 1f;
            }

            if (spacing < 0f)
            {
                spacing = 0f;
            }

            if (previewCount < 0)
            {
                previewCount = 0;
            }

            _previewDirty = true;
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            RefreshPreviewIfNeeded();
        }

        void RefreshPreviewIfNeeded()
        {
            CacheRefs();
            var template = ResolveTemplate();
            int templateId = template != null ? template.GetInstanceID() : 0;
            if (!_previewDirty
                && Mathf.Approximately(_previewItemSize, itemSize)
                && Mathf.Approximately(_previewSpacing, spacing)
                && _previewPadL == PadLeft()
                && _previewPadR == PadRight()
                && _previewPadT == PadTop()
                && _previewPadB == PadBottom()
                && _previewCountSeen == previewCount
                && _previewTemplateId == templateId
                && _previewDirection == direction
                && _preview.Count == Mathf.Max(0, previewCount))
            {
                return;
            }

            RebuildPreview(template);
        }

        void RebuildPreview(RectTransform template)
        {
            bool axisChanged = _previewDirection != direction;
            ClearPreview();
            LockAxis();
            ApplyContentSize(previewCount);
            if (axisChanged)
            {
                var reset = Content();
                if (reset != null && reset.anchoredPosition != Vector2.zero)
                {
                    reset.anchoredPosition = Vector2.zero;
                }
            }

            _previewDirty = false;
            _previewItemSize = itemSize;
            _previewSpacing = spacing;
            _previewPadL = PadLeft();
            _previewPadR = PadRight();
            _previewPadT = PadTop();
            _previewPadB = PadBottom();
            _previewCountSeen = previewCount;
            _previewTemplateId = template != null ? template.GetInstanceID() : 0;
            _previewDirection = direction;
            var content = Content();
            if (template == null || content == null || previewCount <= 0)
            {
                return;
            }

            for (int i = 0; i < previewCount; i++)
            {
                var go = Instantiate(template.gameObject);
                go.name = template.name;
                ApplyPreviewFlags(go);
                go.transform.SetParent(content, false);
                ApplyPreviewFlags(go);
                go.SetActive(true);
                StripNodeBind(go);
                ApplyNavigationNone(go);
                var rect = go.transform as RectTransform;
                ApplyItemRect(rect, i);
                _preview.Add(rect);
            }
        }

        static void ApplyPreviewFlags(GameObject go)
        {
            go.hideFlags = PreviewHideFlags;
            var transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                ApplyPreviewFlags(transform.GetChild(i).gameObject);
            }
        }

        void ClearPreview()
        {
            for (int i = 0; i < _preview.Count; i++)
            {
                if (_preview[i] != null)
                {
                    DestroyImmediate(_preview[i].gameObject);
                }
            }

            _preview.Clear();

            var content = Content();
            if (content == null)
            {
                return;
            }

            for (int i = content.childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i).gameObject;
                if ((child.hideFlags & HideFlags.DontSave) != 0)
                {
                    DestroyImmediate(child);
                }
            }
        }
#endif
    }
}
