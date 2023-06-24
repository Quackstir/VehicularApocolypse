using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class IB_Magnum : MB_ItemBase
    {
        public GameObject Projectile;
        List<Transform> Lt_AllEnemies = new List<Transform>();
        private AudioSource _AS;
        public AudioClip AC_Shoot;


        protected override void Start()
        {
            base.Start();
            _AS = GetComponent<AudioSource>();
        }

        protected override void V_WeaponActivate()
        {
            base.V_WeaponActivate();

            GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Lt_AllEnemies.Clear();

            foreach (var item in Enemies)
            {
                if (item.GetComponent<SpriteRenderer>().isVisible)
                {
                    Lt_AllEnemies.Add(item.transform);
                }
            }

            if (Lt_AllEnemies.Count == 0)
            {
                return;
            }


            Debug.Log(GetClosestEnemy(Lt_AllEnemies));

            if (GetClosestEnemy(Lt_AllEnemies) == null)
            {
                return;
            }

            Vector2 aim = GetClosestEnemy(Lt_AllEnemies).position - transform.position;
            float aimangle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg - 90f;

            GameObject a = Instantiate(Projectile, transform.position, Quaternion.identity);
            _AS.PlayOneShot(AC_Shoot);
            a.GetComponent<Rigidbody2D>().rotation = aimangle;
            transform.rotation = Quaternion.Euler(0f, 0f, aimangle);
            // _characterUnit.GetComponent<Rigidbody2D>().velocity += -(Vector2)transform.up * 10f;
        }



        Transform GetClosestEnemy(List<Transform> enemies)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (Transform t in enemies)
            {
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }
            return tMin;
        }
    }




    //    RaycastHit2D[] EnemiesInArea = Physics2D.CircleCastAll(transform.position, 50, Vector2.up);

    //            if (EnemiesInArea == null)
    //            {
    //                return;
    //            }
    //List<Transform> Lt_SurroundingEnemies = new List<Transform>();

    //foreach (var item in EnemiesInArea)
    //{
    //    if (item.collider.CompareTag("Enemy"))
    //        Lt_SurroundingEnemies.Add(item.transform);
    //}

    //Debug.Log(GetClosestEnemy(Lt_SurroundingEnemies));


    //Vector2 aim = GetClosestEnemy(Lt_SurroundingEnemies).position - transform.position;
    //float aimangle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg - 90f;

    //GameObject a = Instantiate(Projectile, transform.position, Quaternion.identity);
    //a.GetComponent<Rigidbody2D>().rotation = aimangle;

}
