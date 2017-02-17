#region Copyright
// <copyright file="ExcelReader.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/31/2016, 3:32 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_4
using System.Data; 
using System.Data.Odbc;
#endif
using System.IO;
using Excel;

public class ExcelReader : MonoBehaviour 
{
    public static ExcelReader instance;

#if UNITY_4
    //fresh, new datatables
    public DataTable towerStatTable, towersStatTable, levelTable, monsterStatTable;
    //We only have 3 sheets to read from the new excel file, so we only need 3 data tables.  Each array will have it's own unique code to parse through whatever table it has been given.
    //Also, these tables are global but i suppose they could be passed around as arguments. but there should be 0 problems by having them as global.  Maybe in the future this might change but it's not going to be a trouble, even then
    public DataView debugView;	
#endif

    #region VariousGlobals
    public enum SpecialAbilityStat{
        DAMAGE_PERCENTAGE = 0,
        DAMAGE = 1,
        DURATION = 2,
        RANGE = 3,
        REDUCTION = 4,
        PROJECTILES = 5,
        CHANCE_PERCENTAGE = 6
    }

    //properties order for towers
    public const int DAMAGE = 0;
    public const int HP = 1;
    public const int ARMOR = 2;
    public const int RANGE = 3;
    public const int RATE_OF_FIRE = 4;
    public const int COST = 5;
    public const int ARMOR_PENETRATION = 6;
    public const int HEAL_TIME = 7;
    public const int FLYING = 8;

    //Array of ints used as booleans to determine what towers are available in this level
    public int[] allowedTowers = new int[6];
    
    //properties order for enemies
    public const int E_DAMAGE = 0;
    public const int E_HP = 1;
    public const int E_ARMOR = 2;
    public const int E_RANGE = 3;
    public const int E_RATE_OF_FIRE = 4;
    public const int E_GAIN = 5;
    public const int E_SPEED = 6;
    public const int E_MANA = 7;
    public const int E_FLYING = 8;
    //Grid Size
    public int GRID_WIDTH = 8;
    public int GRID_LENGTH = 12;
    
    //variable to store all the materials
    public Material GRASS_MATERIAL;
    public Material PATH_MATERIAL;
    public Material TOWER_MATERIAL;
    public Material ENTRANCE_MATERIAL;
    public Material EXIT_MATERIAL;
    public Material HOVER_ACTIVE_MATERIAL;
    public Material HOVER_INACTIVE_MATERIAL;
    
    //Constants for tags
    public const String PLANE_CAMERA_FOCUS = "Plane_camera_focus";
    public const String PLANE_HOVER = "Plane_hover";
    public const String PLANE_NO_HOVER = "Plane_no_hover";
    //public const String PLANE_WITH_TOWER = "Plane_with_tower";
    
    //Constants for layers
    public const String RAYCAST_LAYER = "Raycast_layer";
    public const String DEFAULT_LAYER = "Default";
    
    //Tower Table size
    public int NUM_PROPERTIES;
    public int NUM_TOWERS;
    public int NUM_ENEMIES;
    [HideInInspector]
    public int numberOfWayPoints = 0 ;
    
    //keeping the grid size fixed
    public int[,] gridArray = new int[8,12];
    public int[,] wayPointsArray = new int[8,12];
    public float[,] towerArray = new float[8,12];
    public float[,] enemyArray = new float[8,12];
    
    //lets use 2D arrays to hold tower stats
    //5 rows, one for each level. in order, the columns are Damage, HP, Armor, Range, Rate of Fire, Cost, and Splash Area/Armor Pen
    //Row 4 and 5 are two different variations of max level tower.

    [System.Serializable]
    public class TowerReader
    {
        public string type;
        public float[,] towerProperties = null;
        public float[,] specialAbilities = null;
        public bool towerFlight;
    }
    private Dictionary<string, TowerReader> towers = new Dictionary<string,TowerReader>();
     
