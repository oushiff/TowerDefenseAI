#pragma strict

internal var size : float = 0.5;
internal var num_blocks_width: int;
internal var num_blocks_length: int;
static var wayPointCounter:int = 0;

private var excel : ExcelReader; 
public var gridRoot : Transform;
public var waypointsRoot : Transform;

function Start()
{
	//NOW get the excelreader script
	excel = this.GetComponent("ExcelReader");
	num_blocks_width = excel.GRID_WIDTH;
	num_blocks_length = excel.GRID_LENGTH;

	for (var i:int=0; i<num_blocks_width; i++)
	{
		for (var j:int=0; j<num_blocks_length; j++)
		{
			if(excel.gridArray[i,j] == 15)	//create grass plane
			{
				createPlane(i, 0, j, excel.GRASS_MATERIAL, excel.PLANE_NO_HOVER, excel.RAYCAST_LAYER);
			}
			else if(excel.gridArray[i,j] == 40)	//create path plane
			{
				createPlane(i, 0, j, excel.PATH_MATERIAL, excel.PLANE_NO_HOVER, excel.RAYCAST_LAYER);
			}
			else if(excel.gridArray[i,j] == 33)	//create tower plane
			{
				createPlane(i, 0, j, excel.TOWER_MATERIAL, excel.PLANE_NO_HOVER, excel.RAYCAST_LAYER);
				createPlane(i, 0.05, j, excel.HOVER_INACTIVE_MATERIAL, excel.PLANE_HOVER, excel.RAYCAST_LAYER);
			}
			else if (excel.gridArray[i,j] == 47) //create entrance
			{
				createPlane(i, 0, j, excel.ENTRANCE_MATERIAL, excel.PLANE_NO_HOVER, excel.RAYCAST_LAYER);
				createWayPoint(i,0.1,j,0);
			}
			else if (excel.gridArray[i,j] == 14) //create exit
			{
				createPlane(i, 0, j, excel.EXIT_MATERIAL, excel.PLANE_NO_HOVER, excel.RAYCAST_LAYER);
				createWayPoint(i,0.1,j,(excel.numberOfWayPoints + 1));
			}
		}
	}
	
	for (i=0; i<num_blocks_width; i++)
	{
		for (j=0; j<num_blocks_length; j++)
		{
			if(excel.wayPointsArray[i,j] != 0)	//create grass plane
			{
				createWayPoint(i, 0.01, j, excel.wayPointsArray[i,j]);
			}
		}
	}
}

function createPlane(x:float, y:float, z:float, mat:Material, tagName:String, layerName:String)
{	
	var mesh : Mesh = new Mesh();
	mesh.name = "GridPlane";
	mesh.vertices = [Vector3(-size, y, -size), Vector3(size, y, -size), Vector3(size, y, size), Vector3(-size, y, size) ];
	mesh.uv = [Vector2 (0, 0), Vector2 (0, 1), Vector2(1, 1), Vector2 (1, 0)];
	mesh.triangles = [0, 2, 1, 0, 3, 2];
	mesh.RecalculateNormals();
	
	var obj : GameObject = new GameObject("GridPlane"+x+z, MeshRenderer, MeshFilter, BoxCollider);
	obj.GetComponent(MeshFilter).mesh = mesh;
	obj.transform.position = Vector3(x, y, z);
	obj.GetComponent.<Renderer>().material = mat;
	obj.tag = tagName;
	obj.layer = LayerMask.NameToLayer(layerName);
	obj.GetComponent(BoxCollider).size = Vector3(1, 0.001, 1); 
	obj.transform.parent = gridRoot;
}

function createWayPoint(x:float, y:float, z:float, wayPointNumber:int)
{
	var obj: GameObject = new GameObject("WayPoint"+wayPointNumber);
	obj.transform.position = Vector3(x, y, z);
	obj.AddComponent(WayPointIcon);
	obj.transform.parent = waypointsRoot;
}