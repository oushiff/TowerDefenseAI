#region Copyright
// <copyright file="Enemy.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/09/2016, 1:14 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public MonsterData.Level baseEnemy;
    public int level = 0;
    
    //variables to store the properties of the tower
    [System.Serializable]
    public class EnemyProperties : Properties
    {
        public float speed = 0;
        public float mana = 0;
        public float originalSpeed = 0;
        public float shield = 0;
        public Vector3 offset = Vector3.zero;

        public void Load(MonsterData.Level baseProperties)
        {
            damage = baseProperties.damage;
            hp = maxHp = baseProperties.hp;
            armor = baseProperties.armor;
            range = baseProperties.range;
            rateOfFire = baseProperties.rateOfFire;
            coins = baseProperties.gain;
            this.originalSpeed = speed = baseProperties.speed;
            mana = baseProperties.mana;
            flying = baseProperties.monster.air;
        }

        public void ResetSpeed()
        {
            speed = this.originalSpeed;
        }
        
        public override void Add(Properties addedProperties){
            base.Add (addedProperties);

            if (addedProperties is EnemyProperties)
            {
                speed += ((EnemyProperties) addedProperties).speed;
                mana += ((EnemyProperties) addedProperties).mana;
                shield += ((EnemyProperties) addedProperties).shield;
                offset += ((EnemyProperties) addedProperties).offset;
            }
        }
        
        public override void Remove(Properties removedProperties){
            base.Remove (removedProperties);

            if (removedProperties is EnemyProperties) {
                speed -= ((EnemyProperties)removedProperties).speed;
                mana -= ((EnemyProperties)removedProperties).mana;
                shield -= ((EnemyProperties)removedProperties).shield;
                offset -= ((EnemyProperties) removedProperties).offset;
            }
        }
    }

    [System.Serializable]
    public class EnemyPropertiesQueue : PropertiesQueue<EnemyProperties>{}
    public EnemyPropertiesQueue properties;

    //path traversal
    public Transform[] wayPoints;
    public int nextPointIndex = 1;
    public float distance;

    public HealthBar healthBar;
 
    List<SoulTower> SoulWellsAttached = new List<SoulTower>();

    void Start()
    {
        this.baseEnemy = GameData.instance.GetMonster(level);

        //load enemy paramaters from the excelsheet
        EnemyProperties baseProperties = new EnemyProperties();
        baseProperties.Load(this.baseEnemy);
        properties = new EnemyPropertiesQueue ();
        properties.Initialize (baseProperties, Time.time);

        InvokeRepeating("Attack", (float) properties.active.rateOfFire, (float) properties.active.rateOfFire);
        //dynamic health bar

        wayPoints = Grid.instance.wayPoints;
        //position the object at the start of the path
        if (nextPointIndex > 0 && nextPointIndex < wayPoints.Length && wayPoints.Length > 0 && wayPoints[nextPointIndex - 1] != null)
        {
            this.transform.position = wayPoints[nextPointIndex-1].position;
        }

        healthBar = UIManager.instance.GetHealthBar();
        healthBar.UpdateName(this.baseEnemy.name);
    }

    void Update()
    {
        properties.Update (Time.deltaTime);
        healthBar.UpdateGUI(/*this.baseEnemy.name, */this.transform.position, properties.active.hp / properties.active.maxHp/*, level*/, 3);

        MoveForward();
    }

    //void OnGUI()
    //{
    //    healthBar.UpdateGUI(/*this.baseEnemy.name, */this.transform.position, properties.active.hp / properties.active.maxHp/*, level*/);
    //}

    void Attack()
    {
        if ((2 * properties.active.hp) <= properties.active.maxHp )
        {
            for (int i = 0; i < GamePlay.instance.activeTowers.Count; i++)
            {
                float distance = Vector3.Distance(this.transform.position, GamePlay.instance.activeTowers[i].transform.position);
                if (distance <= properties.active.range)
                {
                    properties.active.speed = 0;
                    Invoke("FinishAttack", (float) properties.active.rateOfFire  * 0.5f);
                    break;
                }
            }
        }
    }

    void FinishAttack()
    {
        for (int i = 0; i < GamePlay.instance.activeTowers.Count; i++)
    {
            float distance = Vector3.Distance(this.transform.position, GamePlay.instance.activeTowers[i].transform.position);
            if (distance <= properties.active.range)
            {
                GamePlay.instance.activeTowers[i].GetComponent<Tower>().CauseDamage(properties.active.damage);
            }
        }
        properties.active.ResetSpeed();
    }

    public void AddSoulWell(SoulTower tower)
    {
        if (!SoulWellsAttached.Contains(tower))
        {
            SoulWellsAttached.Add(tower);
        }
    }

    public void RemoveSoulWell(SoulTower tower)
    {
        if (SoulWellsAttached.Contains(tower))
        {
            SoulWellsAttached.Remove(tower);
        }
    }

    void notifySoulWellDied()
    {
        for (int i = 0 ; i < SoulWellsAttached.Count ; ++i)
        {
            SoulWellsAttached[i].NotifyDied(transform);
        }
    }

    public void causeDamage(float damageValue, float armorPenetration)
    {
        float inflictedDamage = damageValue - Mathf.Max(0, properties.active.armor - armorPenetration);
        causeDamage(inflictedDamage);
    }
    
    private void causeDamage(float damageValue)
    {
        if (damageValue > 0){
            if (properties.active.shield > 0){
                if (properties.active.shield >= damageValue){
                    properties.active.shield = properties.active.shield - damageValue;
                    damageValue = 0;
                }
                else{
                    damageValue = damageValue - properties.active.shield;
                    properties.active.shield = 0;
                }
            }

            if (damageValue > 0)
            {
                properties.active.hp = properties.active.hp - damageValue;

                // Check remaining health
                if (properties.active.hp <= 0)
                {
                    UIManager.instance.ReleaseHealthBar(healthBar);
                    Currency.instance.GainCoins((int) properties.active.coins);
                    //GamePlay.activeEnemies.Remove(this);
                    GamePlay.instance.KillEnemy(this, false);
                    notifySoulWellDied();
                    Destroy(gameObject);
                }
            }
        }
    }

    public bool isValidTarget (bool flying)
    {
        return flying || properties.active.flying == false;
    }

    private void MoveForward()
    {
        //path traversal
        if (wayPoints.Length > 0 && wayPoints[0] != null){
            Vector3 nextPosition = wayPoints[nextPointIndex].position + properties.active.offset;
            distance = Vector3.Distance(transform.position, nextPosition);

            // if enemy hasn't reached next waypoint, lerp towards it
            if (distance > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * (float) properties.active.speed);
            }
            else
            {
                // If enemy hasn't reached the exit, target next waypoint
                if (nextPointIndex != (wayPoints.Length - 1))
                {
                    nextPointIndex++;
                }
                // Reached exit, destroy the enemy
                else{
                    UIManager.instance.ReleaseHealthBar(healthBar);
                    GamePlay.instance.RemoveEnemy(this);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void PushBack (float pushBackDistance)
    {
        //path traversal
        if (wayPoints.Length > 0 && wayPoints[0] != null)
        {
            int currentPointIndex = nextPointIndex - 1;
            distance = Vector3.Distance(transform.position, wayPoints[currentPointIndex].position + properties.active.offset);
            while (pushBackDistance > 0 && distance > pushBackDistance)
            {
                transform.position = wayPoints[currentPointIndex].position + properties.active.offset;
                pushBackDistance -= distance;
                currentPointIndex--;
                // Returned to starting point
                if (currentPointIndex < 0)
                {
                    nextPointIndex = 1;
                    return;
                }
                distance = Vector3.Distance(transform.position, wayPoints[currentPointIndex].position + properties.active.offset);
            }
            nextPointIndex = currentPointIndex + 1;
            transform.position = transform.position - (pushBackDistance * (wayPoints[nextPointIndex].position - wayPoints[currentPointIndex].position) + properties.active.offset);
        }
    }
}
