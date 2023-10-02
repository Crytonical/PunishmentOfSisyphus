using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;

using Ephymeral.EntityNS;
using Ephymeral.BoulderNS;
using Ephymeral.Events;
using Ephymeral.Data;

// INPUTSYSTEM INS'T WORKING FOR SOME REASON DUE TO DIRECTORY

namespace Ephymeral.PlayerNS
{
    public enum PlayerState
    {
        CarryingBounder,
        Free,
        Dodge,
        Damage,
        Throwing
    }

    public class Player : Entity
    {
        #region References
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private PlayerMovementData playerMovementData;
        #endregion

        #region Fields

        [SerializeField] private PlayerState state;

        // Only exists so that directions input during roll are registered
        // When it ends. Otherwise, you'll need to press the key again
        private Vector2 dodgeDirection;
        #endregion

        #region Properties
        #endregion

        private void OnEnable()
        {
            // Add event listeners
        }

        private void OnDisable()
        {
            // remove event listeners
        }

        /// <summary>
        /// Initializes fields
        /// </summary>
        private void Awake()
        {
            // Declare Entity variables
            speed = playerMovementData.FREE_SPEED;
            health = 100; // FOR TESTING, NOT FINAL

            // Just for testing. Should start with carrying boulder
            state = PlayerState.Free;
            //collision = getComponent(BoxCollider2D);

            // Run parent's awake method
            base.Awake();
        }

        // Update is called once per frame
        private void Update()
        {
            switch(state)
            {
                case PlayerState.CarryingBounder:
                    // Handle anything boulder related here?
                    // Slow speed
                    velocity = direction * speed * Time.deltaTime;
                    break;

                case PlayerState.Free:
                    // Check for collision with boulder

                    velocity = direction * speed * Time.deltaTime;
                    break;

                case PlayerState.Dodge:
                    // Dodge roll has ended. ONLY FOR TESTING
                    if (velocity == Vector2.zero)
                    {
                        state = PlayerState.Free;
                        speed = playerMovementData.FREE_SPEED;
                    }

                    // Reduce the velocity until it reaches (0,0).
                    speed -= playerMovementData.DODGE_DECAY;
                    velocity = dodgeDirection * speed * Time.deltaTime;
                    break;

                case PlayerState.Throwing:
                    // FOR TESTING JUST SWITCH BACK TO FREE
                    state = PlayerState.Free;
                    speed = playerMovementData.FREE_SPEED;
                    break;
            }

            UpdateEventObject();

            // Run parent's update for position and scale change
            base.Update();
        }

        private void UpdateEventObject()
        {
            playerEvent.Direction = direction;
            playerEvent.Velocity = velocity;
            playerEvent.Speed = speed;
            playerEvent.State = state;
            playerEvent.Position = position;
        }

        // Collision Methods
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Boulder"))
            {
                Debug.Log("Pickup");
                state = PlayerState.CarryingBounder;
                speed = playerMovementData.CARRY_SPEED;
                boulderEvent.PickUpBoulder();
            }

        }

        // Input methods
        public void OnMove(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (state != PlayerState.Dodge)
                {
                    if (state == PlayerState.CarryingBounder)
                    {
                        boulderEvent.DropBoulder();
                    }

                    state = PlayerState.Dodge;
                    speed = playerMovementData.DODGE_SPEED;
                    dodgeDirection = direction;
                    velocity = dodgeDirection * playerMovementData.DODGE_SPEED * Time.deltaTime;
                }
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (state == PlayerState.CarryingBounder)
                {
                    state = PlayerState.Throwing;
                    boulderEvent.Throw();
                }
            }
        }
    }
}
