using Game.Mono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class DemoRowData
    {
        public int Index;
        public string Title;
    }

    public sealed class DemoRow : MonoBehaviour, ILoopItem<DemoRowData>
    {
        TMP_Text _text;
        Button _button;
        int _index;

        void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>(true);
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(OnClick);
            }
        }

        public void Bind(DemoRowData data, int index)
        {
            _index = index;
            if (_text != null)
            {
                _text.text = data != null ? data.Title : index.ToString();
            }
        }

        public void Recycle()
        {
        }

        void OnClick()
        {
            Debug.Log("[WndLoopDemo] row " + _index);
        }
    }
}
