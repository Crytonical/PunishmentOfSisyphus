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
        public float CHARGE_SPEED;

        [Header("Combat")]
        public float DAMAGE;
        public float HEALTH;
        public float ATTACK_RANGE;
        public float ATTACK_WIND_UP;
        public float ATTACK_COOLDOWN;
        public float ATTACK_DURATION;
        public float DAMAGE_STUN_DURATION;

        // This can change its usage based on the type of enemy using it
        //      for the slow enemy it is the speed it rotates when spinning
        //      for the ranged enemy it is the speed of the bullet
        public float ATTACK_SPEED_MODIFIER;
    }
}