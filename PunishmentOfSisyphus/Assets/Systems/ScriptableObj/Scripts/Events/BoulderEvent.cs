using UnityEngine.Events;
using UnityEngine;


namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "BoulderEvent", menuName = "EventObjects/BoulderEvent")]
    public class BoulderEvent : ScriptableObject
    {
        #region FIELDS

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent thrownEvent;
        public UnityEvent pickupEvent;
        public UnityEvent dropEvent;
        public UnityEvent ricochetEvent;
        public UnityEvent boulderFail;
        #endregion
        #endregion

        #region PROPERTIES
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (thrownEvent == null)
            {
                thrownEvent = new UnityEvent();
            }
            if (pickupEvent == null)
            {
                pickupEvent = new UnityEvent();
            }
            if (dropEvent == null)
            {
                dropEvent = new UnityEvent();
            }
            if (ricochetEvent == null)
            {
                ricochetEvent = new UnityEvent();
            }
            if (boulderFail == null) 
            {
                boulderFail = new UnityEvent();
            }
            #endregion
        }

        public void DropBoulder()
        {
            dropEvent.Invoke();
        }

        public void PickUpBoulder()
        {
            pickupEvent.Invoke();
        }

        public void Throw()
        {
            thrownEvent.Invoke();
        }

        // --- Events List ---
        // Picked up
        // Dropped
        // Thrown
        // Richochet
        // Deal damage (?)
    }
}
