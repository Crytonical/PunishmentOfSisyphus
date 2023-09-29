using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ephymeral.EntityNS
{
    public class Entity : MonoBehaviour
    {
        #region References
        // Type of collision should be handled by child
        protected Collider2D collision;
        #endregion

        #region Fields
        [SerializeField] private const float MIN_SCALE = 0.8f;
        private const float MAX_SCALE = 1.0f;

        [SerializeField] protected float speed, health;
        [SerializeField] protected Vector2 direction, velocity, position;
        [SerializeField] private Vector2 scale;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Initializes movement vectors
        /// </summary>
        protected void Awake()
        {
            // Create movement vectors
            direction = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            position = new Vector2(0, 0);

            scale = new Vector2(MAX_SCALE, MAX_SCALE);
        }

        // Update is called once per frame
        protected void Update()
        {
            // Update position, alter scale. Children should call parent
            position += velocity;
            UpdateScale();
        }

        /// <summary>
        /// Calculate the scale based on the position relative to the main camera.
        /// Scales to a minimum of 0.8f, and maximum of 1.0f. Values can be adjusted
        /// </summary>
        private void UpdateScale()
        {

        }
    }
}