    public float[,] gunTowerArray = new float[5, 8];
    public float[,] gunTowerSA = new float[2, 8];
    public bool gunTowerFlight;
    public float[,] cannonTowerArray = new float[5, 8];
    public float[,] cannonTowerSA = new float[2, 8];
    public bool cannonTowerFlight;
    public float[,] thresherTowerArray = new float[5, 8];
    public float[,] thresherTowerSA = new float[2, 8];
    public bool thresherTowerFlight;
    public float[,] electricalTowerArray = new float[5, 8];
    public float[,] electricalTowerSA = new float[2, 8];
    public bool electricalTowerFlight;
    public float[,] scytheTowerArray = new float[5, 8];
    public float[,] scytheTowerSA = new float[2, 8];
    public bool scytheTowerFlight;
    public float[,] towernautTowerArray = new float[5, 8];
    public float[,] towernautTowerSA = new float[2, 8];
    public bool towernautTowerFlight;

    public string[,] enemyNameArray = new string[27,2];
    public float[,] enemyStatsArray = new float[27, 9];
    public float[,] zombieArray = new float[3, 9];
    public float[,] joggedMickeyArray = new float[3, 9];
    public float[,] blindTommyArray = new float[3, 9];
    public float[,] frankensteamArray = new float[3, 9];
    public float[,] sicklySueArray = new float[3, 9];
    public float[,] shamanArray = new float[3, 9];
    public float[,] rootReggieArray = new float[3, 9];
    public float[,] mummyDearestArray = new float[3, 9];
    public float[,] werefalloArray = new float[3, 9];

    public float sueCooldown;//for monster summoning
    public int  sueType;//possible options are hardcoded
    public float shamanRadius, shamanStrength, shamanCooldown;//variables for shaman healing
    public float mummyRadius, mummyPower, mummyCooldown;//variables for mummy healing
    public float reggieDistance, reggieSpeed, reggieCooldown;//variables for mummy healing
    public int numOfWaves;
    public MonsterWave[,] wavesLevel1 = new MonsterWave[6,6];
    
    //variable to store the main camera
    public Camera cameraObj;
    
    //variable to store clicked/tapped enemy
    public GameObject selectedEnemy = null;
    
    //string to hold level page info
    public String levelName ="Level";
    
#endregion
    
    public struct MonsterWave
    {
        public int type;
        public float delay;
        public int count;
        public string name;
    };

    void Awake(){
        Destroy(this);
        instance = this;
    }

    public float GetPropertyFloat(int type, int level, int property)
    {
        string key = string.Empty;

        int index = 0;
        foreach (string tower in towers.Keys) {
            if (index == type)
            {
                key = tower;
                break;
            }
            index++;
        }

        if (key == string.Empty)
            return 0.0f;

        return towers[key].towerProperties[level, property];
    }

    public float GetPropertyFloat(string type, int level, int property)
    {
        return towers[type].towerProperties[level, property];
    }

    public bool GetPropertyBool(string type, int property)
    {
        switch (type)
        {
            case "Gun":
                switch (property)
                {
                    case FLYING:
                        return gunTowerFlight;
                }
                break;
            case "Cannon":
                switch (property)
                {
                    case FLYING:
                        return cannonTowerFlight;
                }
                break;
            case "Thresher":
                switch (property)
                {
                    case FLYING:
                        return thresherTowerFlight;
                }
                break;
            case "Electical":
                switch (property)
                {
                    case FLYING:
                        return electricalTowerFlight;
                }
                break;
            case "Scythe":
                switch (property)
                {
                    case FLYING:
                        return scytheTowerFlight;
                }
                break;
            case "Towernaut":
                switch (property)
                {
                    case FLYING:
                        return towernautTowerFlight;
                }
                break;
        }
        return false;
    }

