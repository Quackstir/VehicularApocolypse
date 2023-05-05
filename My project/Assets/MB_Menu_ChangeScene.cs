using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VehicleApocolypse
{
    public class MB_Menu_ChangeScene : MonoBehaviour
    {

        public string Level;
        public void V_LoadLevel()
        {
            SceneManager.LoadScene(Level);
        }
    }
}
