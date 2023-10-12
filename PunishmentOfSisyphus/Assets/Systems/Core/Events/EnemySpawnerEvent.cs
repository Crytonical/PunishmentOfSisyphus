using Ephymeral.EnemyNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ephymeral.Events
{
    public class EnemySpawnerEvent : MonoBehaviour
    {
        #region FIELDS

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent<GameObject> enemyDeathEvent;
        public UnityEvent waveEnd;
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
            #endregion
        }

        public void EnemyDeathEvent(GameObject enemy)
        {
            enemyDeathEvent.Invoke(enemy);
        }

    }
}