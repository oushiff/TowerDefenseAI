#pragma strict

var mainCamera: Transform;
var towers:GameObject[];
var cameraSpeed:float;
var activeEnemies:List.<GameObject>;
var horizontalSpeed : float = 0.1;
var verticalSpeed : float = 0.1;

var soulScript1: SoulTowerScript;
var parentObj1:GameObject;
var towersRoot : Transform;
internal var originalMat: Material;
var hovMaterial:Material;
internal var layer:int;
internal var layerMask:int;
internal var lastHitObj: GameObject;
internal var iceClicked:int;
internal var distance: float;
internal var linkingTower: boolean;
internal var cameraTargetPos: Vector3;

//variable to store the state of the tower menu
var numTowers:int;
internal var showTowerMenu:int = 0;
internal var lastClickObj:GameObject;
internal var drawTowerIndex:int = 0;
var cameraObj:Camera;

//upgrade tower
internal var showUpgradeMenu:int = 0;
internal var towerSelected:GameObject;

//custom box skin
var boxSkin: GUISkin;

//upgrade
var towerLayer:LayerMask;

private var excel : ExcelReader; 

function Awake()
{
	//Get the ExcelReader Script
	excel = gameObject.GetComponent("ExcelReader");
}

function Start () 
{
	//print("In the start, ray cast layer:: " + excel.RAYCAST_LAYER);
	//raycastLayerMask = LayerMask.NameToLayer(excel.RAYCAST_LAYER);
	//print("raycastLayerMask :: " + LayerMask.LayerToName(raycastLayerMask));
	layer = LayerMask.NameToLayer(excel.RAYCAST_LAYER);
	layerMask = 1 << layer;
	cameraTargetPos = mainCamera.position;
	//initialize the number of towers
	numTowers = towers.Length;
	iceClicked = 0;
}

function Update () 
{	

	if (Input.GetMouseButtonDown(0) && iceClicked == 1)
	{	
		activeEnemies = GamePlay.activeEnemies;
		
		iceClicked = 0;
		return;
	}
	else if(Input.GetMouseButtonDown(0) && lastHitObj)
	{
		if(lastHitObj.tag == excel.PLANE_HOVER)
		{
			showTowerMenu = 1;
			lastClickObj = lastHitObj;
		}
	}
	else if(Input.GetMouseButtonDown(0) && towerSelected)
	{
		showUpgradeMenu = 1;
	}
	
	
	
	//camera drag code
	if (Input.GetMouseButton(0) && !lastHitObj && !towerSelected && !iceClicked)
	{
		var h: float = horizontalSpeed * Input.GetAxis("Mouse X"); //The horizontal movement - could use "Horizontal"
		var v: float = verticalSpeed * Input.GetAxis("Mouse Y"); //The vertical movement - could use "Vertical"
		var move: Vector3 = Vector3(-h, 0, -v);
		//print("move :: " + move);
		mainCamera.transform.Translate(move, Space.World);
	}
	
	if(!Input.GetMouseButton(0) && !showTowerMenu && !showUpgradeMenu)
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	//	print (ray);
		var hit: RaycastHit;
		if(Physics.Raycast(ray, hit, 100000, layerMask))
		{
	//		print("sending a ray!!");
			if(hit.collider.gameObject.tag == excel.PLANE_HOVER)
			{
				//assign the lastObj which is the placement plane
				if(lastHitObj)
				{
					lastHitObj.GetComponent.<Renderer>().material = originalMat;
				}
				lastHitObj = hit.collider.gameObject;
				originalMat = lastHitObj.GetComponent.<Renderer>().material;
				lastHitObj.GetComponent.<Renderer>().material = hovMaterial;
				//empty the towerSelected
				towerSelected = null;
			}
			else if(hit.collider.gameObject.tag == "Tower")
			{
				//assign the tower hovering on
				towerSelected = hit.collider.gameObject;
				//empty the lastHitObj
				if(lastHitObj)
				{
					lastHitObj.GetComponent.<Renderer>().material = originalMat;
					lastHitObj = null;
				}
			}
			
			else
			{
				//empty the lastHitObj 
				if(lastHitObj)
				{
					lastHitObj.GetComponent.<Renderer>().material = originalMat;
					lastHitObj = null;
				}
				//empty towerSelected
				towerSelected = null;
			}	
		}
		else
		{
			//empty the lastHitObj
			if(lastHitObj)
			{
				lastHitObj.GetComponent.<Renderer>().material = originalMat;
				lastHitObj = null;
			}
			//empty the towerSelected
			towerSelected = null;
		}
	}
	/*
	if(Input.GetMouseButtonDown(0))
	{
		var ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit2: RaycastHit;
		print("Inside upgrade function");
		if(Physics.Raycast(ray2, hit2, 1000, towerLayer))
		{
			print("Ray hit");
			if(hit2.collider.gameObject.tag == "Tower")
			{
				towerSelected = hit2.collider.gameObject;
				showUpgradeMenu = 1;
			}
		}
	}
	*/
	/*
	if(Input.GetMouseButtonDown(0))
	{
		var ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
		var hit2: RaycastHit;
		print("Inside camera motion function");
		if(Physics.Raycast(ray2, hit2, 1000, layerMask))
		{
			print("Ray hit");
			if(hit2.collider.gameObject.tag == excel.PLANE_NO_HOVER)
			{
				print("no hover");
				cameraTargetPos = hit2.collider.gameObject.transform.position;
				cameraTargetPos = Vector3(cameraTargetPos.x, mainCamera.position.y, cameraTargetPos.z);
			}
		}
	}
	
	//do this every frame and translate the camera if there is a change in the target position
	mainCamera.position = myTranslation(mainCamera.position, cameraTargetPos);
	*/
}

