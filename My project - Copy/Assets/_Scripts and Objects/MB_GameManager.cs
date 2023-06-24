using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VehicleApocolypse
{
    public class MB_GameManager : MonoBehaviour
    {
        public static MB_GameManager I_GameManager { get; private set; }

        public GameObject PlayerVehicle;
        MB_CharacterBase Player;
        public MB_CharacterBase _PlayerRef { get { return Player; } }
        public GameObject ItemSelectMenu;

        public MB_Button_ItemSelect[] ItemSelects;

        [SerializeField]
        private List<SO_Item> ItemsList = new List<SO_Item>();

        public bool b_isNight;
        public List<GameObject> GO_Lights = new List<GameObject>();


        private void Awake()
        {
            if (I_GameManager != null && I_GameManager != this)
            {
                Destroy(this);
            }
            else
            {
                I_GameManager = this;
            }

            GameObject newPlayer = Instantiate(PlayerVehicle);

            Player = newPlayer.GetComponent<MB_CharacterBase>();
            ItemSelectMenu.SetActive(false);
            Player.A_LevelIncrease += V_ItemSelect;

            foreach (GameObject light in GameObject.FindGameObjectsWithTag("Light"))
            {
                GO_Lights.Add(light);
            }

            if (!b_isNight)
            {
                foreach (var item in GO_Lights)
                {
                    item.SetActive(false);
                }
            }
        }

        public void V_ItemSelect(int PlayerLevel)
        {
            Time.timeScale = 0;

            ItemSelectMenu.SetActive(true);
            for (int i = 0; i < ItemSelects.Length; i++)
            {
                ItemSelects[i].I_Item = ItemsList[Random.Range(0, ItemsList.Count)];
            }
        }
    }
}
