/*******************************************************************************
File: CharacterStat.cs
Author: Max Stucker
Date: 8/11/2022
Stat Objects used by characters to derive stats. Handles Stat Behavior such as
getters & updates.

Stats are stored as floats, but with an optional flag to treat them as ints.
I recognize how gross this is but I literally could not think of a way to 
use generics without having to essentially write every function twice so whatever.

Stat Modifiers are how items modify stats. Abilities create StatModifiers and
add them to the modifiers list of the stat they are modifying.

When the UpdateStat() method is called, the Stat runs through its list of
modifiers, and aggregates their effects into different variables based on 
the specified MOD_TYPE value.

Once this is finished, actualValue is calculated using these modifiers.
*******************************************************************************/
using System.Collections.Generic;

namespace VehicleApocolypse
{
    [System.Serializable]
    public class CharacterStat
    {
        public float BaseValue = 0; //Base Value of this stat, before any bonuses
        public float ScalingPerLevel = 0;//How much this value increases per character level (NOTE: Character levels may not be in the final game??)
        private float actualValue = 0; //The actual value of the stat, after calculating bonuses
        private bool treatAsInt = false; //If true, actualValue is cast to an int before being returned by the GetStat() function.
        private List<StatModifier> modifiers = new List<StatModifier>(); // List of Modifiers that this Stat is being affected by.

        //Constructors
        public CharacterStat(float BaseValue, float ScalingPerLevel)
        {
            this.BaseValue = BaseValue;
            this.ScalingPerLevel = ScalingPerLevel;

            UpdateStat();
        }
        public CharacterStat(float BaseValue, float ScalingPerLevel, bool treatAsInt)
        {
            this.BaseValue = BaseValue;
            this.ScalingPerLevel = ScalingPerLevel;
            this.treatAsInt = treatAsInt;

            UpdateStat();
        }

        //Getters
        public float GetStat()
        {
            if (treatAsInt)
            {
                return (int)actualValue;
            }

            return actualValue;
        }

        //Update Stat
        //Updates the actualValue of this stat using the stat's list of stat modifiers
        public void UpdateStat()
        {
            float flatModifier = 0;
            float addedMultiplier = 1;
            float trueMultiplier = 1;

            for (int i = 0; i < modifiers.Count; i++)
            {
                switch (modifiers[i].Type)
                {
                    case StatModifier.MOD_TYPE.FlatAdditive:
                        {
                            flatModifier += modifiers[i].Value;
                            break;
                        }
                    case StatModifier.MOD_TYPE.AddedMultiplicative:
                        {
                            addedMultiplier += modifiers[i].Value;
                            break;
                        }
                    case StatModifier.MOD_TYPE.TrueMultiplicative:
                        {
                            trueMultiplier *= modifiers[i].Value;
                            break;
                        }
                }
            }

            actualValue = (BaseValue + /*(ScalingPerLevel * (LevelOfChar-1))*/ +flatModifier) * addedMultiplier * trueMultiplier;
        }

        //Adds a reference to a stat modifier to the list
        public void AddModifier(StatModifier addMod)
        {
            modifiers.Add(addMod);
        }
        //Removes a reference to a stat modifier from the list
        public void RemoveModifier(StatModifier removeMod)
        {
            modifiers.Remove(removeMod);
        }
    }
}

//Stat Modifier Class
//Used by Abilities to modify this stat. Contains a value & modification type
//TYPES:
//FlatAdditive - Simply adds the value to the stat
//AddedMultiplicative - Any Added Multiplicative Modifiers will first be added together. Then, the stat will be multiplied by the final result
//TrueMultiplicative - Any True Multiplicative Modifiers will be MULTIPLIED together. Then, the stat will be multiplied by the final result
namespace VehicleApocolypse
{
    public class StatModifier
    {
        public enum MOD_TYPE { FlatAdditive = 0, AddedMultiplicative = 1, TrueMultiplicative = 2 }
        float value; //Visual Studio yells at me for this but I want the flexibility of modifying the setter just in case i need to move UpdateStats() out of the update loop
        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public MOD_TYPE Type;

        public StatModifier(float value, MOD_TYPE type)
        {
            Value = value;
            Type = type;
        }
    }
}
