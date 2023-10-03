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
            switch(state)
            {
                case EnemyState.Seeking:
                    direction = ((playerEvent.Position - position).normalized) / 1000;
                    velocity = direction * speed;

                    if ((playerEvent.Position - position).magnitude < 0.5)
                    {
                        state = EnemyState.Attacking;
                    }
                    break;
                case EnemyState.Attacking:
                    direction = Vector2.zero;
                    velocity = direction * speed;
                    Debug.Log("I am ttackgin");
                    break;
                case EnemyState.Damage:
                    break;
            }
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