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
        public TMP_Text TempText_Health;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<MB_CharacterBase>();
            Player.A_LevelIncrease += UpdateLevelText;
            Player.A_ExperienceIncrease += UpdateExperienceText;
            Player.A_HealthUpdate += UpdateHealthText;
        }
        private void UpdateHealthText(float f_Health_Curr, float f_Health_Max)
        {
            TempText_Health.text = f_Health_Curr.ToString();
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
