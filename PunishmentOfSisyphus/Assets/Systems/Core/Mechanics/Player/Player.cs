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
using UnityEngine.SceneManagement;
//using System.Numerics;

// INPUTSYSTEM INS'T WORKING FOR SOME REASON DUE TO DIRECTORY

// NEED TO UPDATE BOULDER WHEN BOULDER DOES THINGS SINCE WE ARE CHANGING ASSEMBLY REFERENCE STUFF

namespace Ephymeral.PlayerNS
{
    public enum PlayerState
    {
        CarryingBounder,
        Free,
        Dodge,
        Damage,
        Throwing,
        Slam,
    }

    public class Player : Entity
    {
        #region References
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private PlayerData playerData;
        //[SerializeField] private SceneEvent sceneEvent;
        [SerializeField] private GameObject levelBounds;
        [SerializeField] private GameObject slamHitbox;
        private SpriteRenderer spriteRenderer;
        #endregion

        #region Fields

        [SerializeField] private PlayerState state;
        SlamScript slamScript;
        private GameObject enemyInfo;
        private int invincibilityFrames, currentActiveIFrames;
        private bool levelTransition;

        // Only exists so that directions input during roll are registered
        // When it ends. Otherwise, you'll need to press the key again
        private UnityEngine.Vector2 dodgeDirection;
        private float dodgeCooldown;
        #endregion


        #region Properties
        #endregion

        private void OnEnable()
        {
            // Add event listeners
            playerEvent.damageEvent.AddListener(TakeDamage);
        }

        private void OnDisable()
        {
            // remove event listeners
            playerEvent.damageEvent.RemoveListener(TakeDamage);
        }

        /// <summary>
        /// Initializes fields
        /// </summary>
        protected override void Awake()
        {
            // Declare Entity variables
            speed = playerData.FREE_SPEED;
            health = playerData.MAX_HP; // FOR TESTING, NOT FINAL

            // Just for testing. Should start with carrying boulder
            state = PlayerState.Free;
            //collision = getComponent(BoxCollider2D);

            // Gets spriterenderer
            spriteRenderer = GetComponent<SpriteRenderer>();

            slamScript = slamHitbox.GetComponent<SlamScript>();
            slamScript.DeactivateHitbox();

            //Unused
            //enemyInfo = GameObject.Find("EnemySpawner");

            invincibilityFrames = playerData.I_FRAMES;

            levelTransition = false;


            // Run parent's awake method
            base.Awake();
        }


        // Update is called once per frame
        protected override void Update()
        {
            if (!levelTransition)
            {
                //Death
                if (health <= 0)
                {
                    Die();
                }

                if (dodgeCooldown > 0)
                    dodgeCooldown -= Time.deltaTime;

                switch (state)
                {
                    case PlayerState.CarryingBounder:
                        // Always predict boulder position when carrying
                        if (!boulderEvent.UpdatePosition)
                        {
                            boulderEvent.Predict();
                            boulderEvent.UpdatePosition = true;
                        }

                        break;

                    case PlayerState.Dodge:
                        // Dodge roll has ended. ONLY FOR TESTING
                        if (speed <= 0)
                        {
                            state = PlayerState.Free;
                            speed = playerData.FREE_SPEED;
                        }

                        // Reduce the velocity until it reaches (0,0). Set to reduce so dodge lasts set amount of time
                        acceleration += (playerData.DODGE_SPEED / playerData.DODGE_DURATION) * (dodgeDirection * -1);

                        // Only used for the sake of determining when the roll is over. Checking with vectors is messier
                        speed -= (playerData.DODGE_SPEED / playerData.DODGE_DURATION) * Time.deltaTime;
                        break;

                    case PlayerState.Throwing:
                        // FOR TESTING JUST SWITCH BACK TO FREE
                        state = PlayerState.Free;
                        speed = playerData.FREE_SPEED;
                        break;
                }

                // No need for switch statements for Free or CarryingBoulder
                if (state != PlayerState.Dodge)
                    velocity = direction * speed;

                UpdateEventObject();

                //Keep in bounds
                if (!levelBounds.GetComponent<RectTransform>().rect.Contains(transform.position))
                {
                    Rect rect = levelBounds.GetComponent<RectTransform>().rect;
                    Vector2 vector = position;

                    // Clamp the x component to be within the rectangle's x boundaries
                    float clampedX = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);

                    // Clamp the y component to be within the rectangle's y boundaries
                    float clampedY = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);

                    // Return the new vector with clamped components
                    position = new Vector2(clampedX, clampedY);
                }
            }

