#region Copyright
// <copyright file="GamePlay.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/09/2016, 8:44 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;


public class GamePlay : MonoBehaviour
{
    public static GamePlay instance;
    public float timeBetweenWaves = 1;
    private bool finishedSpawningEnemies = false;
    private bool paused = false;

    public GameObject[] enemies;
    public static GameObject[] publicEnemies;
    public int enemyCount = 1;
    public int count;
    public float interval;
    public double delay;
    public int microWave = 0;
    public int wave = 0;
    Hashtable waveHashTable;
    public MonsterData.Level nextMonsterType;
    public List<int> indexList = new List<int>();
    public Transform enemiesRoot ;

    public double nextEnemyTime;
    public double nextWaveTime;
    public int generateWaves = 1;
    public List<Enemy> activeEnemies = new List<Enemy>();
    public List<Tower> activeTowers = new List<Tower>();


	public int monsterCount = 0;  
	public int remainLife = 10;  
	//public AI.GameAI gameAI;      // non-EA
	public EA_Run ea_player;   // EA_player
	public Currency currency;   
	public int lastWaveFileIndex = -1;
	public int newWaveFileIndex;

	public string PRJ_ROOT = "/Users/Franz/Documents/524LoopControl/LoopControl/";

	public string GetInfoPath(string targetFolderPath) {
		return targetFolderPath + "info";
	}


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
		Time.timeScale = 2;


        StartCoroutine(SpawnWaves());
        publicEnemies = this.enemies;
        GameObject curr = GameObject.FindGameObjectWithTag("Currency");
        GameObject ai = GameObject.FindGameObjectWithTag("ExcelReader");
        currency =curr.GetComponent<Currency>();
		//gameAI = ai.GetComponent<AI.GameAI>();   // non-EA
        //UnityEngine.Debug.Log(gameAI);     // non-EA
		ea_player = new EA_Run();     // EA_player
//		List<GeneSeq> pool = ea_player.InitSeqsRandom();     // EA_player  init_random
//		ea_player.ExportSeqToFile(pool);     
		ea_player.Run_EA_Loop();

