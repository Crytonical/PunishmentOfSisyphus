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
        private List<string> enemyLevelFiles;
        private string directoryPath = "";
        private string levelFileName = "";
        [SerializeField] private bool enemyLevelFilesLoaded;
        #endregion

        #region PROPERTIES
        public List<string> EnemyLevelFiles { get { return enemyLevelFiles; } }
        #endregion

        private void OnEnable()
        {
            if (!enemyLevelFilesLoaded)
            {
                enemyLevelFiles = new List<string>();
                LoadEnemyLevelFiles();
            }
        }

        private void LoadEnemyLevelFiles()
        {
            Debug.Log("Loading In Enemy Level Files");

            // Get the directory path
            directoryPath = Application.dataPath + "/LevelData";
            Debug.LogError("Directory Path: " + directoryPath);

            // Get the starting level index (lvl 1)
            int levelNameIndex = 1;

            // Update the initial level file name
            levelFileName = $"Level{levelNameIndex}.txt";

            // Save the initial complete path to the first level
            string completePath = Path.Combine(directoryPath, levelFileName);
            Debug.Log("Path: " + completePath);

            // Loop while there is a file of the complete path, theoretically should stop once there is no more files of 'LevelX.txt'
            while (File.Exists(completePath))
            {
                Debug.LogError("Level Path: " + completePath);
                try
                {
                    using (FileStream fileStream = new FileStream(completePath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {   
                            enemyLevelFiles.Add(reader.ReadToEnd());
                        }
                    }

                    levelFileName = $"Level{levelNameIndex}.txt";
                    completePath = Path.Combine(directoryPath, levelFileName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                    completePath = "";
                }
                levelNameIndex++;
            }

            enemyLevelFilesLoaded = true;
        }
    }
}