            base.Update();
        }

        protected override void FixedUpdate()
        {
            if(!levelTransition)
            {
                if (currentActiveIFrames > 0)
                {
                    currentActiveIFrames--;
                    spriteRenderer.color = Color.yellow;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }

            base.FixedUpdate();
        }

        private void UpdateEventObject()
        {
            playerEvent.Direction = direction;
            playerEvent.Velocity = velocity;
            playerEvent.Speed = speed;
            playerEvent.Position = position;
            playerEvent.Health = health;
        }

        // Collision Methods
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check boulder collision
            if (collision.CompareTag("Boulder") && collision is CircleCollider2D)
            {
                state = PlayerState.CarryingBounder;
                speed = playerData.CARRY_SPEED;
                boulderEvent.PickUpBoulder();
            }

            if (collision.CompareTag("EnemyWeapon") || collision.CompareTag("Bullet"))
            {
                // Taking Damage
                playerEvent.TakeDamage(collision.GetComponentInParent<Enemy>().Damage);
            }

            //if (collision.CompareTag("Wall"))
            //{

            //}
        }

        private void TakeDamage(float damage)
        {
            if (currentActiveIFrames <= 0)
            {
                health -= damage;
                currentActiveIFrames = invincibilityFrames;
            }

            UpdateEventObject();
        }

        private void Die()
        {
            //sceneEvent.GameOver("GameOver");
            SceneManager.LoadScene("GameOver");
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
                if (state == PlayerState.Free && dodgeCooldown <= 0)
                {
                    state = PlayerState.Dodge;
                    speed = playerData.DODGE_SPEED;
                    dodgeDirection = direction;
                    velocity = dodgeDirection * playerData.DODGE_SPEED;
                    dodgeCooldown = playerData.DODGE_COOLDOWN;
                }
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            //if (state == PlayerState.CarryingBounder)
            //{
            //    //Show the dotted prediction line while mouse is held
            //    if (context.started)
            //    {
            //        boulderEvent.Predict();
            //        boulderEvent.UpdatePosition = true;
            //    }

            //    // Throw the boulder when mouse is released
            //    else if (context.canceled)
            //    {
            //        state = PlayerState.Throwing;
            //        boulderEvent.UpdatePosition = false;
            //        boulderEvent.Throw();
            //    }
            //}

            if (state == PlayerState.CarryingBounder)
            {
                if (context.started)
                {
                    state = PlayerState.Throwing;
                    boulderEvent.UpdatePosition = false;
                    boulderEvent.Throw();
                }
            }

            // Perform a slam attack when not holding the boulder
            else if (context.started && state != PlayerState.Dodge)
            {
                state = PlayerState.Slam;
                Vector2 lunchDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized;
                StartCoroutine(LunchCo(lunchDir));
            }

            //if (context.started)
            //{
            //    //Debug.Log("Throw processed");

            //    // Boulder throw
            //    if (state == PlayerState.CarryingBounder)
            //    {
            //        state = PlayerState.Throwing;
            //        boulderEvent.Throw();
            //    }

            //    // Slam attack
            //    else
            //    {
            //        state = PlayerState.Slam;
            //        Vector2 lunchDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized;
            //        StartCoroutine(LunchCo(lunchDir));

            //    }
            //}
        }

        IEnumerator LunchCo(Vector2 lunchDir)
        {
            float timer = 0f;

            float startValue = playerData.LUNGE_SPEED;
            float endValue = playerData.FREE_SPEED;

            float duration = playerData.SLAM_DURATION;

            //Debug.Log("Start Slam");

            while (timer < duration)
            {

                //Slam on frame 10
                if (timer == 3)
                {
                    slamScript.ActivateHitbox(((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized);
                }
                if (timer == duration - 3)
                {
                    slamScript.DeactivateHitbox();
                }

                timer += 1;
                yield return new WaitForFixedUpdate();
            }

            if (state == PlayerState.Slam)
            {
                state = PlayerState.Free;
            }


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

        public void resetPlayer()
        {
            this.position.y = -8.5f;
        }

        // Swap between enabled and disabled in one method. MIGHT BE BAD IDEA FOR IMPLEMENTATION
        public void ToggleLevelTransition()
        {
            //levelTransition = !levelTransition;
            position = new Vector3(0.0f, -10.0f, 0.0f);

            transform.position = position;

            // Pickup Boulder
            state = PlayerState.CarryingBounder;
            speed = playerData.CARRY_SPEED;
            boulderEvent.PickUpBoulder();

            // Refill health
            playerEvent.TakeDamage(health - playerData.MAX_HP);
        }
    }
}
