using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_EnemySpawnArea : MonoBehaviour
    {
        public int numObjects = 10;
        public GameObject prefab;
        public float Radius;
        public float f_TimeBetweenSpawn;

        void Start()
        {
            StartCoroutine(IE_WeaponFirerate());
        }


        IEnumerator IE_WeaponFirerate()
        {
            float f_NextShot = 0;

            while (true)
            {
                yield return new WaitForFixedUpdate();

                float a = Time.fixedTime - f_NextShot;

                //Debug.Log("Shoot Firerate:" + a);
                if (Time.fixedTime > f_NextShot)
                {
                    //Debug.Log("Shoot | Fixed Time:" + Time.fixedTime + " | Time Between Shots: " + f_TimeBetweenShots + " | a: " + a);
                    f_NextShot = Time.fixedTime + f_TimeBetweenSpawn;
                    SpawnEnemy();
                }
                yield return null;
            }
        }


        [ContextMenu("Spawn Enemies")]
        void SpawnEnemy()
        {
            Vector3 center = transform.position;
            for (int i = 0; i < numObjects; i++)
            {
                Vector3 pos = RandomCircle(center);
                Quaternion rot = Quaternion.identity;
                Instantiate(prefab, pos, rot);
            }
        }

        Vector3 RandomCircle(Vector3 center)
        {
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + Radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + Radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = 0;
            return pos;
        }
    }
}
