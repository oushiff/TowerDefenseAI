#region Copyright
// <copyright file="UndegroundTravel.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 1:52 AM </date>
#endregion
using UnityEngine;
using System.Collections;

public class UndegroundTravel : EnemyAbility {
    
    public float offset = -1f;
    private float distance = 0;
    private float speed = 0;

    private new Collider collider;

    protected override void Awake()
    {
        base.Awake();

        collider = this.GetComponent<Collider>();
    }

    protected override void InitializeAbility()
    {
        cooldown = enemy.baseEnemy.GetSpecialAbilityFloat("cooldown"); ;
        if (cooldown <= 0)
        {
            cooldown = 6f;
        }
        
        speed = enemy.baseEnemy.GetSpecialAbilityFloat("speed"); ;
        if (speed <=0)
        {
            speed = 1;
        }
        
        distance = enemy.baseEnemy.GetSpecialAbilityFloat("distance"); ;
        if (distance <= 0)
        {
            distance = 2;
        }
    }
    
    protected override void ExecuteAbility ()
    {
        // Change speed
        Enemy enemy = GetComponent<Enemy> ();
        float duration = distance / speed;
        collider.enabled = false;
        if (enemy != null) {
            Enemy.EnemyProperties changeSpeedProperty = new Enemy.EnemyProperties();
            changeSpeedProperty.speed = - (enemy.properties.active.originalSpeed - speed);
            changeSpeedProperty.offset = Vector3.down * offset;
            
            // Add property to change speed
            enemy.properties.AddProperties(changeSpeedProperty, duration);
        }

        // Restore position
        Invoke ("Restore", duration);
    }

    private void Restore()
    {
        collider.enabled = true;
        transform.position += Vector3.up * offset;
    }
}