		if (lastWaveFileIndex == -1) {
			System.IO.StreamReader file = 
				new System.IO.StreamReader(GetInfoPath(PRJ_ROOT + "wave_pool/"));
			lastWaveFileIndex = System.Int32.Parse(file.ReadLine ());
			file.Close ();
		} 

    }
			


    IEnumerator SpawnWaves()
    {
        LevelData levelData = GameData.instance.GetCurrentLevel();

        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (levelData.loaded == false)
        {
            yield return waitTime;
        }

        int noWaves = levelData.waves.Count;
        LevelData.Wave waveData;

        LevelData.Wave.Monster waveMonster;
        int monstersNo;
        WaitForSeconds levelWaitTime = new WaitForSeconds(timeBetweenWaves);
        //float currentWaitTime = 0;

        for (int waveNo = 0; waveNo < noWaves; waveNo++)
        {
            waveData = levelData.GetWave(waveNo);
            monstersNo = waveData.GetMonstersNo();

            //currentWaitTime = 0;
            for (int monsterNo = 0; monsterNo < monstersNo; monsterNo++)
            {
                waveMonster = waveData.GetMonster(monsterNo);


				monsterCount += waveMonster.amount;  //FFFFFFFFFFF
				UnityEngine.Debug.Log ("[Monster Generate] Monster: " + waveMonster.monster.name + ", Num: " + waveMonster.amount);  //FFFF
				UnityEngine.Debug.Log ("[Monsters Total Num] Num: " + monsterCount);  //FFFF


                // Wait until it's time to spawn the monsters
                yield return new WaitForSeconds(waveMonster.seconds);

                StartCoroutine(this.SpawnEnemies(waveMonster));
            }
            yield return levelWaitTime;
        }

        finishedSpawningEnemies = true;

        yield return null;
    }

    IEnumerator SpawnEnemies(LevelData.Wave.Monster waveMonster)
    {
        WaitForSeconds spawnWaitTime = new WaitForSeconds(0.5f);
        GameObject prefab = waveMonster.monster.GetPrefab();

        int prefabLoadingTime = 0;
        while (prefab == null && prefabLoadingTime < 10)
        {
            yield return new WaitForEndOfFrame();
            prefabLoadingTime++;
            prefab = waveMonster.monster.GetPrefab();
        }

        if (prefab == null){
			UnityEngine.Debug.LogWarning("Monster prefab not found");
            yield break;
        }

        for (int counter = 0; counter < waveMonster.amount; counter++){

            GameObject enemy = Instantiate(prefab, transform.position, this.transform.rotation) as GameObject;
            enemy.transform.parent = enemiesRoot;
            activeEnemies.Add(enemy.GetComponent<Enemy>());

            yield return spawnWaitTime;
        }
    }
    void Update()
    {
        
        }/*
        //if its time to spawn
        if (Time.time > nextWaveTime && generateWaves == 1)
        {
            //First, we need to convert back from enemytype, to our index for enemies
            StartCoroutine(this.SpawnEnemies_bck());//because we want to call yield
                                                //and we cannot call yield in the Update function according to Unity,
                                                //we need to put this in its own function

            //now that we've spawned enemies, we should load in the next waveData
            //Advance the microWaveCount
            microWave++;
            if (microWave >= 6)//if we've finished with one wave
            {
                microWave = 0;
                wave++;
            }
            if (wave >= 6)//if we're done with all waves
            {
                generateWaves = 0;//end wave generation
            }
            else//otherwise, load in data for the next wave
            {
                //initialzeWaveData does everything we want here! how convenient
                initializeWaveData();//it updates count, delay, nextWaveTime, and nextMonsterType!
            }
        }
    }

    void initializeWaveData()
    {
        LevelData.Wave waveData = GameData.instance.GetCurrentLevel().GetWave(wave);
        LevelData.Wave.Monster waveMonster = GameData.instance.GetCurrentLevel().GetWaveMonsters(waveData, microWave);
        //this is the initial code.  the first time its run, wave and microWave will be 0.
        count = waveMonster.amount;
        delay = waveMonster.seconds;
        nextWaveTime = Time.time + delay;
        nextMonsterType = waveMonster.monster;
        generateWaves = 1;//kinda redundant, but hey its safe.
    }

    IEnumerator SpawnEnemies_bck()
    {
        //we need to instantiate multiple monsters at once.
        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(nextMonsterType.GetPrefab(), transform.position, this.transform.rotation) as GameObject;
            enemy.transform.parent = enemiesRoot;
            activeEnemies.Add(enemy.GetComponent<Enemy>());
            yield return new WaitForSeconds(0.5f);
        }
    }
    */
	public void KillEnemy(Enemy enemy, bool doesArrive)
    {

		//FFFFFFFFFFFFFFFFFFF
		if (doesArrive) {
			remainLife--;
			UnityEngine.Debug.Log ("[Monster Arrive] Monster: " + enemy.baseEnemy.name + ", Distance: " + enemy.nextPointIndex); 
		} else {
			UnityEngine.Debug.Log("[Monster Dead] Monster: " + enemy.baseEnemy.name + ", Distance: " + enemy.nextPointIndex); 
		}
		monsterCount -= 1;//FFFFFFFFFF
		UnityEngine.Debug.Log ("[Monsters Total Num] Num: " + monsterCount);  //FFFF

        // Notify all towers that enemy died
        for (int i = 0; i < activeTowers.Count; ++i) {
            activeTowers[i].RemoveIntruder(enemy);
        }

        // Remove enemy from active enemies
        activeEnemies.Remove (enemy);


		UnityEngine.Debug.Log (finishedSpawningEnemies);

		if (finishedSpawningEnemies && monsterCount == 0)
			EndLevel();
    }

    public void RemoveEnemy(Enemy enemy)
    {
		KillEnemy(enemy, true);
    }

    public void EndLevel()
    {
        

        UIManager.instance.ShowEndOfLevel();


		UnityEngine.Debug.Log ("!!!!!!  New  Round !!!!  ");



		/*var processInfo = new ProcessStartInfo ("/usr/bin/python", "/Users/Franz/Documents/524Temp/EvaluatorPart/ai2Eva.py");
		processInfo.CreateNoWindow = true;
		processInfo.UseShellExecute = false;
		var process = Process.Start(processInfo);
		process.WaitForExit();
		process.Close();*/


		ClearAllData ();



		while (true) {	
			System.IO.StreamReader file = 
				new System.IO.StreamReader(GetInfoPath(PRJ_ROOT + "wave_pool/"));
		 	newWaveFileIndex = System.Int32.Parse(file.ReadLine ());
			file.Close ();
			if (newWaveFileIndex != lastWaveFileIndex) {
				break;
			} else {
				System.Threading.Thread.Sleep (3000);
			}
		}
		Start ();

    }

    public void Pause()
    {
        paused = !paused;
        Time.timeScale = (paused) ? 0 : 1;
    }



	void ClearAllData() {
		activeEnemies = new List<Enemy>();

		monsterCount = 0; 
		remainLife = 10;   
		finishedSpawningEnemies = false;
	
		foreach (Tower tower in activeTowers) {
			tower.gameObject.SetActive (false);
		}


		activeTowers = new List<Tower>();

		//gameAI.ResetPositionsList ();  // non-EA

		currency.ResetMoney ();

	}
}
