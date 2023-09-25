using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ephymeral.Data;
using Ephymeral.Events;

namespace Ephymeral.Boulder
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
        private float velocityIncrease;
        private double damage;

        // Checks
        private bool isHeld;
        private bool isThrown;
        private bool isRolling;
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
            velocityIncrease = boulderData.velocityIncrease;
            isHeld = false;
            isThrown = false;
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
            if (!isHeld && !isThrown && velocity.y <= boulderData.maxVelocity.y)
            {
                RB.velocity = new Vector2(RB.velocity.x, -velocity.y);

                // Increase Velocity by 2% each frame it is rolling

                velocity.y *= 1.0f + velocityIncrease;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                // Trigger pickup event on player
                // playerEvent.PickupBoulder
                PickedUp();
            }

            if (collision.collider.CompareTag("Enemies"))
            {
                // Trigger damage event on enemy
                //collision.collider.Enemy.dealDamage(damage);
            }
        }

        private void ThrowBoulder()
        {
            isThrown = true;
        }

        private void DropBoulder()
        {
            isHeld = false;
        }

        private void PickedUp()
        {
            isHeld = true;
        }
    }
}
