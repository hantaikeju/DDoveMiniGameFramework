using System;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveAtlas;
using DDoveFramework.Extension.DDoveCfg;
using DDoveFramework.Extension.DDoveUI;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class GameLaunch : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HookSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (scene.name != "Launch")
            {
                return;
            }

            EnsureOnLaunch();
        }

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
            try
            {
                var loaded = await DDoveCfgKit.LoadAsync(this.GetCancellationTokenOnDestroy());
                if (!loaded)
                {
                    DDoveDebug.LogError("GameLaunch", ("reason", "cfg load failed, skip UI"));
                    return;
                }

                var architecture = GameArchitecture.Interface;
                var item = architecture.GetUtility<CfgUtility>().Tables.Tbitem.Get(1001);
                DDoveDebug.Log(DDoveCfgKit.LogTitle, ("demo", item.Id), ("name", item.Name));

                DDoveAtlasKit.Initialize();
                DDoveUIKit.Initialize();
                await DDoveUIKit.OpenAsync<WndHome>();
            }
            catch (Exception e)
            {
                DDoveDebug.LogError("GameLaunch", ("error", e.Message));
                Debug.LogException(e);
            }
        }
    }
}
