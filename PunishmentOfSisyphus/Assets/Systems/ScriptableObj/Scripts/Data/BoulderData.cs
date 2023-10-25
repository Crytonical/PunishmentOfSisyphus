using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "BoulderData", menuName = "DataObjects/BoulderData")]

    public class BoulderData : ScriptableObject
    {
        [Header("General")]
        public float GRAVITY;
        public float FUTURE_PREDICTION;

        [Header("Rolling")]
        public float MAX_ROLL_SPEED;
        public float INITIAL_ROLL_SPEED;

        [Header("Ricochet")]
        public float AIR_TIME;
        public float INITIAL_RICOCHET_SPEED;
        public float RICOCHET_ACCELERATION;
        public float BOUNCE_COEFFICIENT;

        [Header("Throwing")]
        public float INITIAL_THROW_SPEED;
        public float THROW_DECELERATION;

        [Header("Combat")]
        public float DAMAGE;
        public float HIT_SPEED_MIN;
    }
}