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
        #region REFERENCES
        [SerializeField] private Sprite attackSprite;
        [SerializeField] private Animator animator;
        #endregion
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void FixedUpdate()
        {
            // Rotate towards player position
            if (state == EnemyState.Seeking && rotateTowardsPlayer)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                targetRotation *= Quaternion.Euler(0, 0, 270);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f);
            }

            base.FixedUpdate();
        }

        protected override IEnumerator Attack(float duration)
        {
            weaponHitbox.enabled = true;
            speed = enemyData.CHARGE_SPEED;
            while (duration > 0)
            {
                animator.enabled = false;
                spriteRenderer.color = Color.red;
                spriteRenderer.sprite = attackSprite;
                duration -= Time.deltaTime;
                velocity = speed * direction;
                //position = velocity * Time.deltaTime;

                // Check for collision with wall. End early if so
                if (!levelBounds.GetComponent<RectTransform>().rect.Contains(transform.position))
                    duration = 0;

                yield return null;
            }

            state = EnemyState.Seeking;
            speed = enemyData.MOVE_SPEED;
            attackState = AttackState.CoolingDown;
            if (weaponHitbox)
            {
                weaponHitbox.enabled = false;
            }
            animator.enabled = true;
            StartCoroutine(AttackCooldown(attackCooldown));
        }

        protected override IEnumerator AttackWindUP(float time)
        {
            attackState = AttackState.WindingUp;
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.yellow;

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