using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.EnemyNS;
using Ephymeral.PlayerNS;
using UnityEngine.Events;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "EnemyEvent", menuName = "EventObjects/EnemyEvent")]
    public class EnemyEvent : ScriptableObject
    {
        #region FIELDS

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent<float> damageEvent;
        public UnityEvent attackEvent;
        public UnityEvent deathEvent;
        #endregion
        #endregion

        #region PROPERTIES
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (damageEvent == null)
            {
                damageEvent = new UnityEvent<float>();
            }
            if (attackEvent == null)
            {
                attackEvent = new UnityEvent();
            }
            if (deathEvent == null)
            {
                deathEvent = new UnityEvent();
            }
            #endregion
        }

        public void Attack()
        {
            attackEvent.Invoke();
        }

        public void TakeDamage(float damage)
        {
            damageEvent.Invoke(damage);
        }

        public void Die()
        {
            deathEvent.Invoke();
        }

        // Event List
        //  Attack
        //  TakeDamage
        //  Die
        //  DealDamage(?)
    }
}