    void readTables()
    {
        int level = 1;
        if (PlayerPrefs.HasKey ("Level"))
            level = PlayerPrefs.GetInt ("Level");

        levelName += level.ToString();

#if UNITY_4
        towerStatTable = readXLS(Application.dataPath + "/TDDesignData.xls", "TowerStats");//stats for the towers
        //towersStatTable = readXLS(Application.dataPath + "/TDDesignData.xls", "Towers");//stats for the towers
        levelTable =readXLS(Application.dataPath + "/TDDesignData.xls", levelName);//has info on grid, waypoints, and first wave
        monsterStatTable =  readXLS(Application.dataPath + "/TDDesignData.xls", "Monsters Stats");//has info on monster stats
        return;
#endif
    }//need to read in towerStatTable, levelTable, monsterStatTable

#region TowerArrayConstruction
    public float EvaluateSpecialAbility(string type, int currentLevel, SpecialAbilityStat stat, bool force = false)
    {
        if (towers[type].specialAbilities != null){
            var currentSpecialAbility = currentLevel - 4;
            if (currentSpecialAbility >= 0){
                if (force || UnityEngine.Random.Range(0.0f, 1.0f) <
                    (float) towers[type].specialAbilities[currentSpecialAbility, (int) SpecialAbilityStat.CHANCE_PERCENTAGE]){
                    return towers[type].specialAbilities[currentSpecialAbility, (int) stat];
                }
            }
        }
        return -1;
    }

    float[,] setUpGunArray()
    {
        //When we load things into the datatable, column A in excel corresponds to column 0
        //What isn't intuitive is that row 30 in excel is row 28 in code. There is an offset of 2.

        // first, we'll start with the gun tower, and we'll just deal with 7 basic columns for now
        //codewise, we start from gun tower row index = 27, 
        //so, we start at 27,1 as the first gun thing is 29,B
        float[,] newArray = new float[5,8];
        int startRowIndex = 27;
        int startColumnIndex = 1;
#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                //this checks each value and copies it over, only in our own array we start at 0,0 instead of 27,1. makes things a bit easier!
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
            
        }
        //set flying property of gun towers 
        if (Convert.ToSingle(towerStatTable.Rows[123].ItemArray[1]) == 1) gunTowerFlight = true;	
        else gunTowerFlight = false;
#endif
        return newArray;
    }
    float[,] setUpSpecialAbility (int startRowIndex, int startColumnIndex)
    {
        int specialAbilityRow = 2, specialAbilityColumn = Enum.GetNames(typeof(SpecialAbilityStat)).Length;
        float[,] newArray = new float[specialAbilityRow, specialAbilityColumn];
        for (int i= 0; i< specialAbilityRow; i++)//go through 5 rows
        {
            for (int j=0; j<specialAbilityColumn; j++)//go through  7 columns
            {

#if UNITY_4
                //this checks each value and copies it over, only in our own array we start at 0,0 instead of 27,1. makes things a bit easier!
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
#endif
            }
        }
        return newArray;
    }
    float[,] setUpCannonArray()
    {
        float[,] newArray = new float[5,8];
        int startRowIndex = 38;
        int startColumnIndex = 1;

#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
        }
        //set flying property
        if (Convert.ToSingle(towerStatTable.Rows[124].ItemArray[1]) == 1) cannonTowerFlight = true;	
        else cannonTowerFlight = false;
#endif
        return newArray;
    }
    float[,] setUpThresherArray()
    {
        float[,] newArray = new float[5,8];
        int startRowIndex = 52;
        int startColumnIndex = 1;
#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
        }
        //set flying property
        if (Convert.ToSingle(towerStatTable.Rows[125].ItemArray[1]) == 1) thresherTowerFlight = true;	
        else thresherTowerFlight = false;
#endif
        return newArray;
    }
    float[,] setUpElectricalArray()
    {
        float[,] newArray = new float[5,8];
        int startRowIndex = 67;
        int startColumnIndex = 1;
#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
        }
        //set flying property
        if (Convert.ToSingle(towerStatTable.Rows[126].ItemArray[1]) == 1) electricalTowerFlight = true;	
        else electricalTowerFlight = false;
