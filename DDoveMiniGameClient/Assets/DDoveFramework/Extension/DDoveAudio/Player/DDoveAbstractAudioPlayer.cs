using DDoveFramework.Core;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    public abstract class DDoveAbstractAudioPlayer
    {
        public float Volume { get; protected set; }

        internal bool IsPause => AudioSourceProxy.Paused;

        public bool IsLoop
        {
            get => AudioSourceProxy.Loop;
            set => AudioSourceProxy.SetLoop(value);
        }

        protected bool PlayedAudio;

        public string AudioName { get; internal set; }

        internal readonly DDoveAudioSourceProxy AudioSourceProxy = new DDoveAudioSourceProxy();
        internal readonly DDoveAudioPlayerLifeCycle LifeCycle = new DDoveAudioPlayerLifeCycle();
        internal readonly DDoveClipPrepareModeController PrepareModeController = new DDoveClipPrepareModeController();

        protected void OnInit(float volume)
        {
            Volume = volume;
            LifeCycle.Clear();
            AudioSourceProxy.InitParameters();
            AudioSourceProxy.SetVolume(volume);
            DDoveAudioArchitecture.Interface.RegisterEvent<AudioSourceProxySourceVolumeChangeEvent>(OnVolumeChange);
        }

        public void Stop()
        {
            OnBeforeStop();
            ClearDataAndStop();
            OnStop();
        }

        internal void ClearDataAndStop()
        {
            AudioName = null;
            AudioSourceProxy.StopAndClearClip();
            PrepareModeController.PrepareMode.UnPrepareClip();
        }

        private void OnVolumeChange(AudioSourceProxySourceVolumeChangeEvent eventArg)
        {
            if (eventArg.SourceHashCode == AudioSourceProxy.GetHashCode())
            {
                Volume = eventArg.Volume;
            }
        }

        protected void OnDeinit()
        {
            DDoveAudioArchitecture.Interface.UnRegisterEvent<AudioSourceProxySourceVolumeChangeEvent>(OnVolumeChange);
            ClearDataAndStop();
        }

        internal void OnClipPrepareFinished(bool result, AudioClip clip)
        {
            if (!result)
            {
                Stop();
                return;
            }

            AudioSourceProxy.SetClip(clip);

            if (!clip)
            {
                DDoveDebug.LogError(DDoveAudioKit.LogTitle, ("reason", "invalid clip"), ("name", AudioName));
                Stop();
                return;
            }

            if (AudioSourceProxy.AudioSourceIsNull() || AudioSourceProxy.AudioClip == null)
            {
                Stop();
                return;
            }

            if (IsPause)
            {
                return;
            }

            LifeCycle.CallOnStartOnce();

            if (CanPlayAudio())
            {
                PlayedAudio = true;
                OnPlayStarted();
                AudioSourceProxy.Play(OnPlayFinished);
                return;
            }

            PlayedAudio = false;
            Stop();
        }

        internal virtual void OnPlayFinished()
        {
            if (!AudioSourceProxy.Loop)
            {
                LifeCycle.CallOnFinishOnce();
                Stop();
            }
        }

        protected abstract void OnPlayStarted();
        internal abstract bool CanPlayAudio();
        protected abstract void OnBeforeStop();
        protected abstract void OnStop();
    }
}
