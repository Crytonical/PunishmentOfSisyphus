using Ephymeral.BoulderNS;
using Ephymeral.Data;
using Ephymeral.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

using Ephymeral.EntityNS;

namespace Ephymeral.EnemyNS
{
    public enum EnemyState
    {
        Seeking,
        Attacking,
        Damage
    }

    public class Enemy : Entity
    {
        #region REFERENCES
        [SerializeField] protected BoulderEvent boulderEvent;
        [SerializeField] protected BoulderData boulderData;
        [SerializeField] protected PlayerEvent playerEvent;
        [SerializeField] protected EnemyMovementData enemyData;
        [SerializeField] protected EnemyEvent enemyEvent;
        [SerializeField] protected CircleCollider2D hitbox;
        #endregion

        #region FIELDS
        // Inhereted data
        protected double damage;

        // State
        [SerializeField] protected EnemyState state;
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

        private void Awake()
        {
            base.Awake();

            position = new Vector2(20, 20);

            health = enemyData.HEALTH;
            speed = enemyData.MOVE_SPEED;
            damage = enemyData.DAMAGE;
        }

        private void FixedUpdate()
        {
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