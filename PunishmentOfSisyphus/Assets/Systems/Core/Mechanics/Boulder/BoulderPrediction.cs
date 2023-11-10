using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ephymeral.Data;
using Ephymeral.Events;
using Ephymeral.EntityNS;

namespace Ephymeral.BoulderNS
{
    // Dot implementation taken from https://medium.com/@kunaltandon.kt/creating-a-dotted-line-in-unity-ca044d02c3e2
    public class BoulderPrediction : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private Sprite dot;
        [SerializeField] private BoulderData boulderData;
        [SerializeField] private BoulderEvent boulderEvent;
        [SerializeField] private GameObject levelBoundsGO; 
        #endregion

        #region FIELDS
        // PLACEHOLDER, until I can figure out what's going on with the scriptable object
        [SerializeField] private const float PREDICTION_DURATION = 1.5f;
        [SerializeField] private const float INTERVAL = 1.0f / 15; // PLACEHOLDER
        [SerializeField] private const float COLLIDER_DISTANCE = 1.0f;
        [SerializeField] private float dotSize;

        private bool prediction;
        private Vector2 direction, position, velocity, acceleration;

        private Bounds bounds;
        private Rect screenBounds;
        private GameObject[] objects;
        private float[] objectsColChecked;

        private List<Vector2> positions;
        private List<GameObject> dots;
        #endregion

        #region PROPERTIES
        #endregion

        /// <summary>
        /// Initialize the values used by this class
        /// </summary>
        private void Awake()
        {
            // Defaults to not predicting position
            prediction = false;
            acceleration = new Vector2();
            velocity = new Vector2();
            position = new Vector2();
            direction = new Vector2();
            positions = new List<Vector2>();
            dots = new List<GameObject>();

            bounds = GetComponent<Collider2D>().bounds;
            screenBounds = levelBoundsGO.GetComponent<RectTransform>().rect;
        }

        private void OnEnable()
        {
            boulderEvent.predictionEvent.AddListener(EnablePrediction);
        }

        private void OnDisable()
        {
            boulderEvent.predictionEvent.RemoveListener(EnablePrediction);
        }

        // Runs 60 frames per second
        private void FixedUpdate()
        {
            // Reduce stuff happening when not throwing the boulder
            if (boulderEvent.UpdatePosition)
            {
                DestroyAllDots();
                positions.Clear();
                position = boulderEvent.Position;
                direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - position).normalized;

                // Set up arrays for enemy collision (here due to inconsistency between boulder prediction and actual outcome)
                //objects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
                //objectsColChecked = new float[objects.Length];

                PredictFuturePath();
            }

            else if (dots.Count > 0)
                DestroyAllDots();
        }

        /// <summary>
        /// Enables the prediction of the boulder
        /// </summary>
        private void EnablePrediction()
        {
            prediction = true;
        }

        /// <summary>
        /// Predicts the future position of the boulder within a certain time frame
        /// </summary>
        private void PredictFuturePath()
        {
            float elapsedTime = 0.0f;
            bool rolling = false;

            velocity = boulderData.INITIAL_THROW_SPEED * direction;

            // Predict the path of the boulder a certain amount into the future
            while (elapsedTime < PREDICTION_DURATION)
            {
                if (rolling)
                {
                    if (Mathf.Sign(velocity.y) == -1 && velocity.y < -1 * boulderData.MAX_ROLL_SPEED)
                        velocity = new Vector2(velocity.x, boulderData.MAX_ROLL_SPEED * -1);

                    else
                        acceleration += boulderData.GRAVITY * Vector2.down;
                }

                if (!rolling && elapsedTime >= boulderData.AIR_TIME)
                    rolling = true;

                //for(int a = 0; a < objects.Length; a++)
                //{
                //    if (objects[a].tag == "Enemy")
                //    {
                //        if (objectsColChecked[a] > 0.0f)
                //            objectsColChecked[a] -= INTERVAL;

                //        // Handle ricochet. Second conditional is to prevent constant checking of enemy
                //        if (bounds.Intersects(objects[a].GetComponent<Collider2D>().bounds) && objectsColChecked[a] <= 0.0f)
                //        {
                //            Vector2 bounceDirection = new Vector2(0, boulderData.INITIAL_RICOCHET_SPEED);

                //            float dotProduct = Mathf.Abs(Vector2.Dot(Vector2.up, velocity));

                //            bounceDirection += new Vector2(dotProduct * boulderData.BOUNCE_COEFFICIENT * Mathf.Sign(velocity.x), 0);

                //            velocity = bounceDirection;

                //            objectsColChecked[a] += INTERVAL * 2;
                //        }
                //    }
                //}

                //Handle collision checks for enemies
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.tag == "Enemy")
                    {
                        // Handle ricochet
                        if (bounds.Intersects(go.GetComponent<Collider2D>().bounds))
                        {
                            Vector2 bounceDirection = new Vector2(0, boulderData.INITIAL_RICOCHET_SPEED);

                            float dotProduct = Mathf.Abs(Vector2.Dot(Vector2.up, velocity));

                            bounceDirection += new Vector2(dotProduct * boulderData.BOUNCE_COEFFICIENT * Mathf.Sign(velocity.x), 0);

                            velocity = bounceDirection;
                            rolling = true;
                        }
                    }
                }

                // Handle collision check for wall
                if (position.x > screenBounds.xMax || position.x < screenBounds.xMin)
                {
                    position = new Vector2(Mathf.Clamp(position.x, screenBounds.xMin, screenBounds.xMax), position.y);
                    direction *= -1;
                    velocity.x *= -1;
                }


                velocity += acceleration * INTERVAL;
                position += velocity * INTERVAL;
                transform.position = position;
                bounds.center = position;
                elapsedTime += INTERVAL;
                positions.Add(position);

                acceleration = Vector2.zero;
            }

            Render();
        }

        /// <summary>
        /// Generates one of the dots for the dotted prediction line
        /// </summary>
        /// <returns>A single dot</returns>
        private GameObject GetOneDot()
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.localScale = Vector3.one * dotSize;
            gameObject.transform.parent = transform;

            SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = dot;
            return gameObject;
        }

        /// <summary>
        /// Deletes all dots
        /// </summary>
        private void DestroyAllDots()
        {
            foreach (GameObject dot in dots)
            {
                Destroy(dot);
            }

            dots.Clear();
        }

        /// <summary>
        /// Creates each dot for every position predicted
        /// </summary>
        private void Render()
        {
            foreach (Vector2 position in positions)
            {
                GameObject g = GetOneDot();
                g.transform.position = position;
                dots.Add(g);
            }
        }
    }
}