#endif
        return newArray;
    }
    float[,] setUpScytheArray()
    {
        float[,] newArray = new float[5,8];
        int startRowIndex = 82;
        int startColumnIndex = 1;
#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
        }
        //set flying property
        if (Convert.ToSingle(towerStatTable.Rows[127].ItemArray[1]) == 1) scytheTowerFlight = true;	
        else scytheTowerFlight = false;
#endif
        return newArray;
    }
    float[,] setUpTowernautArray()
    {
        float[,] newArray = new float[5,8];
        int startRowIndex = 98;
        int startColumnIndex = 1;
#if UNITY_4
        for (int i= 0; i< 5; i++)//go through 5 rows
        {
            for (int j=0; j<7; j++)//go through  7 columns
            {
                newArray[i,j] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[j+startColumnIndex]);
            }
            newArray[i,7] = Convert.ToSingle(towerStatTable.Rows[i+startRowIndex].ItemArray[9+startColumnIndex]);
        }
        //set flying property
        if (Convert.ToSingle(towerStatTable.Rows[128].ItemArray[1]) == 1) towernautTowerFlight = true;	
        else towernautTowerFlight = false;
#endif
        return newArray;
    }

    void convertTowerTable()
    {
        // Read gun tower data
        TowerReader towerReader = new TowerReader();        
        towerReader.type = "Gun";
        towerReader.towerProperties = setUpGunArray();
        towerReader.specialAbilities = setUpSpecialAbility (32, 1);
        towers.Add(towerReader.type, towerReader);

        // Read cannon tower data
        towerReader = new TowerReader();
        towerReader.type = "Cannon";
        towerReader.towerProperties = setUpCannonArray();
        towerReader.specialAbilities = setUpSpecialAbility(43, 1);
        towers.Add(towerReader.type, towerReader);
        
        // Read thresher tower data
        towerReader = new TowerReader();
        towerReader.type = "Poison";
        towerReader.towerProperties = setUpThresherArray();
        towerReader.specialAbilities = setUpSpecialAbility(57, 1);
        towers.Add(towerReader.type, towerReader);

        // Read electrical tower data
        towerReader = new TowerReader();
        towerReader.type = "Electrical";
        towerReader.towerProperties = setUpElectricalArray();
        towerReader.specialAbilities = setUpSpecialAbility(72, 1);
        towers.Add(towerReader.type, towerReader);
        
        // Read scythe tower data
        towerReader = new TowerReader();
        towerReader.type = "Scythe";
        towerReader.towerProperties = setUpScytheArray();
        towerReader.specialAbilities = setUpSpecialAbility(87, 1);
        towers.Add(towerReader.type, towerReader);

        // Read towernaut tower data
        towerReader = new TowerReader();
        towerReader.type = "Towernaut";
        towerReader.towerProperties = setUpTowernautArray();
        //towerReader.specialAbilities = setUpSpecialAbility(103, 1);
        towers.Add(towerReader.type, towerReader);
    }
#endregion //These are the functions that construct each type of tower array from towerStatTable's data.
    
    int[,] setUpWayPointArray()
    {
        
        int[,] newArray = new int[8,12];
        
        
        int startRowIndex = 41;
        //int startColumnIndex = 1;

#if UNITY_4
        for (int i = 0; i < 8; i++) //go through 8 rows
        {
            //I would REALLY like to just loop through columns, but because of import issues, I can't. I don't have any guarantee that they are adjacent.
            //If that import bug can be fix, a more elegant loop can be done
            newArray[i,0] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[1]);//col B
            newArray[i,1] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[3]);//col D
            newArray[i,2] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[5]);//col F
            newArray[i,3] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[7]);//col H
            newArray[i,4] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[11]);//col L
            newArray[i,5] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[14]);//col O
            newArray[i,6] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[15]);//col P
            newArray[i,7] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[17]);//col R
            newArray[i,8] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[18]);//col S
            newArray[i,9] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[20]);//col U
            newArray[i,10] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[21]);//col V
            newArray[i,11] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[23]);//col X
        }
