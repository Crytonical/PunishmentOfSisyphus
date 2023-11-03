using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using Ephymeral.Events;

namespace Ephymeral.FileLoading
{
    [CreateAssetMenu(fileName = "SpawnerFileHandler", menuName = "FileHandlers/Spawner")]

    public class EnemySpawnerFileHandler : ScriptableObject
    {
        #region FIELDS
        [SerializeField] private List<TextAsset> levelFiles;
        [SerializeField] private List<string> levelFileStrings;
        [SerializeField] private bool enemyLevelFilesLoaded;
        #endregion

        #region PROPERTIES
        public List<string> LevelFileStrings { get { return levelFileStrings; } }
        public List<TextAsset> LevelFiles { get { return levelFiles; } }
        #endregion

        private void OnEnable()
        {
            if (!enemyLevelFilesLoaded || levelFileStrings == null || levelFileStrings.Count == 0)
            {
                levelFileStrings = new List<string>();
                LoadEnemyLevelFiles();
            }
        }

        /// <summary>
        /// Loads in enemy level file strings from the level text files in LevelFiles
        /// </summary>
        private void LoadEnemyLevelFiles()
        {
            // Loops throuh the level files list
            for (int i = 0; i < levelFiles.Count; i++)
            {
                // Adds the string at the current index i to the levelFileStrings list
                levelFileStrings.Add(levelFiles[i].text);
            }

            // Sets enemy level files loaded to true so we dont reload the files
            enemyLevelFilesLoaded = true;
        }
    }
}