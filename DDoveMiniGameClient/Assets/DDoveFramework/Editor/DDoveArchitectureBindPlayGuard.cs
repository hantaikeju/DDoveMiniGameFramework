using DDoveFramework.Core;
using UnityEditor;
using UnityEditor.Compilation;

namespace DDoveFramework.Editor
{
    [InitializeOnLoad]
    internal static class DDoveArchitectureBindPlayGuard
    {
        private const string ResumePlayKey = "DDove.ArchitectureBind.ResumePlay";
        private const string LogTitle = "Architecture";

        static DDoveArchitectureBindPlayGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            TryResumePlay();
        }

        private static void OnCompilationFinished(object _)
        {
            if (SessionState.GetBool(DDoveArchitectureBindGenerator.GenerateAfterCompileKey, false))
            {
                SessionState.SetBool(DDoveArchitectureBindGenerator.GenerateAfterCompileKey, false);
                DDoveArchitectureBindGenerator.GenerateAndRefresh();
            }

            TryResumePlay();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            if (!DDoveArchitectureBindGenerator.TryCollect(out var entries))
            {
                DDoveDebug.LogError(LogTitle, ("reason", "play cancelled: bind collect failed"));
                EditorApplication.isPlaying = false;
                SessionState.SetBool(ResumePlayKey, false);
                return;
            }

            if (DDoveArchitectureBindGenerator.IsGeneratedCurrent(entries))
            {
                return;
            }

            if (!DDoveArchitectureBindGenerator.TryGenerate(out var wroteFile))
            {
                DDoveDebug.LogError(LogTitle, ("reason", "play cancelled: bind generate failed"));
                EditorApplication.isPlaying = false;
                SessionState.SetBool(ResumePlayKey, false);
                return;
            }

            if (!wroteFile)
            {
                return;
            }

            DDoveDebug.LogWarning(
                LogTitle,
                ("reason", "play cancelled: architecture bind stale, will resume after compile"));
            EditorApplication.isPlaying = false;
            SessionState.SetBool(ResumePlayKey, true);
            AssetDatabase.Refresh();
        }

        private static void TryResumePlay()
        {
            if (!SessionState.GetBool(ResumePlayKey, false))
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isPlaying)
            {
                return;
            }

            SessionState.SetBool(ResumePlayKey, false);
            EditorApplication.isPlaying = true;
        }
    }
}
