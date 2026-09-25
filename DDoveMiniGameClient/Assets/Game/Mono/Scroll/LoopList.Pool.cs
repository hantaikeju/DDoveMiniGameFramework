using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
    public sealed partial class LoopList
    {
        const string ExitGhostNamePrefix = "ScrollExitGhost";

        int CalcBuffer()
        {
            float view = ViewMain();
            if (view <= 0f)
            {
                view = itemSize;
            }

            int visible = Mathf.Max(1, Mathf.CeilToInt(view / Stride()));
            return visible + BufferHead + BufferTail;
        }

        bool EnsurePool()
        {
            var template = ResolveTemplate();
            var content = Content();
            if (template == null || content == null)
            {
                return false;
            }

            int want = Mathf.Min(DataCount(), CalcBuffer());
            bool changed = false;
            while (_cells.Count < want)
            {
                _cells.Add(CreateCell(template, content));
                changed = true;
            }

            while (_cells.Count > want)
            {
                int last = _cells.Count - 1;
                RecycleCell(_cells[last]);
                Destroy(_cells[last].Rect.gameObject);
                _cells.RemoveAt(last);
                changed = true;
            }

            return changed;
        }

        Cell CreateCell(RectTransform template, RectTransform content)
        {
            var go = Instantiate(template.gameObject, content, false);
            go.name = template.name;
            go.SetActive(false);
            StripNodeBind(go);
            ApplyNavigationNone(go);
            return new Cell
            {
                Rect = go.transform as RectTransform,
                Index = -1
            };
        }

        void RefreshVisible(bool force)
        {
            int start = CalcStartIndex();
            if (!force && start == _startIndex)
            {
                ApplyFanPoses();
                return;
            }

            if (!force && _startIndex != int.MinValue && _cells.Count > 0)
            {
                int delta = start - _startIndex;
                if (delta == 1 || delta == -1)
                {
                    if (delta == 1)
                    {
                        MoveHeadToTail(start);
                    }
                    else
                    {
                        MoveTailToHead(start);
                    }

                    _startIndex = start;
                    ApplyFanPoses();
                    return;
                }
            }

            _startIndex = start;
            for (int i = 0; i < _cells.Count; i++)
            {
                var cell = _cells[i];
                if (cell.Index >= 0)
                {
                    RecycleCell(cell);
                }

                ShowCell(cell, start + i);
            }

            ApplyFanPoses();
        }

        void MoveHeadToTail(int start)
        {
            var cell = _cells[0];
            _cells.RemoveAt(0);
            RecycleCell(cell);
            ShowCell(cell, start + _cells.Count);
            _cells.Add(cell);
        }

        void MoveTailToHead(int start)
        {
            int last = _cells.Count - 1;
            var cell = _cells[last];
            _cells.RemoveAt(last);
            RecycleCell(cell);
            ShowCell(cell, start);
            _cells.Insert(0, cell);
        }

        void ShowCell(Cell cell, int index)
        {
            int count = DataCount();
            if (index < 0 || index >= count)
            {
                if (cell.Rect != null)
                {
                    cell.Rect.gameObject.SetActive(false);
                }

                return;
            }

            if (!cell.Rect.gameObject.activeSelf)
            {
                cell.Rect.gameObject.SetActive(true);
            }

            ApplyItemRect(cell.Rect, index);
            ApplyNavigationNone(cell.Rect.gameObject);
            if (_source != null)
            {
                _source.Bind(cell.Rect.gameObject, index);
            }

            cell.Index = index;
            ApplyCellClick(cell);
        }

        int CalcStartIndex()
        {
            int count = DataCount();
            if (count <= 0 || _cells.Count == 0)
            {
                return 0;
            }

            var content = Content();
            float scroll = 0f;
            if (content != null)
            {
                scroll = IsHorizontal()
                    ? -content.anchoredPosition.x
                    : content.anchoredPosition.y;
            }

            int firstVisible = Mathf.FloorToInt((scroll - MainPadStart()) / Stride());
            int start = firstVisible - BufferHead;
            int maxStart = Mathf.Max(0, count - _cells.Count);
            return Mathf.Clamp(start, 0, maxStart);
        }

        void RecycleCell(Cell cell)
        {
            if (cell.Index >= 0 && _source != null && cell.Rect != null)
            {
                _source.Recycle(cell.Rect.gameObject);
            }

            cell.Index = -1;
        }

        void ApplyFanPoses()
        {
            if (_scroll == null || _scroll.viewport == null)
            {
                return;
            }

            var view = _scroll.viewport;
            Vector3 pivot = view.TransformPoint(view.rect.center);
            bool horizontal = IsHorizontal();
            for (int i = 0; i < _cells.Count; i++)
            {
                var cell = _cells[i];
                if (cell.Index < 0)
                {
                    continue;
                }

                var tween = ItemTween(cell);
                if (tween == null)
                {
                    continue;
                }

                tween.ApplyPage(pivot, horizontal);
            }
        }

        void StopCellMotion()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                var tween = ItemTween(_cells[i]);
                if (tween == null)
                {
                    continue;
                }

                tween.StopMotion();
            }
        }

        void ClearExitGhosts()
        {
            var content = Content();
            if (content == null)
            {
                return;
            }

            for (int i = content.childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i);
                if (child == null || !child.name.StartsWith(ExitGhostNamePrefix))
                {
                    continue;
                }

                Destroy(child.gameObject);
            }
        }

        static IScrollItemTween ItemTween(Cell cell)
        {
            if (cell == null || cell.Rect == null)
            {
                return null;
            }

            return cell.Rect.GetComponent<IScrollItemTween>();
        }

        void RecycleAll()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                RecycleCell(_cells[i]);
                if (_cells[i].Rect != null)
                {
                    _cells[i].Rect.gameObject.SetActive(false);
                }
            }

            _startIndex = int.MinValue;
        }

        sealed class Cell
        {
            public RectTransform Rect;
            public int Index;
            public bool ClickAttached;
        }
    }
}
