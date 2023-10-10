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
    
    public enum AttackState
    {
        None,
        WindingUp,
        CoolingDown,
        Executing
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
        private BoxCollider2D weaponHitbox;
        private SpriteRenderer spriteRenderer;
        #endregion

        #region FIELDS
        // Inhereted data
        private double damage;
        private bool canAttack;

        // State
        [SerializeField] private EnemyState state;
        [SerializeField] private AttackState attackState;
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

            position = new Vector2(2, 2);

            health = enemyData.HEALTH;
            speed = enemyData.MOVE_SPEED;
            damage = enemyData.DAMAGE;
            // For now this works
            weaponHitbox = GameObject.Find("Sword").GetComponent<BoxCollider2D>();
            weaponHitbox.enabled = false;
            spriteRenderer = GetComponent<SpriteRenderer>();

            state = EnemyState.Seeking;
            attackState = AttackState.None;

            canAttack = true;

            acceleration = new Vector2(1.0f, 1.0f);
        }

        private void FixedUpdate()
        {
            switch(state)
            {
                case EnemyState.Seeking:
                    //direction = ((playerEvent.Position - position).normalized) / 1000;
                    direction = (playerEvent.Position - position).normalized;
                    speed = enemyData.MOVE_SPEED;
                    velocity = direction * speed;

                    // Rotate towards player position
                    Quaternion xToY = Quaternion.LookRotation(Vector3.forward, Vector3.left);
                    Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);
                    transform.rotation = targetRotation * xToY;


                    if (attackState == AttackState.None)
                    {
                        spriteRenderer.color = Color.red;
                        if ((playerEvent.Position - position).magnitude < 1)
                        {
                            state = EnemyState.Attacking;
                        }
                    }
                    break;
                case EnemyState.Attacking:
                    direction = Vector2.zero;
                    velocity = direction * speed;

                    if (attackState == AttackState.None)
                    {
                        Debug.Log("I am attacking");
                        StartCoroutine(AttackWindUP(1));
                    }
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

        IEnumerator Attack(float duration)
        {
            weaponHitbox.enabled = true;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                spriteRenderer.color = Color.yellow;
                yield return null;
            }

            state = EnemyState.Seeking;
            attackState = AttackState.CoolingDown;
            weaponHitbox.enabled = false;
            StartCoroutine(AttackCooldown(4));
        }

        IEnumerator AttackWindUP(float time)
        {
            attackState = AttackState.WindingUp;
            float tTime = time;
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.cyan;

                yield return null;
            }

            attackState = AttackState.Executing;
            StartCoroutine(Attack(0.3f));
        }

        IEnumerator AttackCooldown(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.white;
                yield return null;
            }

            attackState = AttackState.None;
        }
    }
}