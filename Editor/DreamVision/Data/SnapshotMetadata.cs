using System;

namespace KJD.DreamVision.Data
{
    [Serializable]
    public struct SnapshotMetadata
    {
        public string NodeGUID;
        public string ParentGUID;    
        public string BranchName;    
        public long TimestampTicks;  
        public string[] Tags;          
        public string UserNotes;        
        public string SceneFilePath; 
    }
}