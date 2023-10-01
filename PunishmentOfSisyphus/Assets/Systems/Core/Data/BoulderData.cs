using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "BoulderData", menuName = "DataObjects/BoulderData")]

    public class BoulderData : ScriptableObject
    {
        [Header("Rolling")]
        public Vector2 velocity;
        public Vector2 initialVelocity;
        public Vector2 maxVelocity;
        public float velocityPercentIncrease;

        [Header("Ricochet")]
        public Vector2 airTime;

        [Header("Ricochet")]
        public Vector2 airTime;

        [Header("Combat")]
        public float damage;

    }
}