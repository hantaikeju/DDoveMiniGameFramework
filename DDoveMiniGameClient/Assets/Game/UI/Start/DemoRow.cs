using Game.Mono;
using TMPro;
using UnityEngine;

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

        void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>(true);
        }

        public void Bind(DemoRowData data, int index)
        {
            if (_text != null)
            {
                _text.text = data != null ? data.Title : index.ToString();
            }
        }

        public void Recycle()
        {
        }
    }
}
