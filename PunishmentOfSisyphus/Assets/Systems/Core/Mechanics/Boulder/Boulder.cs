using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;
using Ephymeral.EnemyNS;

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
        private float damage;

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
                    // Need to calculate speed not velocity
                    elapsedTime += Time.deltaTime;

                    speed = (boulderData.INITIAL_THROW_SPEED + (-boulderData.THROW_DECELERATION * elapsedTime)) / 1000;

                    velocity = direction * speed;

                    if (speed <= 0)
                    {
                        DropBoulder();
                    }
                    break;
                case BoulderState.Rolling:
                    // Increase Velocity by some percent each frame it is rolling
                    if (Mathf.Abs(velocity.y) <= boulderData.MAX_ROLL_SPEED)
                    {
                        elapsedTime += Time.deltaTime;

                        // Update speed
                        speed = (boulderData.INITIAL_ROLL_SPEED + (boulderData.GRAVITY * elapsedTime)) / 1000;

                        // Update velocity
                        velocity = direction * speed;
                    }
                    break;
                case BoulderState.Ricocheting:
                    if (elapsedTime <= boulderData.AIR_TIME)
                    {
                        elapsedTime += Time.deltaTime;
                        // Update speed
                        speed = (boulderData.INITIAL_RICOCHET_SPEED + (boulderData.RICOCHET_ACCELERATION * elapsedTime)) / 1000;
                        velocity.x = direction.x * speed;

                        // Update velocity
                        // Add lerping / curves
                        speed = (boulderData.INITIAL_RICOCHET_SPEED + (-boulderData.GRAVITY * elapsedTime)) / 1000;
                        velocity.y = direction.y * speed;
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
            if (collision.CompareTag("Enemy") && state == BoulderState.Thrown)
            {
                // Trigger damage event on enemy
                collision.GetComponent<Enemy>().enemyEvent.TakeDamage(damage);

                // Call ricochet function
                Ricochet(collision);
            }

            if (collision.CompareTag("Wall"))
            {
                direction.x *= -1;
            }
        }

        private void ThrowBoulder()
        {
            state = BoulderState.Thrown;
            direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized;
        }

        private void DropBoulder()
        {
            state = BoulderState.Rolling;
            direction = Vector2.down;
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
            direction = new Vector2(-direction.x, 1.0f);
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
