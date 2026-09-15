using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveSave;

namespace Game
{
    [DDoveBindUtility(As = typeof(IGameSave))]
    public sealed class GameSaveUtility : IGameSave
    {
        private string _fileName;

        public void LoadFile(string fileName)
        {
            _fileName = fileName;
            DDoveSaveKit.LoadFile(fileName);
        }

        public T TryGetData<T>()
        {
            return DDoveSaveKit.TryLoad<T>(typeof(T).FullName, out var data) ? data : default;
        }

        public void SaveData<T>(T data)
        {
            var fileName = string.IsNullOrEmpty(_fileName) ? DDoveSaveKit.CurrentFileName : _fileName;
            if (string.IsNullOrEmpty(fileName))
            {
                DDoveDebug.LogError(DDoveSaveKit.LogTitle, ("reason", "save before LoadFile"));
                return;
            }

            DDoveSaveKit.Save(typeof(T).FullName, data);
            DDoveSaveKit.SaveFile(fileName);
        }
    }
}
