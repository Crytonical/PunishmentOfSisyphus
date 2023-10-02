using UnityEngine.Events;
using UnityEngine;
using Codice.CM.Common;

using Ephymeral.BoulderNS;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "BoulderEvent", menuName = "EventObjects/BoulderEvent")]
    public class BoulderEvent : ScriptableObject
    {
        #region FIELDS
        private BoulderState state;

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent thrownEvent;
        public UnityEvent pickupEvent;
        public UnityEvent dropEvent;
        public UnityEvent ricochetEvent;
        #endregion
        #endregion

        #region PROPERTIES
        public BoulderState State { get { return state; } set { state = value; } }
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
