#region Copyright
// <copyright file="MonsterData.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/29/2016, 10:33 PM </date>
#endregion
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public string name;
    public bool air;

    public List<Level> levels = new List<Level>();

    public MonsterData(string _monsterName, int _air)
    {
        this.name = _monsterName;
        this.air = _air > 0.5f;
    }
    
    public class Level
    {
        public MonsterData monster;
        public GameObject prefab;

        public string name;
        public string reference;
        public float damage;
        public float hp;
        public float armor;
        public float range;
        public float rateOfFire;
        public float gain;
        public float speed;
        public float mana;
        public SpecialAbility specialAbility = new SpecialAbility();

        public GameObject GetPrefab()
        {
            if (prefab == null){
                prefab = Resources.Load("Monsters\\" + reference) as GameObject;
            }
            return prefab;
        }

        public string GetSpecialAbilityRaw(string property)
        {
            if (specialAbility != null){
                return specialAbility.GetOption(property);
            }
            return string.Empty;
        }

        public int GetSpecialAbilityInt(string property)
        {
            string stringValue = GetSpecialAbilityRaw(property);
            int returnValue = 0;

            int.TryParse(stringValue, out returnValue);

            return returnValue;
        }

        public float GetSpecialAbilityFloat(string property)
        {
            string stringValue = GetSpecialAbilityRaw(property);
            float returnValue = 0;

            float.TryParse(stringValue, out returnValue);

            return returnValue;
        }

        public string GetSpecialAbilityString(string property)
        {
            return GetSpecialAbilityRaw(property);
        }
    }
    
    public class SpecialAbility
    {
        public string description;
        public List<Option> options = new List<Option>();

        public class Option
        {
            public string name;
            public string value;

            public Option() { }

            public Option(string _name, string _value)
            {
                name = _name;
                value = _value;
            }
        }

        public void AddOption(string _name, string _value)
        {
            options.Add(new Option(_name, _value));
        }

        public string GetOption(string property)
        {
            for (int i = 0; i < options.Count; i++){
                if (options[i].name == property)
                    return options[i].value;
            }
            return string.Empty;
        }
    }

    public Level AddMonster(DataReader.MonsterData monsterData)
    {
        Level monsterLevel = new Level();

        // Update level data
        monsterLevel.name = monsterData.levelName;
        monsterLevel.reference = monsterData.reference;
        monsterLevel.damage = monsterData.damage;
        monsterLevel.reference = monsterData.reference;
        monsterLevel.hp = monsterData.hp;
        monsterLevel.armor = monsterData.armor;
        monsterLevel.range = monsterData.range;
        monsterLevel.rateOfFire = monsterData.rateOfFire;
        monsterLevel.gain = monsterData.gain;
        monsterLevel.mana = monsterData.mana;
        monsterLevel.speed = monsterData.speed;

        // Update special ability
        if (monsterData.specialAbility != string.Empty){
            monsterLevel.specialAbility.description = monsterData.specialAbility;

            if (monsterData.specialAbilityName1 != string.Empty){
                monsterLevel.specialAbility.AddOption(monsterData.specialAbilityName1, monsterData.specialAbilityValue1);
            }
            if (monsterData.specialAbilityName2 != string.Empty){
                monsterLevel.specialAbility.AddOption(monsterData.specialAbilityName2, monsterData.specialAbilityValue2);
            }
            if (monsterData.specialAbilityName3 != string.Empty){
                monsterLevel.specialAbility.AddOption(monsterData.specialAbilityName3, monsterData.specialAbilityValue3);
            }
            if (monsterData.specialAbilityName4 != string.Empty){
                monsterLevel.specialAbility.AddOption(monsterData.specialAbilityName4, monsterData.specialAbilityValue4);
            }
            if (monsterData.specialAbilityName5 != string.Empty){
                monsterLevel.specialAbility.AddOption(monsterData.specialAbilityName5, monsterData.specialAbilityValue5);
            }
        }

        monsterLevel.monster = this;

        levels.Add(monsterLevel);
        return monsterLevel;
    }

    public Level GetLevel(int idx)
    {
        if (idx < levels.Count)
            return levels[idx];
        return null;
    }
}
