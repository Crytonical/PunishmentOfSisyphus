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
    [SerializeField] PlayerData playerData;
    List<Collider2D> collisions;
    Vector2 direction;

    private void Start()
    {
        collisions = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !collisions.Contains(collision))
        {
            //Vector2 knockbackDir = collision.transform.position - transform.position;
            Vector2 knockbackDir = direction;
            Vector2 knockback = knockbackDir * knockbackVal;

            // Trigger damage event on enemy (no damage, just knockback)
            collision.GetComponent<Enemy>().TakeDamage(playerData.SLAM_DAMAGE, knockback);
            collisions.Add(collision);
        }
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void ActivateHitbox(Vector2 dir)
    {
        gameObject.SetActive(true);
        direction = dir;
    }

    public void DeactivateHitbox()
    {
        collisions = new List<Collider2D>();
        gameObject.SetActive(false);
    }
}
