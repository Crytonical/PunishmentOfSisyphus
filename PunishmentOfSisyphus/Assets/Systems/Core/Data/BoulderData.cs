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

        [Header("Rolling")]
        public float MAX_ROLL_SPEED;
        public float INITIAL_ROLL_SPEED;

        [Header("Ricochet")]
        public float AIR_TIME;
        public float INITIAL_RICOCHET_SPEED;
        public float RICOCHET_ACCELERATION;

        [Header("Throwing")]
        public float THROW_SPEED;

        [Header("Combat")]
        public float DAMAGE;
    }
}