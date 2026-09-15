using System;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    public static class DDoveAudioPlayerExtensions
    {
        public static T OnStart<T>(this T self, Action onStart) where T : DDoveAbstractAudioPlayer
        {
            self.LifeCycle.RegisterOnStartOnce(onStart);
            return self;
        }

        public static T OnFinish<T>(this T self, Action onFinish) where T : DDoveAbstractAudioPlayer
        {
            self.LifeCycle.RegisterOnFinishOnce(onFinish);
            return self;
        }

        public static T Pitch<T>(this T self, float pitch) where T : DDoveAbstractAudioPlayer
        {
            self.AudioSourceProxy.SetPitch(pitch);
            return self;
        }

        public static T VolumeScale<T>(this T self, float volumeScale) where T : DDoveAbstractAudioPlayer
        {
            self.AudioSourceProxy.SetVolumeScale(volumeScale);
            return self;
        }

        public static T Volume<T>(this T self, float volume) where T : DDoveAbstractAudioPlayer
        {
            self.AudioSourceProxy.SetVolume(volume);
            return self;
        }

        public static void Pause(this DDoveAbstractAudioPlayer self)
        {
            if (self.IsPause)
            {
                return;
            }

            self.AudioSourceProxy.Pause();
        }

        public static void Resume(this DDoveAbstractAudioPlayer self)
        {
            if (!self.IsPause)
            {
                return;
            }

            self.LifeCycle.CallOnStartOnce();
            self.AudioSourceProxy.Play(self.OnPlayFinished);
        }

        internal static DDoveAbstractAudioPlayer PrepareByNameAsyncAndPlay(this DDoveAbstractAudioPlayer self,
            GameObject root, string name, bool loop)
        {
            if (string.IsNullOrEmpty(name) || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.ByLoaderAsync);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }

        internal static DDoveAbstractAudioPlayer PrepareByNameSyncAndPlay(this DDoveAbstractAudioPlayer self,
            GameObject root, string name, bool loop)
        {
            if (string.IsNullOrEmpty(name) || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.ByLoaderSync);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }

        internal static T PrepareByClipAndPlay<T>(this T self, GameObject root, AudioClip clip, string name,
            bool loop) where T : DDoveAbstractAudioPlayer
        {
            if (clip == null || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.BySetUp.SetClipForPrepare(clip);
            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.BySetUp);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }
    }
}
