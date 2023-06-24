using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_CollectibleBase : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<MB_CharacterBase>() == null)
            {
                return;
            }

            if (collision.GetComponent<MB_CharacterBase>().CharacterType == CharacterType.Player)
            {
                collision.GetComponent<MB_CharacterBase>().V_AddExperience(5);
                Destroy(this.gameObject);
            }
        }
    }
}
