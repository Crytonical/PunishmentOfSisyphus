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
            if (!enemyLevelFilesLoaded)
            {
                levelFileStrings = new List<string>();
                LoadEnemyLevelFiles();
            }
        }

        private void LoadEnemyLevelFiles()
        {
            Debug.Log("Loading In Enemy Level Files");

            for (int i = 0; i < levelFiles.Count; i++)
            {
                levelFileStrings.Add(levelFiles[i].text);
            }

            enemyLevelFilesLoaded = true;
            Debug.Log(levelFileStrings.Count);
        }
    }
}