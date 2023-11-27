using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "EnemySpawnerEvent", menuName = "EventObjects/EnemySpawnerEvent")]
    public class EnemySpawnerEvent : ScriptableObject
    {
        #region FIELDS

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent<GameObject> enemyDeathEvent;
        public UnityEvent waveEnd;
        public UnityEvent levelEnd;
        public UnityEvent spawnWave;
        #endregion
        #endregion

        #region PROPERTIES
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (enemyDeathEvent == null)
            {
                enemyDeathEvent = new UnityEvent<GameObject>();
            }
            if (waveEnd == null)
            {
                waveEnd = new UnityEvent();
            }
            if (levelEnd == null)
            {
                levelEnd = new UnityEvent();
            }
            if (spawnWave == null)
            {
                waveEnd = new UnityEvent();
            }
            #endregion
        }

        public void EnemyDeathEvent(GameObject enemy)
        {
            enemyDeathEvent.Invoke(enemy);
        }

        public void EndWave()
        {
            waveEnd.Invoke();
        }

        public void EndLevel()
        {
            levelEnd.Invoke();
        }

        public void WaveSpawned()
        {
            spawnWave.Invoke();
        }
    }
}