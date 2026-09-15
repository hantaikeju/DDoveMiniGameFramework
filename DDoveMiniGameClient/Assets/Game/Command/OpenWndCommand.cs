using System;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveUI;

namespace Game
{
    public sealed class OpenWndCommand<T> : AbstractCommand
        where T : DDoveUIPanelBase<T>
    {
        public IDDoveUIPanelData Data;

        protected override void OnExecute()
        {
            if (!TryPass(typeof(T), out var reason))
            {
                DDoveDebug.LogWarning("OpenWnd", ("panel", typeof(T).Name), ("reason", reason));
                return;
            }

            DDoveUIKit.OpenAsync<T>(Data).Forget();
        }

        private bool TryPass(Type panel, out string reason)
        {
            reason = null;
            // TODO: 有门槛的窗在这里加 case，GetModel 看进度。不要给每个 Wnd 占空行。
            return true;
        }
    }
}
