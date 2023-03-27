using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VehicleApocolypse
{
    public class MB_UI_LevelText : MonoBehaviour
    {
        MB_CharacterBase Player;

        public TMP_Text TempText_Level;
        public TMP_Text TempText_Experience;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<MB_CharacterBase>();
            Player.A_LevelIncrease += UpdateLevelText;
            Player.A_ExperienceIncrease += UpdateExperienceText;
        }

        private void UpdateLevelText(int Level)
        {
            TempText_Level.text = Level.ToString();
        }

        private void UpdateExperienceText(int ExperienceCurr, int ExperienceToLevel)
        {
            TempText_Experience.text = ExperienceCurr.ToString() + " / " + ExperienceToLevel.ToString();
        }
    }
}