#endif
        
        
        for (int i=0; i<GRID_WIDTH; i++)
        {
            for (int j=0; j<GRID_LENGTH; j++)
            {
                if(newArray[i,j] != 0)	
                {
                    numberOfWayPoints++;
                }
            }
        }
        
        
        return newArray;
    }
    
    int[,] setUpGridArray()
    {
        int[,] newArray = new int[8,12];
        
        
        int startRowIndex = 32;

#if UNITY_4
        for (int i = 0; i < 8; i++) //go through 8 rows
        {
            //I would REALLY like to just loop through columns, but because of import issues, I can't. I don't have any guarantee that they are adjacent.
            //If that import bug can be fix, a more elegant loop can be done
            newArray[i,0] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[1]);//col B
            newArray[i,1] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[3]);//col D
            newArray[i,2] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[5]);//col F
            newArray[i,3] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[7]);//col H
            newArray[i,4] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[11]);//col L
            newArray[i,5] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[14]);//col O
            newArray[i,6] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[15]);//col P
            newArray[i,7] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[17]);//col R
            newArray[i,8] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[18]);//col S
            newArray[i,9] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[20]);//col U
            newArray[i,10] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[21]);//col V
            newArray[i,11] = Convert.ToInt32( levelTable.Rows[i+startRowIndex].ItemArray[23]);//col X
        }
#endif
        return newArray;
    }
    
    public string GetEnemyReference(string enemyName){
        for (int i = 0 ; i < enemyNameArray.Length ; i++)
        {
            if (enemyNameArray[i, 0] == enemyName)
                return enemyNameArray[i, 1];
        }
        return string.Empty;
    }

    public int GetEnemyLevel(string enemyName){
        for (int i = 0 ; i < enemyNameArray.Length ; i++)
        {
            if (i >= enemyNameArray.Length)
            {
                Debug.Log("Enemy name array out of bounds, looking for " + enemyName);
                return -1;
            }
            if (enemyNameArray[i, 0] == enemyName)
                return i;
        }
        return -1;
    }

    string[,] setUpEnemyNameArray(){
        string[,] newArray = new string[27, 2];

        int startRowIndex = 7;
        int startColIndex = 1;
        int rowSkip = 0;
#if UNITY_4
        do{
            for (int i = 0; i < 3; i++)
            {
                newArray[rowSkip+i, 0] = Convert.ToString(monsterStatTable.Rows[i+startRowIndex].ItemArray[startColIndex]);
                newArray[rowSkip+i, 1] = Convert.ToString(monsterStatTable.Rows[i+startRowIndex].ItemArray[startColIndex + 9]);
            }
            rowSkip += 3;
            startRowIndex += 7;
        }while (Convert.ToString(monsterStatTable.Rows[startRowIndex].ItemArray[startColIndex]) != string.Empty);
#endif
        return newArray;
    }
    float[,] setUpEnemyStatArray(){
        float[,] newArray = new float[27,9];
        //columns are always 2-9, + 11
        //load in zombies, rows 7 8 9
        int startRowIndex = 7;
        int startColIndex = 2;
        int rowSkip = 0;

#if UNITY_4
        do {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 8; j++) {
                    newArray [rowSkip + i, j] = Convert.ToSingle (monsterStatTable.Rows [i + startRowIndex].ItemArray [j + startColIndex]);	
                }
                newArray [rowSkip + i, 8] = Convert.ToSingle (monsterStatTable.Rows [i + startRowIndex].ItemArray [11]);
            }
            rowSkip += 3;
            startRowIndex += 7;
        } while (Convert.ToString(monsterStatTable.Rows[startRowIndex].ItemArray[startColIndex-1]) != string.Empty);

        sueCooldown = Convert.ToSingle(monsterStatTable.Rows[38].ItemArray[3]);
        sueType = Convert.ToInt32(monsterStatTable.Rows[38].ItemArray[5]);
        shamanCooldown = Convert.ToSingle(monsterStatTable.Rows[45].ItemArray[7]);
        shamanRadius = Convert.ToSingle(monsterStatTable.Rows[45].ItemArray[3]);
        shamanStrength = Convert.ToSingle(monsterStatTable.Rows[45].ItemArray[5]);
        reggieDistance = Convert.ToSingle(monsterStatTable.Rows[52].ItemArray[3]);
        reggieSpeed = Convert.ToSingle(monsterStatTable.Rows[52].ItemArray[5]);
        reggieCooldown = Convert.ToSingle(monsterStatTable.Rows[52].ItemArray[7]);
        mummyRadius = Convert.ToSingle(monsterStatTable.Rows[59].ItemArray[3]);
        mummyPower = Convert.ToSingle(monsterStatTable.Rows[59].ItemArray[5]);
        mummyCooldown = Convert.ToSingle(monsterStatTable.Rows[59].ItemArray[7]);
