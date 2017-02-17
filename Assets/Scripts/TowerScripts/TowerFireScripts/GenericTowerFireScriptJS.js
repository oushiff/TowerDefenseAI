#pragma strict

public var level: int;
var damage : double;
var speed: double;
var hasSplash:boolean;
var flying: boolean;
public var myTarget:GameObject;
public var myTargetTransform : Transform;

public var followEnemy : boolean;

internal var enemiesInRange:ArrayList;
internal var splashArea:double;


function Update () {
    if (followEnemy && myTarget)
    {
        myTargetTransform = myTarget.transform;
    }

    //Get the vector in the direction of travel;
    var step: float = (speed * Time.deltaTime);
    transform.position = Vector3.MoveTowards(transform.position, myTargetTransform.position, step);

    if (Vector3.Distance(transform.position, myTargetTransform.position) == 0)
    {
        //Reached Target
        var ms : MonsterScript = myTarget.GetComponent(MonsterScript);
        ms.causeDamage(damage);
       	Debug.Log("Explode");
       	
       	//Splash Damage
       	var size : int = enemiesInRange.Count;
       	Debug.Log(size);
       	for( var x: GameObject in enemiesInRange)
       	{
			if(x && x!=	myTarget)
			{
				var msx : MonsterScript = x.GetComponent(MonsterScript);
       			msx.causeDamage(damage);
       			Debug.Log("Splash");
			}       	
       	}
        
        
        Destroy(gameObject);
    }
}

function initValues()
{
    speed = 3.0;
    myTargetTransform = myTarget.transform;
    
    enemiesInRange = new ArrayList();
	this.GetComponent(SphereCollider).radius = splashArea;
}

function OnTriggerEnter(intruder: Collider)
{
	if(intruder.tag == "Enemy")
	{
		var other:MonsterScript = intruder.gameObject.GetComponent(MonsterScript);
		if (other.flying == 0 || flying == true)
		{
			enemiesInRange.Add(intruder.gameObject);
		}
	}
}

function OnTriggerExit(intruder: Collider)
{
	if(intruder.tag == "Enemy")
	{
		enemiesInRange.Remove(intruder.gameObject);
	}
}