using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Ephymeral.Events;

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
        }

        private void OnDisable()
        {
            enemySpawnEvent.enemyDeathEvent.RemoveAllListeners();
            enemySpawnEvent.waveEnd.RemoveAllListeners();
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
            levelWaves["Level1"] = new List<List<string>>
            {
                new List<string>() {"s", "s"},                     // Wave 1: 1 fast enemy
                new List<string>() {"r"},                          // Wave 2: 1 ranged enemy
                new List<string>() {"s", "f"},                     // Wave 3: 1 strong enemy
                new List<string>() {"s", "r"},                     // Wave 4: 2 enemies
                new List<string>() {"s", "f", "r"},                // Wave 5: 3 enemies
                new List<string>() {"s", "s", "f", "r"},           // Wave 6: 4 enemies
                new List<string>() {"s", "s", "f", "f", "r", "r"}  // Wave 7: 6 enemies
            };


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
                // IncrementLevel();

                // For right now.
                wavesText.text = "You win!";
                Debug.Log("Going to next stage");
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

            enemiesText.text = $"{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}\r\nEnemies Killed";
        }

        private void SpawnWave()
        {
            string levelKey = "Level" + levelNum;
            for (int i = 0; i < levelWaves[levelKey][waveNum].Count; i++)
            {
                switch (levelWaves[levelKey][waveNum][i])
                {
                    case "r":
                        enemiesAlive.Add(Instantiate(rangedPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;

                    case "s":
                        enemiesAlive.Add(Instantiate(strongPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;

                    case "f":
                        enemiesAlive.Add(Instantiate(fastPrefab, new Vector2(0.0f, 0.0f), Quaternion.identity));
                        break;
                }
            }

            maxEnemiesInWave = levelWaves["Level" + levelNum][waveNum].Count;

            wavesText.text = $"Wave\r\n {waveNum + 1} / {maxWaves}";

            enemiesText.text = $"Enemies Killed\r\n{maxEnemiesInWave - enemiesAlive.Count} / {maxEnemiesInWave}";
        }

        private void LoadEnemyWaveFromFile()
        {
            // Will get enemy wave from file
        }

        private void IncrementLevel()
        {
            // Go to next level
        }

        private List<List<string>> GetRandomStartingLevel()
        {
            List<List<string>> level = new List<List<string>>();
            return level;
        }
    }
}