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
        //protected override void Awake()
        //{
        //    base.Awake();
        //}

        protected override IEnumerator Attack(float duration)
        {
            weaponHitbox.enabled = true;
            while (duration > 0)
            {
                spriteRenderer.color = Color.yellow;
                duration -= Time.deltaTime;
                velocity = enemyData.CHARGE_SPEED * direction;
                //position = velocity * Time.deltaTime;

                // Check for collision with wall. End early if so
                if (!levelBounds.GetComponent<RectTransform>().rect.Contains(transform.position))
                    duration = 0;

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

        protected override IEnumerator AttackWindUP(float time)
        {
            attackState = AttackState.WindingUp;
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.cyan;

                // Lerp direction away from player
                direction = (playerEvent.Position - position).normalized;
                //Quaternion xToY = Quaternion.LookRotation(Vector3.forward, Vector3.left);
                //Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);
                //transform.rotation = targetRotation * xToY;
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
            //Gizmos.DrawWireSphere(position, attackRadius);
            Gizmos.DrawWireSphere(position, attackRange);
        }
    }
}