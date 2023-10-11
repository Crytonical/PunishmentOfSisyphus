using UnityEngine.Events;
using UnityEngine;
using UnityEditor;

using Ephymeral.PlayerNS;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "PlayerEvent", menuName = "EventObjects/PlayerEvent")]
    public class PlayerEvent : ScriptableObject
    {
        #region FIELDS
        private bool canThrow;
        private float speed;
        private Vector2 direction, velocity, position;
        private PlayerState state;

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent dodgeEvent;
        public UnityEvent throwEvent;
        public UnityEvent<float> damageEvent;
        public UnityEvent takeDamageEvent;
        public UnityEvent attackEvent;
        #endregion
        #endregion

        #region PROPERTIES
        public float Speed { get { return speed; } set {  speed = value; } }
        public bool CanThrow { get {  return canThrow; } set {  canThrow = value; } }
        public Vector2 Direction { get { return direction; } set {  direction = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public PlayerState State { get { return state; } set { state = value; } }
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
            if (takeDamageEvent == null)
            {
                takeDamageEvent = new UnityEvent();
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
