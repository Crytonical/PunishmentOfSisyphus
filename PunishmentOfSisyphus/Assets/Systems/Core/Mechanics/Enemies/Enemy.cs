using Ephymeral.BoulderNS;
using Ephymeral.Data;
using Ephymeral.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace Ephymeral.EnemyNS
{
    public enum EnemyState
    {
        Seeking,
        Attacking,
        Damage
    }

    public class Enemy : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private BoulderData boulderData;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private EnemyMovementData enemyData;
        [SerializeField] public EnemyEvent enemyEvent;
        [SerializeField] private CircleCollider2D hitbox;
        #endregion

        #region FIELDS
        // Inhereted data
        private double damage;
        private double health = 100;

        // State
        [SerializeField] private EnemyState state;
        #endregion

        #region PROPERTIES

        #endregion

        private void OnEnable()
        {
            enemyEvent.damageEvent.AddListener(TakeDamage);
            enemyEvent.deathEvent.AddListener(Die);
        }

        private void OnDisable()
        {
            enemyEvent.damageEvent.RemoveListener(TakeDamage);
            enemyEvent.deathEvent.AddListener(Die);
        }

        

        private void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
            {
                enemyEvent.Die();
            }
        }

        private void Die()
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}