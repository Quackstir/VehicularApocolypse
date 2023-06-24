using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VehicleApocolypse
{

    [System.Serializable]
    public struct WeaponStats
    {
        [Header("Damage Stats")]
        public float f_Damage;
        public float f_CritChance;
        public float f_AmountPierce;

        public CharacterStat Damage;
        [Space(8)]
        [Header("Lizard Brain Stats")]
        public float f_Projectiles;
        public float f_Size;
        [Space(8)]
        [Header("Speed Stats")]
        public float f_RateOfFire;
        public float f_Haste;
        public float f_Boost;
    };

    public enum SlopeType
    {
        Linear, Exponential, Hyperbolic
    }

    public enum ItemType
    {
        Weapon, Support
    }

    public abstract class MB_ItemBase : MonoBehaviour
    {
        protected MB_CharacterBase _characterUnit;

        [SerializeField] protected SO_Item _scriptableObject;

        [SerializeField] protected float f_MaxLevel = 7;
        protected float f_CurrLevel = 0;
        //{
        //    get { return f_CurrLevel; }
        //    set
        //    {
        //        f_CurrLevel = value;
        //        UpgradeItem();
        //    }
        //}
        [SerializeField] protected ItemType eIT_ItemType;

        [SerializeField] protected WeaponStats[] sWS_WeaponStats_LevelUps;
        [SerializeField] protected WeaponStats sWS_WeaponLevelStat_Initial;


        [Header("Damage Stats")]
        protected float f_Damage = 0;
        protected float f_CritChance = 0;
        protected float f_AmountPierce = 0;

        [Space(8)]
        [Header("Lizard Brain Stats")]
        protected float f_Projectiles = 0;
        protected float f_Size = 0;
        float f_TimeBetweenShots;

        [Space(8)]
        [Header("Speed Stats")]
        protected float f_RateOfFire = 0;
        protected float f_Haste = 0;
        protected float f_Boost = 0;


        public Action<float, float> A_WeaponFirerate;
        protected virtual void Start()
        {
            _characterUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<MB_CharacterBase>();


            f_Damage = sWS_WeaponLevelStat_Initial.f_Damage;
            f_CritChance = sWS_WeaponLevelStat_Initial.f_CritChance;
            f_AmountPierce = sWS_WeaponLevelStat_Initial.f_AmountPierce;

            f_Projectiles = sWS_WeaponLevelStat_Initial.f_Projectiles;
            f_Size = sWS_WeaponLevelStat_Initial.f_Size;

            f_RateOfFire = sWS_WeaponLevelStat_Initial.f_RateOfFire;
            f_Haste = sWS_WeaponLevelStat_Initial.f_Haste;
            f_Boost = sWS_WeaponLevelStat_Initial.f_Boost;

            f_TimeBetweenShots = 60 / f_RateOfFire;
            if (eIT_ItemType == ItemType.Weapon)
                StartCoroutine(IE_WeaponFirerate());
        }

        IEnumerator IE_WeaponFirerate()
        {
            float f_NextShot = 0;

            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (A_WeaponFirerate != null)
                    A_WeaponFirerate(f_NextShot, Time.time - f_NextShot);

                float a = Time.fixedTime - f_NextShot;

                //Debug.Log("Shoot Firerate:" + a);
                if (Time.fixedTime > f_NextShot)
                {
                    Debug.Log("Shoot | Fixed Time:" + Time.fixedTime + " | Time Between Shots: " + f_TimeBetweenShots + " | a: " + a);
                    f_NextShot = Time.fixedTime + f_TimeBetweenShots;
                    V_WeaponActivate();
                }
                yield return null;
            }
        }

        protected virtual void V_WeaponActivate()
        {

        }

        public virtual void GetCharacterUnit(MB_CharacterBase characterUnit)
        {
            //_characterUnit = characterUnit;
        }

        [ContextMenu("Upgrade Item")]
        public virtual void V_UpgradeItem()
        {
            if (f_CurrLevel == f_MaxLevel)
            {
                return;
            }

            V_UpdateItem();
            f_CurrLevel++;
        }

        private void V_UpdateItem()
        {
            if (eIT_ItemType == ItemType.Weapon)
                StopAllCoroutines();

            for (int i = 0; i < sWS_WeaponStats_LevelUps.Length; i++)
            {
                if (f_CurrLevel == i)
                {
                    f_Damage += sWS_WeaponStats_LevelUps[i].f_Damage;
                    f_CritChance += sWS_WeaponStats_LevelUps[i].f_CritChance;
                    f_AmountPierce += sWS_WeaponStats_LevelUps[i].f_AmountPierce;

                    f_Projectiles += sWS_WeaponStats_LevelUps[i].f_Projectiles;
                    f_Size += sWS_WeaponStats_LevelUps[i].f_Size;

                    f_RateOfFire += sWS_WeaponStats_LevelUps[i].f_RateOfFire;
                    f_Haste += sWS_WeaponStats_LevelUps[i].f_Haste;
                    f_Boost += sWS_WeaponStats_LevelUps[i].f_Boost;
                }
            }

            f_TimeBetweenShots = 60 / f_RateOfFire;
            if (eIT_ItemType == ItemType.Weapon)
                StartCoroutine(IE_WeaponFirerate());
        }

        //protected virtual void updateMultipler(SlopeType SlopeWant, float initialValue, float increaseRate, bool IncreasePercentage)
        //{
        //    currentMultiplier = pickSlope(SlopeWant, initialValue, increaseRate, IncreasePercentage);

        //}

        //public virtual void IncreaseStackAmt(int IncreaseBy)
        //{
        //    //GlobalDebugs.DebugPM(this, "Current stack amount is " + StackAmount);
        //    StackAmount += IncreaseBy;
        //    UpdateEffects();
        //}

        //protected virtual float pickSlope(SlopeType SlopeWant, float initialValue, float increaseRate, bool IncreasePercentage)
        //{
        //    increaseRate = IncreasePercentage ? (increaseRate * 0.01f * initialValue) : increaseRate;

        //    switch (SlopeWant)
        //    {
        //        case SlopeType.Linear:
        //            return linearSlope(initialValue, increaseRate);
        //        case SlopeType.Exponential:
        //            return exponentialSlope(initialValue, increaseRate);
        //        case SlopeType.Hyperbolic:
        //            return hyperbolicSlope(initialValue, increaseRate);
        //        default:
        //            return linearSlope(initialValue, increaseRate);
        //    }
        //}

        //protected virtual float linearSlope(float initalValue, float increaseRate)
        //{
        //    return initalValue * increaseRate * StackAmount;
        //}

        //protected virtual float exponentialSlope(float initalValue, float increaseRate)
        //{
        //    return initalValue + Mathf.Pow(StackAmount, StackAmount);
        //}

        //protected virtual float hyperbolicSlope(float initalValue, float increaseRate)
        //{
        //    return (((increaseRate * StackAmount) + 1) / 1) - initalValue;
        //}
    }
}
