using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace VehicleApocolypse
{
    public class MB_UI_LevelText : MonoBehaviour
    {
        MB_CharacterBase Player;

        public TMP_Text TempText_Level;
        public TMP_Text TempText_Health;
        public Image TempImage_ExperienceBar;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<MB_CharacterBase>();
            Player.A_LevelIncrease += UpdateLevelText;
            Player.A_ExperienceIncrease += UpdateExperienceText;
            Player.HealthUpdate_Event += UpdateHealthText;
        }
        private void UpdateHealthText(float Health_Current, float Health_Maximum)
        {
            TempText_Health.text = Health_Current.ToString();
        }


        private void UpdateLevelText(int Level)
        {
            TempText_Level.text = Level.ToString();
        }

        private void UpdateExperienceText(int ExperienceCurr, int ExperienceToLevel)
        {
            TempImage_ExperienceBar.fillAmount = (float)ExperienceCurr / (float)ExperienceToLevel;
        }
    }
}
