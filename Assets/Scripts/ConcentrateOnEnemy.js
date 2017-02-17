#pragma strict

var enemyLayer: LayerMask;

private var excel : ExcelReader;

function Awake()
{
	//Get the ExcelReader Script
	excel = this.GetComponent("ExcelReader");
}

function Update () 
{
	if(Input.GetMouseButtonDown(0))
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit: RaycastHit;
		if(Physics.Raycast(ray, hit, 1000, enemyLayer))
		{
			if(hit.collider.gameObject.tag == "Enemy")
			{
				//print("enemy!!!");
				excel.selectedEnemy = hit.collider.gameObject;
			}
		}
	}
}