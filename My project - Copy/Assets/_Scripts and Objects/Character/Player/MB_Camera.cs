using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_Camera : MonoBehaviour
    {
        public Transform _T_Player;
        public MB_CharacterBase _CB_Player;
        Camera _camera;
        private float InitalCamSize;
        private float CamSize;

        // Start is called before the first frame update
        void Start()
        {
            _camera = Camera.main;
            _T_Player = MB_GameManager.I_GameManager._PlayerRef.GetComponent<Transform>();
            _CB_Player = MB_GameManager.I_GameManager._PlayerRef.GetComponent<MB_CharacterBase>();
            //_CB_Player.A_ExpIncrease += IncreaseSize;
        }

        private void LateUpdate()
        {
            if (_T_Player != null)
                transform.position = new Vector3(_T_Player.position.x, _T_Player.position.y, -10);
        }
    }
}
