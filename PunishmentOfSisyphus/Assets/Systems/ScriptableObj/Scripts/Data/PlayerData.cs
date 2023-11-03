using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ephymeral.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "DataObjects/PlayerData")]

    public class PlayerData : ScriptableObject
    {
        [Header("Speeds")]
        public float CARRY_SPEED;
        public float FREE_SPEED;
        public float DODGE_SPEED;

        [Header("Dodging")]
        public float DODGE_DURATION;
        public float DODGE_COOLDOWN;

        [Header("Attacking")]
        public float LUNGE_SPEED;
        public int SLAM_DURATION;
        public float SLAM_DAMAGE;

        [Header("Health")]
        public float MAX_HP;
    }
}