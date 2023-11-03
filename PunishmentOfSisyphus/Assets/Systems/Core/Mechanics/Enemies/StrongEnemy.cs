using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.EnemyNS;
using Ephymeral.Data;
using Ephymeral.Events;

namespace Ephymeral.EnemyNS
{
    public class StrongEnemy : Enemy
    {
        protected override IEnumerator Attack(float duration)
        {
            weaponHitbox.enabled = true;
            while (duration > 0)
            {
                duration -= Time.deltaTime;
                spriteRenderer.color = Color.yellow;
                // lerp towards player
                transform.Rotate(new Vector3(0.0f, 0.0f, enemyData.ATTACK_SPEED_MODIFIER));
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
    }
}
