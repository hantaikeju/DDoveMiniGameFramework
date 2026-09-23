using System;
using Game.Mono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class HomeEntryRowData
    {
        public string Title;
        public Action Open;
    }

    public sealed class HomeEntryRow : MonoBehaviour, ILoopItem<HomeEntryRowData>
    {
        TMP_Text _text;
        Button _button;
        HomeEntryRowData _data;

        void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>(true);
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(OnClick);
            }
        }

        public void Bind(HomeEntryRowData data, int index)
        {
            _data = data;
            if (_text != null)
            {
                _text.text = _data != null ? _data.Title : string.Empty;
            }
        }

        public void Recycle()
        {
            _data = null;
        }

        void OnClick()
        {
            if (_data != null && _data.Open != null)
            {
                _data.Open();
            }
        }
    }
}
