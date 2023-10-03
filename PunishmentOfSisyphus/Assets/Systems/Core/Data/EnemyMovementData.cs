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
    }
}