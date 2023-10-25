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
        if (collision.CompareTag("Enemy"))
        {
            Vector2 knockbackDir = collision.transform.position - transform.position;
            Vector2 knockback = knockbackDir * knockbackVal;

            // Trigger damage event on enemy (no damage, just knockback)
            collision.GetComponent<Enemy>().TakeDamage(0, knockback);
        }
    }

    public void ActivateHitbox()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        gameObject.SetActive(false);
    }
}