#endif
        return newArray;
    }
#region Functions that set up individual monster stat arrays.  could maybe be combined.
    void setUpZombieArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                zombieArray[i,j] = enemyStatsArray[i,j];
            }
        }	
        return;
    }

    void setUpJoggedMickeyArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                joggedMickeyArray[i,j] = enemyStatsArray[i+3,j];
            }
        }	
        return;
        
    }

    void setUpBlindTommyArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                blindTommyArray[i,j] = enemyStatsArray[i+6,j];
            }
        }	
        return;
    }
    void setUpFrankensteamArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                frankensteamArray[i,j] = enemyStatsArray[i+9,j];
            }
        }	
        return;
    }
    void setUpSicklySueArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                sicklySueArray[i,j] = enemyStatsArray[i+12,j];
            }
        }	
        return;
    }
    void setUpShamanArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                shamanArray[i,j] = enemyStatsArray[i+15,j];
            }
        }	
        return;
    }
    void setUpRootReggieArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                rootReggieArray[i,j] = enemyStatsArray[i+18,j];
            }
        }	
        return;
    }
    void setUpMummyDearestArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                mummyDearestArray[i,j] = enemyStatsArray[i+21,j];
            }
        }	
        return;
    }
    void setUpWerefalloArray()
    {
        for (int i=0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)	
            {
                werefalloArray[i,j] = enemyStatsArray[i+24,j];
            }
        }	
        return;
    }
#endregion
    
    MonsterWave[,] setUpWaveArray()
    {
#if UNITY_4
        numOfWaves = Convert.ToInt32(levelTable.Rows[91].ItemArray[1]);
#endif
        //each row of the wave array is a wave of monsters
        //each column is a monster attack structure, holding info on what type of monster is there, as well as the delay and number
        MonsterWave[,] newArray = new MonsterWave[numOfWaves,6];
        int grp;
        int cat;
        int lvl;
        int delayOffset = 14;
        int typeOffset = 97;
#if UNITY_4
        for (int i = 0; i < numOfWaves;i++)
        {
            for (int j = 0; j < 6; j++)
            {	
                MonsterWave newMonster = new MonsterWave();
                newMonster.delay = Convert.ToInt32(levelTable.Rows[j].ItemArray[(3*i)+delayOffset]);
                newMonster.count = Convert.ToInt32(levelTable.Rows[j].ItemArray[(3*i)+delayOffset+1]);
                newMonster.type = Convert.ToInt32(levelTable.Rows[(j+typeOffset)].ItemArray[7]);
                grp = newMonster.type / 100;
                cat = newMonster.type / 10;
                cat = cat % 10;
                lvl = newMonster.type % 10;
                if (grp == 1)
                {
                    if (cat ==1)
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[97].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[98].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[99].ItemArray[16]);
                        }
                        
                    }
                    
                    else if (cat == 2)	
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[100].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[101].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[102].ItemArray[16]);
                        }
                        
                    }
                    
                    else
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[103].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[104].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[105].ItemArray[16]);
                        }
                        
                    }
                }
                
                
                else if (grp ==2)
                {
                    if (cat ==1)
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[106].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[107].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[108].ItemArray[16]);
                        }
                        
                    }
                    
                    else if (cat == 2)	
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[109].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[110].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[111].ItemArray[16]);
                        }
                        
                    }
                    
                    else
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[112].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[113].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[114].ItemArray[16]);
                        }
                        
                    }
                    
                }
                
                else
                {
                    if (cat ==1)
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[115].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[116].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[117].ItemArray[16]);
                        }
                        
                    }
                    
                    else if (cat == 2)	
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[118].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[119].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[120].ItemArray[16]);
                        }
                        
                    }
                    
                    else
                    {
                        if (lvl == 1)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[121].ItemArray[16]);
                        }
                        
                        else if (lvl == 2)
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[122].ItemArray[16]);
                        }
                        
                        else
                        {
                            newMonster.name=Convert.ToString(levelTable.Rows[123].ItemArray[16]);
                        }
                        
                    }
                }
            if (newMonster.type == 0)
            {
                newMonster.name = "None";	
            }
            newArray[i,j]=newMonster;	
        }
        
        typeOffset += 10;
            
    }
