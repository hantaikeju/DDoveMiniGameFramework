using UnityEditor;
using UnityEditor.Compilation;

namespace DDoveFramework.Editor
{
    [InitializeOnLoad]
    internal static class DDoveArchitectureBindPlayGuard
    {
        private const string ResumePlayKey = "DDove.ArchitectureBind.ResumePlay";

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
                EditorApplication.isPlaying = false;
                SessionState.SetBool(ResumePlayKey, false);
                return;
            }

            EditorApplication.isPlaying = false;
            SessionState.SetBool(ResumePlayKey, true);
            if (wroteFile)
            {
                AssetDatabase.Refresh();
                return;
            }

            EditorApplication.delayCall += TryResumePlay;
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
