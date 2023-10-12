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
    // State of enemy
    public enum EnemyState
    {
        Seeking,
        Attacking,
        Damage
    }
    
    // State of enemy when attacking
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
        [SerializeField] protected BoulderEvent boulderEvent;
        [SerializeField] protected BoulderData boulderData;
        [SerializeField] protected PlayerEvent playerEvent;
        [SerializeField] protected EnemyMovementData enemyData;
        [SerializeField] protected EnemyEvent enemyEvent;
        [SerializeField] protected CircleCollider2D hitbox;
        [SerializeField] protected EnemySpawnerEvent spawnerEvent;
        protected BoxCollider2D weaponHitbox;
        protected SpriteRenderer spriteRenderer;
        #endregion

        #region FIELDS
        // Inhereted data
        protected float damage, attackRange, attackWindUp, attackDuration, attackCooldown;
        protected bool canAttack;

        // State
        [SerializeField] protected EnemyState state;
        [SerializeField] protected AttackState attackState;
        #endregion

        #region PROPERTIES
        public EnemyEvent EnemyEvent {  get { return enemyEvent; } }
        public float Damage { get { return damage; } }
        #endregion

        private void OnEnable()
        {
            enemyEvent.deathEvent.AddListener(Die);
        }

        private void OnDisable()
        {
            enemyEvent.deathEvent.AddListener(Die);
        }

        protected override void Awake()
        {
            base.Awake();

            // Inherit data from data handler
            health = enemyData.HEALTH;
            speed = enemyData.MOVE_SPEED;
            damage = enemyData.DAMAGE;
            attackRange = enemyData.ATTACK_RANGE;
            attackWindUp = enemyData.ATTACK_WIND_UP;
            attackDuration = enemyData.ATTACK_DURATION;
            attackCooldown = enemyData.ATTACK_COOLDOWN;

            // Get a reference to the hitbox, disable it 
            weaponHitbox = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>();
            weaponHitbox.enabled = false;
            spriteRenderer = GetComponent<SpriteRenderer>();

            state = EnemyState.Seeking;
            attackState = AttackState.None;

            canAttack = true;

            acceleration = new Vector2(1.0f, 1.0f);

            position = enemyData.SPAWN_POSITION;
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
                        if ((playerEvent.Position - position).magnitude < attackRange)
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
                        StartCoroutine(AttackWindUP(attackWindUp));
                    }
                    break;
                case EnemyState.Damage:
                    spriteRenderer.color = Color.gray;
                    StartCoroutine(DamageStun(enemyData.DAMAGE_STUN_DURATION));
                    break;
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log(gameObject.name + " Is taking damage: " + damage);
            state = EnemyState.Damage;
            attackState = AttackState.None;
            health -= damage;

            FXManager.Instance.ScreenFreeze(7);

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            enabled = false;
            spawnerEvent.EnemyDeathEvent(gameo);
            StopAllCoroutines();
        }

        protected virtual IEnumerator Attack(float duration)
        {
            weaponHitbox.enabled = true;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                spriteRenderer.color = Color.yellow;
                // lerp towards player
                yield return null;
            }

            state = EnemyState.Seeking;
            attackState = AttackState.CoolingDown;
            if (weaponHitbox)
            {
                weaponHitbox.enabled = false;
            }
            StartCoroutine(AttackCooldown(attackCooldown));
        }

        protected virtual IEnumerator AttackWindUP(float time)
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
            StartCoroutine(Attack(attackDuration));
        }

        protected virtual IEnumerator AttackCooldown(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.white;
                yield return null;
            }

            attackState = AttackState.None;
        }

        protected virtual IEnumerator DamageStun(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.gray;
                velocity = Vector2.zero;
                yield return null;
            }
            state = EnemyState.Seeking;
        }
    }
}