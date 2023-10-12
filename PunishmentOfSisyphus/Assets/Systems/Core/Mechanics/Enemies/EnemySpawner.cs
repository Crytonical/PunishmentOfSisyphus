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
        #endregion

        #region FIELDS
        private List<GameObject> enemiesAlive;
        private int waveNum, levelNum, maxWaves;
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

        }

        private void Update()
        {
            
        }

        private void IncrementWave()
        {
            if (waveNum < maxWaves)
            {
                waveNum++;
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
            enemiesAlive.Remove(enemy);
            Destroy(enemy);
        }

        private void LoadEnemyWaveFromFile()
        {
            // Will get enemy wave from file
        }
    }
}