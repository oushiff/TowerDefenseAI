#region Copyright
// <copyright file="SoulTower.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/08/2016, 11:57 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using SimpleJSON;

public class SoulTower : Tower {
    private int numKills;
    public double powershotPower = 0;
    
    private List<Tower> linkedTowers = new List<Tower>();
    public new PropertiesQueue<SoulProperties> properties;

    public class SoulProperties : Tower.TowerProperties{
        public JSONNode N;

        public float powerShotPower;
        public int requiredKills;
        
        public void Read (string jsonString)
        {
            N = JSON.Parse (jsonString);
        }

        public override void Load(string type, int level)
        {
            damage = N[level]["damage"].AsFloat;
            maxHp = hp = N[level]["hp"].AsFloat;
            armor = N[level]["armor"].AsFloat;
            range = N[level]["range"].AsFloat;
            rateOfFire = N[level]["rateOfFire"].AsFloat;
            coins = N[level]["cost"].AsFloat;
            armorPenetration = N[level]["splashArea"].AsFloat;
            healTime = N[level]["healTime"].AsFloat;
            flying = N[level]["flying"].AsBool;
            requiredKills = N[level]["requiredKills"].AsInt;
            powerShotPower = N[level]["powershotPower"].AsFloat;
        }
        
        public override void Add(Properties addedProperties){
            base.Add (addedProperties);
            
            if (addedProperties is SoulProperties) {
                powerShotPower += ((SoulProperties) addedProperties).powerShotPower;
                requiredKills += ((SoulProperties) addedProperties).requiredKills;
            }
        }
        
        public override void Remove(Properties removedProperties){
            base.Remove (removedProperties);
            
            if (removedProperties is SoulProperties) {
                powerShotPower -= ((SoulProperties) removedProperties).powerShotPower;
                requiredKills -= ((SoulProperties) removedProperties).requiredKills;
            }
        }
    }

    protected override void Awake(){
        properties = new PropertiesQueue<SoulProperties>();
        string jsonString = string.Empty;

#if !UNITY_WEBPLAYER
        jsonString = System.IO.File.ReadAllText(Application.dataPath +  "/soultowerstats.json");
        SoulProperties baseProperties = new SoulProperties ();
        baseProperties.Read(jsonString);
        properties.Initialize (baseProperties, Time.time);
#endif
    }

    protected override void Start()
    {
        type = "Soul";
        properties.SyncTime (Time.time);
        base.Start ();
    }

    protected override void Update(){
        CheckIfDestroyed ();
    }
    
    public override void AddIntruder (Enemy intruder)
    {
        base.AddIntruder (intruder);
        intruder.AddSoulWell (this);
    }
    
    public override void RemoveIntruder (Enemy intruder)
    {
        base.RemoveIntruder (intruder);
        intruder.RemoveSoulWell (this);
    }

    public void LinkTower(Tower tower)
    {
        if(tower != null && tower != this)
        {
            linkedTowers.Add(tower);
        }
    }

    public void NotifyDied(Transform enemy)
    {
        if (colliderList.Contains(enemy))
        {
            colliderList.Remove(enemy);
        
            if(++numKills == ((SoulProperties) properties.active).requiredKills)
            {
                numKills = 0;
                sendSuperPower();
            }
        }
    }
    
    public void sendSuperPower()
    {
        for (int i = 0; i < linkedTowers.Count; i++)
            linkedTowers [i].PowerShot (((SoulProperties) properties.active).powerShotPower / linkedTowers.Count);
    }
}