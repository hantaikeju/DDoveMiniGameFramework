using DDoveFramework.Core;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    public class DDoveAudioKitSettingsModel : AbstractModel
    {
        private const string KeySoundOn = "KEY_AUDIO_MANAGER_SOUND_ON";
        private const string KeyMusicOn = "KEY_AUDIO_MANAGER_MUSIC_ON";
        private const string KeyVoiceOn = "KEY_AUDIO_MANAGER_VOICE_ON";
        private const string KeySoundVolume = "KEY_AUDIO_MANAGER_SOUND_VOLUME";
        private const string KeyMusicVolume = "KEY_AUDIO_MANAGER_MUSIC_VOLUME";
        private const string KeyVoiceVolume = "KEY_AUDIO_MANAGER_VOICE_VOLUME";

        public bool IsSoundOn { get; private set; }
        public bool IsMusicOn { get; private set; }
        public bool IsVoiceOn { get; private set; }
        public float SoundVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float VoiceVolume { get; private set; }

        protected override void OnInit()
        {
            IsSoundOn = PlayerPrefs.GetInt(KeySoundOn, 1) == 1;
            IsMusicOn = PlayerPrefs.GetInt(KeyMusicOn, 1) == 1;
            IsVoiceOn = PlayerPrefs.GetInt(KeyVoiceOn, 1) == 1;
            SoundVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeySoundVolume, 1.0f));
            MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyMusicVolume, 1.0f));
            VoiceVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(KeyVoiceVolume, 1.0f));
        }

        public void SetIsSoundOn(bool value)
        {
            if (IsSoundOn == value)
            {
                return;
            }

            IsSoundOn = value;
            Save();
            this.SendEvent(new SoundOnOffChangeEvent { IsOn = value });
        }

        public void SetIsMusicOn(bool value)
        {
            if (IsMusicOn == value)
            {
                return;
            }

            IsMusicOn = value;
            Save();
            this.SendEvent(new MusicOnOffChangeEvent { IsOn = value });
        }

        public void SetIsVoiceOn(bool value)
        {
            if (IsVoiceOn == value)
            {
                return;
            }

            IsVoiceOn = value;
            Save();
            this.SendEvent(new VoiceOnOffChangeEvent { IsOn = value });
        }

        public void SetSoundVolume(float value)
        {
            if (Mathf.Abs(SoundVolume - value) <= 0.001f)
            {
                return;
            }

            SoundVolume = Mathf.Clamp01(value);
            Save();
            this.SendEvent(new SoundVolumeChangeEvent { VolumeValue = value });
        }

        public void SetMusicVolume(float value)
        {
            if (Mathf.Abs(MusicVolume - value) <= 0.001f)
            {
                return;
            }

            MusicVolume = Mathf.Clamp01(value);
            Save();
            this.SendEvent(new MusicVolumeChangeEvent { VolumeValue = value });
        }

        public void SetVoiceVolume(float value)
        {
            if (Mathf.Abs(VoiceVolume - value) <= 0.001f)
            {
                return;
            }

            VoiceVolume = Mathf.Clamp01(value);
            Save();
            this.SendEvent(new VoiceVolumeChangeEvent { VolumeValue = value });
        }

        public void SetAllAudioVolume(float value)
        {
            var needSave = false;
            if (Mathf.Abs(SoundVolume - value) > 0.001f)
            {
                SoundVolume = Mathf.Clamp01(value);
                needSave = true;
            }

            if (Mathf.Abs(MusicVolume - value) > 0.001f)
            {
                MusicVolume = Mathf.Clamp01(value);
                needSave = true;
            }

            if (Mathf.Abs(VoiceVolume - value) > 0.001f)
            {
                VoiceVolume = Mathf.Clamp01(value);
                needSave = true;
            }

            if (needSave)
            {
                Save();
            }
        }

        public float GetAllAudioVolume() => MusicVolume;

        public void SetAll(bool isSoundOn, bool isMusicOn, bool isVoiceOn,
            float soundVolume, float musicVolume, float voiceVolume)
        {
            var needSave = false;
            if (IsSoundOn != isSoundOn)
            {
                IsSoundOn = isSoundOn;
                needSave = true;
            }

            if (IsMusicOn != isMusicOn)
            {
                IsMusicOn = isMusicOn;
                needSave = true;
            }

            if (IsVoiceOn != isVoiceOn)
            {
                IsVoiceOn = isVoiceOn;
                needSave = true;
            }

            if (Mathf.Abs(SoundVolume - soundVolume) > 0.001f)
            {
                SoundVolume = Mathf.Clamp01(soundVolume);
                needSave = true;
            }

            if (Mathf.Abs(MusicVolume - musicVolume) > 0.001f)
            {
                MusicVolume = Mathf.Clamp01(musicVolume);
                needSave = true;
            }

            if (Mathf.Abs(VoiceVolume - voiceVolume) > 0.001f)
            {
                VoiceVolume = Mathf.Clamp01(voiceVolume);
                needSave = true;
            }

            if (needSave)
            {
                Save();
            }
        }

        public void Save()
        {
            PlayerPrefs.SetInt(KeySoundOn, IsSoundOn ? 1 : 0);
            PlayerPrefs.SetInt(KeyMusicOn, IsMusicOn ? 1 : 0);
            PlayerPrefs.SetInt(KeyVoiceOn, IsVoiceOn ? 1 : 0);
            PlayerPrefs.SetFloat(KeySoundVolume, SoundVolume);
            PlayerPrefs.SetFloat(KeyMusicVolume, MusicVolume);
            PlayerPrefs.SetFloat(KeyVoiceVolume, VoiceVolume);
            PlayerPrefs.Save();
        }
    }
}
