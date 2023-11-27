using Ephymeral.EntityNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.EnemyNS
{
    public class RangedEnemy : Enemy
    {
        #region REFERENCES
        [SerializeField] private GameObject bulletPrefab;
        #endregion

        #region FIELDS
        #endregion

        #region PROPERTIES
        #endregion

        protected override void Awake()
        {
            base.Awake();

            rotateTowardsPlayer = false;
        }
        
        protected override IEnumerator Attack(float duration)
        {
            // Get the direction of the attack (towards player)
            direction = (playerEvent.Position - position).normalized;

            // Spawn a bullet, get reference to its bullet script
            GameObject bullet = Instantiate(bulletPrefab, transform);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // Set its direction, set its velocity
            bulletScript.Damage = damage;
            bulletScript.Position = position;
            bulletScript.Direction = direction;
            bulletScript.Velocity = bulletScript.Direction * enemyData.ATTACK_SPEED_MODIFIER;

            // Change its color while its attacking
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                spriteRenderer.color = Color.white;
                yield return null;
            }

            state = EnemyState.Seeking;
            attackState = AttackState.CoolingDown;
            StartCoroutine(AttackCooldown(attackCooldown));
        }

        protected override IEnumerator AttackWindUP(float time)
        {
            attackState = AttackState.WindingUp;
            while (time > 0)
            {
                time -= Time.deltaTime;
                spriteRenderer.color = Color.red;

                // Rotate towards player
                //direction = (playerEvent.Position - position).normalized;
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
                spriteRenderer.color = Color.green;
                yield return null;
            }

            attackState = AttackState.None;
        }

    }
}