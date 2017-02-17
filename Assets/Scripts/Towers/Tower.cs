#region Copyright
// <copyright file="Tower.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/09/2016, 8:16 AM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {
    
    //variable to store the current level of upgrades from 0 to 4
    public int level = 0;
    public string type;
    private bool online;
    public TowerData.Level baseTower;
    
    [System.Serializable]
    public class TowerProperties : Properties{
        public float healTime;
        public float armorPenetration;
        
        public override void Load(string type, int level)
        {
            TowerData.Level baseTower = GameData.instance.GetTower(type, level);
            damage = baseTower.damage;
            hp = maxHp = baseTower.hp;
            armor = baseTower.armor;
            range = baseTower.range;
            rateOfFire = baseTower.rateOfFire;
            coins = baseTower.cost;
            armorPenetration = baseTower.effect;
            healTime = baseTower.rebootTime;
            flying = baseTower.tower.air;
        }
        
        public override void Add(Properties addedProperties){
            base.Add (addedProperties);

            if (addedProperties is TowerProperties) {
                healTime += ((TowerProperties) addedProperties).healTime;
                armorPenetration += ((TowerProperties) addedProperties).armorPenetration;
            }
        }
        
        public override void Remove(Properties removedProperties){
            base.Remove (removedProperties);

            if (removedProperties is TowerProperties) {
                healTime -= ((TowerProperties) removedProperties).healTime;
                armorPenetration -= ((TowerProperties) removedProperties).armorPenetration;
            }
        }
    }

    [System.Serializable]
    public class TowerPropertiesQueue : PropertiesQueue<TowerProperties>{}
    public TowerPropertiesQueue properties;

    public int firePauseTime = 0;
    public float rotationSpeed = 5;
    
    //variables used in calculation
    private float nextFireTime;
    private float nextMoveTime;
    protected List<Transform> colliderList = new List<Transform>();
    
    public GameObject myProjectile;
    Transform myTarget;
    public Transform[] nozzles;
    public Transform turretSphere;

    public HealthBar healthBar;

    //variable to store the placement on the grid
    public float extradamage;
    public Sprite sprite;

    protected virtual void Awake()
    {
        properties = null;
    }

    protected virtual void Start () {
        if (properties == null) {
            baseTower = GameData.instance.GetTower(type, level);

            TowerProperties baseProperties = new TowerProperties ();
            baseProperties.Load (type, level);
            properties = new TowerPropertiesQueue ();
            properties.Initialize (baseProperties, Time.time);
        }
        SetRange ();

        online = true;
        //assign the towerRange to the sphere collider radius
        this.GetComponent<SphereCollider>().radius = 2 * (float) properties.active.range;

        extradamage = 0;
        nextFireTime = Time.time + (float) properties.active.rateOfFire;

        healthBar = UIManager.instance.GetHealthBar();
        healthBar.UpdateName("Tower: " + type);
        healthBar.UpdateGUI(transform.position, properties.active.hp / properties.active.maxHp, 4);

    }

    public float EvaluateSpecialAbility(GameData.GameProperies stat, bool force = false)
    {
        if (baseTower.specialAbility != null)
        {
            if (force || UnityEngine.Random.Range(0.0f, 1.0f) < baseTower.GetSpecialProperty(GameData.GameProperies.CHANCE_PERCENTAGE))
            {
                return baseTower.GetSpecialProperty(stat);
            }
        }
        return -1;
    }

    private void SetRange()
    {
        if (GetComponent<Collider>() is SphereCollider)
            ((SphereCollider)GetComponent<Collider>()).radius = properties.active.range;
    }

    protected virtual void Update () {
        healthBar.UpdateHp(properties.active.hp / properties.active.maxHp);
        properties.Update (Time.deltaTime);

        CheckIfDestroyed ();

        Enemy selectedEnemy = UIManager.instance.GetSelectedEnemy();
        if (selectedEnemy != null && Vector3.Distance(this.transform.position, selectedEnemy.transform.position) <= properties.active.range)
        {
            if (myTarget == null)
                nextFireTime = Time.time + (float) properties.active.rateOfFire;
            myTarget = selectedEnemy.transform;            
        }	
        else if(colliderList != null && colliderList.Count > 0)
        {
            if(colliderList[0] != null)
            {
                myTarget = colliderList[0];
            }
            else
            {
                colliderList.RemoveAt(0);
            }
        }
        else
        {
            myTarget = null;
        }
        
        if(myTarget != null)
        {
            if(Time.time > nextMoveTime && online)
            {
                Quaternion desiredRotation = getRotation(myTarget.position + myTarget.forward * 0.25f);
                turretSphere.rotation = Quaternion.Lerp(turretSphere.rotation, desiredRotation, Time.deltaTime * rotationSpeed);

                if (Quaternion.Angle(turretSphere.rotation, desiredRotation) > 20f)
                {
                    if (Time.time >= nextFireTime)
                        nextFireTime = Time.time + Time.deltaTime;
                    else
                        nextFireTime = Time.time + (float) properties.active.rateOfFire;
                }
            }

            if(Time.time >= nextFireTime)
            {
                if (online)
                {
                    Fire(1);
                }
                nextFireTime = Time.time + (float) properties.active.rateOfFire;
                nextMoveTime = Time.time + firePauseTime;
            }	
        }
    }
    
    //void OnGUI()
    //{
    //    healthBar.UpdateGUI(/*type + "Tower: ", */transform.position, properties.active.hp / properties.active.maxHp/*, level*/);
    //}

    public virtual void AddIntruder (Enemy intruder)
    {
        colliderList.Add (intruder.transform);
    }

    public virtual void RemoveIntruder (Enemy intruder)
    {
        if (colliderList.Contains(intruder.transform))
            colliderList.Remove (intruder.transform);
    }

    public void NotifyEnemyDeath (Enemy enemy)
    {
        RemoveIntruder (enemy);
    }
    
    protected virtual void OnTriggerEnter(Collider intruder)
    {
        if (intruder.CompareTag ("Enemy")) {
            Enemy enemy = intruder.GetComponent<Enemy> ();
            if (enemy == null && intruder.transform.parent != null)
                enemy = intruder.transform.parent.GetComponent<Enemy> ();
            
            if (enemy != null) {
                //only aim at targets we can hit
                if (enemy.properties.active.flying == false || properties.active.flying == true) {
                    AddIntruder (enemy);
                }
            }
        }
    }
    
    protected virtual void OnTriggerExit(Collider intruder)
    {
        if (intruder.CompareTag ("Enemy")) {
            Enemy enemy = intruder.GetComponent<Enemy> ();
            if (enemy == null && intruder.transform.parent != null)
                enemy = intruder.transform.parent.GetComponent<Enemy> ();
            
            if (enemy != null) {
                //only aim at targets we can hit
                if (enemy.properties.active.flying == false || properties.active.flying == true) {
                    RemoveIntruder (enemy);
                }
            }
        }
    }
    
    Quaternion getRotation(Vector3 targetPos)
    {
        var aimPoint = targetPos - transform.position;
        return Quaternion.LookRotation(aimPoint);
    }
    
    public bool IsTowerUpgradable()
    {
        return level < 3;
    }

    public bool UpgradeToTower(int upgradeToAmount)
    {
        if (IsTowerUpgradable() && Currency.instance.UseCoins((int) GameData.instance.GetTower(type, upgradeToAmount).GetProperty(GameData.GameProperies.COST)))
        {
            level = upgradeToAmount;
            properties.Reset(type, level);
            SetRange();
            return true;
        }
        return false;
    }


    public bool UpgradeTower(int upgradeAmount)
    {
        return UpgradeToTower(level + upgradeAmount);
    }

    protected void CheckIfDestroyed ()
    {
        if (properties.active.hp <= 0 && online) {
            properties.active.hp = 0;
            gameObject.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
            online = false;
            InvokeRepeating ("Regenerate", 1, 1);
        }
    }
    
    protected void Regenerate ()
    {
        if (!online)
        {
            properties.active.hp += properties.active.maxHp/ properties.active.healTime;
        }
        
        if (properties.active.hp >= properties.active.maxHp)
        {
            properties.active.hp = properties.active.maxHp;
            online = true;
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            CancelInvoke("Regenerate");
        }
    }

    public virtual void Fire (int noProjectiles)
    {
        for( int j = 0 ; j < nozzles.Length ; ++j)
        {
            for (var i=0 ; i < noProjectiles ; i++)
            {

                //assign the level to the projectile
                SpawnProjectile(nozzles[j].position, nozzles[j].rotation);

                //Reset after firing
                extradamage = 0;
            }
        }
    }

    public virtual TowerFire SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        if (myTarget != null){
            GameObject spawnedProjectile = Instantiate(myProjectile, position, rotation) as GameObject;
            if (spawnedProjectile != null){
                spawnedProjectile.transform.parent = transform.parent;

                TowerFire towerFire = spawnedProjectile.GetComponent<TowerFire>();
                towerFire.target = myTarget;

                towerFire.tower = this;
                towerFire.level = level;
                towerFire.flying = properties.active.flying;
                towerFire.extradamage = extradamage;

                Physics.IgnoreCollision(spawnedProjectile.GetComponent<Collider>(),
                    turretSphere.GetComponent<Collider>());

                return towerFire;
            }
        }
        return null;
    }

    public virtual void CauseDamage(float damageaAmount)
    {
        if(online)
        {
            if(damageaAmount > properties.active.armor)
            {
                properties.active.hp-=(damageaAmount - properties.active.armor);
            }
            if(properties.active.hp <= 0.0f)
            {
                properties.active.hp = 0.0f;
            }
        }
    }

    public virtual void PowerShot(float damage){}

    public bool IsNextUpgradeSpecial()
    {
        return GameData.instance.GetTower(type, level + 1).HasSpecialAbility();
    }

    public TowerData.Level GetUpgrade(int addedIdx)
    {
        return GameData.instance.GetTower(type, level + addedIdx);
    }
}
