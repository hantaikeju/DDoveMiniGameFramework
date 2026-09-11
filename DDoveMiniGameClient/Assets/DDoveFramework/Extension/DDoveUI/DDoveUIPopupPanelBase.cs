using UnityEngine;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI
{
    public abstract class DDoveUIPopupPanelBase<TPanel> : DDoveUIPanelBase<TPanel>
        where TPanel : DDoveUIPopupPanelBase<TPanel>
    {
        public override DDoveUILayer DefaultLayer => DDoveUILayer.Popup;

        protected virtual bool EnableMask => true;
        protected virtual Color MaskColor => new Color(0f, 0f, 0f, 0.7f);

        private GameObject _mask;

        public override void Show()
        {
            CreateMask();
            base.Show();
        }

        protected virtual void CloseSelf()
        {
            DDoveUIKit.Close(GetType());
        }

        private void CreateMask()
        {
            if (_mask != null || !EnableMask)
            {
                return;
            }

            _mask = new GameObject("PopupMask", typeof(RectTransform), typeof(Image), typeof(Button));
            _mask.transform.SetParent(transform, false);
            _mask.transform.SetAsFirstSibling();

            var rect = _mask.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = _mask.GetComponent<Image>();
            image.color = MaskColor;
            image.raycastTarget = true;

            var button = _mask.GetComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(CloseSelf);
        }
    }
}
