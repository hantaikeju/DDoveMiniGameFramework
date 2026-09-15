namespace DDoveFramework.Extension.DDoveAudio
{
    internal class DDoveClipPrepareModeController
    {
        private IDDoveClipPrepareMode _prepareMode;

        internal IDDoveClipPrepareMode PrepareMode => _prepareMode ?? (_prepareMode = ByLoaderAsync);

        internal readonly DDovePrepareClipBySetUp BySetUp = new DDovePrepareClipBySetUp();
        internal readonly DDovePrepareClipByLoaderAsync ByLoaderAsync = new DDovePrepareClipByLoaderAsync();
        internal readonly DDovePrepareClipByLoaderSync ByLoaderSync = new DDovePrepareClipByLoaderSync();

        internal void ChangePrepareMode(IDDoveClipPrepareMode mode)
        {
            if (PrepareMode == mode)
            {
                return;
            }

            PrepareMode?.UnPrepareClip();
            _prepareMode = mode;
        }
    }
}
