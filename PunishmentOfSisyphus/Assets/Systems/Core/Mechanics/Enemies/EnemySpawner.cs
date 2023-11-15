using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ephymeral.Events;
using Ephymeral.FileLoading;
using System.Linq;
using System;

namespace Ephymeral.EnemyNS
{
    public class EnemySpawner : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private EnemySpawnerEvent enemySpawnEvent;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private GameObject rangedPrefab;
        [SerializeField] private GameObject fastPrefab;
        [SerializeField] private GameObject strongPrefab;
        [SerializeField] private EnemySpawnerFileHandler enemyFileData;
        private Text wavesText;
        private Text enemiesText;
        #endregion

        #region FIELDS
        private List<GameObject> enemiesAlive;
        private int waveNum, levelNum, maxWaves, maxEnemiesInWave;
        [SerializeField] private Dictionary<string, List<List<string>>> levelWaves;
        [SerializeField] private float screenTransitionDuration = 2.0f;
        #endregion

        #region PROPERTIES
        public int EnemiesCnt
        {
            get { return enemiesAlive.Count; }

        }
        #endregion

        private void OnEnable()
        {
            // Setup events
            enemySpawnEvent.enemyDeathEvent.AddListener(RemoveEnemy);
            enemySpawnEvent.waveEnd.AddListener(IncrementWave);
            enemySpawnEvent.levelEnd.AddListener(IncrementLevel);
        }

        private void OnDisable()
        {
            // Remove events
            enemySpawnEvent.enemyDeathEvent.RemoveAllListeners();
            enemySpawnEvent.waveEnd.RemoveAllListeners();
            enemySpawnEvent.levelEnd.RemoveAllListeners();
        }

        private void Awake()
        {
            // Get text objects 
            enemiesText = GameObject.Find("Enemies Killed Text").GetComponent<Text>();
            wavesText = GameObject.Find("Enemy Waves Text").GetComponent<Text>();

            // Initialize lists and dictionaries
            levelWaves = new Dictionary<string, List<List<string>>>();
            enemiesAlive = new List<GameObject>();

            // File IO/Level loading notes
            // Need an array/list of possible levels that is loaded from a file
            //  Each level file will be titled 'LevelX.txt' where X is the level num
            //  File format:
            //  4 - # of waves
            //  "r","f","s" - split each enemy type in a wave with a comma

            // Check if the list of level strings isn't empty
            if (enemyFileData.LevelFileStrings.Count != 0)
            {
                // Loop through the list of level strings
                for (int i = 0; i < enemyFileData.LevelFileStrings.Count; i++)
                {
                    // Get a random index for a level from 0 to the # of level strings
                    int randomLevelIndex = UnityEngine.Random.Range(0, enemyFileData.LevelFileStrings.Count);

                    // Make a new entry in the dictionary with a key of level + i + 1
                    //  (so for level one AKA i = 0, the key would be 'Level1'
                    levelWaves["Level" + (i + 1)] = LoadEnemyWaveFromFile(randomLevelIndex);
                }
            }
            else // Default loaded wave if the list is empty
            {
                levelWaves["Level1"] = new List<List<string>> 
                { 
                    new List<string> { "s" }
                };
            }

            waveNum = 0;
            levelNum = 0;
            //// Initialize default wave info
            //waveNum = 0; // 1
            //levelNum = 1;
            //maxWaves = levelWaves["Level" + levelNum].Count;
            //maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            // Spawn initial wave
            //SpawnWave();
        }

        /// <summary>
        /// Increments to the next wave
        /// </summary>
        private void IncrementWave()
        {
            // Increase wave num
            waveNum++;

            // Check if the wave num is less than the max waves
            if (waveNum < maxWaves)
            {
                // If so, we can spawn a new wave
                SpawnWave();
            }
            else
            {
                // If not, then increment level
                return;
            }
        }

        /// <summary>
        /// Removes an enemy from the alive enemies list
        /// </summary>
        /// <param name="enemy"> The enemy to remove </param>
        private void RemoveEnemy(GameObject enemy)
        {
            // Removes an enemy from the enemies alive list
            enemiesAlive.Remove(enemy);

            // Destroy the enemy
            Destroy(enemy);

            // If the enemy countis 0, invoke the end wave event
            if (enemiesAlive.Count == 0)
            {
                enemySpawnEvent.EndWave();
            }

            // Update the enemies killed text
            enemiesText.text = $"Enemies Killed\r\n{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}";
        }

        /// <summary>
        /// Spawn a new enemy wave
        /// </summary>
        private void SpawnWave()
        {
            // Save the level key (i.e 'Level1')
            string levelKey = "Level" + levelNum;

            // Loop through the list of enemies at the given level key
            for (int i = 0; i < levelWaves[levelKey][waveNum].Count; i++)
            {
                // Save the enemy character taken from the enemy list at the given key and wavenum
                string enemyChar = levelWaves[levelKey][waveNum][i];

                // Check what the enemy character is
                switch (enemyChar.ToString().ToLower().Trim())
                {
                    // If the character is r, spawn in a ranged enemy
                    case "r":
                        enemiesAlive.Add(Instantiate(rangedPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;

                    // If the character is s, spawn in a slow enemy
                    case "s":
                        enemiesAlive.Add(Instantiate(strongPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;

                    // If the character is f, spawn in a fast enemy
                    case "f":
                        enemiesAlive.Add(Instantiate(fastPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;
                }
            }

            // Update UI text values
            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            wavesText.text = $"Wave\r\n {waveNum + 1} / {maxWaves}";

            enemiesText.text = $"Enemies Killed\r\n{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}";
        }

        /// <summary>
        /// Load an enemy wave from the level file at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private List<List<string>> LoadEnemyWaveFromFile(int index)
        {
            // Splits the level file string at the given index into an array, using the pipe symbol
            string[] enemyFileLines = enemyFileData.LevelFileStrings[index].Split("|");

            // Save the number of waves in the level
            int waveCount = int.Parse(enemyFileLines[0]);

            // Make a list of waves
            List<List<string>> waves = new List<List<string>>();

            // Loop for each wave
            for (int i = 0; i < waveCount; i++)
            {
                // Make a list of enemies in the wave
                List<string> wave = new List<string>();

                // Add a range of strings to the wave list by spliting the string in the 
                //   enemy file lines array using the ',' character, add 1 to the index to skip the first line
                wave.AddRange(enemyFileLines[i + 1].Split(","));

                // Add the wave to the waves list
                waves.Add(wave);
            }

            return waves;
        }

        /// <summary>
        /// Goes to the next level
        /// </summary>
        public void IncrementLevel()
        {
            // Updates values
            levelNum++;

            waveNum = 0;
            maxWaves = levelWaves["Level" + levelNum].Count;
            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            // Spawn initial wave
            SpawnWave();
        }

        public void ToggleLevelTransition()
        {
            //foreach (GameObject enemy in enemiesAlive)
            //    enemy.GetComponent<Enemy>.ToggleLevelTransition();
        }
    }
}