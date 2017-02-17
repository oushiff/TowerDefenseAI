#region Copyright
// <copyright file="DataReader.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 05/28/2016, 1:53 AM </date>
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Application = UnityEngine.Application;

public class DataReader : MonoBehaviour
{
    private static DataReader _instance = null;

    public static DataReader instance
    {
        set
        {
            _instance = value;
            if (_instance != null){
                _instance.Initialize();
            }
        }
        get
        {
            if (_instance == null){
                _instance = (DataReader) FindObjectOfType(typeof (DataReader));

                if (_instance == null){
                    GameObject newInstance = new GameObject("DataReader");
                    _instance = newInstance.AddComponent<DataReader>();
                    DontDestroyOnLoad(newInstance);
                }
            }

            return _instance;
        }
    }

#if UNITY_WEBGL
    public string baseUrl = "http://easleydunnproductions.com/Spurpunk//";
    public bool loadedMonsters, loadedTowers;

    public delegate void LevelLoaded(LevelInfo info);
    public delegate void DataLoaded();
#endif

    #region Static Calls


    public static string GetPath()
    {
        string path = string.Empty;
#if UNITY_EDITOR
        path = Path.Combine(Application.dataPath, "Data");
#else
#if UNITY_ANDROID
#elif UNITY_IOS
#else
        path = Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, "Data");
#endif
#endif
        return path;
    }

    public static string GetPath(string filename)
    {
        string path = string.Empty;
        path = Path.Combine(GetPath(), filename);
        return path;
    }

    #endregion

    #region Levels
    private int noLevels = -1;

    /// <summary>
    /// Get the total number of available levels
    /// </summary>
    /// <returns></returns>
    public int GetNolevels()
    {
        if (noLevels < 0)
            this.CountLevels();
        return noLevels;
    }

    private void CountLevels()
    {
#if !UNITY_WEBPLAYER
        string path = GetPath();
        if (Directory.Exists(path)){
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("Level*.xml");
            noLevels = info.Length;
        }
        else{
            Debug.LogWarning("No Data folder found");
        }
#endif
    }

    public LevelInfo LoadLevel(int id)
    {
        string levelPath = GetPath("Level" + id + ".xml");
        if (File.Exists(levelPath)){
            var serializer = new XmlSerializer(typeof (LevelInfo));
            var stream = new FileStream(levelPath, FileMode.Open);
            LevelInfo level = serializer.Deserialize(stream) as LevelInfo;
            stream.Close();
            return level;
        }
        return null;
    }

    [System.Serializable]
    [XmlRoot("Level")]
    public class LevelInfo
    {
        [XmlElement("Grid")] public Grid grid;

        [XmlArray("Waypoints")] [XmlArrayItem("row")] public List<string> waypoints = new List<string>();

        [XmlArray("Waves")] [XmlArrayItem("Wave")] public List<Wave> waves = new List<Wave>();

        [XmlArray("Towers")] [XmlArrayItem("Tower")] public List<string> towers = new List<string>();

        [XmlElement("Money")] public int money;
    }

    [XmlType("Grid")]
    public class Grid
    {
        [XmlAttribute("rows")] public int rowsCount;
        [XmlAttribute("columns")] public int columnsCount;
        [XmlElement("row")] public List<string> rows;
    }

    public class Wave
    {
        [XmlElement("Monster")] public List<Monster> monsters = new List<Monster>();
    }

    public class Monster
    {
        public float Seconds;
        public int Amount;
        public int ID;
    }

    #endregion

    #region Towers

    public TowersArray towers;

    public void LoadTowers()
    {
        string towersPath = GetPath("Towers.xml");
        if (File.Exists(towersPath)){
            var serializer = new XmlSerializer(typeof (TowersArray));
            var stream = new FileStream(towersPath, FileMode.Open);
            towers = serializer.Deserialize(stream) as TowersArray;
            stream.Close();
        }
    }

    [System.Serializable]
    [XmlRoot("Towers")]
    public class TowersArray
    {
        [XmlElement("Tower")] public List<TowerData> towers = new List<TowerData>();
    }

    [System.Serializable]
    public class TowerData
    {
        public string TowerName;
        public int air;
        [XmlElement("Level")] public TowerLevelData levelData;
    }

    [System.Serializable]
    public class TowerLevelData
    {
        public string levelName;
        public float damage;
        public float hp;
        public float armor;
        public float range;
        public float rateOfFire;
        public float cost;
        public float effect;
        public float rebootTime;
        [XmlElement("SpecialAbility")] public TowerSpeciallAbilityData specialAbility;
    }

    [System.Serializable]
    public class TowerSpeciallAbilityData
    {
        [XmlElement("sa_DamagePercentage")] public float damagePercentage;
        [XmlElement("sa_Damage")] public float damage;
        [XmlElement("sa_Duration")] public float duration;
        [XmlElement("sa_Range")] public float range;
        [XmlElement("sa_Reduction")] public float reduction;
        [XmlElement("sa_Projectiles")] public float projectiles;
        [XmlElement("sa_chancePercentage")] public float chancePercentage;
    }

    #endregion

    #region Monsters

    public MonstersArray monsters;

    public void LoadMonsters()
    {
        string monstersPath = GetPath("Monsters.xml");
        if (File.Exists(monstersPath)){
            var serializer = new XmlSerializer(typeof (MonstersArray));
            var stream = new FileStream(monstersPath, FileMode.Open);
            monsters = serializer.Deserialize(stream) as MonstersArray;
            stream.Close();
        }
    }

    [System.Serializable]
    [XmlRoot("Monsters")]
    public class MonstersArray
    {
        [XmlElement("Monster")] public List<MonsterData> monsters = new List<MonsterData>();
    }

    [System.Serializable]
    public class MonsterData
    {
        public string MonsterName;
        public string levelName;
        public float damage;
        public float hp;
        public float armor;
        public float range;
        public float rateOfFire;
        public float gain;
        public float speed;
        public float mana;
        public string reference;
        public int air;
        public string specialAbility;
        public string specialAbilityName1;
        public string specialAbilityValue1;
        public string specialAbilityName2;
        public string specialAbilityValue2;
        public string specialAbilityName3;
        public string specialAbilityValue3;
        public string specialAbilityName4;
        public string specialAbilityValue4;
        public string specialAbilityName5;
        public string specialAbilityValue5;
    }

    #endregion

    #region Cards
    public CardsArray cards;

    public void LoadCards()
    {
        string monstersPath = GetPath("Cards.xml");
        if (File.Exists(monstersPath))
        {
            var serializer = new XmlSerializer(typeof(CardsArray));
            var stream = new FileStream(monstersPath, FileMode.Open);
            cards = serializer.Deserialize(stream) as CardsArray;
            stream.Close();
        }
    }

    [System.Serializable]
    [XmlRoot("Cards")]
    public class CardsArray
    {
        [XmlElement("Card")]
        public List<CardData> cards = new List<CardData>();
    }

    [System.Serializable]
    public class CardData
    {
        public string Name;
        [XmlElement("Base")]
        public CardStats baseStats;
        
        [XmlElement("Upgrade")]
        public CardStats upgradeStats;
    }

    [System.Serializable]
    public class CardStats
    {
        public int BlockPath;
        public float CharacterSpeed;
        public int Quantity;
        public float Duration;
        public float Radius;
        public int PerformonCollision;
        public float ProportionalEffect;
        public float DamageInterval;
        public int TargetDoesDamage;
        public int TargetTakesDamage;
        public string TargetCharacter;
        public float Cooldown;
        public string Description;
    }
    #endregion

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
#if UNITY_WEBGL
        _instance.StartCoroutine(LoadMonstersWeb());
        _instance.StartCoroutine(LoadTowersWeb());
