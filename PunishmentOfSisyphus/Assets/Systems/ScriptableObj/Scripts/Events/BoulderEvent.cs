using UnityEngine.Events;
using UnityEngine;


namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "BoulderEvent", menuName = "EventObjects/BoulderEvent")]
    public class BoulderEvent : ScriptableObject
    {
        #region FIELDS
        private Vector2 position;
        private bool updatePosition;
        #endregion

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent thrownEvent;
        public UnityEvent pickupEvent;
        public UnityEvent dropEvent;
        public UnityEvent ricochetEvent;
        public UnityEvent boulderFail;
        public UnityEvent predictionEvent;
        #endregion

        #region PROPERTIES
        public Vector2 Position { get { return position; } set { position = value; } }
        public bool UpdatePosition { get { return updatePosition; } set { updatePosition = value; } }
        #endregion

        private void OnEnable()
        {
            // Initialize fields
            position = new Vector2();
            updatePosition = false;

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
            if (predictionEvent == null)
            {
                predictionEvent = new UnityEvent();
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

        // Predict future position of boulder
        public void Predict()
        {
            predictionEvent.Invoke();
        }

        // --- Events List ---
        // Picked up
        // Dropped
        // Thrown
        // Richochet
        // Deal damage (?)
    }
}
