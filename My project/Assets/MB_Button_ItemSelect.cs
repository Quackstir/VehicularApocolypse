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
            //Player = MB_GameManager.I_GameManager._PlayerRef;
            _TextMeshPro = GetComponentInChildren<TMP_Text>();
            _TextMeshPro.text = I_Item.s_Name;
        }

        public void V_GiveItem()
        {
            MB_GameManager.I_GameManager._PlayerRef.V_AddItem(I_Item);
            Time.timeScale = 1;
        }
    }
}
