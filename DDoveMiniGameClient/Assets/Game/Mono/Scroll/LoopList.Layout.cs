using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
    public sealed partial class LoopList
    {
        int PadLeft()
        {
            return padding != null ? padding.left : 0;
        }

        int PadRight()
        {
            return padding != null ? padding.right : 0;
        }

        int PadTop()
        {
            return padding != null ? padding.top : 0;
        }

        int PadBottom()
        {
            return padding != null ? padding.bottom : 0;
        }

        float Stride()
        {
            return Mathf.Max(0.01f, itemSize + spacing);
        }

        bool IsHorizontal()
        {
            return direction == LoopDirection.Horizontal;
        }

        float MainPadStart()
        {
            return IsHorizontal() ? PadLeft() : PadTop();
        }

        float MainPadEnd()
        {
            return IsHorizontal() ? PadRight() : PadBottom();
        }

        float ContentMain(int count)
        {
            float main = MainPadStart() + MainPadEnd();
            if (count > 0)
            {
                main += count * itemSize + (count - 1) * spacing;
            }

            return main;
        }

        float ViewMain(RectTransform viewport)
        {
            if (viewport == null)
            {
                return 0f;
            }

            return IsHorizontal() ? viewport.rect.width : viewport.rect.height;
        }

        float ViewMain()
        {
            return ViewMain(Viewport());
        }

        LoopAlign EffectiveAlign(LoopAlign align)
        {
            if (IsHorizontal())
            {
                switch (align)
                {
                    case LoopAlign.Left:
                    case LoopAlign.Middle:
                    case LoopAlign.Right:
                        return align;
                    default:
                        return LoopAlign.Middle;
                }
            }

            if (align == LoopAlign.Left || align == LoopAlign.Right)
            {
                return LoopAlign.Middle;
            }

            return align;
        }

        float AlignOffset(LoopAlign align, float itemStart, float view)
        {
            switch (align)
            {
                case LoopAlign.Middle:
                    return itemStart + itemSize * 0.5f - view * 0.5f;
                case LoopAlign.Bottom:
                case LoopAlign.Right:
                    return itemStart + itemSize - view;
                default:
                    return itemStart;
            }
        }

        void LockAxis()
        {
            if (_scroll == null)
            {
                return;
            }

            bool horizontal = IsHorizontal();
            _scroll.horizontal = horizontal;
            _scroll.vertical = !horizontal;
            _scroll.movementType = ScrollRect.MovementType.Clamped;
            var content = _scroll.content;
            if (content == null)
            {
                return;
            }

            var pos = content.anchoredPosition;
            if (horizontal)
            {
                if (pos.y != 0f)
                {
                    pos.y = 0f;
                    content.anchoredPosition = pos;
                }
            }
            else if (pos.x != 0f)
            {
                pos.x = 0f;
                content.anchoredPosition = pos;
            }
        }

        void ApplyContentAnchors(RectTransform content)
        {
            Vector2 anchorMin;
            Vector2 anchorMax;
            Vector2 pivot;
            if (IsHorizontal())
            {
                anchorMin = new Vector2(0f, 0f);
                anchorMax = new Vector2(0f, 1f);
                pivot = new Vector2(0f, 0.5f);
            }
            else
            {
                anchorMin = new Vector2(0f, 1f);
                anchorMax = new Vector2(1f, 1f);
                pivot = new Vector2(0.5f, 1f);
            }

            if (content.anchorMin != anchorMin)
            {
                content.anchorMin = anchorMin;
            }

            if (content.anchorMax != anchorMax)
            {
                content.anchorMax = anchorMax;
            }

            if (content.pivot != pivot)
            {
                content.pivot = pivot;
            }
        }

        void ApplyCrossSize(RectTransform content)
        {
            var size = content.sizeDelta;
            if (IsHorizontal())
            {
                if (size.y != 0f)
                {
                    size.y = 0f;
                    content.sizeDelta = size;
                }
            }
            else if (size.x != 0f)
            {
                size.x = 0f;
                content.sizeDelta = size;
            }
        }

        void ApplyContentSize(int count)
        {
            var content = Content();
            if (content == null)
            {
                return;
            }

            ApplyContentAnchors(content);
            ApplyCrossSize(content);
            var axis = IsHorizontal()
                ? RectTransform.Axis.Horizontal
                : RectTransform.Axis.Vertical;
            content.SetSizeWithCurrentAnchors(axis, ContentMain(count));
        }

        void ApplyItemRect(RectTransform item, int index)
        {
            if (IsHorizontal())
            {
                float left = PadLeft() + index * Stride();
                int padT = PadTop();
                int padB = PadBottom();
                item.anchorMin = new Vector2(0f, 0f);
                item.anchorMax = new Vector2(0f, 1f);
                item.pivot = new Vector2(0f, 0.5f);
                item.sizeDelta = new Vector2(itemSize, -(padT + padB));
                item.anchoredPosition = new Vector2(left, (padB - padT) * 0.5f);
                return;
            }

            float top = PadTop() + index * Stride();
            int padL = PadLeft();
            int padR = PadRight();
            item.anchorMin = new Vector2(0f, 1f);
            item.anchorMax = new Vector2(1f, 1f);
            item.pivot = new Vector2(0.5f, 1f);
            item.sizeDelta = new Vector2(-(padL + padR), itemSize);
            item.anchoredPosition = new Vector2((padL - padR) * 0.5f, -top);
        }
    }
}
