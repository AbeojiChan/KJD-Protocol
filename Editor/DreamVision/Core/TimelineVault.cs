using System.IO;
using UnityEngine;

namespace KJD.DreamVision.Core
{
    public static class TimelineVault
    {
        private static string VaultDirectory => Path.Combine(Application.dataPath, "~DreamVisionVault");

        public static void InitializeVault()
        {
            if (!Directory.Exists(VaultDirectory))
            {
                Directory.CreateDirectory(VaultDirectory);
                Debug.Log($"[DreamVision] Secret memory vault forged at: {VaultDirectory}");
            }
        }

        public static void SaveSnapshotMetadata(Data.SnapshotMetadata metadata)
        {
            InitializeVault();
            string nodePath = Path.Combine(VaultDirectory, metadata.NodeGUID);
            if (!Directory.Exists(nodePath))
            {
                Directory.CreateDirectory(nodePath);
            }
            string jsonPath = Path.Combine(nodePath, "metadata.json");
            string jsonContent = JsonUtility.ToJson(metadata, true);

            File.WriteAllText(jsonPath, jsonContent);

            Debug.Log($"[DreamVision] Snapshot '{metadata.BranchName}' sealed securely in the vault.");
        }
        public static System.Collections.Generic.List<Data.SnapshotMetadata> GetAllSnapshots()
        {
            var allSnapshots = new System.Collections.Generic.List<Data.SnapshotMetadata>();
            InitializeVault();
            string[] nodeFolders = Directory.GetDirectories(VaultDirectory);

            foreach (string folderPath in nodeFolders)
            {
                string jsonPath = Path.Combine(folderPath, "metadata.json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    var crystal = JsonUtility.FromJson<Data.SnapshotMetadata>(jsonContent);
                    allSnapshots.Add(crystal);
                }
            }
            allSnapshots.Sort((a, b) => b.TimestampTicks.CompareTo(a.TimestampTicks));

            return allSnapshots;
        }
        public static void SaveActiveNodeGUID(string guid)
        {
            InitializeVault();
            string path = Path.Combine(VaultDirectory, "state.json");
            File.WriteAllText(path, guid);
        }
        public static string GetActiveNodeGUID()
        {
            string path = Path.Combine(VaultDirectory, "state.json");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return "";
        }
    }
}