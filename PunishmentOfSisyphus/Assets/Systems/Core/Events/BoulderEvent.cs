using UnityEngine.Events;
using UnityEngine;

namespace Ephymeral.Events
{
    [CreateAssetMenu(fileName = "BoulderEvent", menuName = "EventObjects/BoulderEvent")]
    public class BoulderEvent : ScriptableObject
    {
        #region FIELDS
        private bool isHeld;
        private bool isThrown;
        private bool isRolling;

        #region EVENTS
        [System.NonSerialized]
        public UnityEvent throwEvent;
        public UnityEvent pickupEvent;
        public UnityEvent dropEvent;
        #endregion
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (throwEvent == null)
            {
                throwEvent = new UnityEvent();
            }
            if (pickupEvent == null)
            {
                pickupEvent = new UnityEvent();
            }
            if (dropEvent == null)
            {
                dropEvent = new UnityEvent();
            }
            #endregion
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // --- Events List ---
         // Picked up
         // Dropped
         // Thrown
         // 
    }
}
