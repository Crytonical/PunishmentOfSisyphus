using Codice.CM.Common;
using Ephymeral.BoulderNS;
using Ephymeral.Data;
using Ephymeral.EnemyNS;
using Ephymeral.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamScript : MonoBehaviour
{
    [SerializeField] float knockbackVal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Changed "&& state == BoulderState.Thrown" so that will always hit when not held by the player
        if (collision.CompareTag("Enemy"))
        {
            Vector2 knockbackDir = collision.transform.position - transform.position;
            Vector2 knockback = knockbackDir * knockbackVal;
            // Trigger damage event on enemy
            collision.GetComponent<Enemy>().TakeDamage(0, knockback);

        }

        if (collision.CompareTag("Wall"))
        {
            direction *= -1;
            velocity.x = velocity.x * -1;
        }

        if (collision.CompareTag("ScreenBounds"))
        {
            boulderEvent.BoulderFail();
        }
    }
}
