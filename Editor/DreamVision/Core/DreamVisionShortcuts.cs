using UnityEditor;
using UnityEngine;
using KJD.DreamVision.Editor;

namespace KJD.DreamVision.Core
{
    public static class DreamVisionShortcuts
    {
        [MenuItem("Tools/DreamVision/⚡ Quick Capture %&s")]
        public static void QuickCapture()
        {
            string autoName = $"QuickSave_{System.DateTime.Now.ToString("HH'h'mm")}";

            SnapshotEngine.CaptureCurrentScene(autoName);
            Debug.Log($"[DreamVision] ⚡ Sauvegarde rapide exécutée : {autoName}");

            if (EditorWindow.HasOpenInstances<DreamVisionWindow>())
            {
                var window = EditorWindow.GetWindow<DreamVisionWindow>();
                window.Repaint();
            }
        }

        [MenuItem("Tools/DreamVision/⏱️ Ouvrir le Portail %&z")]
        public static void OpenTimelineShortcut()
        {
            DreamVisionWindow.ShowWindow();
        }
    }
}