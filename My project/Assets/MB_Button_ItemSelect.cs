using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VehicleApocolypse
{
    public class MB_Button_ItemSelect : MonoBehaviour
    {
        MB_CharacterBase Player;
        public SO_Item I_Item;
        public TMP_Text _TextMeshPro;


        private void OnEnable()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<MB_CharacterBase>();
            _TextMeshPro = GetComponentInChildren<TMP_Text>();
            _TextMeshPro.text = I_Item.s_Name;
        }


        public void V_GiveItem()
        {
            Player.V_AddItem(I_Item);
            Time.timeScale = 1;
        }
    }
}
