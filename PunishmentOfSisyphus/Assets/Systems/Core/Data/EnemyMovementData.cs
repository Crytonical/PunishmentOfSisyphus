using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "EnemyMovementData", menuName = "DataObjects/EnemyMovementData")]

    public class EnemyMovementData : ScriptableObject
    {
        [Header("General")]
        public float GRAVITY;

        [Header("Movement")]
        public float MOVE_SPEED;

        [Header("Combat")]
        public float DAMAGE;
        public float HEALTH;
        public float ATTACK_RANGE;
        public float ATTACK_WIND_UP;
        public float ATTACK_COOLDOWN;
        public float ATTACK_DURATION;
    }
}