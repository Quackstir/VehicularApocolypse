using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal;

public enum InterpolType
{
    LerpPosition,
    LerpForward,
    Rigidbody2DMoveTowards,
    TransformMoveTowards
}

public enum CharacterType { Player, Enemy, Neutral }

namespace VehicleApocolypse
{
    public class MB_CharacterBase : MonoBehaviour
    {

        // protected Dictionary<string, ItemBase> ItemsDictionary = new Dictionary<string, ItemBase>();

        public Action<int> AddItemAction;

        //public Action<InputActionPhase> A_ShootAbility;
        //public CharacterStat CS_DamageBase = new CharacterStat(5, 4, true);

        //public CharacterStat CS_HealthVar = new CharacterStat(5, 4, true);
        [Header("General Variables")]
        public CharacterType CharacterType;
        public bool DebugOn;
        public bool InvincibilityOn;
        public float CarAngle;



        [Header("Health")]
        [SerializeField]
        private float _health_Maximum;
        public float Get_Health_Maximum
        {
            get { return _health_Maximum; }
        }

        [SerializeField]
        private float _health_Current;
        public float Get_Health_Curr
        {
            get { return _health_Current; }
        }
        //----------------
        public Action<float> Healing_Event;
        public Action<float> A_Damage;
        public Action<float, float> HealthUpdate_Event;
        public Action<float> A_Death;
        //----------------
        [SerializeField]
        private UnityEvent UE_OnDamaged;

        [SerializeField]
        private UnityEvent UE_OnDeath;
        //------------------------------//



        //------------MOVEMENT------------//
        [Header("Movement")]
        //public CharacterStat CS_MovementVar = new CharacterStat(5, 5, false);
        public float f_MoveSpeed;

        public InterpolType eIT_Interpolation;
        public float f_Acceleration;
        public Action<Vector2> A_Movement;
        protected Vector2 v2_InputMoveVector;
        private bool b_Move_Is = false;
        private bool b_Move_IsSelected = false;

        public Action<Vector3> A_Looking;

        bool b_DrivingSFX_isPlaying = false;
        public AudioSource AS_DrivingSFX;


        //------------------------------//


        //------------TURNING------------//
        [Header("Turning")]
        public float f_RotationSpeed;
        public float f_DriftAngle;

        public GameObject[] go_Tires;

        bool b_DriftingSFX_isPlaying = false;
        public AudioSource AS_DriftingSFX;
        public SpriteRenderer Sprite_Drift;
        public SpriteRenderer Sprite_Base;
        //------------------------------//


        //------------LEVEL------------//
        [Header("Level")]
        public int f_Experience_Curr;
        public int f_Experience_ToLevel = 5;

        public int f_Level_Curr;
        public Action A_ExpIncrease;
        public Action<int> A_LevelIncrease;
        public Action<int, int> A_ExperienceIncrease;

        //------------BOOST------------//
        //[Header("Boost")]
        //public float f_Boost_Speed;
        //public int i_Boost_Curr;
        //public int i_Boost_Max;
        //public int i_Boost_Charge;
        //public GameObject GO_DamageBox;
        //public ParticleSystem PS_BoostParticles;

        //public AudioSource AS_BoostingSFX;
        //------------------------------//


        #region RigidBody2D
        [SerializeField]
        protected Rigidbody2D _Rb2D;
        //public Rigidbody2D _rb2D { get { return _Rb2D; } }
        #endregion

        private PlayerInput _PI_Player;


        //---------------------------------//
        //------------FUNCTIONS------------//
        //---------------------------------//

        protected virtual void Start()
        {
            if (this.GetComponent<Rigidbody2D>() != null)
                _Rb2D = this.GetComponent<Rigidbody2D>();

            if (this.GetComponent<PlayerInput>() != null)
                _PI_Player = this.GetComponent<PlayerInput>();

            _health_Current = _health_Maximum;

            if (HealthUpdate_Event != null)
                HealthUpdate_Event(_health_Current, _health_Maximum);

            Sprite_Base.enabled = true;
            Sprite_Drift.enabled = false;
        }

