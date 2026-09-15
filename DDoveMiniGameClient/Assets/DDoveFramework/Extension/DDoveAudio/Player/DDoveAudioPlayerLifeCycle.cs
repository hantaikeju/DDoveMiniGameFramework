using System;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveAudioPlayerLifeCycle
    {
        internal Action OnStart;
        internal Action OnFinish;

        internal void RegisterOnStartOnce(Action onStart)
        {
            if (onStart == null)
            {
                return;
            }

            OnStart += onStart;
        }

        internal void RegisterOnFinishOnce(Action onFinish)
        {
            if (onFinish == null)
            {
                return;
            }

            OnFinish += onFinish;
        }

        internal void Clear()
        {
            OnStart = null;
            OnFinish = null;
        }

        internal void CallOnStartOnce()
        {
            OnStart?.Invoke();
            OnStart = null;
        }

        internal void CallOnFinishOnce()
        {
            OnFinish?.Invoke();
            OnFinish = null;
        }
    }
}
