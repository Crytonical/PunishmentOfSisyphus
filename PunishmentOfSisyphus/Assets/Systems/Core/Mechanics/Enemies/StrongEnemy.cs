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
        private void FixedUpdate()
        {
            switch (state)
            {
                case EnemyState.Seeking:
                    //direction = ((playerEvent.Position - position).normalized) / 1000;
                    direction = ((playerEvent.Position - position).normalized);
                    velocity = direction * speed;

                    if ((playerEvent.Position - position).magnitude <= 1.0f)
                    {
                        state = EnemyState.Attacking;
                    }
                    break;
                case EnemyState.Attacking:
                    direction = Vector2.zero;
                    velocity = direction * speed;
                    Debug.Log("I am attacking");
                    break;
                case EnemyState.Damage:
                    break;
            }
        }
    }
}
