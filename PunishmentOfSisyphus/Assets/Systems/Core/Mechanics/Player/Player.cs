using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;

using Ephymeral.EntityNS;
using Ephymeral.BoulderNS;
using Ephymeral.Events;
using Ephymeral.Data;
using Ephymeral.EnemyNS;

// INPUTSYSTEM INS'T WORKING FOR SOME REASON DUE TO DIRECTORY

namespace Ephymeral.PlayerNS
{
    public enum PlayerState
    {
        CarryingBounder,
        Free,
        Dodge,
        Damage,
        Throwing,
        Lunging
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
        protected void Awake()
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
        protected void Update()
        {
            if (health <= 0)
            {
                Debug.Log("Player has died");
                Die();
            }

            switch (state)
            {
                case PlayerState.Dodge:
                    // Dodge roll has ended. ONLY FOR TESTING
                    if (speed <= 0)
                    {
                        state = PlayerState.Free;
                        speed = playerMovementData.FREE_SPEED;
                    }

                    // Reduce the velocity until it reaches (0,0). Set to reduce so dodge lasts set amount of time
                    acceleration += (playerMovementData.DODGE_SPEED / playerMovementData.DODGE_DURATION) * (dodgeDirection * -1);

                    // Only used for the sake of determining when the roll is over. Checking with vectors is messier
                    speed -= (playerMovementData.DODGE_SPEED / playerMovementData.DODGE_DURATION) * Time.deltaTime;
                    break;

                case PlayerState.Throwing:
                    // FOR TESTING JUST SWITCH BACK TO FREE
                    state = PlayerState.Free;
                    speed = playerMovementData.FREE_SPEED;
                    break;
            }

            // No need for switch statements for Free or CarryingBoulder
            if (state != PlayerState.Dodge)
                velocity = direction * speed;

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
                state = PlayerState.CarryingBounder;
                speed = playerMovementData.CARRY_SPEED;
                boulderEvent.PickUpBoulder();
            }

            if (collision.CompareTag("EnemyWeapon"))
            {
                // Taking Damage
                TakeDamage(collision.GetComponentInParent<Enemy>().Damage);
            }
        }

        private void TakeDamage(float damage)
        {
            health -= damage;
        }

        private void Die()
        {
            enabled = false;
            Destroy(gameObject);
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
                    // Don't have them dodge when firing the boulder. JUST FOR NOW
                    if (state == PlayerState.CarryingBounder)
                    {
                        boulderEvent.DropBoulder();
                        speed = playerMovementData.FREE_SPEED;
                        state = PlayerState.Free;
                    }

                    else
                    {
                        state = PlayerState.Dodge;
                        speed = playerMovementData.DODGE_SPEED;
                        dodgeDirection = direction;
                        velocity = dodgeDirection * playerMovementData.DODGE_SPEED;
                    }
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
                else
                {
                    state = PlayerState.Lunging;
                    Vector2 lunchDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized;
                    StartCoroutine(LunchCo(lunchDir));

                }
            }
        }

        IEnumerator LunchCo(Vector2 lunchDir)
        {
            float timer = 0f;

            float startValue = playerMovementData.LUNGE_SPEED;
            float endValue = playerMovementData.FREE_SPEED;

            float duration = playerMovementData.LUNGE_DURATION;

            float prog; //Easing Vars
            float t;

            while (timer < duration)
            {
                if (state == PlayerState.Lunging) //Cancel if the lunch is interupted.
                {
                    t = timer / duration;
                    prog = t*t;

                    float currentValue = Mathf.Lerp(startValue, endValue, prog);

                    velocity = lunchDir * currentValue * Time.deltaTime;

                    timer += Time.deltaTime;
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            if(timer >= duration)
                state = PlayerState.Free;

        }

        // Cubic Bezier easing function, courtesy of stack overflow
        float CubicBezier(float t, float p0, float p1, float p2, float p3)
        {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float p = uuu * p0; // (1-t)^3 * P0
            p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * P1
            p += 3 * u * tt * p2; // 3 * (1-t) * t^2 * P2
            p += ttt * p3; // t^3 * P3

            return p;
        }
    }
}
