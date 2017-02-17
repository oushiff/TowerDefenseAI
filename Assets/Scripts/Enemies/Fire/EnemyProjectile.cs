#region Copyright
// <copyright file="EnemyProjectile.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 11:10 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public class EnemyProjectile : Projectile
{
    protected Enemy enemy;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start ();
        SetStats (level);
    }

    public void SetEnemy(Enemy _enemy)
    {
        enemy = _enemy;
    }

    public void SetStats(int level)
    {
        MonsterData.Level monster = GameData.instance.GetMonster(level);

        if (monster != null)
        {
            damage = monster.damage;
            range = monster.range;
        }
    }

    public void SetStats(Enemy enemy)
    {
        if (enemy != null){
            damage = enemy.properties.active.damage;
            range = enemy.properties.active.range;
        }
    }

    public void SetStats(float _damage, float _range, float _speed)
    {
        damage = _damage;
        range = _range;
        speed = _speed;
    }
}
