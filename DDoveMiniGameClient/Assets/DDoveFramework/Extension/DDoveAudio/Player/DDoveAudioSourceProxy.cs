using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveAudioSourceProxy
    {
        internal float VolumeScale { get; private set; } = 1.0f;
        internal float Volume { get; private set; } = 1.0f;
        internal float Pitch { get; private set; } = 1.0f;
        internal AudioClip AudioClip { get; private set; }
        internal bool Paused { get; private set; }
        internal bool Loop { get; private set; }
        internal AudioSource AudioSource { get; private set; }

        private Action _onSoundPlayFinish;
        private CancellationTokenSource _playFinishCts;

        internal void InitParameters()
        {
            Volume = 1.0f;
            VolumeScale = 1.0f;
            Pitch = 1.0f;
            AudioClip = null;
            Loop = false;
            Paused = false;
            _onSoundPlayFinish = null;
            _playFinishCts = null;
        }

        internal void SetPitch(float pitch)
        {
            Pitch = pitch;
            if (AudioSource)
            {
                AudioSource.pitch = pitch;
            }
        }

        internal void SetVolumeScale(float volumeScale)
        {
            VolumeScale = volumeScale;
            UpdateVolume();
        }

        internal void SetVolume(float volume)
        {
            Volume = volume;
            DDoveAudioArchitecture.Interface.SendEvent(new AudioSourceProxySourceVolumeChangeEvent
            {
                Volume = Volume,
                SourceHashCode = GetHashCode(),
            });
            UpdateVolume();
        }

        internal void SetClip(AudioClip clip)
        {
            AudioClip = clip;
            if (AudioSource)
            {
                AudioSource.clip = clip;
            }
        }

        internal void SetLoop(bool loop)
        {
            Loop = loop;
            if (AudioSource)
            {
                AudioSource.loop = loop;
            }
        }

        internal void ApplyParameters()
        {
            SetClip(AudioClip);
            SetLoop(Loop);
            SetVolume(Volume);
            SetVolumeScale(VolumeScale);
            SetPitch(Pitch);
        }

        internal void CreateOrUpdateAudioSource(GameObject root, string name)
        {
            if (!AudioSource)
            {
                AudioSource = new GameObject(name).AddComponent<AudioSource>();
            }

            if (AudioSource.transform.parent != root.transform)
            {
                AudioSource.transform.SetParent(root.transform);
                AudioSource.transform.localPosition = Vector3.zero;
            }

            if (AudioSource.gameObject.name != name)
            {
                AudioSource.gameObject.name = name;
            }

            if (!AudioSource.gameObject.activeSelf)
            {
                AudioSource.gameObject.SetActive(true);
            }
        }

        internal void Play(Action onSoundPlayFinish)
        {
            if (AudioSource != null && !AudioSource.gameObject.activeSelf)
            {
                AudioSource.gameObject.SetActive(true);
            }

            ApplyParameters();
            Paused = false;
            AudioSource.Play();
            _onSoundPlayFinish = onSoundPlayFinish;
            RegisterOnSoundPlayFinish();
        }

        internal void Pause()
        {
            if (!AudioSource)
            {
                return;
            }

            AudioSource.Pause();
            Paused = true;
        }

        internal void Stop()
        {
            CancelPlayFinishCheck();
            if (AudioSource)
            {
                AudioSource.Stop();
            }
        }

        internal void StopAndClearClip()
        {
            Paused = false;
            if (AudioSourceIsNull())
            {
                return;
            }

            if (AudioSource.clip == AudioClip)
            {
                Stop();
                SetClip(null);
            }
        }

        internal void OnParentRecycled()
        {
            if (AudioSourceIsNull())
            {
                return;
            }

            if (DDoveAudioKit.AudioRoot != null)
            {
                AudioSource.transform.SetParent(DDoveAudioKit.AudioRoot);
                AudioSource.transform.localPosition = Vector3.zero;
            }

            AudioSource.gameObject.SetActive(false);
        }

        internal bool AudioSourceIsNull() => AudioSource == null;

        private void UpdateVolume()
        {
            if (AudioSource)
            {
                AudioSource.volume = VolumeScale * Volume;
            }
        }

        private void CancelPlayFinishCheck()
        {
            if (_playFinishCts == null)
            {
                return;
            }

            _playFinishCts.Cancel();
            _playFinishCts.Dispose();
            _playFinishCts = null;
        }

        private void RegisterOnSoundPlayFinish()
        {
            CancelPlayFinishCheck();
            if (_onSoundPlayFinish == null)
            {
                return;
            }

            CheckPlayFinishAsync().Forget();
        }

        private async UniTaskVoid CheckPlayFinishAsync()
        {
            if (AudioSource == null || _onSoundPlayFinish == null)
            {
                return;
            }

            _playFinishCts = new CancellationTokenSource();
            var token = _playFinishCts.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                    if (AudioSource == null)
                    {
                        break;
                    }

                    if (!Paused && !AudioSource.isPlaying)
                    {
                        _onSoundPlayFinish?.Invoke();
                        _onSoundPlayFinish = null;
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _playFinishCts?.Dispose();
                _playFinishCts = null;
            }
        }
    }
}
