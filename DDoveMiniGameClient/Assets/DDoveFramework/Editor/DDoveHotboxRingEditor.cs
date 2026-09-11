using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    internal sealed class DDoveHotboxRingEditor
    {
        private const string CatalogIdKey = "DDoveHotbox.CatalogId";

        private DDoveHotboxConfig _config;
        private int _ring;
        private string _search = string.Empty;
        private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
        private VisualElement _catalogList;
        private VisualElement _previewBox;
        private VisualElement _ringChrome;
        private VisualElement _slotHost;
        private float _pieLaidWidth;
        private float _pieLaidHeight;
        private int _pieLaidRing;
        private int _pieLaidSlots;
        private string _pieLaidName;
        private bool _movingPie;
        private float _pieSize;
        private float _pieItemWidth;
        private float _pieItemHeight;
        private VisualElement _pieLayer;

        public VisualElement Build()
        {
            _config = DDoveHotboxScanner.GetOrCreateConfig();
            var root = new VisualElement();
            root.AddToClassList("ddove-split");
            if (_config == null)
            {
                root.Add(new HelpBox("无法创建 DDoveHotboxConfig。", HelpBoxMessageType.Error));
                return root;
            }

            if (_config.Rings.Count == 0)
            {
                _config.Rings.Add(new DDoveHotboxRing { Name = "UI" });
                Save();
            }

            _ring = Mathf.Clamp(_ring, 0, _config.Rings.Count - 1);

            var catalog = new VisualElement();
            catalog.AddToClassList("ddove-card");
            catalog.AddToClassList("ddove-catalog");
            var catalogTitle = new Label("指令");
            catalogTitle.AddToClassList("ddove-card-title");
            catalog.Add(catalogTitle);
            var search = new TextField { value = _search };
            search.AddToClassList("ddove-field");
            search.style.marginTop = 0;
            search.RegisterValueChangedCallback(evt =>
            {
                _search = evt.newValue ?? string.Empty;
                RebuildCatalog();
            });
            catalog.Add(search);
            var catalogScroll = new ScrollView(ScrollViewMode.Vertical);
            catalogScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            catalogScroll.style.flexGrow = 1;
            catalogScroll.style.minHeight = 0;
            _catalogList = new VisualElement();
            catalogScroll.Add(_catalogList);
            catalog.Add(catalogScroll);

            var zones = new VisualElement();
            zones.AddToClassList("ddove-card");
            zones.AddToClassList("ddove-zones");
            _ringChrome = new VisualElement();
            _ringChrome.AddToClassList("ddove-hotbox-chrome");
            zones.Add(_ringChrome);
            _previewBox = BuildPreview();
            zones.Add(_previewBox);
            _slotHost = new VisualElement();
            _slotHost.AddToClassList("ddove-hotbox-slots");
            zones.Add(_slotHost);

            root.Add(catalog);
            root.Add(Divider());
            root.Add(zones);
            RebuildCatalog();
            RebuildRing();
            return root;
        }

        private static VisualElement Divider()
        {
            var divider = new VisualElement();
            divider.AddToClassList("ddove-divider");
            return divider;
        }

        private void RebuildCatalog()
        {
            if (_catalogList == null)
            {
                return;
            }

            _catalogList.Clear();
            var groups = new Dictionary<string, List<DDoveHotboxEntry>>();
            foreach (var entry in DDoveHotboxScanner.ScanAll())
            {
                if (!string.IsNullOrEmpty(_search)
                    && entry.Label.IndexOf(_search, StringComparison.OrdinalIgnoreCase) < 0
                    && entry.Group.IndexOf(_search, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                if (!groups.TryGetValue(entry.Group, out var list))
                {
                    list = new List<DDoveHotboxEntry>();
                    groups.Add(entry.Group, list);
                }

                list.Add(entry);
            }

            if (groups.Count == 0)
            {
                _catalogList.Add(new HelpBox("没有 [DDoveHotboxEntry]。给静态无参方法挂这个属性。", HelpBoxMessageType.Info));
                return;
            }

            foreach (var pair in groups)
            {
                if (!_foldouts.TryGetValue(pair.Key, out var open))
                {
                    open = true;
                    _foldouts[pair.Key] = true;
                }

                var fold = new Foldout { text = pair.Key, value = open };
                fold.RegisterValueChangedCallback(evt => _foldouts[pair.Key] = evt.newValue);
                foreach (var entry in pair.Value)
                {
                    fold.Add(CatalogRow(entry));
                }

                _catalogList.Add(fold);
            }
        }

        private static VisualElement CatalogRow(DDoveHotboxEntry entry)
        {
            var row = new Label(entry.Label);
            row.tooltip = entry.Tooltip;
            row.AddToClassList("ddove-chip");
            row.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = Array.Empty<UnityEngine.Object>();
                DragAndDrop.SetGenericData(CatalogIdKey, entry.Id);
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                DragAndDrop.StartDrag(entry.Label);
                evt.StopPropagation();
            });
            return row;
        }

        private void RebuildRing()
        {
            if (_ringChrome == null || _slotHost == null || _config == null)
            {
                return;
            }

            _ring = Mathf.Clamp(_ring, 0, Mathf.Max(_config.Rings.Count - 1, 0));
            _ringChrome.Clear();
            _slotHost.Clear();

            var title = new Label("热盒");
            title.AddToClassList("ddove-card-title");
            _ringChrome.Add(title);

            var toolbar = new VisualElement();
            toolbar.AddToClassList("ddove-hotbox-toolbar");
            var tabs = new VisualElement();
            tabs.AddToClassList("ddove-tabs");
            for (var i = 0; i < _config.Rings.Count; i++)
            {
                var index = i;
                var name = string.IsNullOrEmpty(_config.Rings[i].Name) ? "热盒" : _config.Rings[i].Name;
                var tab = new Button(() =>
                {
                    _ring = index;
                    RebuildRing();
                })
                {
                    text = name
                };
                tab.AddToClassList("ddove-tab");
                if (_ring == i)
                {
                    tab.AddToClassList("ddove-tab--active");
                }

                tabs.Add(tab);
            }

            toolbar.Add(tabs);
            var add = new Button(() =>
            {
                Undo.RecordObject(_config, "DDoveHotbox Add Ring");
                _config.Rings.Add(new DDoveHotboxRing { Name = "热盒" + (_config.Rings.Count + 1) });
                _ring = _config.Rings.Count - 1;
                Save();
                RebuildRing();
            })
            {
                text = "+"
            };
            add.tooltip = "新增热盒";
            add.AddToClassList("ddove-hotbox-add");
            if (_config.Rings.Count > 0)
            {
                var current = _config.Rings[_ring];
                var scatter = new Button(() =>
                {
                    if (_previewBox == null)
                    {
                        return;
                    }

                    Undo.RecordObject(_config, "DDoveHotbox Scatter");
                    DDoveHotboxPieLayout.Scatter(current);
                    Save();
                    _pieLaidSlots = -1;
                    RelayoutPie();
                })
                {
                    text = "均匀排布"
                };
                scatter.tooltip = "按一圈重新排开";
                scatter.AddToClassList("ddove-hotbox-tool");
                toolbar.Add(scatter);
                if (_config.Rings.Count > 1)
                {
                    var remove = new Button(() =>
                    {
                        if (!EditorUtility.DisplayDialog("删除热盒", $"删除「{current.Name}」？", "删除", "取消"))
                        {
                            return;
                        }

                        Undo.RecordObject(_config, "DDoveHotbox Remove Ring");
                        _config.Rings.RemoveAt(_ring);
                        _ring = Mathf.Max(0, _ring - 1);
                        Save();
                        RebuildRing();
                    })
                    {
                        text = "删除"
                    };
                    remove.AddToClassList("ddove-hotbox-tool");
                    toolbar.Add(remove);
                }
            }

            toolbar.Add(add);
            _ringChrome.Add(toolbar);

            if (_config.Rings.Count == 0)
            {
                _previewBox?.Clear();
                return;
            }

            var ring = _config.Rings[_ring];
            var nameRow = new VisualElement();
            nameRow.AddToClassList("ddove-hotbox-name");
            var nameField = new TextField("名称") { value = ring.Name };
            nameField.AddToClassList("ddove-field");
            nameField.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(_config, "DDoveHotbox Rename Ring");
                ring.Name = evt.newValue;
                Save();
            });
            nameField.RegisterCallback<FocusOutEvent>(_ => RebuildRing());
            nameRow.Add(nameField);
            _ringChrome.Add(nameRow);

            var slotScroll = new ScrollView(ScrollViewMode.Vertical);
            slotScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            slotScroll.style.flexGrow = 1;
            slotScroll.style.minHeight = 0;
            slotScroll.Add(BuildSlotList(ring));
            _slotHost.Add(slotScroll);
            RelayoutPie();
        }

        private VisualElement BuildPreview()
        {
            var box = new VisualElement();
            box.AddToClassList("ddove-pie-preview");
            RegisterDrop(box, -1);
            box.RegisterCallback<GeometryChangedEvent>(_ => RelayoutPie());
            return box;
        }

        private void RelayoutPie()
        {
            if (_movingPie || _previewBox == null || _config == null || _config.Rings.Count == 0)
            {
                return;
            }

            _ring = Mathf.Clamp(_ring, 0, _config.Rings.Count - 1);
            var ring = _config.Rings[_ring];
            var width = _previewBox.layout.width;
            var height = _previewBox.layout.height;
            if (width < 8f || height < 8f)
            {
                return;
            }

            if (Mathf.Abs(width - _pieLaidWidth) < 0.5f
                && Mathf.Abs(height - _pieLaidHeight) < 0.5f
                && _pieLaidRing == _ring
                && _pieLaidSlots == ring.Slots.Count
                && _pieLaidName == ring.Name
                && _previewBox.childCount > 0)
            {
                return;
            }

            _pieLaidWidth = width;
            _pieLaidHeight = height;
            _pieLaidRing = _ring;
            _pieLaidSlots = ring.Slots.Count;
            _pieLaidName = ring.Name;
            LayoutPie(_previewBox, ring, width, height);
        }

        private void LayoutPie(VisualElement box, DDoveHotboxRing ring, float width, float height)
        {
            box.Clear();
            var count = Mathf.Max(ring.Slots.Count, 1);
            _pieSize = Mathf.Min(width, height);
            var scale = _pieSize / DDoveHotboxPieLayout.PieSize(count);
            _pieItemWidth = DDoveHotboxPieLayout.ItemWidth * scale;
            _pieItemHeight = DDoveHotboxPieLayout.ItemHeight * scale;
            var centerSize = DDoveHotboxPieLayout.Center * scale;
            _pieLayer = new VisualElement();
            _pieLayer.style.position = Position.Absolute;
            _pieLayer.style.width = _pieSize;
            _pieLayer.style.height = _pieSize;
            _pieLayer.style.left = (width - _pieSize) * 0.5f;
            _pieLayer.style.top = (height - _pieSize) * 0.5f;
            box.Add(_pieLayer);
            var centerPos = DDoveHotboxPieLayout.CenterOrigin(_pieSize, centerSize);
            var center = new Label(string.IsNullOrEmpty(ring.Name) ? "热盒" : ring.Name);
            center.AddToClassList("ddove-pie-center");
            center.style.left = centerPos.x;
            center.style.top = centerPos.y;
            center.style.width = centerSize;
            center.style.height = centerSize;
            center.style.borderTopLeftRadius = centerSize * 0.5f;
            center.style.borderTopRightRadius = centerSize * 0.5f;
            center.style.borderBottomLeftRadius = centerSize * 0.5f;
            center.style.borderBottomRightRadius = centerSize * 0.5f;
            _pieLayer.Add(center);
            if (ring.Slots.Count == 0)
            {
                var empty = new Label("拖到这里，再按住指令挪位置");
                empty.AddToClassList("ddove-hint");
                empty.style.position = Position.Absolute;
                empty.style.left = 0;
                empty.style.right = 0;
                empty.style.bottom = 16;
                empty.style.unityTextAlign = TextAnchor.MiddleCenter;
                empty.pickingMode = PickingMode.Ignore;
                _pieLayer.Add(empty);
                return;
            }

            for (var i = 0; i < ring.Slots.Count; i++)
            {
                var index = i;
                var slot = ring.Slots[i];
                var found = DDoveHotboxScanner.TryFind(slot.EntryId, out var discovered);
                var item = new Label(found ? discovered.Label : "[?]");
                item.tooltip = "按住拖动可改位置";
                item.AddToClassList("ddove-pie-item");
                if (!found)
                {
                    item.AddToClassList("ddove-pie-item--missing");
                }

                var origin = DDoveHotboxPieLayout.SlotOrigin(
                    slot,
                    index,
                    count,
                    _pieSize,
                    _pieItemWidth,
                    _pieItemHeight);
                item.style.left = origin.x;
                item.style.top = origin.y;
                item.style.width = _pieItemWidth;
                item.style.height = _pieItemHeight;
                item.style.borderTopLeftRadius = _pieItemHeight * 0.5f;
                item.style.borderTopRightRadius = _pieItemHeight * 0.5f;
                item.style.borderBottomLeftRadius = _pieItemHeight * 0.5f;
                item.style.borderBottomRightRadius = _pieItemHeight * 0.5f;
                RegisterDrop(item, index);
                RegisterMove(item, slot);
                _pieLayer.Add(item);
            }
        }

        private VisualElement BuildSlotList(DDoveHotboxRing ring)
        {
            var list = new VisualElement();
            list.AddToClassList("ddove-card-plate");
            list.AddToClassList("ddove-card-plate--lead");
            if (ring.Slots.Count == 0)
            {
                list.Add(new HelpBox("从左边拖到投放区。按住指令可挪位置，均匀则重新围一圈。", HelpBoxMessageType.Info));
                return list;
            }

            for (var i = 0; i < ring.Slots.Count; i++)
            {
                var index = i;
                var found = DDoveHotboxScanner.TryFind(ring.Slots[i].EntryId, out var discovered);
                var row = new VisualElement();
                row.AddToClassList("ddove-zone-entry");
                var label = new Label((index + 1) + ". " + (found ? discovered.Label : "[?] " + ring.Slots[i].EntryId));
                label.AddToClassList("ddove-chip");
                if (!found)
                {
                    label.AddToClassList("ddove-chip--missing");
                }

                var remove = new Button(() =>
                {
                    Undo.RecordObject(_config, "DDoveHotbox Remove Slot");
                    ring.Slots.RemoveAt(index);
                    Save();
                    RebuildRing();
                })
                {
                    text = "×"
                };
                remove.style.width = 22;
                remove.style.height = 24;
                row.Add(label);
                row.Add(remove);
                list.Add(row);
            }

            return list;
        }

        private void RegisterDrop(VisualElement target, int insertAt)
        {
            target.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                if (!(DragAndDrop.GetGenericData(CatalogIdKey) is string))
                {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                target.AddToClassList("ddove-drop--active");
                evt.StopPropagation();
            });
            target.RegisterCallback<DragLeaveEvent>(_ => target.RemoveFromClassList("ddove-drop--active"));
            target.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (!(DragAndDrop.GetGenericData(CatalogIdKey) is string id))
                {
                    return;
                }

                DragAndDrop.AcceptDrag();
                target.RemoveFromClassList("ddove-drop--active");
                AddSlot(id, insertAt);
                evt.StopPropagation();
            });
        }

        private void RegisterMove(VisualElement item, DDoveHotboxSlot slot)
        {
            var grab = Vector2.zero;
            item.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                grab = evt.localMousePosition;
                _movingPie = true;
                item.CaptureMouse();
                item.AddToClassList("ddove-pie-item--moving");
                evt.StopPropagation();
            });
            item.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!_movingPie || !item.HasMouseCapture() || _previewBox == null)
                {
                    return;
                }

                var host = _pieLayer != null ? _pieLayer : _previewBox;
                var local = host.WorldToLocal(evt.mousePosition);
                var origin = DDoveHotboxPieLayout.ClampOrigin(
                    local.x - grab.x,
                    local.y - grab.y,
                    _pieSize,
                    _pieItemWidth,
                    _pieItemHeight);
                item.style.left = origin.x;
                item.style.top = origin.y;
                evt.StopPropagation();
            });
            item.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!item.HasMouseCapture())
                {
                    return;
                }

                item.ReleaseMouse();
                item.RemoveFromClassList("ddove-pie-item--moving");
                _movingPie = false;
                var left = item.resolvedStyle.left;
                var top = item.resolvedStyle.top;
                var normalized = DDoveHotboxPieLayout.ToNormalized(
                    left,
                    top,
                    _pieSize,
                    _pieItemWidth,
                    _pieItemHeight);
                Undo.RecordObject(_config, "DDoveHotbox Move Slot");
                slot.Placed = true;
                slot.X = normalized.x;
                slot.Y = normalized.y;
                Save();
                evt.StopPropagation();
            });
        }

        private void AddSlot(string entryId, int insertAt)
        {
            var ring = _config.Rings[_ring];
            if (ring.Slots.Exists(item => item.EntryId == entryId))
            {
                return;
            }

            Undo.RecordObject(_config, "DDoveHotbox Add Slot");
            var slot = new DDoveHotboxSlot { EntryId = entryId };
            if (insertAt >= 0 && insertAt <= ring.Slots.Count)
            {
                ring.Slots.Insert(insertAt, slot);
            }
            else
            {
                ring.Slots.Add(slot);
            }

            var placedAt = insertAt >= 0 ? insertAt : ring.Slots.Count - 1;
            var normalized = DDoveHotboxPieLayout.CircleNormalized(placedAt, ring.Slots.Count);
            slot.Placed = true;
            slot.X = normalized.x;
            slot.Y = normalized.y;

            Save();
            RebuildRing();
        }

        private void Save()
        {
            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssetIfDirty(_config);
        }
    }
}
