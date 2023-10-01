using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ephymeral.EntityNS;
using Ephymeral.BoulderNS;
using System.Threading;

// INPUTSYSTEM INS'T WORKING FOR SOME REASON DUE TO DIRECTORY

namespace Ephymeral.PlayerNS
{
    public enum PlayerState
    {
        CarryingBounder,
        Free,
        Dodge,
        Damage
    }

    public class Player : Entity
    {
        #region References
        private Boulder boulder;
        #endregion

        #region Fields
        // Movement constants
        [SerializeField] private const float CARRY_SPEED = 1.5f;
        [SerializeField] private const float FREE_SPEED = 5f;
        [SerializeField] private const float DODGE_SPEED = 20f;
        [SerializeField] private const float DODGE_DECAY = 1f;

        [SerializeField] private PlayerState state;

        // Only exists so that directions input during roll are registered
        // When it ends. Otherwise, you'll need to press the key again
        private Vector2 dodgeDirection;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Initializes fields
        /// </summary>
        private void Awake()
        {
            // Declare Entity variables
            speed = FREE_SPEED;
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
                        speed = FREE_SPEED;
                    }

                    // Reduce the velocity until it reaches (0,0).
                    speed -= DODGE_DECAY;
                    velocity = dodgeDirection * speed * Time.deltaTime;
                    break;
            }

            // Run parent's update for position and scale change
            base.Update();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if(state != PlayerState.Dodge)
            {
                state = PlayerState.Dodge;
                speed = DODGE_SPEED;
                dodgeDirection = direction;
                velocity = dodgeDirection * DODGE_SPEED * Time.deltaTime;
            }
        }
    }
}