        //----------LEVEL FUNCTIONS----------//
        public virtual void V_AddExperience(int AddExp)
        {
            float a = 0.03f;

            f_Experience_Curr++;


            if (A_ExpIncrease != null)
                A_ExpIncrease();

            //if (f_Experience_Curr >= f_Experience_ToLevel)
            //{
            //    f_Level_Curr++;
            //    f_Experience_Curr -= f_Experience_ToLevel;
            //    f_Experience_ToLevel += 10;

            //    V_LevelUp();
            //}

            //A_ExperienceIncrease(f_Experience_Curr, f_Experience_ToLevel);
        }

        protected virtual void V_LevelUp()
        {
            if (A_LevelIncrease != null)
                A_LevelIncrease(f_Level_Curr);

            // V_AddItem(lI_Item[0]);
        }

        //----------HEALTH FUNCTIONS----------//
        public virtual void V_Healing(float healthIncrease)
        {
            _health_Current += healthIncrease;

            if (_health_Current > _health_Maximum) _health_Current = _health_Maximum;

            if (Healing_Event != null) Healing_Event(healthIncrease);

            if (HealthUpdate_Event != null) HealthUpdate_Event(_health_Current, _health_Maximum);
        }

        public virtual void V_Damage(float Damage)
        {
            if (InvincibilityOn)
            {
                return;
            }

            _health_Current -= Damage;
            UE_OnDamaged.Invoke();
            if (A_Damage != null) A_Damage(Damage);

            if (HealthUpdate_Event != null) HealthUpdate_Event(_health_Current, _health_Maximum);

            if (_health_Current <= 0) V_Death();
        }

        protected virtual void V_Death()
        {
            UE_OnDeath.Invoke();

            Destroy(this.gameObject);
        }



        //----------MOVEMENT FUNCTIONS----------//
        //Mouse Movement
        public void V_Moving_Input(InputAction.CallbackContext context)
        {
            Vector2 InputVector = context.ReadValue<Vector2>();

            if (DebugOn)
            {
                Debug.DrawLine(transform.position, (Vector3)v2_InputMoveVector + transform.position, Color.red);
                Debug.Log("Movement Vector: " + context.phase + " | Input Vector: " + InputVector);
            }


            if (context.phase == InputActionPhase.Canceled)
            {
                V_Moving(Vector2.zero);
                return;
            }

            if (!b_DrivingSFX_isPlaying)
            {
                AS_DrivingSFX.Play();
                b_DrivingSFX_isPlaying = true;
            }

            if (_PI_Player.currentControlScheme == "Mouse")
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InputVector = Camera.main.ScreenToWorldPoint(InputVector);
                InputVector = InputVector - (Vector2)transform.position;
                InputVector = Vector2.ClampMagnitude(InputVector, 1.0f);
                InputVector.Normalize();
                if (!b_Move_IsSelected)
                {
                    return;
                }
                V_Moving(InputVector);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                InputVector = Vector2.ClampMagnitude(InputVector, 1.0f);
                V_Moving(InputVector);
            }

        }

        public void V_Select_Input(InputAction.CallbackContext context)
        {
            if (DebugOn)
                Debug.Log("Select Input: " + context.phase);

            if (context.phase == InputActionPhase.Started)
            {
                b_Move_IsSelected = true;
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                V_Moving(Vector2.zero);
                b_Move_IsSelected = false;

            }
        }

        protected virtual void V_Moving(Vector2 MoveVector)
        {
            v2_InputMoveVector = MoveVector;

            if (!b_Move_Is)
                StartCoroutine(IE_AccelerateMovement());
        }

        protected virtual void V_Looking(Vector3 LookVector)
        {
            if (A_Looking != null)
                A_Looking(LookVector);
        }


        IEnumerator IE_AccelerateMovement()
        {
            b_Move_Is = true;

            while (v2_InputMoveVector != Vector2.zero)
            {
                yield return new WaitForFixedUpdate();

                if (DebugOn)
                    Debug.DrawLine(transform.position, (Vector3)v2_InputMoveVector + transform.position);

                float oldRotation = transform.rotation.z;

                switch (eIT_Interpolation)
                {
                    case InterpolType.LerpPosition:
                        _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, v2_InputMoveVector * f_MoveSpeed, Time.deltaTime * f_Acceleration);
                        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, v2_InputMoveVector);
                        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * f_RotationSpeed);
                        CarAngle = Vector2.Angle(v2_InputMoveVector, _Rb2D.velocity);
                        break;

