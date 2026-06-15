using System.IO;
using UnityEngine;
using UnityEditor;

namespace KJD.Editor.FeatureGenerator
{
    public class FeatureGeneratorWindow : EditorWindow
    {
        #region Private Fields

        private string _featureName = "NewFeature";
        private bool _includeManagerPlaceholder = true;
        private bool _isEditorOnlyAssembly = false;

        #endregion

        #region Unity Menu API

        [MenuItem("KJD/Feature Generator 🚀")]
        public static void ShowWindow()
        {
            FeatureGeneratorWindow window = GetWindow<FeatureGeneratorWindow>("Feature Gen");
            window.minSize = new Vector2(400, 280);
            window.Show();
        }

        #endregion

        #region Layout & UI

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("🚀 KJD Protocol - Générateur de Feature", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Génère l'architecture dans _/CODE/GAME/ avec configuration automatique du Root Namespace et des plateformes.", MessageType.Info);

            GUILayout.Space(15);

            _featureName = EditorGUILayout.TextField("Nom de la Feature :", _featureName);

            _includeManagerPlaceholder = EditorGUILayout.Toggle("Créer script d'init (Src)", _includeManagerPlaceholder);
            _isEditorOnlyAssembly = EditorGUILayout.Toggle("Assembly destiné à l'Editor ?", _isEditorOnlyAssembly);

            GUILayout.Space(25);

            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.18f, 0.55f, 0.34f);

            if (GUILayout.Button("⚡ Déployer la Feature", GUILayout.Height(40)))
            {
                if (string.IsNullOrWhiteSpace(_featureName))
                {
                    EditorUtility.DisplayDialog("Erreur", "Le nom de la feature ne peut pas être vide !", "Ok");
                    return;
                }

                _featureName = System.Text.RegularExpressions.Regex.Replace(_featureName, @"[^a-zA-Z0-9_]", "");
                ExecuteFeatureGeneration();
            }

            GUI.backgroundColor = defaultColor;
        }

        #endregion

        #region Core Generation Logic

        private void ExecuteFeatureGeneration()
        {
            string baseGamePath = Path.Combine(Application.dataPath, "_", "CODE", "GAME");
            string featureRootPath = Path.Combine(baseGamePath, _featureName);

            string runtimePath = Path.Combine(featureRootPath, "Runtime");
            string srcPath = Path.Combine(featureRootPath, "Src");

            if (Directory.Exists(featureRootPath))
            {
                if (!EditorUtility.DisplayDialog("Feature Existante", $"Le dossier de la feature '{_featureName}' existe déjà. Continuer ?", "Oui", "Annuler"))
                {
                    return;
                }
            }

            Directory.CreateDirectory(runtimePath);
            Directory.CreateDirectory(srcPath);

            string asmdefName = $"KJD.Game.{_featureName}";
            string asmdefFileName = $"{asmdefName}.asmdef";
            string asmdefFullPath = Path.Combine(runtimePath, asmdefFileName);

            if (!File.Exists(asmdefFullPath))
            {
                string includePlatformsValue = _isEditorOnlyAssembly ? "[\n        \"Editor\"\n    ]" : "[]";

                string asmdefContent = "{\n" +
                                       $"    \"name\": \"{asmdefName}\",\n" +
                                       $"    \"rootNamespace\": \"{asmdefName}\",\n" +
                                       $"    \"references\": [],\n" +
                                       $"    \"includePlatforms\": {includePlatformsValue},\n" + 
                                       "    \"excludePlatforms\": [],\n" +
                                       "    \"allowUnsafeCode\": false,\n" +
                                       "    \"overrideReferences\": false,\n" +
                                       "    \"precompiledReferences\": [],\n" +
                                       "    \"autoReferenced\": true,\n" +
                                       "    \"defineConstraints\": [],\n" +
                                       "    \"versionDefines\": [],\n" +
                                       "    \"noEngineReferences\": false\n" +
                                       "}";

                File.WriteAllText(asmdefFullPath, asmdefContent);
            }

            if (_includeManagerPlaceholder)
            {
                string scriptFileName = $"{_featureName}Manager.cs";
                string scriptFullPath = Path.Combine(srcPath, scriptFileName);

                if (!File.Exists(scriptFullPath))
                {
                    string scriptContent = "using UnityEngine;\n\n" +
                                           $"namespace KJD.Game.{_featureName}\n" +
                                           "{\n" +
                                           $"    public class {_featureName}Manager : MonoBehaviour\n" +
                                           "    {\n" +
                                           "        #region Private Fields\n\n" +
                                           "        #endregion\n\n" +
                                           "        #region Unity Lifecycle\n\n" +
                                           "        private void Awake()\n" +
                                           "        {\n" +
                                           $"            Debug.Log(\"<b>[{_featureName}]</b> Initialized successfully.\");\n" +
                                           "        }\n\n" +
                                           "        #endregion\n\n" +
                                           "        #region Public API\n\n" +
                                           "        #endregion\n" +
                                           "    }\n" +
                                           "}";

                    File.WriteAllText(scriptFullPath, scriptContent);
                }
            }

            AssetDatabase.Refresh();

            Debug.Log($"<b>[FeatureGenerator]</b> Feature '{_featureName}' configurée et déployée avec succès ! 🚀");
            ShowNotification(new GUIContent("Feature Déployée !"));
        }

        #endregion
    }
}