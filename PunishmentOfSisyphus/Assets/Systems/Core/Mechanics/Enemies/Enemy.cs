using Ephymeral.BoulderNS;
using Ephymeral.Data;
using Ephymeral.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Image hpBar;
        protected BoxCollider2D weaponHitbox;
        protected SpriteRenderer spriteRenderer;
        protected GameObject levelBounds;
        #endregion

        #region FIELDS
        // Inhereted data
        protected float damage, attackRange, attackWindUp, attackDuration, attackCooldown;
        protected bool canAttack, rotateTowardsPlayer;
        private bool levelTransition;
        protected Vector2 goal;
        [SerializeField] protected Vector3 hpBarOffset;

        // State
        [SerializeField] protected EnemyState state;
        [SerializeField] protected AttackState attackState;
        #endregion

        #region PROPERTIES
        public EnemyEvent EnemyEvent { get { return enemyEvent; } }
        public float Damage { get { return damage; } }

        float boulderInvulSec;
        float boulderInvul;
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

            boulderInvulSec = 0.5f;

            // Get a reference to the hitbox, disable it 
            if (weaponHitbox = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>())
            {
                weaponHitbox.enabled = false;
            }
            spriteRenderer = GetComponent<SpriteRenderer>();

            state = EnemyState.Seeking;
            attackState = AttackState.None;

            canAttack = true;

            acceleration = new Vector2(1.0f, 1.0f);

            // Find better way to get this
            levelBounds = GameObject.Find("LevelBounds");

            Rect levelRect = levelBounds.GetComponent<RectTransform>().rect;

            // Spawn enemies in a random position between given constrictions
            //float enemyX = Random.Range(-8, 8);
            //float enemyY = Random.Range(-4, 7);
            float enemyX = Random.Range(levelRect.xMin * 0.80f, levelRect.xMax * 0.80f);
            //float enemyY = Random.Range(levelRect.yMin * 0.40f, levelRect.yMax * 0.70f) + levelRect.height;
            float enemyY = Random.Range(levelRect.yMin * 0.40f, levelRect.yMax * 0.70f);
            position = new Vector2(enemyX, enemyY);

            hpBar.fillAmount = health / enemyData.HEALTH;

            rotateTowardsPlayer = true;
        }

        protected override void Update()
        {
            if (!levelTransition)
            {
                if (boulderInvul < boulderInvulSec)
                {
                    boulderInvul += Time.deltaTime;
                }

                // Update hp bar
                // Counteract parent rotation
                hpBar.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z * -1.0f);
                // Keep position 'above' enemy
                hpBar.transform.position = transform.position + hpBarOffset;

                // Should force enemies inside the bounds of the level
                if (!levelBounds.GetComponent<RectTransform>().rect.Contains(transform.position))
                {
                    Rect rect = levelBounds.GetComponent<RectTransform>().rect;
                    Vector2 vector = position;

                    // Clamp the x component to be within the rectangle's x boundaries
                    float clampedX = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);

                    // Clamp the y component to be within the rectangle's y boundaries
                    float clampedY = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);

                    // Return the new vector with clamped components
                    position = new Vector2(clampedX, clampedY);
                }
            }

            base.Update();
        }

        protected override void FixedUpdate()
        {
            if (!levelTransition)
            {
                switch (state)
                {
                    case EnemyState.Seeking:
                        //direction = ((playerEvent.Position - position).normalized) / 1000;
                        direction = (playerEvent.Position - position).normalized;

                        if ((playerEvent.Position - position).magnitude > attackRange)
                        {
                            speed = enemyData.MOVE_SPEED;
                            velocity = direction * speed;
                        }
                        else
                        {
                            speed = 0;
                            velocity = direction * speed;
                        }

                        // Rotate towards player position
                        if (rotateTowardsPlayer)
                        {
                            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, -direction);
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);
                        }


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
                        if (attackState == AttackState.None)
                        {
                            direction = Vector2.zero;
                            velocity = direction * speed;
                            StartCoroutine(AttackWindUP(attackWindUp));
                        }
                        break;

                    case EnemyState.Damage:
                        spriteRenderer.color = Color.gray;
                        StartCoroutine(DamageStun(enemyData.DAMAGE_STUN_DURATION));
                        break;
                }
            }

            base.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("Enemy Collision run");
        }

        private void Die()
        {
            enabled = false;
            spawnerEvent.EnemyDeathEvent(gameObject);
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

        public void BoulderHit(float damage, Vector2 knockback)
        {
            if (boulderInvul >= boulderInvulSec)
            {
                TakeDamage(damage, knockback);
                boulderInvul = 0;
            }
        }

        /// <summary>
        /// Called when the Enemy 
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="knockback"></param>
        public virtual void TakeDamage(float damage, Vector2 knockback)
        {
            if (state != EnemyState.Damage)
            {
                state = EnemyState.Damage;
                attackState = AttackState.None;
                health -= damage;

                hpBar.fillAmount = health / enemyData.HEALTH;

                //position += knockback;

                //FXManager.Instance.ShakeScreen(0.08f, 8);
                FXManager.Instance.ShakeScreen(0.18f, 10);


                if (health <= 0)
                {
                    Die();
                }

                ApplyKnockback(knockback, 15);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="knockback"></param>
        /// <param name="duration"></param>
        public virtual void ApplyKnockback(Vector2 knockback, int durationFrames)
        {
            StartCoroutine(KnockbackCo(knockback, durationFrames));
        }

        protected virtual IEnumerator KnockbackCo(Vector2 knockback, int durationFrames)
        {
            while (durationFrames > 0)
            {
                durationFrames -= 1;
                position += knockback;
                knockback *= 0.9f;
                yield return new WaitForFixedUpdate();
            }
        }

        // Swap between enabled and disabled in one method. MIGHT BE BAD IDEA FOR IMPLEMENTATION
        public void ToggleLevelTransition()
        {
            levelTransition = !levelTransition;
        }

    }
}