                    case InterpolType.LerpForward:
                        _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, transform.up * f_MoveSpeed * v2_InputMoveVector.magnitude, Time.deltaTime * f_Acceleration);
                        toRotation = Quaternion.LookRotation(Vector3.forward, v2_InputMoveVector);
                        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * f_RotationSpeed);
                        CarAngle = Vector2.Angle(v2_InputMoveVector, _Rb2D.velocity);
                        break;

                    case InterpolType.Rigidbody2DMoveTowards:
                        _Rb2D.velocity = Vector2.MoveTowards(_Rb2D.velocity, v2_InputMoveVector * f_MoveSpeed, Time.deltaTime * f_Acceleration);
                        break;

                    case InterpolType.TransformMoveTowards:
                        transform.position = Vector2.MoveTowards(this.transform.position, v2_InputMoveVector, Time.deltaTime * f_MoveSpeed);
                        break;

                    default:
                        _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, v2_InputMoveVector * f_MoveSpeed, Time.deltaTime * f_Acceleration);
                        break;
                }

                if (A_Movement != null)
                    A_Movement(v2_InputMoveVector);




                if (CarAngle > f_DriftAngle && v2_InputMoveVector != Vector2.zero)
                {
                    if (DebugOn)
                        Debug.Log("DRIFITNG");

                    Sprite_Drift.flipX = IsPlayerTurningLeft(oldRotation, transform.rotation.z);

                    V_Drifting();
                }
                else
                {
                    if (AS_DriftingSFX != null)
                    {
                        AS_DriftingSFX.Stop();
                        b_DriftingSFX_isPlaying = false;

                        Sprite_Base.enabled = true;
                        Sprite_Drift.enabled = false;
                    }

                    foreach (var item in go_Tires)
                    {
                        var PS = item.GetComponent<ParticleSystem>().emission;
                        PS.rateOverDistance = 0;
                    }
                }

                yield return null;
            }

            if (v2_InputMoveVector == Vector2.zero)
            {
                StartCoroutine(IE_SlowMovement());
                yield break;
            }
        }

        bool IsPlayerTurningLeft(float oldRotation, float newRotation)
        {
            return oldRotation - newRotation > 0 ? true : false;
        }


        void V_Drifting()
        {
            Sprite_Base.enabled = false;
            Sprite_Drift.enabled = true;

            if (!b_DriftingSFX_isPlaying && AS_DriftingSFX != null)
            {
                AS_DriftingSFX.Play();
                b_DriftingSFX_isPlaying = true;
            }

            //if (i_Boost_Curr < i_Boost_Max)
            //{
            //    i_Boost_Curr += i_Boost_Charge;
            //}

            foreach (var item in go_Tires)
            {
                var PS = item.GetComponent<ParticleSystem>().emission;
                PS.rateOverDistance = 10;
            }
        }

        IEnumerator IE_SlowMovement()
        {
            b_Move_Is = false;

            //var PS = PS_BoostParticles.emission;
            //PS.rateOverDistance = 10;

            AS_DrivingSFX.Stop();
            b_DrivingSFX_isPlaying = false;

            if (AS_DriftingSFX != null)
            {
                AS_DriftingSFX.Stop();
                b_DriftingSFX_isPlaying = false;

                Sprite_Base.enabled = true;
                Sprite_Drift.enabled = false;
            }

            foreach (var item in go_Tires)
            {
                var PS2 = item.GetComponent<ParticleSystem>().emission;
                PS2.rateOverDistance = 0;
            }

            //if (i_Boost_Curr > 0 && i_Boost_Curr > 50)
            //    AS_BoostingSFX.Play();

            //while (i_Boost_Curr > 0 && i_Boost_Curr > 50)
            //{
            //    GO_DamageBox.SetActive(true);
            //    yield return new WaitForFixedUpdate();
            //    _Rb2D.velocity = transform.up * f_Boost_Speed;
            //    i_Boost_Curr -= 4;
            //}

            //i_Boost_Curr = 0;

            //PS.rateOverDistance = 0;

            while (_Rb2D.velocity.magnitude > 0)
            {
                yield return new WaitForFixedUpdate();
                _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, Vector2.zero, Time.deltaTime * f_Acceleration);
                if (v2_InputMoveVector != Vector2.zero)
                {
                    //GO_DamageBox.SetActive(false);
                    //AS_BoostingSFX.Stop();
                    yield break;
                }
                //GO_DamageBox.SetActive(false);
                //AS_BoostingSFX.Stop();
                yield return null;
            }
        }
    }
}
