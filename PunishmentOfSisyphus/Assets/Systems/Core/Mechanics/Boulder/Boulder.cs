using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ephymeral.Data;
using Ephymeral.Events;

namespace Ephymeral.BoulderNS
{
    public class Boulder : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private BoulderData boulderData;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private Rigidbody2D RB;
        [SerializeField] private CircleCollider2D hitbox;
        [SerializeField]
        #endregion

        #region FIELDS
        // Inhereted data
        private Vector2 velocity;
        private Vector2 ricochetVelocity;
        private float velocityIncrease;
        private double damage;

        // Checks
        private bool isHeld;
        private bool isThrown;
        private bool isRolling;
        private bool isRicocheting;
        private bool canThrow;
        #endregion

        #region PROPERTIES

        #endregion

        private void Awake()
        {
            // Assign References
            RB = GetComponent<Rigidbody2D>();
            hitbox = GetComponent<CircleCollider2D>();

            // Initialize Variables
            velocity = boulderData.initialVelocity;
            damage = boulderData.damage;
            velocityIncrease = boulderData.velocityPercentIncrease;

            // Default values
            isHeld = true;
            isThrown = false;
            isRolling = false;
            isRicocheting = false;
            canThrow = true;
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
            if (!isRicocheting && !isHeld && !isThrown && velocity.y <= boulderData.maxVelocity.y)
            {
                // Check if we are starting to roll, i.e we were not rolling last frame
                if (!isRolling)
                {
                    Debug.Log("Initiating Rolling");

                    // Set rolling to true
                    isRolling = true;
                }
                // Set the rigid body's velocity to the increased velocity
                RB.velocity = new Vector2(RB.velocity.x, -velocity.y);

                // Increase Velocity by some percent each frame it is rolling
                velocity.y *= velocityIncrease;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy") && isThrown)
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
                isThrown = true;
            }
        }

        private void DropBoulder()
        { 
            // Sets trigger
            isHeld = false;

            // Resets velocity
            velocity = boulderData.initialVelocity;
        }

        private void PickedUp()
        {
            // Sets checks
            isHeld = true;
            isRolling = false;

            // Stops boulder
            velocity = Vector2.zero;
            RB.velocity = velocity;

            // TESTING
            DropBoulder();
        }

        private void Ricochet(Collision2D collision)
        {
            // Choose a direction
            isRicocheting = true;
            // Determine speeds based on direction and airtime


            // Direction needs to be a percent of the current velocity, but in the opposite direction 
            // Negate x, use constant 'gravity' mechanic to handle y direction
            // Will create inconsistent angles, but might look good
            // Increase scale as the boulder bounces
        }
    }
}
