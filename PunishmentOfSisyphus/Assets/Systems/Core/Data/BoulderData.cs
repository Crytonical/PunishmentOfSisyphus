using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "BoulderData", menuName = "DataObjects/BoulderData")]

    public class BoulderData : ScriptableObject
    {
        [Header("Rolling")]
        public float MAX_ROLL_SPEED;
        public float ROLL_SPEED_INCREASE;
        public float INITIAL_ROLL_SPEED;

        [Header("Ricochet")]
        public float AIR_TIME;
        public float RICOCHET_SPEED;

        [Header("Combat")]
        public float DAMAGE;
        public float THROW_SPEED;

    }
}