using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;

namespace Ephymeral.BoulderNS
{
    public class BoulderPrediction : Entity
    {
        #region REFERENCES
        [SerializeField] private BoulderData boulderData;
        #endregion

        #region FIELDS
        [SerializeField] private const float OPACITY = 0.5f;
        private Color spriteColor;
        private bool visible;
        #endregion

        #region PROPERTIES
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public Vector2 Position
        { 
            get { return position; }
            set { position = value; }
        }

        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        #endregion

        protected override void Awake()
        {
            // Assign SpriteRenderer to manipulate opacity
            spriteColor = GetComponent<SpriteRenderer>().color;

            base.Awake();
        }

        // Runs 60 frames per second
        protected override void FixedUpdate()
        {
            // Only show prediction when throwing boulder
            spriteColor.a = (visible ? OPACITY : 0f);
            base.FixedUpdate();
        }

        /// <summary>
        /// Predict the future position of the boulder based on time
        /// </summary>
        /// <param name="time">Seconds into the future</param>
        public void PredictFuturePosition(float time)
        {
            velocity = boulderData.INITIAL_THROW_SPEED * direction;
            acceleration = boulderData.GRAVITY * Vector2.down;

            for(int a = 0; a < time * 60; a++)
            {
                velocity += acceleration * 1.0f / 60;
                position += velocity * 1.0f / 60;
                transform.position = position;
            }
        }
    }
}
