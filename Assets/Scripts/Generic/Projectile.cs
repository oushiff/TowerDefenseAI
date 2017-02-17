#region Copyright
// <copyright file="Projectile.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 8:48 AM </date>
#endregion
using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public string type;
    
    //assigning the level before instantiating it
    public int level = 0;
    public float damage;
    public float range;
    
    //not coming from excel
    public Transform target;
    public float speed = 10;
    public float distance;
    public int maxHits = 1;
    
    protected virtual void Awake()
    {
    }
    
    // Use this for initialization
    protected virtual void Start ()
    {
    }
    
    // Update is called once per frame
    protected virtual void Update ()
    {
        if (target != null){
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime*speed);
        }
        else{
            Destroy(gameObject);
        }
    }
}
