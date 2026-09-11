using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    [InitializeOnLoad]
    internal static class DDoveHotboxPopup
    {
        private const string RingKey = "DDove.Hotbox.Ring";

        private static VisualElement _host;
        private static SceneView _view;
        private static DDoveHotboxConfig _config;
        private static int _ring;
        private static Vector2 _origin;

        static DDoveHotboxPopup()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            var ev = Event.current;
            if (ev.type != EventType.KeyDown || ev.keyCode != KeyCode.Space)
            {
                return;
            }

            if (_host != null)
            {
                Close();
                ev.Use();
                return;
            }

            _config = DDoveHotboxScanner.GetOrCreateConfig();
            if (!DDoveHotboxScanner.HasSlots(_config))
            {
                return;
            }

            _ring = Mathf.Clamp(SessionState.GetInt(RingKey, 0), 0, _config.Rings.Count - 1);
            if (_config.Rings[_ring].Slots.Count == 0)
            {
                _ring = FirstFilledRing();
            }

            _view = sceneView;
            _origin = ev.mousePosition;
            Show(sceneView);
            ev.Use();
        }

        private static int FirstFilledRing()
        {
            for (var i = 0; i < _config.Rings.Count; i++)
            {
                if (_config.Rings[i].Slots.Count > 0)
                {
                    return i;
                }
            }

            return 0;
        }

        private static void Show(SceneView sceneView)
        {
            Close();
            var root = sceneView.rootVisualElement;
            if (root == null)
            {
                return;
            }

            _host = new VisualElement();
            _host.AddToClassList("ddove-pie-overlay");
            _host.pickingMode = PickingMode.Position;
            DDoveEditorUi.ApplySheet(_host);
            _host.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.target == _host)
                {
                    Close();
                    evt.StopPropagation();
                }
            });

            var pie = new VisualElement();
            pie.AddToClassList("ddove-pie");
            pie.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
            _host.Add(pie);
            BuildPie(pie);
            root.Add(_host);
            sceneView.Repaint();
        }

        private static void BuildPie(VisualElement pie)
        {
            pie.Clear();
            var ring = _config.Rings[_ring];
            var count = Mathf.Max(ring.Slots.Count, 1);
            var size = DDoveHotboxPieLayout.PieSize(count);
            pie.style.width = size;
            pie.style.height = size;
            pie.style.left = _origin.x - size * 0.5f;
            pie.style.top = _origin.y - size * 0.5f;

            var centerPos = DDoveHotboxPieLayout.CenterOrigin(size);
            var center = new Label(string.IsNullOrEmpty(ring.Name) ? "热盒" : ring.Name);
            center.AddToClassList("ddove-pie-center");
            center.pickingMode = PickingMode.Position;
            center.style.left = centerPos.x;
            center.style.top = centerPos.y;
            center.tooltip = _config.Rings.Count > 1 ? "切换热盒" : ring.Name;
            center.RegisterCallback<ClickEvent>(_ =>
            {
                if (_config.Rings.Count <= 1)
                {
                    return;
                }

                _ring = (_ring + 1) % _config.Rings.Count;
                SessionState.SetInt(RingKey, _ring);
                BuildPie(pie);
            });
            pie.Add(center);

            for (var i = 0; i < ring.Slots.Count; i++)
            {
                pie.Add(PlaceSlot(ring.Slots[i], i, count, size));
            }
        }

        private static VisualElement PlaceSlot(DDoveHotboxSlot slot, int index, int count, float pieSize)
        {
            var found = DDoveHotboxScanner.TryFind(slot.EntryId, out var discovered);
            var button = new Label(found ? discovered.Label : "[?]");
            button.tooltip = found ? discovered.Tooltip : "条目已丢失，到 HotBox 页重新拖入。";
            button.pickingMode = PickingMode.Position;
            button.AddToClassList("ddove-pie-item");
            if (!found)
            {
                button.AddToClassList("ddove-pie-item--missing");
            }

            var origin = DDoveHotboxPieLayout.SlotOrigin(slot, index, count, pieSize);
            button.style.left = origin.x;
            button.style.top = origin.y;
            button.RegisterCallback<ClickEvent>(_ =>
            {
                Close();
                if (found)
                {
                    discovered.Execute?.Invoke();
                }
            });
            return button;
        }

        private static void Close()
        {
            if (_host != null)
            {
                _host.RemoveFromHierarchy();
                _host = null;
            }

            if (_view != null)
            {
                _view.Repaint();
                _view = null;
            }
        }
    }
}
