#region Copyright
// <copyright file="TowerFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 5:34 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class TowerFire : Projectile
{
    public Tower tower;
    public bool flying;
    public float armorPenetration;
    public float extradamage;
    public int hitCount = 0;

    protected override void Awake ()
    {
        base.Awake ();
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start ();
        damage = tower.properties.active.damage;
        range = tower.properties.active.range;
        armorPenetration = tower.properties.active.armorPenetration;
        extradamage = 0;
    }

    protected override void Update()
    {
        base.Update ();
        if (extradamage > 0)
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
    }
    
    protected virtual void OnTriggerEnter(Collider intruderCollider)
    {
        if(intruderCollider.CompareTag("Enemy"))
        {
            Enemy intruder = intruderCollider.GetComponent<Enemy>();
            //if (intruder == null)
            //	intruder = intruderCollider.transform.parent.GetComponent<Enemy>();
            
            if (intruder != null && intruder.isValidTarget(flying))
            {
                if (maxHits-- > 0)
                {
                    HitEnemy(intruder, true);
                }
            }
        }
    }

    protected virtual void HitEnemy(Enemy enemy, bool destroyProjectile){
        if (enemy.isValidTarget(flying)) {
            enemy.causeDamage (damage + extradamage, armorPenetration);
            
            if (destroyProjectile)
                Destroy (gameObject);
        }
    }
}