#else
        _instance.LoadMonsters();
        _instance.LoadTowers();
        _instance.LoadCards();
#endif
    }

#if UNITY_WEBGL
    public IEnumerator LoadMonstersWeb()
    {
        WWW www = null;
#if !UNITY_EDITOR
        int indexPageIndex = Application.absoluteURL.IndexOf("index.html");
        if (indexPageIndex < 0) indexPageIndex = Application.absoluteURL.Length;
        baseUrl = Application.absoluteURL.Substring(0, indexPageIndex-1);
#endif

        www = new WWW(string.Format("{0}/Data/Monsters.xml", this.baseUrl));
        yield return www;

        if (string.IsNullOrEmpty(www.error)){
            var serializer = new XmlSerializer(typeof(MonstersArray));
            TextReader textReader = new StringReader(www.text);
            monsters = serializer.Deserialize(textReader) as MonstersArray;
            textReader.Close();
        }
        loadedMonsters = true;
    }

    public IEnumerator LoadTowersWeb()
    {
        WWW www = null;
#if !UNITY_EDITOR
        int indexPageIndex = Application.absoluteURL.IndexOf("index.html");
        if (indexPageIndex < 0) indexPageIndex = Application.absoluteURL.Length;
        baseUrl = Application.absoluteURL.Substring(0, indexPageIndex-1);
#endif

        www = new WWW(string.Format("{0}/Data/Towers.xml", this.baseUrl));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            var serializer = new XmlSerializer(typeof(TowersArray));
            TextReader textReader = new StringReader(www.text);
            towers = serializer.Deserialize(textReader) as TowersArray;
            textReader.Close();
        }
        loadedTowers = true;
    }

    public IEnumerator WaitMonsterData(DataLoaded dataLoaded)
    {
        while (!loadedMonsters)
            yield return new WaitForSeconds(0.25f);
        dataLoaded();
    }

    public IEnumerator WaitTowerData(DataLoaded dataLoaded)
    {
        while (!loadedTowers)
            yield return new WaitForSeconds(0.25f);
        dataLoaded();
    }

    public IEnumerator LoadLevelWeb(int id, LevelLoaded levelLoaded)
    {
        while (!loadedMonsters || !loadedTowers)
            yield return new WaitForSeconds(0.25f);

        WWW www = null;
#if !UNITY_EDITOR
        int indexPageIndex = Application.absoluteURL.IndexOf("index.html");
        if (indexPageIndex < 0) indexPageIndex = Application.absoluteURL.Length;
        baseUrl = Application.absoluteURL.Substring(0, indexPageIndex-1);
#endif

        www = new WWW(string.Format("{0}/Data/Level{1}.xml", this.baseUrl, id));
        yield return www;

        if (string.IsNullOrEmpty(www.error)){
            var serializer = new XmlSerializer(typeof (LevelInfo));
            TextReader textReader = new StringReader(www.text);
            LevelInfo levelInfo = serializer.Deserialize(textReader) as LevelInfo;
            textReader.Close();
            levelLoaded(levelInfo);
        }
        else{
            levelLoaded(null);
        }
    }
#endif
}
