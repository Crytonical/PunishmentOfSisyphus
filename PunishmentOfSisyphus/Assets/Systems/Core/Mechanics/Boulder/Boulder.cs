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
        private Vector2 initialVelocity;
        private float elapsedTime;
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
            // State machine
            switch(state)
            {
                // Held
                case BoulderState.Held:
                    velocity = playerEvent.Velocity;
                    break;

                // Throwing
                case BoulderState.Thrown:
                    // Throw stuff
                    break;
                
                // Rolling
                case BoulderState.Rolling:
                    break;

                // Ricocheting
                case BoulderState.Ricocheting:
                    
                    break;
            }

            UpdateEventObject();

            base.Update();
        }

        private void FixedUpdate()
        {
            switch (state) 
            {
                case BoulderState.Held:
                    // Held
                    break;
                case BoulderState.Thrown:
                    velocity = direction * speed * Time.deltaTime;
                    break;
                case BoulderState.Rolling:
                    // Increase Velocity by some percent each frame it is rolling
                    if (Mathf.Abs(velocity.y) <= boulderData.MAX_ROLL_SPEED)
                    {
                        // Update velocity
                        elapsedTime += Time.deltaTime;
                        Debug.Log(elapsedTime);

                        // Acceleration is just gravity for straight downward movement
                        velocity.y = (initialVelocity.y - (boulderData.GRAVITY * elapsedTime)) / 1000;
                        velocity.x = 0.0f;
                    }
                    break;
                case BoulderState.Ricocheting:
                    if (elapsedTime <= boulderData.AIR_TIME)
                    {
                        // Decrease ricochet timer
                        elapsedTime += Time.deltaTime;

                        // Update velocity
                        velocity.y = direction.y + (speed - boulderData.GRAVITY) * elapsedTime;
                        velocity.x = direction.x + (speed * elapsedTime);
                    }

                    // Check if we should end the ricochet
                    if (elapsedTime >= boulderData.AIR_TIME)
                    {
                        DropBoulder();
                    }
                    break;
            }
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
            speed = boulderData.THROW_ACCELERATION;
        }

        private void DropBoulder()
        {
            // Resets velocity
            initialVelocity.y = -0.01f;
            state = BoulderState.Rolling;
            UpdatePhysicsValues();
        }

        private void PickedUp()
        {
            state = BoulderState.Held;
            position = playerEvent.Position;
            UpdatePhysicsValues();
        }

        private void Ricochet(Collider2D collision)
        {
            state = BoulderState.Ricocheting;
            direction = new Vector2(-direction.x, direction.y);
            speed = boulderData.RICOCHET_ACCELERATION;
            UpdatePhysicsValues();
        }

        private void UpdateEventObject()
        {
            boulderEvent.State = state;
        }

        private void UpdatePhysicsValues()
        {
            elapsedTime = 0;
        }
    }
}
