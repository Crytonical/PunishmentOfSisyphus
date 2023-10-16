using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.EnemyNS;
using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;

namespace Ephymeral.EnemyNS
{
    public class RangedProjectile : Enemy
    {
        #region REFERENCES
        //[SerializeField] private EnemyProjectileData projectileData;
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES
        public Vector2 Direction { set {  direction = value; } }
        #endregion

        // Start is called before the first frame update
        protected override void Awake()
        {
            // Didn't call Enemy due to unneeded code
            state = EnemyState.Seeking;
            attackState = AttackState.None;

            // Inherit data from projectile data
            health = projectileData.HEALTH;
            speed = projectileData.MOVE_SPEED;
        }

        // Send projectile in direction enemy faced until it gets removed
        private void FixedUpdate()
        {
            velocity = speed * direction;
        }

        private void Die()
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}
