using UnityEngine;

namespace KJD.Framework.Deployment
{
    [CreateAssetMenu(fileName = "KJDBuildVersionData", menuName = "KJD/Build Data")]
    public sealed class KJDBuildVersionData : ScriptableObject
    {
        [Header("Version Metadata")]
        [SerializeField] private string m_versionTag = "v0.0.0";
        [SerializeField] private string m_commitHash = "none";
        [SerializeField] private string m_buildDate = "none";

        public string VersionTag => m_versionTag;
        public string CommitHash => m_commitHash;
        public string BuildDate => m_buildDate;

        public void UpdateMetadata(string versionTag, string commitHash, string buildDate)
        {
            m_versionTag = versionTag;
            m_commitHash = commitHash;
            m_buildDate = buildDate;
        }
    }
}