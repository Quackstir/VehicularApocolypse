using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_Camera : MonoBehaviour
    {
        public Transform _T_Player;

        // Start is called before the first frame update
        void Start()
        {
            _T_Player = MB_GameManager.I_GameManager._PlayerRef.GetComponent<Transform>();
        }

        private void LateUpdate()
        {
            if (_T_Player != null)
                transform.position = new Vector3(_T_Player.position.x, _T_Player.position.y, -10);
        }
    }
}
