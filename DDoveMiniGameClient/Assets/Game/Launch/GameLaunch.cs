using Cysharp.Threading.Tasks;
using DDoveFramework.Extension.DDoveUI;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class GameLaunch : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureOnLaunch()
        {
            if (SceneManager.GetActiveScene().name != "Launch")
            {
                return;
            }

            if (FindFirstObjectByType<GameLaunch>() != null)
            {
                return;
            }

            var go = new GameObject(nameof(GameLaunch));
            go.AddComponent<GameLaunch>();
        }

        private void Start()
        {
            RunAsync().Forget();
        }

        private async UniTaskVoid RunAsync()
        {
            _ = GameArchitecture.Interface;
            DDoveUIKit.Initialize();
            await DDoveUIKit.OpenAsync<WndHome>();
        }
    }
}
