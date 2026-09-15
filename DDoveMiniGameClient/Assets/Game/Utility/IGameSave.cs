using DDoveFramework.Core;

namespace Game
{
    public interface IGameSave : IUtility
    {
        void LoadFile(string fileName);

        T TryGetData<T>();

        void SaveData<T>(T data);
    }
}
