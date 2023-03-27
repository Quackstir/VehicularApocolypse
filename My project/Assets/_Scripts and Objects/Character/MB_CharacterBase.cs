using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;


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
        public CharacterType eCT_CharacterType;
        public bool b_DebugOn;
        public bool b_InvincibilityOn;

        //------------HEALTH------------//
        [Header("Health")]
        [SerializeField]
        private float f_Health_Max;
        public float Ff_Health_Max
        {
            get { return f_Health_Max; }
        }
        //----------------
        [SerializeField]
        private float f_Health_Curr;
        public float Ff_Health_Curr
        {
            get { return f_Health_Curr; }
        }
        //----------------
        public Action<float> A_Heal;
        public Action<float> A_Damage;
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
        //------------------------------//


        //------------TURNING------------//
        [Header("Turning")]
        public float f_RotationSpeed;
        public float f_DriftAngle;

        public GameObject[] go_Tires;


        //------------------------------//


        //------------LEVEL------------//
        [Header("Level")]
        public int f_Experience_Curr;
        public int f_Experience_ToLevel = 5;

        public int f_Level_Curr;
        public Action<int> A_LevelIncrease;
        public Action<int, int> A_ExperienceIncrease;

        [SerializeField]
        protected List<SO_Item> lI_Item = new List<SO_Item>();
        protected Dictionary<string, MB_ItemBase> D_ItemAcquired = new Dictionary<string, MB_ItemBase>();
        protected Dictionary<string, MB_ItemBase> D_ItemWeapons = new Dictionary<string, MB_ItemBase>();
        protected Dictionary<string, MB_ItemBase> D_ItemPassives = new Dictionary<string, MB_ItemBase>();

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

            f_Health_Curr = f_Health_Max;
        }

        //----------LEVEL FUNCTIONS----------//
        public virtual void V_AddExperience(int AddExp)
        {
            f_Experience_Curr += AddExp;

            if (f_Experience_Curr >= f_Experience_ToLevel)
            {
                f_Level_Curr++;
                f_Experience_Curr -= f_Experience_ToLevel;
                f_Experience_ToLevel += 10;

                V_LevelUp();
            }

            A_ExperienceIncrease(f_Experience_Curr, f_Experience_ToLevel);
        }

        protected virtual void V_LevelUp()
        {
            if (A_LevelIncrease != null)
                A_LevelIncrease(f_Level_Curr);

            // V_AddItem(lI_Item[0]);
        }

        public void V_AddItem(SO_Item newItem)
        {
            if (D_ItemWeapons.ContainsKey(newItem.name))
            {
                D_ItemWeapons[newItem.name].V_UpgradeItem();
                return;
            }

            GameObject GO_newItem = Instantiate(lI_Item[0].GO_ItemPrefab, transform.position, Quaternion.identity);
            GO_newItem.transform.parent = this.transform;

            GO_newItem.GetComponent<MB_ItemBase>().GetCharacterUnit(this);
            D_ItemAcquired.Add(newItem.name, GO_newItem.GetComponent<MB_ItemBase>());

            switch (newItem.eIT_ItemType)
            {
                case ItemType.Weapon:
                    D_ItemWeapons.Add(newItem.name, GO_newItem.GetComponent<MB_ItemBase>());
                    break;
                case ItemType.Support:
                    D_ItemPassives.Add(newItem.name, GO_newItem.GetComponent<MB_ItemBase>());
                    break;
                default:
                    break;
            }
        }

        //----------ITEM FUNCTIONS----------//
        //public void AddItem(int AmtAdd, ItemScriptableObject Item)
        //{
        //    if (ItemsDictionary.ContainsKey(Item.Name))
        //    {
        //        ItemsDictionary[Item.Name].IncreaseStackAmt(AmtAdd);
        //    }
        //    else
        //    {
        //        GameObject newItem = Instantiate(Item.ItemPrefab);
        //        newItem.transform.parent = this.transform;
        //        ItemsDictionary.Add(Item.Name, newItem.GetComponent<ItemBase>());
        //        newItem.GetComponent<ItemBase>().GetCharacterUnit(this);
        //    }

        //    if (ItemRarityAmountDictionary.ContainsKey(Item.itemRarity))
        //    {
        //        ItemRarityAmountDictionary[item.itemRarity] += AmtAdd;
        //    }
        //    else
        //    {
        //        ItemRarityAmountDictionary.Add(item.itemRarity, AmtAdd);
        //    }

        //    if (AddItemAction != null)
        //        AddItemAction(AmtAdd);

        //    UpdateVariables();
        //}


        //----------HEALTH FUNCTIONS----------//
        public virtual void V_Healing(float Heal)
        {
            f_Health_Curr += Heal;

            if (f_Health_Curr > f_Health_Max)
            {
                f_Health_Curr = f_Health_Max;
            }
        }

        public virtual void V_Damage(float Damage)
        {
            if (b_InvincibilityOn)
            {
                return;
            }

            f_Health_Curr -= Damage;

            if (A_Damage != null)
                A_Damage(Damage);

            if (f_Health_Curr <= 0)
                V_Death();

            UE_OnDamaged.Invoke();
        }

        protected virtual void V_Death()
        {
            UE_OnDeath.Invoke();

            Destroy(this.gameObject);

            //if (itemcrap != null)
            //    itemcrap.text = getItemRarityAMT(Rarity.Common).ToString();
        }



        //----------MOVEMENT FUNCTIONS----------//
        //Mouse Movement
        public void V_Moving_Input(InputAction.CallbackContext context)
        {
            Vector2 InputVector = context.ReadValue<Vector2>();

            if (b_DebugOn)
            {
                Debug.DrawLine(transform.position, (Vector3)v2_InputMoveVector + transform.position, Color.red);
                Debug.Log("Movement Vector: " + context.phase + " | Input Vector: " + InputVector);
            }


            if (context.phase == InputActionPhase.Canceled)
            {
                V_Moving(Vector2.zero);
                return;
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
            if (b_DebugOn)
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

                if (b_DebugOn)
                    Debug.DrawLine(transform.position, (Vector3)v2_InputMoveVector + transform.position);

                switch (eIT_Interpolation)
                {
                    case InterpolType.LerpPosition:
                        _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, v2_InputMoveVector * f_MoveSpeed, Time.deltaTime * f_Acceleration);
                        Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, v2_InputMoveVector);
                        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * f_RotationSpeed);
                        float carAngle = Vector2.Angle(v2_InputMoveVector, _Rb2D.velocity);
                        break;

                    case InterpolType.LerpForward:
                        _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, transform.up * f_MoveSpeed * v2_InputMoveVector.magnitude, Time.deltaTime * f_Acceleration);
                        toRotation = Quaternion.LookRotation(Vector3.forward, v2_InputMoveVector);
                        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * f_RotationSpeed);
                        carAngle = Vector2.Angle(v2_InputMoveVector, _Rb2D.velocity);
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

                //if (carAngle > f_DriftAngle)
                //{
                //    Debug.Log("DRIFITNG");
                //    V_Drifting();
                //}
                //else
                //{
                //    foreach (var item in go_Tires)
                //    {
                //        item.SetActive(false);
                //    }
                //}

                yield return null;
            }

            if (v2_InputMoveVector == Vector2.zero)
            {
                StartCoroutine(IE_SlowMovement());
                yield break;
            }
        }

        void V_Drifting()
        {
            foreach (var item in go_Tires)
            {
                item.SetActive(true);
            }
        }

        IEnumerator IE_SlowMovement()
        {
            b_Move_Is = false;

            while (_Rb2D.velocity.magnitude > 0)
            {
                yield return new WaitForFixedUpdate();
                _Rb2D.velocity = Vector2.Lerp(_Rb2D.velocity, Vector2.zero, Time.deltaTime * f_Acceleration);
                if (v2_InputMoveVector != Vector2.zero)
                {
                    yield break;
                }
                yield return null;
            }
        }
    }
}
