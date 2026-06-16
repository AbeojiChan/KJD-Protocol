#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace KJD.Framework.Deployment
{
    public enum PipelineSafetyMode
    {
        StrictVerifyCleanRepository,
        BypassUncommittedChanges
    }

    public sealed class KJDBuildManagerWindow : EditorWindow
    {
        private string m_currentBranch = "Unknown";
        private string m_currentTag = "Unknown";
        private string m_currentHash = "Unknown";
        private string m_repoStatus = "Unknown";

        private int m_major = 1;
        private int m_minor = 0;
        private int m_patch = 0;
        private string m_releaseNotes = "";

        [MenuItem("[Framework]/Deployment/Build Manager")]
        public static void ShowWindow()
        {
            GetWindow<KJDBuildManagerWindow>("KJD Build Manager");
        }

        private void OnEnable()
        {
            RefreshRepositoryState();
        }

        private void OnGUI()
        {
            DrawGitInformationSection();
            DrawVersionConfigurationSection();
            DrawReleaseNotesSection();

            EditorGUILayout.Space(15);

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("EXECUTE PIPELINE (BUILD RELEASE)", GUILayout.Height(40)))
            {
                string targetVersion = $"v{m_major}.{m_minor}.{m_patch}";
                ExecuteReleasePipeline(targetVersion, PipelineSafetyMode.StrictVerifyCleanRepository);
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawGitInformationSection()
        {
            GUILayout.Label("1. Git Repository State", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox($"Branch: {m_currentBranch}\nTag: {m_currentTag}\nCommit: {m_currentHash}\nStatus: {m_repoStatus}", MessageType.Info);
            
            if (GUILayout.Button("Refresh Repository State"))
            {
                RefreshRepositoryState();
            }
            EditorGUILayout.Space(10);
        }

        private void DrawVersionConfigurationSection()
        {
            GUILayout.Label("2. Semantic Version Target", EditorStyles.boldLabel);
            m_major = EditorGUILayout.IntField("Major (Breaking Change)", m_major);
            m_minor = EditorGUILayout.IntField("Minor (Feature Add)", m_minor);
            m_patch = EditorGUILayout.IntField("Patch (Bug Fix)", m_patch);
            
            EditorGUILayout.LabelField("Target Output Tag:", $"v{m_major}.{m_minor}.{m_patch}", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
        }

        private void DrawReleaseNotesSection()
        {
            GUILayout.Label("3. Release Notes Generation", EditorStyles.boldLabel);
            m_releaseNotes = EditorGUILayout.TextArea(m_releaseNotes, GUILayout.Height(60));
        }

        private void RefreshRepositoryState()
        {
            m_currentBranch = KJDGitUtility.RunCommand("branch --show-current");
            m_currentHash = KJDGitUtility.RunCommand("rev-parse --short HEAD");
            
            string rawTag = KJDGitUtility.RunCommand("describe --tags --abbrev=0");
            m_currentTag = rawTag.StartsWith("Error:") ? "v0.0.0 (No tags found)" : rawTag;

            string statusCheck = KJDGitUtility.RunCommand("status --porcelain");
            m_repoStatus = string.IsNullOrEmpty(statusCheck) ? "Clean" : "Modified Portfolio";
        }

        private void ExecuteReleasePipeline(string versionTag, PipelineSafetyMode safetyMode)
        {
            RefreshRepositoryState();

            if (safetyMode == PipelineSafetyMode.StrictVerifyCleanRepository && m_repoStatus != "Clean")
            {
                EditorUtility.DisplayDialog("KJD Pipeline Blocked", "Deployment aborted: Uncommitted local alterations detected.", "Acknowledge");
                return;
            }

            KJDGitUtility.RunCommand($"tag {versionTag}");
            KJDGitUtility.RunCommand($"push origin {versionTag}");

            KJDBuildVersionData dataAsset = Resources.Load<KJDBuildVersionData>("KJDBuildVersionData");
            if (dataAsset == null)
            {
                EditorUtility.DisplayDialog("Pipeline Error", "Target asset 'KJDBuildVersionData' missing from Resources directory.", "OK");
                return;
            }

            dataAsset.UpdateMetadata(versionTag, m_currentHash, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            EditorUtility.SetDirty(dataAsset);
            AssetDatabase.SaveAssets();

            string buildPath = Path.Combine("Builds", versionTag);
            if (!Directory.Exists(buildPath)) 
            {
                Directory.CreateDirectory(buildPath);
            }

            string binaryName = $"{Application.productName}.exe";
            string fullOutputPath = Path.Combine(buildPath, binaryName);
            string[] activeScenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

            UnityEngine.Debug.Log($"[KJD Framework] Compilation engine triggered for version {versionTag}...");
            var report = BuildPipeline.BuildPlayer(activeScenes, fullOutputPath, BuildTarget.StandaloneWindows64, BuildOptions.None);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                // Écriture du fichier de notes standard
                File.WriteAllText(Path.Combine(buildPath, "release_notes.txt"), m_releaseNotes);
                
                // --- AJOUT KJD PROTOCOL : Génération du manifeste JSON ---
                string jsonPath = Path.Combine(buildPath, "version.json");
                string jsonContent = JsonUtility.ToJson(dataAsset, true);
                File.WriteAllText(jsonPath, jsonContent);
                
                UnityEngine.Debug.Log("[KJD Framework] Automated release process compiled successfully. Manifest generated.");
                EditorUtility.RevealInFinder(fullOutputPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Compilation Failure", "Unity engine build process failed.", "Close");
            }

            RefreshRepositoryState();
        }
    }
}
#endif