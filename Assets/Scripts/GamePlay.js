#pragma strict

import System.Collections.Generic;

var enemies:GameObject[];
static var publicEnemies:GameObject[];
var enemyCount:int = 1;
var count:int;
internal var interval:float;
var delay:double;
var microWave:int = 0;
var wave:int = 0;
var waveHashTable:Hashtable;
var nextMonsterType:int;
var nextMonsterName:String;
var indexList:List.<int>;
//var indx:int = 0;
var enemiesRoot : Transform;

internal var nextEnemyTime:double;
internal var nextWaveTime:double;
internal var generateWaves:int = 1;
private var excel : ExcelReader; 
static var activeEnemies:List.<GameObject>;
static var activeTowers:List.<GameObject>;
function Awake()
{
    
}

function Start () 
{
    activeEnemies = new List.<GameObject>();
    publicEnemies = this.enemies;
    activeTowers = new List.<GameObject>();
//Get the ExcelReader Script

    excel = this.GetComponent("ExcelReader");
    initializeWaveData();
//	print("wave count " + indexList.Count);
}
function OnGUI()
{

}
function getEnemyIndex()
{
    //this looks at nextMonsterType and returns a value from 0 to 276
    //very imilar to the grp/cat/lvl code in excelreader
    var index:int;
    var group:int = nextMonsterType / 100;
    var cat:int = nextMonsterType / 10;
    cat = cat % 10;
    var lvl:int = nextMonsterType % 10;
    //each group is a group of 3 categories, 9 monster entries. so we can reverse this with math, since we're just using numbers this time.
    index = 9*(group-1);//group 1:index 0-8, group 2: index 9-17, group 3:Index 18-26
    index = index + 3*(cat-1);//similarly, each group has 3 categories..
    index = index + (lvl-1);//and each category has 3 levels
    
    return index;
}

function spawnLoop(enemiesIndex)
{
    //we need to instantiate multiple monsters at once.
    var incount : int = count;
    for (var i:int = 0; i < incount; i++)
    {
        var enemy : GameObject;
        enemy = Instantiate(enemies[enemiesIndex],transform.position, this.transform.rotation);
        enemy.transform.parent = enemiesRoot;
        activeEnemies.Add(enemy);
        yield WaitForSeconds(0.5);
    }
}

function Update()
{
    //if its time to spawn
    if (Time.time > nextWaveTime && generateWaves ==1)
    {
        //First, we need to convert back from enemytype, to our index for enemies
        var enemiesIndex:int;
        enemiesIndex = getEnemyIndex();
        
        spawnLoop(enemiesIndex);//because we want to call yield
        //and we cannot call yield in the Update function according to Unity, we need to put this in its own function
    
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


function initializeWaveData()
{
    //this is the initial code.  the first time its run, wave and microWave will be 0.
    count = excel.wavesLevel1[wave,microWave].count;
    delay = excel.wavesLevel1[wave,microWave].delay;
    nextWaveTime = Time.time + delay;
    nextMonsterType = excel.wavesLevel1[wave,microWave].type;
    nextMonsterName = excel.wavesLevel1[wave,microWave].name;//useful to record for debugging purposes, but not currently used in code
    generateWaves = 1;//kinda redundant, but hey its safe.
    return;
}

