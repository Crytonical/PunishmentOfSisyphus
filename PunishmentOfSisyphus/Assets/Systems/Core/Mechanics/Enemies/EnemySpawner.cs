using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.Events;
using Codice.Client.Common.GameUI;

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
        #endregion

        #region FIELDS
        private List<GameObject> enemiesAlive;
        private int waveNum, levelNum, maxWaves;
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
            // Fill levelWaves with information from a file
            // Initialize default wave info
            waveNum = 0; // 1
            levelNum = 1;
            maxWaves = 4;

            levelWaves = new Dictionary<string, List<List<string>>>();
            enemiesAlive = new List<GameObject>();

            // FOR TESTING, CHANGE WHEN WE HAVE FILE IO
            levelWaves["Level1"] = new List<List<string>>
            {
                new List<string>() {"s", "r"},           // Wave 1: 2 enemies
                new List<string>() {"s", "r", "r"},      // Wave 2: 3 enemies
                new List<string>() {"s", "s", "r", "r"}, // Wave 3: 4 enemies
                new List<string>() {"s", "s", "s", "r", "r", "r"}  // Wave 4: 6 enemies
            };

            // SPawn initial wave
            SpawnWave();
        }

        private void IncrementWave()
        {
            if (waveNum < maxWaves)
            {
                waveNum++;
                SpawnWave();
            }
            else
            {
                // IncrementLevel();
                Debug.Log("Going to next stage");
                return;
            }


        }

        private void RemoveEnemy(GameObject enemy)
        {
            Debug.Log("Removing: " + enemy);
            enemiesAlive.Remove(enemy);
            Destroy(enemy);

            if (enemiesAlive.Count == 0)
            {
                enemySpawnEvent.EndWave();
            }
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
        }

        private void LoadEnemyWaveFromFile()
        {
            // Will get enemy wave from file
        }

        private void IncrementLevel()
        {
            // Go to next level
        }
    }
}