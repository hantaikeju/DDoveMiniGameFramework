using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveUI;
using Game.Mono;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public partial class WndLoopDemo : DDoveUIPanelBase<WndLoopDemo>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndLoopDemo";

        LoopList _list;
        IList<DemoRowData> _bags;

        protected override void OnOpen()
        {
            var back = FindNode<Button>("BtnBack");
            if (back != null)
            {
                AddClick(back, () => DDoveUIKit.BackAsync().Forget());
            }

            _list = GetComponentInChildren<LoopList>(true);
            var listGo = transform.Find("LoopList");
            if (_list == null && listGo != null)
            {
                _list = listGo.gameObject.AddComponent<LoopList>();
            }

            if (_list == null)
            {
                DDoveDebug.LogError("WndLoopDemo", ("reason", "missing LoopList"));
                return;
            }

            _list.BindHierarchy();
            var template = listGo != null
                ? listGo.Find("Viewport/Content/itemTemplate")
                : _list.transform.Find("Viewport/Content/itemTemplate");
            if (template != null && template.GetComponent<DemoRow>() == null)
            {
                template.gameObject.AddComponent<DemoRow>();
            }

            _bags = new List<DemoRowData>(80);
            for (int i = 0; i < 80; i++)
            {
                _bags.Add(new DemoRowData { Index = i, Title = i.ToString() });
            }

            _list.Create(_bags);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            Tween.StopAll();
            _bags = null;
        }

        T FindNode<T>(string name) where T : Component
        {
            var child = transform.Find(name);
            var component = child != null ? child.GetComponent<T>() : null;
            if (component == null)
            {
                DDoveDebug.LogError("WndLoopDemo", ("reason", "missing scene node"), ("name", name));
            }

            return component;
        }
    }
}
