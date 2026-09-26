using UnityEngine;
using UnityEngine.UI;

namespace Game.Mono
{
    public sealed class TabOnOff : MonoBehaviour
    {
        [SerializeField] private GameObject onRoot;
        [SerializeField] private GameObject offRoot;

        private Toggle toggle;

        private void OnEnable()
        {
            toggle = GetComponent<Toggle>();
            if (toggle == null)
            {
                return;
            }

            Apply(toggle.isOn);
            toggle.onValueChanged.AddListener(Apply);
        }

        private void OnDisable()
        {
            if (toggle == null)
            {
                return;
            }

            toggle.onValueChanged.RemoveListener(Apply);
            toggle = null;
        }

        public void Refresh()
        {
            Toggle current = toggle != null ? toggle : GetComponent<Toggle>();
            if (current == null)
            {
                return;
            }

            Apply(current.isOn);
        }

        private void Apply(bool isOn)
        {
            if (onRoot == null || offRoot == null)
            {
                return;
            }

            onRoot.SetActive(isOn);
            offRoot.SetActive(!isOn);
        }
    }
}
