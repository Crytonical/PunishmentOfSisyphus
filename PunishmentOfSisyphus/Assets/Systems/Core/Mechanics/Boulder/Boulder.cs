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
        [SerializeField] GameObject levelBounds;
        #endregion

        #region FIELDS
        // Inhereted data
        private Vector2 initialVelocity;
        private float elapsedTime;  // Currently unused
        private float ricochetTime;
        private float damage;
        private float comboCount;   // Increases damage as the combo continues
        private Rect bounds;

        // State
        [SerializeField] private BoulderState state;
        #endregion

        #region PROPERTIES
        private float Speed
        {
            get { return (velocity.magnitude); }
        }

        #endregion

        protected override void Awake()
        {
            // Run base Awake function
            base.Awake();

            // Assign References
            hitbox = GetComponent<CircleCollider2D>();

            // Initialize Variables
            damage = boulderData.DAMAGE;

            // Default State
            state = BoulderState.Held;

            bounds = levelBounds.GetComponent<RectTransform>().rect;
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
        protected override void Update()
        {
            // State machine
            switch (state)
            {
                // Held
                case BoulderState.Held:
                    position = playerEvent.Position;

                    if (boulderEvent.UpdatePosition)
                        boulderEvent.Position = position;
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

            if (state != BoulderState.Held)
            {
                //Kill the player if the boulder falls out of the screen
                //if (transform.position.y < bounds.yMin)
                //{
                //    playerEvent.TakeDamage(100);
                //}

                //Bounce against the wall
                if (transform.position.x > bounds.xMax || transform.position.x < bounds.xMin)
                {
                    position = new Vector2( Mathf.Clamp(position.x, bounds.xMin, bounds.xMax), position.y);
                    direction *= -1;
                    velocity.x = velocity.x * -1;
                    state = BoulderState.Rolling;
                    
                }
            }

            //base.Update();
        }

        protected override void FixedUpdate()
        {
            switch (state)
            {
                case BoulderState.Thrown:
                    elapsedTime += Time.deltaTime;

                    if(elapsedTime >= boulderData.AIR_TIME)
                    {
                        state = BoulderState.Rolling;
                        elapsedTime = 0;
                    }

                    break;

                case BoulderState.Rolling:
                    // Apply gravity while rolling
                    if (Mathf.Sign(velocity.y) == -1 && velocity.y < -1 * boulderData.MAX_ROLL_SPEED) 
                    {
                        velocity = new Vector2( velocity.x, boulderData.MAX_ROLL_SPEED * -1) ;
                    }
                    else
                    {
                        //velocity += boulderData.GRAVITY * Vector2.down;
                        acceleration += boulderData.GRAVITY * Vector2.down;
                    }

                    //TODO: Might feel better to have gravity ramp up a little bit over time to increase "hang time"

                    break;

                case BoulderState.Held:
                    //acceleration += Vector2.down * boulderData.GRAVITY;
                    break;
            }

            base.FixedUpdate();

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Changed "&& state == BoulderState.Thrown" so that will always hit when not held by the player
            if (collision.CompareTag("Enemy") && state != BoulderState.Held && Speed >= boulderData.HIT_SPEED_MIN)
            {
                // Trigger damage event on enemy
                collision.GetComponent<Enemy>().BoulderHit(damage, velocity.normalized*boulderData.KNOCKBACK);

                // Call ricochet function
                Ricochet(collision);
            }

            if (collision.CompareTag("Wall"))
            {
                Debug.Log("Wall collision with boulder");
                direction *= -1;
                //velocity.x = velocity.x * -1;
            }

            if (collision.CompareTag("ScreenBounds"))
            {
                playerEvent.TakeDamage(100);
            }
        }

        private void ThrowBoulder()
        {
            state = BoulderState.Thrown;
            direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - playerEvent.Position).normalized;
            velocity = boulderData.INITIAL_THROW_SPEED * direction;
            UpdatePhysicsValues();
        }

        private void DropBoulder()
        {
            state = BoulderState.Rolling;
            //direction = Vector2.down;
            //velocity = Vector2.zero;
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
            //state = BoulderState.Ricocheting;
            state = BoulderState.Rolling;
            Vector2 bounceDirection = new Vector2(0, boulderData.INITIAL_RICOCHET_SPEED);

            float dotProduct = Mathf.Abs(Vector2.Dot(Vector2.up, velocity) );

            bounceDirection += new Vector2(dotProduct * boulderData.BOUNCE_COEFFICIENT * Mathf.Sign(velocity.x), 0);

            velocity = bounceDirection;
             
            UpdatePhysicsValues();
        }

        private void UpdateEventObject()
        {
            //boulderEvent.State = state;
        }

        private void UpdatePhysicsValues()
        {
            elapsedTime = 0;
        }
    }
}
