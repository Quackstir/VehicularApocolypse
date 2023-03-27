using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_Projectile : MonoBehaviour
    {

        private void FixedUpdate()
        {
            GetComponent<Rigidbody2D>().velocity = transform.up * 5;

            if (!GetComponent<SpriteRenderer>().isVisible)
            {
                Destroy(this.gameObject);
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<MB_CharacterBase>().V_Damage(5);
                Destroy(this.gameObject);
            }
        }
    }
}
