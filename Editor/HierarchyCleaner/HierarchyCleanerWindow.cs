using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BLT_Games.EditorTools
{
    public class HierarchyCleanerWindow : EditorWindow
    {
        private GameObject m_SelectedParent;
        private List<GameObject> m_InactiveChildren = new List<GameObject>();

        private int m_ActiveCount = 0;
        private int m_InactiveCount = 0;
        private int m_TotalScannedCount = 0;

        private bool m_ShouldRemoveInactive = true;
        private bool m_ShouldRemoveEmpty = false;

        [MenuItem("Tools/Hierarchy Cleaner")]
        public static void ShowWindow()
        {
            HierarchyCleanerWindow window = GetWindow<HierarchyCleanerWindow>("Hierarchy Cleaner");
            window.minSize = new Vector2(350, 400);
        }

        private void OnGUI()
        {
            GUILayout.Label("Hierarchy Cleaner Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            m_SelectedParent = Selection.activeGameObject;

            if (m_SelectedParent == null)
            {
                DrawWarningMessage();
                return;
            }

            DrawSelectionInfo();

            EditorGUILayout.Space();

            DrawOptionsGroup();

            EditorGUILayout.Space();

            ExecuteHierarchyScan();

            DrawStatisticsGroup();

            EditorGUILayout.Space();

            DrawPreviewGroup();

            EditorGUILayout.Space();

            DrawCleanupActionButton();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }
        private void DrawWarningMessage()
        {
            EditorGUILayout.HelpBox("Please select a GameObject in the Hierarchy.", MessageType.Warning);
        }

        private void DrawSelectionInfo()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Selected Object:", EditorStyles.miniLabel);
            GUILayout.Label(m_SelectedParent.name, EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawOptionsGroup()
        {
            GUILayout.Label("Cleanup Options", EditorStyles.boldLabel);
            m_ShouldRemoveInactive = EditorGUILayout.Toggle("Remove inactive GameObjects", m_ShouldRemoveInactive);
            m_ShouldRemoveEmpty = EditorGUILayout.Toggle("Remove empty GameObjects (TBD)", m_ShouldRemoveEmpty);
        }

        private void DrawStatisticsGroup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Scan Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Inactive objects found: {m_InactiveCount}");
            EditorGUILayout.LabelField($"Active objects scanned: {m_ActiveCount}");
            EditorGUILayout.LabelField($"Total objects scanned: {m_TotalScannedCount}");
            EditorGUILayout.EndVertical();
        }

        private void DrawPreviewGroup()
        {
            GUILayout.Label("Objects to remove:", EditorStyles.boldLabel);

            if (m_InactiveChildren.Count == 0)
            {
                GUILayout.Label(" - None", EditorStyles.miniLabel);
                return;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (GameObject obj in m_InactiveChildren)
            {
                GUILayout.Label($" - {obj.name}", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCleanupActionButton()
        {
            GUI.enabled = m_InactiveChildren.Count > 0 && m_ShouldRemoveInactive;

            if (GUILayout.Button("Remove Inactive Children", GUILayout.Height(30)))
            {
                bool isUserConfirmed = EditorUtility.DisplayDialog(
                    "Confirmation Dialog",
                    $"You are about to delete {m_InactiveCount} inactive GameObjects.\n\nContinue?",
                    "Yes",
                    "No"
                );

                if (isUserConfirmed)
                {
                    ExecuteCleanup();
                }
            }

            GUI.enabled = true;
        }
        private void ExecuteHierarchyScan()
        {
            m_InactiveChildren.Clear();
            m_ActiveCount = 0;
            m_InactiveCount = 0;
            m_TotalScannedCount = 0;

            if (m_SelectedParent == null) return;
            Transform[] allChildren = m_SelectedParent.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject == m_SelectedParent) continue;

                m_TotalScannedCount++;
                if (!child.gameObject.activeSelf)
                {
                    m_InactiveCount++;
                    m_InactiveChildren.Add(child.gameObject);
                }
                else
                {
                    m_ActiveCount++;
                }
            }
        }

        private void ExecuteCleanup()
        {
            int removedCount = m_InactiveChildren.Count;
            Undo.RegisterCompleteObjectUndo(m_SelectedParent, "Clean Inactive Children");
            for (int i = m_InactiveChildren.Count - 1; i >= 0; i--)
            {
                if (m_InactiveChildren[i] != null)
                {
                    Undo.DestroyObjectImmediate(m_InactiveChildren[i]);
                }
            }
            Repaint();

            string reportMessage = $"Cleanup completed.\n{removedCount} inactive GameObjects removed.";
            EditorUtility.DisplayDialog("Report", reportMessage, "OK");
            Debug.Log($"[Hierarchy Cleaner] {reportMessage}");
        }
    }
}