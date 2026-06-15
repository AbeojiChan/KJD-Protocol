using System.Collections.Generic;
using UnityEngine;

namespace KJD.Editor.HierarchyFastForward
{
    [CreateAssetMenu(fileName = "NewProjectProfile", menuName = "KJD/Hierarchy Fast Forward/Project Profile")]
    public class ProjectStructureProfile : ScriptableObject
    {
        #region Private and Protected

        [Header("Profile Identity")]
        [Tooltip("Nom du profil de structure (ex: Prototype, Full 2D, TTRPG Engine).")]
        [SerializeField] private string _profileName = "Nouveau Profil";

        [Tooltip("Description rapide de l'architecture cible.")]
        [TextArea(2, 4)]
        [SerializeField] private string _description;

        [Header("Architecture")]
        [Tooltip("Liste des dossiers à générer à partir de 'Assets/' (ex: 'Art/Sprites', 'Code/Scripts').")]
        [SerializeField]
        private List<string> _targetFolders = new List<string>()
        {
            "Art",
            "Audio",
            "Code/Scripts",
            "Code/Prefabs",
            "Scenes"
        };

        #endregion


        #region Main API
        public string ProfileName => _profileName;
        public string Description => _description;
        public List<string> TargetFolders => _targetFolders;

        #endregion
    }
}