function myTranslation(start: Vector3, end: Vector3)
{
//	print("translate!! :: " + end);
	distance = Vector3.Distance(end, start);
	if (distance > 0)
	{
		//var result:Vector3 = Vector3.Lerp (start, end, Time.deltaTime* cameraSpeed/distance);
		var result:Vector3 = Vector3.Lerp (start, end, Mathf.Sin(cameraSpeed/distance * Mathf.PI * 0.5f));
		return result;
	}
	return start;
}

function spawnTower()
{
	var newTower: GameObject = Instantiate(towers[drawTowerIndex], lastClickObj.transform.position, Quaternion.identity);
	newTower.transform.localEulerAngles.y = Random.Range(0, 360);
	newTower.transform.parent = towersRoot;
	var towerCost:double;
	lastClickObj.tag = excel.PLANE_NO_HOVER;
	if(drawTowerIndex == 0)
	{
		var gunTowerScript:GunTowerScript = newTower.GetComponent("GunTowerScript");
		gunTowerScript.gridObj = lastClickObj;
		towerCost = excel.gunTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 1)
	{
		var cannonTowerScript:CannonTowerScript = newTower.GetComponent("CannonTowerScript");
		cannonTowerScript.gridObj = lastClickObj;
		towerCost = excel.cannonTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 2)
	{
		var thresherTowerScript:ThresherTowerScript = newTower.GetComponent("ThresherTowerScript");
		thresherTowerScript.gridObj = lastClickObj;
		towerCost = excel.thresherTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 3)
	{
		var electricalTowerScript:ElectricalTowerScript = newTower.GetComponent("ElectricalTowerScript");
		electricalTowerScript.gridObj = lastClickObj;
		towerCost = excel.electricalTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 4)
	{
		var scytheTowerScript:ScytheTowerScript = newTower.GetComponent("ScytheTowerScript");
		scytheTowerScript.gridObj = lastClickObj;
		towerCost = excel.scytheTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 5)
	{
		var towernautTowerScript:TowernautTowerScript = newTower.GetComponent("TowernautTowerScript");
		towernautTowerScript.gridObj = lastClickObj;
		towerCost = excel.towernautTowerArray[0, excel.COST];
	}
	else if(drawTowerIndex == 6)
	{
	    var genericTowerScript:GenericTowerScriptJS = newTower.GetComponent("GenericTowerScriptJS");
	    genericTowerScript.gridObj = lastClickObj;
	    towerCost = 100;
	}
	else if(drawTowerIndex == 7)
	{
		var soulTowerScript:SoulTowerScript = newTower.GetComponent("SoulTowerScript");
		soulTowerScript.gridObj = lastClickObj;
		towerCost = 100;
	}
	//subtract the cost 
	//print("tower cost " + towerCost);
	reduceMoney(towerCost);
	GamePlay.activeTowers.Add(newTower);
}

