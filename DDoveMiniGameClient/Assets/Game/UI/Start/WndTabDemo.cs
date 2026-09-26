//------------------------------------------------------------------------------
// Business logic. Re-export does not overwrite this file.
//------------------------------------------------------------------------------
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveUI;
using Game.Mono;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public partial class WndTabDemo : DDoveUIPanelBase<WndTabDemo>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndTabDemo";

        int _shownTab;
        UnityAction<bool> _onTabA;
        UnityAction<bool> _onTabB;

        protected override void OnOpen()
        {
            _shownTab = 0;

            var back = FindNode<Button>("BtnBack");
            if (back != null)
            {
                AddClick(back, () => DDoveUIKit.BackAsync().Forget());
            }

            var tabA = FindNode<Toggle>("BtnTabA");
            var tabB = FindNode<Toggle>("BtnTabB");
            BindTab(tabA, tabB, true, ref _onTabA);
            BindTab(tabB, tabA, false, ref _onTabB);

            if (tabA != null)
            {
                tabA.SetIsOnWithoutNotify(true);
                RefreshTab(tabA);
            }

            if (tabB != null)
            {
                tabB.SetIsOnWithoutNotify(false);
                RefreshTab(tabB);
            }

            OpenShownTab(true);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        void BindTab(Toggle self, Toggle other, bool pageA, ref UnityAction<bool> bound)
        {
            if (self == null || bound != null)
            {
                return;
            }

            bound = isOn =>
            {
                if (isOn && other != null && other.isOn)
                {
                    other.SetIsOnWithoutNotify(false);
                    RefreshTab(other);
                }

                if (isOn)
                {
                    OpenShownTab(pageA);
                }
            };
            self.onValueChanged.AddListener(bound);
        }

        static void RefreshTab(Toggle toggle)
        {
            if (toggle == null)
            {
                return;
            }

            var onOff = toggle.GetComponent<TabOnOff>();
            if (onOff != null)
            {
                onOff.Refresh();
            }
        }

        void OpenShownTab(bool pageA)
        {
            var tab = pageA ? 1 : 2;
            if (_shownTab == tab)
            {
                return;
            }

            _shownTab = tab;
            if (pageA)
            {
                DDoveUIKit.OpenChildAsync<WndTabPageA>(this).Forget();
            }
            else
            {
                DDoveUIKit.OpenChildAsync<WndTabPageB>(this).Forget();
            }
        }

        T FindNode<T>(string name) where T : Component
        {
            var nodes = GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].name != name)
                {
                    continue;
                }

                var component = nodes[i].GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            DDoveDebug.LogError("WndTabDemo", ("reason", "missing scene node"), ("name", name));
            return null;
        }
    }
}
