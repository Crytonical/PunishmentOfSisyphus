using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;

namespace Ephymeral.BoulderNS
{
    public enum BoulderState
    { 
        Held,
        Thrown,
        Rolling,
        Ricocheting
    }

    public class Boulder : Entity
    {
        #region REFERENCES
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private BoulderData boulderData;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private CircleCollider2D hitbox;
        #endregion

        #region FIELDS
        // Inhereted data
        private Vector2 ricochetVelocity;
        private float velocityIncrease;
        private double damage;

        // Checks
        private bool canThrow;

        // State
        private BoulderState state;
        #endregion

        #region PROPERTIES

        #endregion

        private void Awake()
        {
            // Run base Awake function
            base.Awake();

            // Assign References
            hitbox = GetComponent<CircleCollider2D>();

            // Initialize Variables
            damage = boulderData.damage;
            velocityIncrease = boulderData.velocityPercentIncrease;

            // Default values
            canThrow = true;

            // Default State
            state = BoulderState.Held;
        }

        private void OnEnable()
        {
            boulderEvent.throwEvent.AddListener(ThrowBoulder);
            boulderEvent.dropEvent.AddListener(DropBoulder);
            boulderEvent.pickupEvent.AddListener(PickedUp);
        }

        private void OnDisable()
        {
            boulderEvent.throwEvent.RemoveListener(ThrowBoulder);
            boulderEvent.dropEvent.RemoveListener(DropBoulder);
            boulderEvent.pickupEvent.RemoveListener(PickedUp);
        }

        // Update is called once per frame
        void Update()
        {
            // Rolling Down
            // Check if the boulder isn't being held or thrown,
            //      and that it's current velocity is less than the maximum velocity

            switch(state)
            {
                case BoulderState.Held:
                    // Set vel to 0
                    velocity = Vector2.zero;
                    break;

                case BoulderState.Thrown:
                    // Throw stuff
                    break;

                case BoulderState.Rolling:
                    // Increase Velocity by some percent each frame it is rolling
                    velocity.y *= velocityIncrease;
                    break;

                case BoulderState.Ricocheting:
                    break;
            }

            transform.position = position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                // Trigger damage event on enemy

                // Call ricochet function
                Ricochet(collision);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("EA");
                // Trigger pickup event on player
                // playerEvent.PickupBoulder
                PickedUp();
            }
        }

        private void ThrowBoulder()
        {
            if (canThrow)
            {

            }
        }

        private void DropBoulder()
        { 
            // Resets velocity
            velocity = boulderData.initialVelocity;
        }

        private void PickedUp()
        {

            // Stops boulder
            velocity = Vector2.zero;

            // TESTING
            DropBoulder();
        }

        private void Ricochet(Collision2D collision)
        {
            // Choose a direction
            // Determine speeds based on direction and airtime


            // Direction needs to be a percent of the current velocity, but in the opposite direction 
            // Negate x, use constant 'gravity' mechanic to handle y direction
            // Will create inconsistent angles, but might look good
            // Increase scale as the boulder bounces
        }
    }
}