function OnGUI()
{
	GUI.skin = boxSkin;
	//if (iceClicked == 0)
	//{
	//	if (GUI.Button(Rect(10,10,80,100),"ice"))
	//	{
	//		iceClicked = 1;
	//	}
	//}
	
	if (showTowerMenu && lastHitObj)
	{
		//Time.timeScale = 0;
		
		var boxPosition:Vector3 = cameraObj.WorldToScreenPoint(lastClickObj.transform.position);

		//var boxX: int = 10;
		//var boxY: int = 10;
		var boxWidth: int = 100;
		var boxHeight: int = 100;
		var boxX: int = boxPosition.x - boxWidth/2;
		var boxY: int = Screen.height - boxPosition.y - boxHeight/2;
		GUI.Box (Rect (boxX,boxY,boxWidth,boxHeight), "");

		if (excel.allowedTowers[0] == 1)
		{
			if (GUI.Button (Rect (boxX,boxY,20,20), "1")) 
			{
				drawTowerIndex = 0;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		
		}
		
		if (excel.allowedTowers[1] == 1)
		{
			if (GUI.Button (Rect (boxX+40,boxY,20,20), "2")) 
			{
				drawTowerIndex = 1;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		}
		
		if (excel.allowedTowers[2] == 1)
		{
			if (GUI.Button (Rect (boxX+80,boxY,20,20), "3")) 
			{
				drawTowerIndex = 2;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		}
		
			
		
		if (GUI.Button (Rect (boxX+40,boxY+40,20,20), "X")) 
			{
				showTowerMenu = 0;
				//Time.timeScale = 1;
				lastHitObj.GetComponent.<Renderer>().material = originalMat;
				lastHitObj = null;
			}
			
		if (excel.allowedTowers[3] == 1)
		{
			if (GUI.Button (Rect (boxX,boxY+80,20,20), "4")) 
			{
				drawTowerIndex = 3;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		}
		
			
		if (excel.allowedTowers[4] == 1)
		{
			if (GUI.Button (Rect (boxX+40,boxY+80,20,20), "5")) 
			{
				drawTowerIndex = 4;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		}
		
			
		if (excel.allowedTowers[5] == 1)
		{
			if (GUI.Button (Rect (boxX+80,boxY+80,20,20), "6")) 
			{
				drawTowerIndex = 5;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
			}
		}

	    //GENERATE Generic Tower
		if (GUI.Button (Rect (boxX+80,boxY+120,20,20), "G")) 
		{
		    drawTowerIndex = 6;
		    showTowerMenu = 0;
		    //Time.timeScale = 1;
		    spawnTower();
		}
		
		//GENERATE SOUL WELL
		if (GUI.Button (Rect (boxX,boxY+120,20,20), "S")) 
		{
				drawTowerIndex = 7;
				showTowerMenu = 0;
				//Time.timeScale = 1;
				spawnTower();
		}
	}
	
	else if (showUpgradeMenu && towerSelected)
	{
		var upgradeBoxPosition:Vector3 = cameraObj.WorldToScreenPoint(towerSelected.transform.position);

		//var boxX: int = 10;
		//var boxY: int = 10;
		var upgradeBoxWidth: int = 100;
		var upgradeBoxHeight: int = 100;
		var upgradeBoxX: int = upgradeBoxPosition.x - upgradeBoxWidth/2;
		var upgradeBoxY: int = Screen.height - upgradeBoxPosition.y - upgradeBoxHeight/2;
		GUI.Box (Rect (upgradeBoxX,upgradeBoxY,upgradeBoxWidth,upgradeBoxHeight), "");
		
		if(!linkingTower)
		{
			if (towerSelected.transform.parent.gameObject.tag == "SoulWell") 
			{
				if (GUI.Button (Rect (upgradeBoxX+15,upgradeBoxY-35,70,20), "Link")) 
				{
					parentObj1 = towerSelected.transform.parent.gameObject;
					soulScript1 = parentObj1.GetComponent("SoulTowerScript");
					linkingTower = true;
					towerSelected = null;
					showUpgradeMenu = 0;
				}
			}
			
			if (getTowerLevel() >= 3)
			{
				towerSelected = null;
				showUpgradeMenu = 0;
			}
			else
			{
				if (getTowerLevel() < 2)
				{
					if (GUI.Button (Rect (upgradeBoxX+15,upgradeBoxY+15,70,20), "Upgrade")) 
					{
						upgradeTower();
						towerSelected = null;
						showUpgradeMenu = 0;
					}
				}
				else{
					if (GUI.Button (Rect (upgradeBoxX+15,upgradeBoxY+15,70,20), "Special1"))
					{
						upgradeTower();
						towerSelected = null;
						showUpgradeMenu = 0;
					}
					
					if (GUI.Button (Rect (upgradeBoxX+15,upgradeBoxY+40,70,20), "Special2"))
					{
						upgradeTower(2);
						towerSelected = null;
						showUpgradeMenu = 0;
					}
				}
				
				if (GUI.Button (Rect (upgradeBoxX+15,upgradeBoxY+65,70,20), "Cancel")) 
					{
						towerSelected = null;
						showUpgradeMenu = 0;
					}
			}
		}
		else
		{
			linkTower();
			showUpgradeMenu = 0;
			towerSelected = null;
			linkingTower = false;
		}
	}
}

function linkTower()
{	
	soulScript1.linkTower(towerSelected.transform.parent.gameObject);
}

function getTowerLevel() : int
{
	var parentObj:GameObject = towerSelected.transform.parent.gameObject;
	if(parentObj.tag == "GunTower")
	{
		var gunTowerScript:GunTowerScript = parentObj.GetComponent("GunTowerScript");
		return gunTowerScript.level;
	}
	else if(parentObj.tag == "CanonTower")
	{
		var cannonTowerScript:CannonTowerScript = parentObj.GetComponent("CannonTowerScript");
		return cannonTowerScript.level;
	}
	else if(parentObj.tag == "ThresherTower")
	{
		var thresherTowerScript:ThresherTowerScript = parentObj.GetComponent("ThresherTowerScript");
		return thresherTowerScript.level;
	}
	else if(parentObj.tag == "ElectricalTower")
	{
		var electricalTowerScript:ElectricalTowerScript = parentObj.GetComponent("ElectricalTowerScript");
		return electricalTowerScript.level;
	}
	else if(parentObj.tag == "ScytheTower")
	{
		var scytheTowerScript:ScytheTowerScript = parentObj.GetComponent("ScytheTowerScript");
		return scytheTowerScript.level;
	}
	else if(parentObj.tag == "TowernautTower")
	{
		var towernautTowerScript:TowernautTowerScript = parentObj.GetComponent("TowernautTowerScript");
		return towernautTowerScript.level;
	}
	
	else if(parentObj.tag == "SoulWell")
	{
		var soulTowerScript:SoulTowerScript = parentObj.GetComponent("SoulTowerScript");
		return soulTowerScript.level;
	}
}

function upgradeTower()
{
	upgradeTower(1);
}

function upgradeTower(upgradeAmount : int)
{
	var parentObj:GameObject = towerSelected.transform.parent.gameObject;
	var towerCost:double;
	
	if(parentObj.tag == "GunTower")
	{
		var gunTowerScript:GunTowerScript = parentObj.GetComponent("GunTowerScript");
		if (gunTowerScript.level <3)
		{
			gunTowerScript.level += upgradeAmount;
			towerCost = excel.gunTowerArray[gunTowerScript.level, excel.COST];
			gunTowerScript.loadTowerParameters();
		} 
	}
	else if(parentObj.tag == "CanonTower")
	{
		var cannonTowerScript:CannonTowerScript = parentObj.GetComponent("CannonTowerScript");
		if (cannonTowerScript.level <3)
		{
			cannonTowerScript.level += upgradeAmount;
			towerCost = excel.cannonTowerArray[cannonTowerScript.level, excel.COST];
			cannonTowerScript.loadTowerParameters();
		}
	}
	else if(parentObj.tag == "ThresherTower")
	{
		var thresherTowerScript:ThresherTowerScript = parentObj.GetComponent("ThresherTowerScript");
		if (thresherTowerScript.level <3)
		{
			thresherTowerScript.level += upgradeAmount;
			towerCost = excel.thresherTowerArray[thresherTowerScript.level, excel.COST];
			thresherTowerScript.loadTowerParameters();
		}
	}
	else if(parentObj.tag == "ElectricalTower")
	{
		var electricalTowerScript:ElectricalTowerScript = parentObj.GetComponent("ElectricalTowerScript");
		if (electricalTowerScript.level <3)
		{
			electricalTowerScript.level += upgradeAmount;
			towerCost = excel.electricalTowerArray[electricalTowerScript.level, excel.COST];
			electricalTowerScript.loadTowerParameters();
		}
	}
	else if(parentObj.tag == "ScytheTower")
	{
		var scytheTowerScript:ScytheTowerScript = parentObj.GetComponent("ScytheTowerScript");
		if (scytheTowerScript.level <3)
		{
			scytheTowerScript.level += upgradeAmount;
			towerCost = excel.scytheTowerArray[scytheTowerScript.level, excel.COST];
			scytheTowerScript.loadTowerParameters();
		}
	}
	else if(parentObj.tag == "TowernautTower")
	{
		var towernautTowerScript:TowernautTowerScript = parentObj.GetComponent("TowernautTowerScript");
		if (towernautTowerScript.level <3)
		{
			towernautTowerScript.level += upgradeAmount;
			towerCost = excel.towernautTowerArray[towernautTowerScript.level, excel.COST];
			towernautTowerScript.loadTowerParameters();
		}
	}
	
	else if(parentObj.tag == "SoulWell")
	{
		var soulTowerScript:SoulTowerScript = parentObj.GetComponent("SoulTowerScript");
		if (soulTowerScript.level <3)
		{
			soulTowerScript.level += upgradeAmount;
			soulTowerScript.loadTowerParameters();
			towerCost = soulTowerScript.cost;
		}
	}
	//subtract the cost 
	print("tower cost " + towerCost);
	reduceMoney(towerCost);
}

function reduceMoney(towerCost:double)
{
	var scriptsObject:GameObject = GameObject.FindGameObjectWithTag("ExcelReader");
	var moneyScript:MoneyScript = scriptsObject.GetComponent("MoneyScript");
	moneyScript.startingMoney -= towerCost;
}