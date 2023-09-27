using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ephymeral.Entity
{
    public class Entity : MonoBehaviour
    {
        #region References
        [SerializeField] private Collider2D collisionBox;
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Update position, alter scale
        }
    }
}
