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
    public partial class WndHome : DDoveUIPanelBase<WndHome>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndHome";

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
            _itemChanged = this.RegisterEvent<PlayerItemChangedEvent>(OnItemChanged);
            CreateBagUi();
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

        private void CreateBagUi()
        {
            _bag = CreateDemoText("TxtBag", new Vector2(0f, 260f), new Vector2(480f, 48f));
            var button = CreateDemoButton("BtnCollect", "领取", new Vector2(0f, 200f));
            AddClick(button, () => this.SendCommand(new CollectDemoItemCommand()));
        }

        private TMP_Text CreateDemoText(string name, Vector2 anchored, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;
            var text = go.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 28;
            text.color = Color.white;
            var font = TMP_Settings.defaultFontAsset
                ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null)
            {
                text.font = font;
            }

            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            return text;
        }

        private Button CreateDemoButton(string name, string label, Vector2 anchored)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = new Vector2(200f, 56f);
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 1f);
            var text = CreateDemoText(name + "Label", Vector2.zero, new Vector2(200f, 56f));
            text.rectTransform.SetParent(rect, false);
            text.rectTransform.anchoredPosition = Vector2.zero;
            SetText(text, label);
            return go.GetComponent<Button>();
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
