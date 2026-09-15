using DDoveFramework.Extension.DDovePool;

namespace DDoveFramework.Extension.DDoveAudio
{
    public class DDoveAudioPlayer : DDoveAbstractAudioPlayer, IDDovePoolObject
    {
        internal DDovePlaySoundModes PlaySoundMode { get; private set; } = DDovePlaySoundModes.EveryOne;

        private string _nameForPool;

        internal DDoveAudioPlayer SetPlaySoundMode(DDovePlaySoundModes? playSoundMode)
        {
            if (playSoundMode != null)
            {
                PlaySoundMode = playSoundMode.Value;
            }

            return this;
        }

        internal override bool CanPlayAudio()
            => DDoveAudioKit.PlaySoundChannelSystem.CanPlaySound(this)
               && DDoveAudioKit.Settings.IsSoundOn;

        protected override void OnPlayStarted()
        {
            if (!PlayedAudio)
            {
                return;
            }

            _nameForPool = AudioName;
            DDoveAudioKit.PlayingSoundPool.AddSoundPlayer2Pool(_nameForPool, this);
        }

        protected override void OnBeforeStop()
        {
            if (PlayedAudio && _nameForPool != null)
            {
                DDoveAudioKit.PlayingSoundPool.RemoveSoundPlayerFromPool(_nameForPool, this);
            }
        }

        protected override void OnStop()
        {
            DDovePoolKit.Release(this);
        }

        internal override void OnPlayFinished()
        {
            if (PlayedAudio)
            {
                DDoveAudioKit.PlaySoundChannelSystem.SoundFinish(this);
            }

            base.OnPlayFinished();
        }

        public void OnAcquire()
        {
        }

        public void OnRelease()
        {
            _nameForPool = null;
            OnDeinit();
            AudioSourceProxy.OnParentRecycled();
        }

        internal static DDoveAudioPlayer Allocate(float volume, DDovePlaySoundModes? playSoundMode = null)
        {
            var player = DDovePoolKit.Get<DDoveAudioPlayer>();
            player.SetPlaySoundMode(playSoundMode ?? DDoveAudioKit.DefaultPlaySoundMode);
            player.OnInit(volume);
            return player;
        }
    }
}
