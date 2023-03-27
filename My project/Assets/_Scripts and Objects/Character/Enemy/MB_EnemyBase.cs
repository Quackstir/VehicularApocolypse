using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_EnemyBase : MB_CharacterBase
    {
        [SerializeField] Transform _T_Player;
        private bool b_OnPlayer;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _T_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_T_Player != null)
                V_Moving(_T_Player.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                b_OnPlayer = true;
                StartCoroutine(IE_DamagePlayer(collision.gameObject.GetComponent<MB_CharacterBase>()));
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                b_OnPlayer = false;
                StopCoroutine("IE_DamagePlayer");
            }
        }

        IEnumerator IE_DamagePlayer(MB_CharacterBase Player)
        {
            while (b_OnPlayer)
            {
                yield return new WaitForSeconds(1);
                Player.V_Damage(5);
                yield return null;
            }

        }

        public void SpawnCollectible(GameObject CollectibleSpawn)
        {
            Instantiate(CollectibleSpawn, transform.position, Quaternion.identity);
        }
    }
}
