using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Ephymeral.FileLoading
{
    [CreateAssetMenu(fileName = "SpawnerFileHandler", menuName = "FileHandlers/Spawner")]

    public class EnemySpawnerFileHandler : ScriptableObject
    {
        #region FIELDS
        private List<string> enemyLevelFiles;
        private string directoryPath = "";
        private string levelFileName = "";
        #endregion

        #region PROPERTIES
        public List<string> EnemyLevelFiles { get { return enemyLevelFiles; } }
        #endregion

        private void OnEnable()
        {
            Debug.Log("Enable Enemey Spawner ");
            if (enemyLevelFiles.Count == 0)
            {
                enemyLevelFiles = new List<string>();
                LoadEnemyLevelFiles();
                Debug.Log(enemyLevelFiles.Count);
            }
        }

        private void LoadEnemyLevelFiles()
        {
            // Get the directory path
            directoryPath = Application.persistentDataPath;

            // Get the starting level index (lvl 1)
            int levelNameIndex = 1;

            // Update the initial level file name
            levelFileName = $"Level{levelNameIndex}.txt";

            // Save the initial complete path to the first level
            string completePath = Path.Combine(directoryPath, levelFileName);
            Debug.Log("Path: " + completePath);

            // Loop while there is a file of the complete path, theoretically should stop once there is no more files of 'LevelX.txt'
            //while (File.Exists(completePath))
            //{
            //}
            try
            {
                levelFileName = $"Level{levelNameIndex}.txt";
                completePath = Path.Combine(directoryPath, levelFileName);

                using (FileStream fileStream = new FileStream(completePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        enemyLevelFiles.Add(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                completePath = "";
            }
        }
    }
}
