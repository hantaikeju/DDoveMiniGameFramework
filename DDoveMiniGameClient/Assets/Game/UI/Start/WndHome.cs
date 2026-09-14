using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DDoveFramework.Extension.DDoveAtlas;
using DDoveFramework.Extension.DDoveUI;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public partial class WndHome : DDoveUIPanelBase<WndHome>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndHome";

        private const string DotUrl = "Start/dot";
        private const string MarkUrl = "Start/mark";

        private Image _left;
        private Image _right;
        private Sprite _dot;
        private Sprite _mark;
        private bool _swapped;
        private CancellationTokenSource _swapCts;

        protected override void OnOpen()
        {
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
            StopDemo();
            DDoveAtlasKit.ReleaseScope(GetInstanceID());
        }

        private async UniTaskVoid RunDemoAsync()
        {
            var ct = this.GetCancellationTokenOnDestroy();
            _left = CreateDemoImage("ImgLeft", new Vector2(-180f, 0f));
            _right = CreateDemoImage("ImgRight", new Vector2(180f, 0f));

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

        private Image CreateDemoImage(string name, Vector2 anchored)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(160f, 160f);
            var image = go.GetComponent<Image>();
            image.preserveAspect = true;
            return image;
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
