using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.EnemyNS;
using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.PlayerNS;

namespace Ephymeral.EnemyNS
{
    public class FastEnemy : Enemy
    {
        float attackRadius;
        float attackDelay;
        float chargeSpeed;
        float turnSpeed;

        protected override void Awake()
        {
            base.Awake();
            attackRadius = 5f;
            attackDelay = 2;
        }

        protected override void FixedUpdate()
        {
            //Enemy state logic
            switch (state)
            {
                case EnemyState.Seeking:
                    direction = (playerEvent.Position - position).normalized;

                    // Rotate towards player position
                    Quaternion xToY = Quaternion.LookRotation(Vector3.forward, Vector3.left);
                    Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);
                    transform.rotation = targetRotation * xToY;

                    if ( (position - playerEvent.Position).magnitude <= attackRadius)
                    {
                        StartCoroutine(Attack(0));
                        state = EnemyState.Attacking;
                    }
                    break;

                case EnemyState.Attacking:

                    break;

                case EnemyState.Damage:
                    spriteRenderer.color = Color.gray;
                    StartCoroutine(DamageStun(enemyData.DAMAGE_STUN_DURATION));
                    break;
            }
            
        }

        protected override IEnumerator Attack(float duration)
        {
            while (duration < 16)
            {
                duration++;
                direction = (playerEvent.Position - position).normalized;

                // Rotate towards player position
                Quaternion xToY = Quaternion.LookRotation(Vector3.forward, Vector3.left);
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);
                transform.rotation = targetRotation * xToY;
                yield return new WaitForFixedUpdate();
            }
            while (duration >= 16 && duration <= 24)
            {
                duration++;
                velocity = chargeSpeed * direction;
                position += velocity;
                yield return new WaitForFixedUpdate();
            }

            state = EnemyState.Seeking;
            attackState = AttackState.CoolingDown;

            if (weaponHitbox)
            {
                weaponHitbox.enabled = false;
            }
            StartCoroutine(AttackCooldown(attackCooldown));
            
        }

        protected override IEnumerator AttackWindUP(float time)
        {
            attackState = AttackState.WindingUp;
            float tTime = time;
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.cyan;
                // Lerp away from player

                direction = (playerEvent.Position - position).normalized;
                Quaternion xToY = Quaternion.LookRotation(Vector3.forward, Vector3.left);
                Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);
                transform.rotation = targetRotation * xToY;
                yield return null;
            }

            attackState = AttackState.Executing;
            StartCoroutine(Attack(attackDuration));
        }

        protected override IEnumerator AttackCooldown(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.white;
                yield return null;
            }

            attackState = AttackState.None;
        }

        public override void TakeDamage(float damage, Vector2 knockback)
        {
            base.TakeDamage(damage, knockback);

            StopCoroutine("Attack");
            StopCoroutine("AttackWindUP");
            StopCoroutine("AttackCooldown");
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(position, attackRadius);
        }
    }
}