#endif

        return newArray;
        
    }
    
    //this function is a hub for a series of functions that fill out our various tower arrays.

    
    void convertMonsterTable(){
        
        //set up overall monster stat array
        enemyNameArray = setUpEnemyNameArray();
        enemyStatsArray = setUpEnemyStatArray();
        //zombie array is the first 3 rows of enemyStatsArray;
        setUpZombieArray();
        setUpJoggedMickeyArray();
        setUpFrankensteamArray();
        setUpSicklySueArray();
        setUpShamanArray();
        setUpRootReggieArray();
        setUpMummyDearestArray();
        setUpWerefalloArray();

        
    }
    
    int[] setUpAllowedTowers()
    {
        int[] newArray = new int[6];
#if UNITY_4
        for (int i = 0; i < 6; i++)
        {
            newArray[i] = Convert.ToInt32(levelTable.Rows[52+i].ItemArray[1]);
        }
#endif
        return newArray;
    }
    
    void convertLevelTable(){
        wayPointsArray = setUpWayPointArray();
        gridArray = setUpGridArray();
        wavesLevel1 = setUpWaveArray();
        allowedTowers = setUpAllowedTowers();
        return;
    }//this function is a hub for a series of functions that fill out waypoint, grid, and wave arrays
    
    void Start() {
        
        //assign camera
        cameraObj  = GameObject.Find("Main Camera").GetComponent<Camera>();
        readTables();//read in all the info from the excel sheet and store it in our global data tables
    
        convertTowerTable();//first, let's convert everything in the tower tables.  since we're working with globals anyways, I'm going to use a lot of nested functions for organizational purposes
        convertMonsterTable();//Monster stat info
        convertLevelTable();//Level/wave layout info
        
    }

#if UNITY_4
    DataTable readXLS( string filetoread, string sheetName)
    {
        // Must be saved as excel 2003 workbook, not 2007, mono issue really
        string con = "Driver={Microsoft Excel Driver (*.xls)}; DriverId=790; Dbq="+filetoread+"; Extended Properties=\"Excel ?.0;IMEX=1;\"";
        //Debug.Log(con);
        string yourQuery = "SELECT * FROM ["+sheetName+"$]"; 
        // our odbc connector 
        OdbcConnection oCon = new OdbcConnection(con); 
        // our command object 
        OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
        // table to hold the data 
        DataTable dtYourData = new DataTable("YourData"); 
        // open the connection 
        oCon.Open(); 
        // lets use a datareader to fill that table! 
        OdbcDataReader rData = oCmd.ExecuteReader(); 
        // now lets blast that into the table by sheer man power! 
        dtYourData.Load(rData); 
        // close that reader! 
        rData.Close(); 
        // close your connection to the spreadsheet! 
        oCon.Close(); 
        // wow look at us go now! we are on a roll!!!!! 
        // lets now see if our table has the spreadsheet data in it, shall we? 
        return dtYourData;
    }
#endif
}
