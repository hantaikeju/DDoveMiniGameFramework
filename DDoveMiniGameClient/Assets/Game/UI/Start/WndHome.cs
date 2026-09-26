using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveUI;
using Game.Mono;

namespace Game.UI
{
    public partial class WndHome : DDoveUIPanelBase<WndHome>
    {
        public override string StageName => "Start";
        public override string PanelName => "WndHome";

        LoopList _list;

        protected override void OnOpen()
        {
            _list = GetComponentInChildren<LoopList>(true);
            var listGo = transform.Find("LoopList");
            if (_list == null && listGo != null)
            {
                _list = listGo.gameObject.AddComponent<LoopList>();
            }

            if (_list == null)
            {
                DDoveDebug.LogError("WndHome", ("reason", "missing LoopList"));
                return;
            }

            _list.BindHierarchy();
            var template = listGo != null
                ? listGo.Find("Viewport/Content/itemTemplate")
                : _list.transform.Find("Viewport/Content/itemTemplate");
            if (template != null && template.GetComponent<HomeEntryRow>() == null)
            {
                template.gameObject.AddComponent<HomeEntryRow>();
            }

            IList<HomeEntryRowData> entries = new List<HomeEntryRowData>
            {
                new HomeEntryRowData
                {
                    Title = "LoopList",
                    Open = () => DDoveUIKit.NavigateToAsync<WndLoopDemo>().Forget()
                },
                new HomeEntryRowData
                {
                    Title = "领取/换图",
                    Open = () => DDoveUIKit.NavigateToAsync<WndCollectDemo>().Forget()
                },
                new HomeEntryRowData
                {
                    Title = "LoopH",
                    Open = () => DDoveUIKit.NavigateToAsync<WndLoopHDemo>().Forget()
                },
                new HomeEntryRowData
                {
                    Title = "Tab",
                    Open = () => DDoveUIKit.NavigateToAsync<WndTabDemo>().Forget()
                }
            };
            _list.Create(entries);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            _list = null;
        }
    }
}
