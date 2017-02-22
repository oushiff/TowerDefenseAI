#region Copyright
// <copyright file="TowerData.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/29/2016, 7:43 AM </date>
#endregion
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class TowerData
{
	public string name;
	public bool air;
	protected GameObject prefab;

	public List<Level> levels = new List<Level> ();

	public class Level
	{
		public TowerData tower;

		public int index;

		public string name;
		public float damage;
		public float hp;
		public float armor;
		public float range;
		public float rateOfFire;
		public float cost;
		public float effect;
		public float rebootTime;

		public SpecialAbility specialAbility = new SpecialAbility ();

		public float GetProperty (GameData.GameProperies property)
		{
			switch (property) {
			case GameData.GameProperies.DAMAGE:
				return damage;
			case GameData.GameProperies.HP:
				return hp;
			case GameData.GameProperies.ARMOR:
				return armor;
			case GameData.GameProperies.RANGE:
				return range;
			case GameData.GameProperies.RATE_OF_FIRE:
				return rateOfFire;
			case GameData.GameProperies.COST:
				return cost;
			case GameData.GameProperies.EFFECT:
				return effect;
			case GameData.GameProperies.REBOOT_TIME:
				return rebootTime;
			}
			return 0;
		}

		public float GetSpecialProperty (GameData.GameProperies property)
		{
			if (specialAbility != null)
				return specialAbility.GetProperty (property);
			return 0;
		}

		public bool HasSpecialAbility ()
		{
			return specialAbility != null && (specialAbility.chancePercentage > 0);
		}
	}

	public class SpecialAbility
	{
		public float damagePercentage;
		public float damage;
		public float duration;
		public float range;
		public float reduction;
		public float projectiles;
		public float chancePercentage;

		public float GetProperty (GameData.GameProperies property)
		{
			switch (property) {
			case GameData.GameProperies.DAMAGE_PERCENTAGE:
				return damagePercentage;
			case GameData.GameProperies.DAMAGE:
				return damage;
			case GameData.GameProperies.DURATION:
				return duration;
			case GameData.GameProperies.RANGE:    
				return range;
			case GameData.GameProperies.REDUCTION:
				return reduction;
			case GameData.GameProperies.PROJECTILES:
				return projectiles;
			case GameData.GameProperies.CHANCE_PERCENTAGE:
				return chancePercentage;
			}
			return 0;
		}
	}

	public TowerData (string _name, float _air)
	{
		name = _name;
		air = _air > 0.5f;
	}

	public void AddTower (DataReader.TowerData inputData, int index = -1)
	{
		Level towerLevel = new Level ();
		towerLevel.index = index;

		// Update level data
		towerLevel.name = inputData.levelData.levelName;
		towerLevel.armor = inputData.levelData.armor;
		towerLevel.cost = inputData.levelData.cost;
		towerLevel.damage = inputData.levelData.damage;
		towerLevel.effect = inputData.levelData.effect;
		towerLevel.hp = inputData.levelData.hp;
		towerLevel.range = inputData.levelData.range;
		towerLevel.rateOfFire = inputData.levelData.rateOfFire;
		towerLevel.rebootTime = inputData.levelData.rebootTime;

		// Update special ability
		bool hasSpecialAbility = (inputData.levelData.specialAbility.chancePercentage > 0);
		if (hasSpecialAbility) {
			towerLevel.specialAbility = new SpecialAbility ();
			towerLevel.specialAbility.damagePercentage = inputData.levelData.specialAbility.damagePercentage;
			towerLevel.specialAbility.damage = inputData.levelData.specialAbility.damage;
			towerLevel.specialAbility.duration = inputData.levelData.specialAbility.duration;
			towerLevel.specialAbility.range = inputData.levelData.specialAbility.range;
			towerLevel.specialAbility.reduction = inputData.levelData.specialAbility.reduction;
			towerLevel.specialAbility.projectiles = inputData.levelData.specialAbility.projectiles;
			towerLevel.specialAbility.chancePercentage = inputData.levelData.specialAbility.chancePercentage;
		}
		// Reference tower
		towerLevel.tower = this;

		levels.Add (towerLevel);
	}

	public Level GetTower (int level)
	{
		if (level < levels.Count) {
			return levels [level];
		}
		return null;
	}

	public GameObject GetPrefab ()
	{
		if (prefab == null) {
			prefab = Resources.Load ("Towers\\" + name) as GameObject;
		}
		return prefab;
	}
}
