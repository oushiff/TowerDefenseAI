#pragma strict

//assigning the level before instantiating it
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
	excel = GameObject.FindGameObjectWithTag("ExcelReader").GetComponent("ExcelReader");
}

function Start () {
	damage = excel.gunTowerArray[level, excel.DAMAGE];
	range = excel.gunTowerArray[level, excel.RANGE];
	armorPenetration = excel.gunTowerArray[level, excel.ARMOR_PENETRATION];
	extradamage = 0;
}

function Update () {
	transform.Translate(Vector3.forward * Time.deltaTime * speed);
	distance += Time.deltaTime * speed;
	if( distance >= range)
	{
		Destroy(gameObject);
	}
}