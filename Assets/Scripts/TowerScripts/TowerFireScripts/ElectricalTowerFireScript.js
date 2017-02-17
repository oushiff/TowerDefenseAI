#pragma strict

//assigning the level and flying before instantiating it
var level:int = 0;
var flying:boolean;
var damage:double;
var armorPenetration:double;
var range:double;
var extradamage: double;
//not coming from excel
var speed: float = 10;
internal var distance: float;

internal var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = GameObject.Find("Scripts").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.electricalTowerArray[level, excel.DAMAGE];
	range = excel.electricalTowerArray[level, excel.RANGE];
	armorPenetration = excel.electricalTowerArray[level, excel.ARMOR_PENETRATION];
}

function Update () {
	transform.Translate(Vector3.forward * Time.deltaTime * speed);
	distance += Time.deltaTime * speed;
	if( distance >= range)
	{
		Destroy(gameObject);
	}
}