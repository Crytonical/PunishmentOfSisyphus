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
        #endregion

        #region PROPERTIES

        #endregion

        private void OnEnable()
        {
            enemySpawnEvent.enemyDeathEvent.AddListener(RemoveEnemy);
            enemySpawnEvent.waveEnd.AddListener(IncrementWave);
            enemySpawnEvent.levelEnd.AddListener(IncrementLevel);
        }

        private void OnDisable()
        {
            enemySpawnEvent.enemyDeathEvent.RemoveAllListeners();
            enemySpawnEvent.waveEnd.RemoveAllListeners();
            enemySpawnEvent.levelEnd.RemoveAllListeners();
        }

        private void Awake()
        {
            enemiesText = GameObject.Find("Enemies Killed Text").GetComponent<Text>();
            wavesText = GameObject.Find("Enemy Waves Text").GetComponent<Text>();
            levelWaves = new Dictionary<string, List<List<string>>>();
            enemiesAlive = new List<GameObject>();

            // File IO/Level loading notes
            // Need an array/list of possible levels that is loaded from a file
            //  Each level file will be titled 'LevelX.txt' where X is the level num
            //  File format:
            //  4 - # of waves
            //  "r","f","s" - split each enemy type in a wave with a comma

            // FOR TESTING, CHANGE WHEN WE HAVE FILE IO
            if (enemyFileData.LevelFileStrings.Count != 0)
            {
                for (int i = 0; i < enemyFileData.LevelFileStrings.Count; i++)
                {
                    int randomLevelIndex = UnityEngine.Random.Range(0, enemyFileData.LevelFileStrings.Count);
                    levelWaves["Level" + (i + 1)] = LoadEnemyWaveFromFile(randomLevelIndex);
                }
                Debug.Log(levelWaves.Count);
            }
            else // Default loaded wave
            {
                levelWaves["Level1"] = new List<List<string>> 
                { 
                    new List<string> { "s" }
                };
            }

            // Fill levelWaves with information from a file 
            // Initialize default wave info
            waveNum = 0; // 1
            levelNum = 1;
            maxWaves = levelWaves["Level" + levelNum].Count;
            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            // Spawn initial wave
            SpawnWave();
        }

        private void IncrementWave()
        {
            waveNum++;
            if (waveNum < maxWaves)
            {
                SpawnWave();
            }
            else
            {
                // For right now.
                wavesText.text = "Level Complete";
                Debug.Log("Going to next stage");
                IncrementLevel();
                return;
            }
        }

        private void RemoveEnemy(GameObject enemy)
        {
            enemiesAlive.Remove(enemy);
            Destroy(enemy);

            if (enemiesAlive.Count == 0)
            {
                enemySpawnEvent.EndWave();
            }

            enemiesText.text = $"Enemies Killed\r\n{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}";
        }

        private void SpawnWave()
        {
            string levelKey = "Level" + levelNum;
            for (int i = 0; i < levelWaves[levelKey][waveNum].Count; i++)
            {
                string enemyChar = levelWaves[levelKey][waveNum][i];
                switch (enemyChar.ToString().ToLower().Trim())
                {
                    case "r":
                        enemiesAlive.Add(Instantiate(rangedPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        Debug.Log("Ranged Enemy Spawned");
                        break;

                    case "s":
                        enemiesAlive.Add(Instantiate(strongPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        Debug.Log("Strong Enemy Spawned");
                        break;

                    case "f":
                        enemiesAlive.Add(Instantiate(fastPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        Debug.Log("Fast Enemy Spawned");
                        break;
                }
            }

            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            wavesText.text = $"Wave\r\n {waveNum + 1} / {maxWaves}";

            enemiesText.text = $"Enemies Killed\r\n{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}";
        }

        private List<List<string>> LoadEnemyWaveFromFile(int index)
        {
            // Will get enemy wave from file
            string[] enemyFileLines = enemyFileData.LevelFileStrings[index].Split("|");
            int waveCount = int.Parse(enemyFileLines[0]);
            List<List<string>> waves = new List<List<string>>();

            for (int i = 0; i < waveCount; i++)
            {
                List<string> wave = new List<string>();
                // Add 1 to skip first line
                wave.AddRange(enemyFileLines[i + 1].Split(","));
                waves.Add(wave);
            }

            Debug.Log(waves.Count);

            return waves;
        }

        private void IncrementLevel()
        {
            levelNum++;
            waveNum = 0;
            maxWaves = levelWaves["Level" + levelNum].Count;
            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            // Spawn initial wave
            SpawnWave();
        }
    }
}