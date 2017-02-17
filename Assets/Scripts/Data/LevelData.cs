#region Copyright
// <copyright file="LevelData.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/29/2016, 1:56 PM </date>
#endregion
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.NetworkSystem;

[System.Serializable]
public class LevelData
{
    public class Wave
    {
        public class Monster
        {
            public float seconds;
            public int amount;
            public MonsterData.Level monster;
        }
        public List<Monster> monsters = new List<Monster>();

        public void Empty()
        {
            monsters.Clear();
        }

        public void AddMonster(int _amount, float _seconds, MonsterData.Level _monster)
        {
            Monster newMonster = new Monster();
            newMonster.amount = _amount;
            newMonster.seconds = _seconds;
            newMonster.monster = _monster;
            monsters.Add(newMonster);
        }

        public int GetMonstersNo()
        {
            if (monsters != null)
                return monsters.Count;
            return 0;
        }

        public Monster GetMonster(int monstersIdx)
        {
            if (monstersIdx < monsters.Count)
            {
                return monsters[monstersIdx];
            }

            return null;
        }
    }

    public string name;
    public int startingMoney;
    public int width;
    public int length;
    public int waypointsNo;

    public List<int[]> grid = new List<int[]>();
    public List<int[]> waypoints = new List<int[]>();
    public List<Wave> waves = new List<Wave>();     
    public List<TowerData.Level> towers = new List<TowerData.Level>();
    public bool loaded;

    public void EmptyLevel()
    {
        loaded = false;
        if (grid != null && grid.Count > 0){
            grid.Clear();
        } 
        if (waypoints != null && waypoints.Count > 0){
            waypoints.Clear();
        } 
        if (towers != null && towers.Count > 0){
            towers.Clear();
        } 
        if (waves != null && waves.Count > 0){
            for (int i = 0 ; i < waves.Count ; i++)
                waves[i].Empty();
            waves.Clear();
        }    
    }

    public void Load()
    {
        loaded = false;
        // Empty previous level
        this.EmptyLevel();

        // Get current level
        int level = 1;
        if (PlayerPrefs.HasKey("Level"))
            level = PlayerPrefs.GetInt("Level");
        
        name = "Level" + level.ToString();

#if UNITY_WEBGL
        DataReader.instance.StartCoroutine(DataReader.instance.LoadLevelWeb(level, LoadLevel));
#else
        // Load level data
        DataReader.LevelInfo loadedLevel = DataReader.instance.LoadLevel(level);
        LoadLevel(loadedLevel);
#endif
    }

    public void LoadLevel(DataReader.LevelInfo loadLevel)
    {
        if (loadLevel != null){
            startingMoney = loadLevel.money;
            width = loadLevel.grid.rowsCount;
            length = loadLevel.grid.columnsCount;

            int counter = 0;
            int innerCounter = 0;
            // Load grid
            for (counter = 0; counter < loadLevel.grid.rows.Count; counter++){
                int[] row = Array.ConvertAll<string, int>(loadLevel.grid.rows[counter].Split(','), int.Parse);
                grid.Add(row);
            }

            // Load waypoints
            waypointsNo = 0;
            for (counter = 0; counter < loadLevel.waypoints.Count; counter++){
                int[] row = Array.ConvertAll<string, int>(loadLevel.waypoints[counter].Split(','), int.Parse);
                for (innerCounter = 0; innerCounter < row.Length; innerCounter++)
                    if (row[innerCounter] > 0)
                        waypointsNo++;
                waypoints.Add(row);
            }

            // Load towers
            for (counter = 0; counter < loadLevel.towers.Count; counter++){
                towers.Add(GameData.instance.GetTower(loadLevel.towers[counter], 0));
            }

            // Load monster waves
            Wave wave;
            List<DataReader.Monster> waveMonsters;

            for (counter = 0; counter < loadLevel.waves.Count; counter++){
                wave = new Wave();
                waveMonsters = loadLevel.waves[counter].monsters;
                for (innerCounter = 0; innerCounter < waveMonsters.Count; innerCounter++){
                    wave.AddMonster(waveMonsters[innerCounter].Amount, waveMonsters[innerCounter].Seconds,
                        GameData.instance.GetMonster(waveMonsters[innerCounter].ID));
                }
                waves.Add(wave);
            }
        }
        loaded = true;
    }

    public Wave GetWave(int waveIdx)
    {
        if (waveIdx < waves.Count){
            return waves[waveIdx];
        }
        return null;
    }
    
    public LevelData.Wave.Monster GetWaveMonsters(Wave wave, int monstersIdx)
    {
        if (wave != null && monstersIdx < wave.monsters.Count)
        {
            return wave.monsters[monstersIdx];
        }
        return null;
    }
}
