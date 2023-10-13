using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "PlayerMovementData", menuName = "DataObjects/PlayerMovementData")]

    public class PlayerMovementData : ScriptableObject
    {
        [Header("Speeds")]
        public float CARRY_SPEED;
        public float FREE_SPEED;
        public float DODGE_SPEED;

        [Header("Dodging")]
        public float DODGE_DURATION;

        [Header("Attacking")]
        public float LUNGE_SPEED;
        public float LUNGE_DURATION;
    }
}