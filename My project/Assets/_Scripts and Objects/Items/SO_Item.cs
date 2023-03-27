using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
    public class SO_Item : ScriptableObject
    {
        public string s_Name;
        public GameObject GO_ItemPrefab;

        public ItemType eIT_ItemType;

        public WeaponStats[] sWS_WeaponStats_LevelUps;
        public WeaponStats sWS_WeaponLevelStat_Initial;
    }
}
