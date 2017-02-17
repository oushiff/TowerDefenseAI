#pragma strict

//assigning the level before instantiating it
var level:int = 0;
var flying:boolean;
var damage:double;
var slowEffect:double;
var range:double;

//not coming from excel
var speed: float = 10;
internal var distance: float;

internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.thresherTowerArray[level, excel.DAMAGE];
	range = excel.thresherTowerArray[level, excel.RANGE];
	slowEffect = 0.2;
	//armorPenetration = excel.gunTowerArray[level, excel.ARMOR_PENETRATION];
}

function Update () {
	transform.Translate(Vector3.forward * Time.deltaTime * speed);
	distance += Time.deltaTime * speed;
	if( distance >= range)
	{
		Destroy(gameObject);
	}
}