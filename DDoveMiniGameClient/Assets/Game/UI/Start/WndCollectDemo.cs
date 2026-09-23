using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveAtlas;
using DDoveFramework.Extension.DDoveUI;
using Game;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public partial class WndCollectDemo : DDoveUIPanelBase<WndCollectDemo>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndCollectDemo";

        private const string DotUrl = "Start/dot";
        private const string MarkUrl = "Start/mark";

        private Image _left;
        private Image _right;
        private TMP_Text _bag;
        private Sprite _dot;
        private Sprite _mark;
        private bool _swapped;
        private CancellationTokenSource _swapCts;
        private IUnRegister _itemChanged;

        protected override void OnOpen()
        {
            CacheNodes();
            _itemChanged = this.RegisterEvent<PlayerItemChangedEvent>(OnItemChanged);
            BindCollect();
            BindBack();
            RefreshBag(this.GetModel<PlayerModel>());
            RunDemoAsync().Forget();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            _itemChanged?.UnRegister();
            _itemChanged = null;
            StopDemo();
            DDoveAtlasKit.ReleaseScope(GetInstanceID());
        }

        private void CacheNodes()
        {
            _bag = FindNode<TMP_Text>("TxtBag");
            _left = FindNode<Image>("ImgLeft");
            _right = FindNode<Image>("ImgRight");
        }

        private void BindCollect()
        {
            var button = FindNode<Button>("BtnCollect");
            if (button == null)
            {
                return;
            }

            AddClick(button, () => this.SendCommand(new CollectDemoItemCommand()));
        }

        private void BindBack()
        {
            var button = FindNode<Button>("BtnBack");
            if (button == null)
            {
                return;
            }

            AddClick(button, () => DDoveUIKit.BackAsync().Forget());
        }

        private T FindNode<T>(string name) where T : Component
        {
            var child = transform.Find(name);
            var component = child != null ? child.GetComponent<T>() : null;
            if (component == null)
            {
                DDoveDebug.LogError("WndCollectDemo", ("reason", "missing scene node"), ("name", name));
            }

            return component;
        }

        private async UniTaskVoid RunDemoAsync()
        {
            var ct = this.GetCancellationTokenOnDestroy();
            var owner = GetInstanceID();
            _dot = await DDoveAtlasKit.LoadSpriteAsync(DotUrl, owner, ct);
            _mark = await DDoveAtlasKit.LoadSpriteAsync(MarkUrl, owner, ct);
            ApplySprites();

            StopDemo();
            _swapCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            SwapLoopAsync(_swapCts.Token).Forget();
        }

        private async UniTaskVoid SwapLoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ct);
                    _swapped = !_swapped;
                    ApplySprites();
                    await PulseAsync(_left, ct);
                    await PulseAsync(_right, ct);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void ApplySprites()
        {
            SetImage(_left, _swapped ? _mark : _dot);
            SetImage(_right, _swapped ? _dot : _mark);
        }

        private static async UniTask PulseAsync(Image image, CancellationToken ct)
        {
            if (image == null)
            {
                return;
            }

            var tween = Tween.Scale(image.transform, 1.2f, 0.2f, Ease.OutQuad, 2, CycleMode.Yoyo);
            await tween;
            ct.ThrowIfCancellationRequested();
        }

        private void OnItemChanged(PlayerItemChangedEvent e)
        {
            RefreshBag(e.Name, e.Count);
        }

        private void RefreshBag(PlayerModel player)
        {
            if (player == null)
            {
                return;
            }

            RefreshBag(player.Name, player.Count);
        }

        private void RefreshBag(string name, int count)
        {
            SetText(_bag, name + " x" + count);
        }

        private void StopDemo()
        {
            if (_left != null)
            {
                Tween.StopAll(_left.transform);
            }

            if (_right != null)
            {
                Tween.StopAll(_right.transform);
            }

            if (_swapCts == null)
            {
                return;
            }

            _swapCts.Cancel();
            _swapCts.Dispose();
            _swapCts = null;
        }
    }
}
