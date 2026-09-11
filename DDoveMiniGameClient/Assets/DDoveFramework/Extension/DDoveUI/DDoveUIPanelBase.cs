using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI
{
    public abstract class DDoveUIPanelBase<TPanel> : MonoBehaviour, IDDoveUIPanel
        where TPanel : DDoveUIPanelBase<TPanel>
    {
        public abstract string StageName { get; }
        public abstract string PanelName { get; }

        public IDDoveUIPanelData PanelData { get; private set; }

        public virtual DDoveUILayer DefaultLayer => DDoveUILayer.Normal;
        public virtual bool EnableClose => true;

        private bool _isOpen;
        private bool _isVisible;
        private readonly List<Button> _buttons = new List<Button>();
        private readonly List<EventTrigger> _clickTriggers = new List<EventTrigger>();
        private readonly List<EventTrigger> _longPressTriggers = new List<EventTrigger>();
        private readonly List<CancellationTokenSource> _longPressCts = new List<CancellationTokenSource>();

        public virtual bool CanOpen()
        {
            return !_isOpen && OnCanOpen();
        }

        public virtual bool OnCanOpen()
        {
            return true;
        }

        public virtual async UniTask OpenAsync(IDDoveUIPanelData data = null)
        {
            PanelData = data;
            if (!CanOpen())
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", PanelName), ("reason", "cannot open"));
                return;
            }

            _isOpen = true;
            Clear();
            OnOpen();
            await UniTask.Yield();
            Show();
        }

        public virtual void Show()
        {
            if (_isVisible)
            {
                return;
            }

            _isVisible = true;
            gameObject.SetActive(true);
            OnShow();

            var selectable = GetDefaultSelectable();
            if (selectable != null)
            {
                DDoveUIKit.SetDefaultSelection(selectable);
            }
        }

        public virtual void Hide()
        {
            if (!_isVisible)
            {
                return;
            }

            _isVisible = false;
            var es = EventSystem.current;
            if (es != null
                && es.currentSelectedGameObject != null
                && es.currentSelectedGameObject.transform.IsChildOf(transform))
            {
                DDoveUIKit.ClearSelection();
            }

            gameObject.SetActive(false);
            OnHide();
        }

        public virtual void Close()
        {
            if (!_isOpen)
            {
                return;
            }

            try
            {
                Clear();
                try
                {
                    OnClose();
                }
                catch (Exception e)
                {
                    DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", PanelName), ("onClose", e.Message));
                }
            }
            finally
            {
                _isVisible = false;
                _isOpen = false;
                Destroy(gameObject);
            }
        }

        protected virtual Selectable GetDefaultSelectable()
        {
            return null;
        }

        protected void SetText(Text text, string content)
        {
            if (text != null)
            {
                text.text = content;
            }
        }

        protected void SetImage(Image image, Sprite sprite, bool setNativeSize = false)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        }

        protected void AddClick(Button button, Action action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
            _buttons.Add(button);
        }

        protected void AddClick(GameObject go, Action action)
        {
            if (go == null)
            {
                return;
            }

            var trigger = GetOrAddEventTrigger(go);
            trigger.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerClick);
            var click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            click.callback.AddListener(_ => action?.Invoke());
            trigger.triggers.Add(click);
            _clickTriggers.Add(trigger);
        }

        protected void AddLongPressRepeat(GameObject go, Action onRepeat, float interval = 0.1f, float delay = 0.3f)
        {
            if (go == null || onRepeat == null)
            {
                return;
            }

            var trigger = GetOrAddEventTrigger(go);
            CancellationTokenSource cts = null;
            AddTrigger(trigger, EventTriggerType.PointerDown, _ =>
            {
                cts = new CancellationTokenSource();
                _longPressCts.Add(cts);
                LongPressRepeatAsync(onRepeat, interval, delay, cts.Token).Forget();
            });
            AddTrigger(trigger, EventTriggerType.PointerUp, _ => CancelLongPress(ref cts));
            AddTrigger(trigger, EventTriggerType.PointerExit, _ => CancelLongPress(ref cts));
            _longPressTriggers.Add(trigger);
        }

        protected abstract void OnOpen();
        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnClose();

        private void Clear()
        {
            foreach (var button in _buttons)
            {
                button?.onClick.RemoveAllListeners();
            }

            _buttons.Clear();

            foreach (var trigger in _clickTriggers)
            {
                trigger?.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerClick);
            }

            _clickTriggers.Clear();

            foreach (var cts in _longPressCts)
            {
                cts?.Cancel();
                cts?.Dispose();
            }

            _longPressCts.Clear();

            foreach (var trigger in _longPressTriggers)
            {
                trigger?.triggers.RemoveAll(entry =>
                    entry.eventID == EventTriggerType.PointerDown
                    || entry.eventID == EventTriggerType.PointerUp
                    || entry.eventID == EventTriggerType.PointerExit);
            }

            _longPressTriggers.Clear();
        }

        private async UniTaskVoid LongPressRepeatAsync(Action onRepeat, float interval, float delay, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                while (!token.IsCancellationRequested)
                {
                    onRepeat.Invoke();
                    await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelLongPress(ref CancellationTokenSource cts)
        {
            if (cts == null)
            {
                return;
            }

            cts.Cancel();
            cts.Dispose();
            _longPressCts.Remove(cts);
            cts = null;
        }

        private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            trigger.triggers.RemoveAll(entry => entry.eventID == type);
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        private static EventTrigger GetOrAddEventTrigger(GameObject go)
        {
            var trigger = go.GetComponent<EventTrigger>();
            return trigger != null ? trigger : go.AddComponent<EventTrigger>();
        }
    }
}
