using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace VehicleApocolypse
{
    public class IB_Cannon : MB_ItemBase
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

            GameObject a = Instantiate(Projectile, transform.position, Quaternion.identity);
            _AS.PlayOneShot(AC_Shoot);
            float aimangle = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg - 90f;


            a.GetComponent<Rigidbody2D>().rotation = aimangle;
        }

    }
}
