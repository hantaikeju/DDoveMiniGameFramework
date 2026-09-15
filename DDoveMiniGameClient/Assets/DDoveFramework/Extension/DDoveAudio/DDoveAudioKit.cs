using System;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDovePool;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    public enum DDovePlaySoundModes
    {
        EveryOne,
        IgnoreSameSoundInGlobalFrames,
        IgnoreSameSoundInSoundFrames,
    }

    public static class DDoveAudioKit
    {
        public const string LogTitle = "DDoveAudio";

        private static Transform _audioRoot;
        private static bool _ownsRoot;
        private static bool _eventsRegistered;
        private static DDoveMusicPlayer _musicPlayer;
        private static DDoveMusicPlayer _voicePlayer;

        public static Transform AudioRoot => _audioRoot;

        public static DDovePlaySoundModes DefaultPlaySoundMode { get; set; } = DDovePlaySoundModes.EveryOne;

        public static DDoveAudioKitSettingsModel Settings
            => DDoveAudioArchitecture.Interface.GetModel<DDoveAudioKitSettingsModel>();

        public static DDoveMusicPlayer MusicPlayer
            => _musicPlayer ??= new DDoveMusicPlayer(Settings.IsMusicOn ? Settings.MusicVolume : 0f);

        public static DDoveMusicPlayer VoicePlayer
            => _voicePlayer ??= new DDoveMusicPlayer(Settings.IsVoiceOn ? Settings.VoiceVolume : 0f, false);

        internal static DDoveMusicPlayer MusicPlayerOrNull => _musicPlayer;
        internal static DDoveMusicPlayer VoicePlayerOrNull => _voicePlayer;

        internal static string CurrentMusicName;
        internal static string CurrentVoiceName;

        internal static Func<IDDoveAudioLoader> GetLoader { get; private set; }

        internal static DDovePlaySoundChannelSystem PlaySoundChannelSystem
            => DDoveAudioArchitecture.Interface.GetSystem<DDovePlaySoundChannelSystem>();

        internal static DDovePlayingSoundPoolModel PlayingSoundPool
            => DDoveAudioArchitecture.Interface.GetModel<DDovePlayingSoundPoolModel>();

        public static int SoundFrameCountForIgnoreSameSound
        {
            get => PlaySoundChannelSystem.IgnoreInSoundFramesChannel.SoundFrameCountForIgnoreSameSound;
            set => PlaySoundChannelSystem.IgnoreInSoundFramesChannel.SoundFrameCountForIgnoreSameSound = value;
        }

        public static int GlobalFrameCountForIgnoreSameSound
        {
            get => PlaySoundChannelSystem.IgnoreInGlobalFramesChannel.GlobalFrameCountForIgnoreSameSound;
            set => PlaySoundChannelSystem.IgnoreInGlobalFramesChannel.GlobalFrameCountForIgnoreSameSound = value;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _audioRoot = null;
            _ownsRoot = false;
            _eventsRegistered = false;
            _musicPlayer = null;
            _voicePlayer = null;
            CurrentMusicName = null;
            CurrentVoiceName = null;
            GetLoader = null;
            DefaultPlaySoundMode = DDovePlaySoundModes.EveryOne;
        }
#endif

        public static void Initialize(Transform audioRoot = null)
        {
            if (_audioRoot != null)
            {
                return;
            }

            if (audioRoot == null)
            {
                var go = new GameObject("[DDoveAudio]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                _audioRoot = go.transform;
                _ownsRoot = true;
                go.AddComponent<DDoveAudioLifetime>();
            }
            else
            {
                _audioRoot = audioRoot;
                _ownsRoot = false;
            }

            _ = DDoveAudioArchitecture.Interface;
            GetLoader = DDovePoolKit.Get<DDoveDefaultAudioLoader>;
            RegisterAudioEvents();
        }

        public static void Shutdown()
        {
            StopAllSound();
            StopMusic();
            StopVoice();
            UnregisterAudioEvents();
            _musicPlayer?.Deinit();
            _voicePlayer?.Deinit();
            _musicPlayer = null;
            _voicePlayer = null;
            CurrentMusicName = null;
            CurrentVoiceName = null;
            GetLoader = null;
            DDoveAudioArchitecture.Reset();

            if (_ownsRoot && _audioRoot != null)
            {
                UnityEngine.Object.Destroy(_audioRoot.gameObject);
            }

            _audioRoot = null;
            _ownsRoot = false;
        }

        public static void PlayMusic(string musicName, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
            => DDovePlayMusicCommand.Execute(musicName, loop, onBeganCallback, onEndCallback, volume);

        public static void PlayMusic(AudioClip clip, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
            => DDovePlayMusicWithClipCommand.Execute(clip, loop, onBeganCallback, onEndCallback, volume);

        public static void StopMusic() => DDoveStopMusicCommand.Execute();
        public static void PauseMusic() => DDovePauseMusicCommand.Execute();
        public static void ResumeMusic() => DDoveResumeMusicCommand.Execute();

        public static void PlayVoice(string voiceName, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null)
            => DDovePlayVoiceCommand.Execute(voiceName, loop, onBeganCallback, onEndedCallback);

        public static void PlayVoice(AudioClip clip, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null, float volumeScale = 1.0f)
            => DDovePlayVoiceWithClipCommand.Execute(clip, loop, onBeganCallback, onEndedCallback, volumeScale);

        public static void StopVoice() => DDoveStopVoiceCommand.Execute();
        public static void PauseVoice() => DDovePauseVoiceCommand.Execute();
        public static void ResumeVoice() => DDoveResumeVoiceCommand.Execute();

        public static DDoveAudioPlayer PlayVoiceOnce(string voiceName, Action<DDoveAudioPlayer> callBack = null,
            float volumeScale = 1.0f)
            => DDovePlayVoiceOnceCommand.Execute(voiceName, callBack, volumeScale);

        public static DDoveAudioPlayer PlaySound(string soundName, bool loop = false,
            Action<DDoveAudioPlayer> callBack = null, float volume = 1.0f, float pitch = 1,
            DDovePlaySoundModes? playSoundMode = null)
            => DDovePlaySoundCommand.Execute(soundName, loop, callBack, volume, pitch, playSoundMode);

        public static DDoveAudioPlayer PlaySound(AudioClip clip, bool loop = false,
            Action<DDoveAudioPlayer> callBack = null, float volume = 1.0f, float pitch = 1,
            DDovePlaySoundModes? playSoundMode = null)
            => DDovePlaySoundWithClipCommand.Execute(clip, loop, callBack, volume, pitch, playSoundMode);

        public static void StopAllSound() => DDoveStopAllSoundCommand.Execute();

        internal static void ReturnLoader(IDDoveAudioLoader loader)
        {
            if (loader is DDoveDefaultAudioLoader pooled)
            {
                DDovePoolKit.Release(pooled);
                return;
            }

            loader?.Unload();
        }

        internal static bool TryGetRoot(out Transform root)
        {
            root = _audioRoot;
            if (root != null)
            {
                return true;
            }

            DDoveDebug.LogError(LogTitle, ("reason", "not initialized"));
            return false;
        }

        internal static void EnsureAudioListener()
        {
            if (_audioRoot == null)
            {
                return;
            }

            if (_audioRoot.GetComponentInChildren<AudioListener>() == null)
            {
                _audioRoot.gameObject.AddComponent<AudioListener>();
            }
        }

        private static void RegisterAudioEvents()
        {
            if (_eventsRegistered)
            {
                return;
            }

            var architecture = DDoveAudioArchitecture.Interface;
            architecture.RegisterEvent<MusicOnOffChangeEvent>(OnMusicOnOff);
            architecture.RegisterEvent<VoiceOnOffChangeEvent>(OnVoiceOnOff);
            architecture.RegisterEvent<MusicVolumeChangeEvent>(OnMusicVolume);
            architecture.RegisterEvent<VoiceVolumeChangeEvent>(OnVoiceVolume);
            architecture.RegisterEvent<SoundOnOffChangeEvent>(OnSoundOnOff);
            architecture.RegisterEvent<SoundVolumeChangeEvent>(OnSoundVolume);
            _eventsRegistered = true;
        }

        private static void UnregisterAudioEvents()
        {
            if (!_eventsRegistered)
            {
                return;
            }

            var architecture = DDoveAudioArchitecture.Interface;
            architecture.UnRegisterEvent<MusicOnOffChangeEvent>(OnMusicOnOff);
            architecture.UnRegisterEvent<VoiceOnOffChangeEvent>(OnVoiceOnOff);
            architecture.UnRegisterEvent<MusicVolumeChangeEvent>(OnMusicVolume);
            architecture.UnRegisterEvent<VoiceVolumeChangeEvent>(OnVoiceVolume);
            architecture.UnRegisterEvent<SoundOnOffChangeEvent>(OnSoundOnOff);
            architecture.UnRegisterEvent<SoundVolumeChangeEvent>(OnSoundVolume);
            _eventsRegistered = false;
        }

        private static void OnMusicOnOff(MusicOnOffChangeEvent e)
            => _musicPlayer?.AudioSourceProxy.SetVolume(e.IsOn ? Settings.MusicVolume : 0f);

        private static void OnVoiceOnOff(VoiceOnOffChangeEvent e)
            => _voicePlayer?.AudioSourceProxy.SetVolume(e.IsOn ? Settings.VoiceVolume : 0f);

        private static void OnMusicVolume(MusicVolumeChangeEvent e)
        {
            if (Settings.IsMusicOn)
            {
                _musicPlayer?.AudioSourceProxy.SetVolume(e.VolumeValue);
            }
        }

        private static void OnVoiceVolume(VoiceVolumeChangeEvent e)
        {
            if (Settings.IsVoiceOn)
            {
                _voicePlayer?.AudioSourceProxy.SetVolume(e.VolumeValue);
            }
        }

        private static void OnSoundOnOff(SoundOnOffChangeEvent e)
        {
            if (!e.IsOn)
            {
                PlayingSoundPool.ForEachAllSound(p => p.Stop());
            }
        }

        private static void OnSoundVolume(SoundVolumeChangeEvent e)
        {
            PlayingSoundPool.ForEachAllSound(p =>
            {
                if (p != null)
                {
                    p.Volume(e.VolumeValue);
                }
            });
        }

        private sealed class DDoveAudioLifetime : MonoBehaviour
        {
            private void OnApplicationQuit()
            {
                Shutdown();
            }
        }
    }
}
