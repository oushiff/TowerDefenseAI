#region Copyright
// <copyright file="EnemyFire.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 03/08/2016, 11:07 PM </date>
#endregion
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFire : MonoBehaviour {

    public float damage = 2;
    public float range = 2;
    public float speed = 1;
    public float originalSpeed = 1;
    public float rateOfFire = 1;
    public GameObject myProjectile;

    private Enemy enemy;
    List<Transform> colliderList;
    Quaternion desiredRotation;
    float nextFireTime;

    void Awake(){
        enemy = transform.parent.GetComponent<Enemy> ();
    }

    // Use this for initialization
    void Start () {
        this.GetComponent<SphereCollider>().radius = (float) range;
        colliderList = new List<Transform>();
        nextFireTime = Time.time + rateOfFire;
    }

    void Update () {
        loadParameters();

        if(Time.time > nextFireTime)
        {
            if(colliderList.Count > 0)
            {
                if(colliderList[colliderList.Count-1])
                {
                    if(originalSpeed > speed)
                    {
                        Transform myTarget = colliderList[colliderList.Count-1];
                        this.transform.rotation = getRotation(myTarget.position);
                        StartCoroutine(FireProjectile());
                    }
                }
            }
            nextFireTime = Time.time + rateOfFire;
        }
    }

    IEnumerator FireProjectile()
    {
        if (myProjectile != null) {
            int repeats = 5;
            float waitTime = (float)rateOfFire / (2 * repeats);
            EnemyProjectile projectile = null;
            do {
                GameObject newProjectile = Instantiate (myProjectile, this.transform.position, this.transform.rotation) as GameObject;
                newProjectile.transform.parent = transform.parent.parent;
                if (newProjectile != null)
                    projectile = newProjectile.GetComponent<EnemyProjectile> ();
                else {
                    Debug.LogWarning ("Unable to spawn projectile for " + gameObject.name);
                    yield return new WaitForSeconds (waitTime);
                }

                if (projectile != null){
                    projectile.SetEnemy(enemy);
                    projectile.SetStats(damage, range, speed);
                }
                else{
                    Debug.LogWarning("Unable to get projectile for " + gameObject.name);
                    yield return new WaitForSeconds(waitTime);
                }
            } while (projectile == null && repeats-- > 0);
        }
    }
    
    void OnTriggerEnter(Collider intruder)
    {
        if(intruder.gameObject.tag == "Tower")
        {
            colliderList.Add(intruder.transform);
        }
    }

    void OnTriggerExit(Collider intruder)
    {
        if(intruder.gameObject.tag == "Tower")
        {
            colliderList.Remove(intruder.transform);
        }
    }
    
    Quaternion getRotation(Vector3 targetPos)
    {
        var aimPoint = targetPos - transform.position;
        return Quaternion.LookRotation(aimPoint);
    }
    
    void loadParameters()
    {
        this.damage = enemy.properties.active.damage;
        this.range = enemy.properties.active.range;
        this.speed = enemy.properties.active.speed;
        this.rateOfFire = enemy.properties.active.rateOfFire;
        this.originalSpeed = enemy.properties.active.originalSpeed;
    }
}
