#region Copyright
// <copyright file="GameData.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/28/2016, 1:53 PM </date>
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	private static GameData _instance = null;

	public static GameData instance {
		get {
			if (_instance == null) {
				_instance = (GameData)FindObjectOfType (typeof(GameData));

				if (_instance == null) {
					GameObject newInstance = new GameObject ("GameData");
					_instance = newInstance.AddComponent<GameData> ();
				}
			}

			return _instance;
		}
	}

	#region Game data

	public enum GameProperies
	{
		NAME,
		REFERENCE,
		DAMAGE,
		HP,
		ARMOR,
		RANGE,
		RATE_OF_FIRE,
		COST,
		REWARD,
		SPEED,
		MANA,
		EFFECT,
		REBOOT_TIME,
		DAMAGE_PERCENTAGE,
		DURATION,
		REDUCTION,
		PROJECTILES,
		CHANCE_PERCENTAGE
	}

	private List<MonsterData> monstersList;
	private Dictionary<string, MonsterData.Level> monsters;
     
	private List<TowerData> towersList;
	private Dictionary<string, TowerData> towers;

	private LevelData level = new LevelData ();

	#endregion

	private void Awake ()
	{
	}

	private void Start ()
	{
#if UNITY_WEBGL
        DataReader.instance.StartCoroutine(DataReader.instance.WaitMonsterData(LoadMonsterData));
        DataReader.instance.StartCoroutine(DataReader.instance.WaitTowerData(LoadTowerData));
#else
		LoadMonsterData ();
		LoadTowerData ();
#endif
		level.Load ();
	}

	#region Load Data

	private void LoadMonsterData ()
	{
		// Initialize lists
		if (monstersList == null) {
			monstersList = new List<MonsterData> ();
		}
		monstersList.Clear ();

		if (monsters == null) {
			monsters = new Dictionary<string, MonsterData.Level> ();
		}
		monsters.Clear ();

		// Load all monster data
		List<DataReader.MonsterData> inputMonsterData = DataReader.instance.monsters.monsters;
		MonsterData lastMonster = null;
		for (int i = 0; i < inputMonsterData.Count; i++) {
			if (lastMonster == null || (lastMonster != null && lastMonster.name != inputMonsterData [i].MonsterName)) {
				lastMonster = new MonsterData (inputMonsterData [i].MonsterName, inputMonsterData [i].air);
				monstersList.Add (lastMonster);
			}

			monsters.Add (inputMonsterData [i].levelName, lastMonster.AddMonster (inputMonsterData [i]));
		}
	}

	private void LoadTowerData ()
	{
		// Initialize list
		if (towers == null) {
			towers = new Dictionary<string, TowerData> ();
		}
		towers.Clear ();

		if (towersList == null) {
			towersList = new List<TowerData> ();
		}
		towersList.Clear ();

		// Load all tower data
		List<DataReader.TowerData> inputTowerData = DataReader.instance.towers.towers;
		TowerData newTower = null;
		for (int i = 0; i < inputTowerData.Count; i++) {
			if (!towers.ContainsKey (inputTowerData [i].TowerName)) {
				newTower = new TowerData (inputTowerData [i].TowerName, inputTowerData [i].air);
				towersList.Add (newTower);
				towers.Add (inputTowerData [i].TowerName, newTower);
			}

			towers [inputTowerData [i].TowerName].AddTower (inputTowerData [i], i);
		}
	}

	#endregion

	#region Monsters

	public MonsterData.Level GetMonster (int detailedIdx)
	{
		int monsterIdx = detailedIdx / 3;
		MonsterData monster = monstersList [monsterIdx];
		if (monster != null) {
			return monster.GetLevel (detailedIdx % 3);
		}
		return null;
	}

	#endregion

	public void Load ()
	{
		LoadMonsterData ();
		LoadTowerData ();
		level.Load ();	
	}

	public LevelData GetCurrentLevel ()
	{
		return level;
	}

	public TowerData.Level GetTower (string type, int level)
	{
		if (towers != null && towers.ContainsKey (type)) {
			return towers [type].GetTower (level);
		}
		return null;
	}

	internal TowerData.Level GetTower (int type, int level)
	{
		if (towersList != null && type < towersList.Count) {
			return towersList [type].GetTower (level);
		}
		return null;
	}



}
