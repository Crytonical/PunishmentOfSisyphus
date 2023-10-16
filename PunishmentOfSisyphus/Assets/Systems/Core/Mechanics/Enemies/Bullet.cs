using Ephymeral.Data;
using Ephymeral.EntityNS;
using Ephymeral.Events;
using Ephymeral.PlayerNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Ephymeral.EnemyNS
{
    public class Bullet : Entity
    {
        #region REFERENCES
        [SerializeField] private PlayerEvent playerEvent;
        #endregion

        #region FIELDS
        private float damage;
        #endregion

        #region properties
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Direction { get { return direction; } set { direction = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public float Damage { get { return damage; } set { damage = value; } }
        #endregion

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("ScreenBounds"))
            {
                Destroy(gameObject);
            }

            if (collision.CompareTag("Player"))
            {
                Debug.Log("Player hit!");
                playerEvent.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}