#region Copyright
// <copyright file="EnemyAbility.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 11:16 PM </date>
#endregion
using UnityEngine;
using System.Collections;

public abstract class EnemyAbility : MonoBehaviour
{
    public Enemy enemy;

    protected float cooldown = 0f;
    protected Vector2 position = Vector2.zero;
    
    protected abstract void InitializeAbility ();

    protected virtual void ScheduleAbility(){
        InvokeRepeating ("ExecuteAbility", cooldown, cooldown);
    }

    protected abstract void ExecuteAbility();

    protected virtual void Awake()
    {
        enemy = this.GetComponent<Enemy>();
    }
    protected virtual void Start()
    {
        InitializeAbility ();
        ScheduleAbility ();
    }

    protected virtual void UpdatePosition()
    {
        position.x = transform.position.x;
        position.y = transform.position.z;
    }

    protected float Distance(float x, float y)
    {
        float diffX = x - position.x, diffY = y - position.y;
        return Mathf.Sqrt(diffX * diffX + diffY * diffY);
    }
}
