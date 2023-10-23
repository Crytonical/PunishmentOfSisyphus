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
        private static GameObject levelBounds;
        #endregion

        #region Fields
        [SerializeField] private float BASE_SCALE = 1.0f;
        [SerializeField] private float SCALE_DEVIATION = 0.1f;
        [SerializeField] private const float SMELLY = 12f;

        [SerializeField] protected float speed, health;
        [SerializeField] protected Vector2 direction, acceleration, velocity, position;
        private Vector3 scale;
        private float scalar; 

        private static Rect bounds;
        #endregion

        #region Properties
        #endregion

        /// <summary>
        /// Initializes movement vectors
        /// </summary>
        protected virtual void Awake()
        {
            // Create movement vectors
            direction = new Vector2(0, 0);
            acceleration = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            position = new Vector2(0, 0);

            scale = new Vector3(1.0f, 1.0f, 1.0f);

            // Get height of bounding box to manipulate scale
            levelBounds = GameObject.Find("LevelBounds");
            bounds = levelBounds.GetComponent<RectTransform>().rect;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // Update position, alter scale. Children should call parent
            velocity += acceleration * Time.deltaTime;
            position += velocity * Time.deltaTime;
            transform.position = position;
            UpdateScale();

            acceleration = Vector2.zero;
        }


        protected virtual void FixedUpdate()
        {
            // Update position, alter scale. Children should call parent
            velocity += acceleration * Time.deltaTime;
            position += velocity * Time.deltaTime;
            transform.position = position;
            UpdateScale();

            acceleration = Vector2.zero;
        }

        /// <summary>
        /// Calculate the scale based on the position relative to the main camera.
        /// Scales to a minimum of 0.8f, and maximum of 1.0f. Values can be adjusted
        /// </summary>
        protected virtual void UpdateScale()
        {
            scalar = ((position.y * 2) / bounds.height) * SCALE_DEVIATION;

            scale.x = BASE_SCALE + scalar;
            scale.y = BASE_SCALE + scalar;
            transform.localScale = scale;
        }
    }
}
