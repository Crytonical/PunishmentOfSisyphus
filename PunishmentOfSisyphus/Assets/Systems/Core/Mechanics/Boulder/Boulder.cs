using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

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
        private float velocityIncrease, ricochetTimer;
        private double damage;

        // Checks
        private bool canThrow;

        // State
        [SerializeField] private BoulderState state;
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
            damage = boulderData.DAMAGE;
            velocityIncrease = boulderData.ROLL_SPEED_INCREASE;

            // Default values
            canThrow = true;

            // Default State
            state = BoulderState.Held;
        }

        private void OnEnable()
        {
            boulderEvent.thrownEvent.AddListener(ThrowBoulder);
            boulderEvent.dropEvent.AddListener(DropBoulder);
            boulderEvent.pickupEvent.AddListener(PickedUp);
        }

        private void OnDisable()
        {
            boulderEvent.thrownEvent.RemoveListener(ThrowBoulder);
            boulderEvent.dropEvent.RemoveListener(DropBoulder);
            boulderEvent.pickupEvent.RemoveListener(PickedUp);
        }

        // Update is called once per frame
        void Update()
        {
            // Rolling Down
            // Check if the boulder isn't being held or thrown,
            //      and that it's current velocity is less than the maximum velocity

            Debug.Log(Input.mousePosition);

            switch(state)
            {
                case BoulderState.Held:
                    velocity = playerEvent.Velocity;
                    break;

                case BoulderState.Thrown:
                    // Throw stuff
                    velocity = direction * speed * Time.deltaTime;
                    break;

                case BoulderState.Rolling:
                    // Increase Velocity by some percent each frame it is rolling
                    if (Mathf.Abs(velocity.y) <= boulderData.MAX_ROLL_SPEED)
                    {
                        velocity = direction * speed * Time.deltaTime;
                        speed += velocityIncrease;
                    }
                    break;

                case BoulderState.Ricocheting:
                    if (ricochetTimer > 0)
                    {
                        ricochetTimer -= Time.deltaTime;

                        if (Mathf.Abs(velocity.y) <= boulderData.MAX_ROLL_SPEED)
                        {
                            velocity = direction * speed * Time.deltaTime;
                        }
                    }

                    if (ricochetTimer <= 0)
                    {
                        state = BoulderState.Rolling;
                    }
                    break;
            }

            UpdateEventObject();

            base.Update();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                // Trigger damage event on enemy

                // Call ricochet function
                Ricochet(collision);
            }

            if (collision.CompareTag("Wall"))
            {
                direction *= -1;
            }
        }

        private void ThrowBoulder()
        {
            state = BoulderState.Thrown;
            direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized; 
            speed = boulderData.THROW_SPEED;
        }

        private void DropBoulder()
        {
            // Resets velocity
            direction = Vector2.down;
            speed = boulderData.INITIAL_ROLL_SPEED;
            state = BoulderState.Rolling;
        }

        private void PickedUp()
        {
            state = BoulderState.Held;
            position = playerEvent.Position;
            Debug.Log("Picked Up");
        }

        private void Ricochet(Collider2D collision)
        {
            state = BoulderState.Ricocheting;
            ricochetTimer = boulderData.AIR_TIME;
            direction = new Vector2(-direction.x, -1.0f);
            speed = boulderData.RICOCHET_SPEED;
            // Choose a direction
            // Determine speeds based on direction and airtime


            // Direction needs to be a percent of the current velocity, but in the opposite direction 
            // Negate x, use constant 'gravity' mechanic to handle y direction
            // Will create inconsistent angles, but might look good
            // Increase scale as the boulder bounces
        }

        private void UpdateEventObject()
        {
            boulderEvent.State = state;
        }
    }
}
