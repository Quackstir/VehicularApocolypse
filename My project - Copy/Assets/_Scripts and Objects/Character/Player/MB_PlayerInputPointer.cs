using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{
    public class MB_PlayerInputPointer : MonoBehaviour
    {
        private MB_CharacterBase CB_Player;

        private void OnEnable()
        {
            CB_Player = GetComponentInParent<MB_CharacterBase>();
            CB_Player.A_Movement += V_ChangePosition;
        }

        private void V_ChangePosition(Vector2 Input)
        {
            transform.position = (Input * 2) + (Vector2)CB_Player.transform.position;


            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, Input);
            transform.rotation = toRotation;
        }
    }
}
