using UnityEngine.Events;
using UnityEngine;
using UnityEditor;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "PlayerEvent", menuName = "EventObjects/PlayerEvent")]
    public class PlayerEvent : ScriptableObject
    {
        #region FIELDS
        private bool canThrow;
        private float speed, health;
        private Vector2 direction, velocity, position;

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent dodgeEvent;
        public UnityEvent throwEvent;
        public UnityEvent<float> damageEvent;
        public UnityEvent attackEvent;
        #endregion
        #endregion

        #region PROPERTIES
        public float Speed { get { return speed; } set {  speed = value; } }
        public bool CanThrow { get {  return canThrow; } set {  canThrow = value; } }
        public float Health { get { return health; } set { health = value; } }
        public Vector2 Direction { get { return direction; } set {  direction = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (throwEvent == null)
            {
                throwEvent = new UnityEvent();
            }
            if (attackEvent == null)
            {
                attackEvent = new UnityEvent();
            }
            if (dodgeEvent == null)
            {
                dodgeEvent = new UnityEvent();
            }
            if (damageEvent == null) 
            { 
                damageEvent = new UnityEvent<float>();
            }
            #endregion
        }

        public void TakeDamage(float damage)
        {
            damageEvent.Invoke(damage);
        }

        // Event list
        //  dodge
        //  throw
        //  take damage
        //  deal damage (?)
        //  punch things
        //  
    }
}
