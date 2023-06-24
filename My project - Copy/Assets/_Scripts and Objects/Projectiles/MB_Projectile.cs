using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_Projectile : MonoBehaviour
    {
        public bool isDestroy = true;
        public bool isMoving = true;
        public int DamageAMT = 5;

        private void FixedUpdate()
        {
            if (!isMoving) { return; }
            GetComponent<Rigidbody2D>().velocity = transform.up * 5;

            if (GetComponent<SpriteRenderer>().isVisible)
            {
                return;
            }

            if (!isDestroy) { return; }

            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<MB_CharacterBase>().V_Damage(DamageAMT);
            }
            if (!isDestroy) { return; }
            Destroy(this.gameObject);
        }
    }
}
