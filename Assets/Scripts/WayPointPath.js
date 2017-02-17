#pragma strict

var enemyNumber: int = 1;

var count:int;
var originalSpeed:float;
internal var currentSpeed:float;
var wayPoints: Transform[];
internal var nextPointIndex:int = 1;
internal var pos: float = 0.0;
internal var distance: float;

private var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
}

function Start () {

	if(enemyNumber > excel.NUM_ENEMIES)
	{
		enemyNumber = excel.NUM_ENEMIES;
	}
	var enemyIndex: int = enemyNumber - 1;
	originalSpeed = excel.enemyArray[enemyIndex, 1];
	currentSpeed = originalSpeed;
	wayPoints = new Transform[count];
	for (var n=0; n < count; ++n) 
	{
//		print (n);
        wayPoints[n] = GameObject.Find("WayPoint"+(n+1)).transform;
        
    }
    
	//position the object at the start of the path
	if (count > 0)
	{
		this.transform.position = wayPoints[0].position;
		
	}
}

function Update () {

	if (count > 0)
	{
		//pos += speed * Time.deltaTime;
		distance = Vector3.Distance(transform.position, wayPoints[nextPointIndex].position);
		if (distance > 0)
		{
			transform.position = Vector3.Lerp(transform.position, wayPoints[nextPointIndex].position, Time.deltaTime* currentSpeed/distance);
		}
		//this.transform.position = Vector3.Lerp(wayPoints[nextPointIndex-1].position, wayPoints[nextPointIndex].position, pos);
		//print("current position :: "+ this.transform.position + "waypoint :: "+wayPoints[nextPointIndex].position);
		if(this.transform.position == wayPoints[nextPointIndex].position && nextPointIndex != count-1)
		{
			nextPointIndex++;
			pos = 0.0;
		}
		if(this.transform.position == wayPoints[count-1].position)
		{
			Destroy(gameObject);
		}
	}
}