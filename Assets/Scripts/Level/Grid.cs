#region Copyright
// <copyright file="Grid.cs">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Fotos Frangoudes</author>
// <date> 04/09/2016, 2:25 PM </date>
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class Grid : MonoBehaviour {
    public static Grid instance;
    public Transform gridRoot;
    public Transform waypointsRoot;
    public Transform[] wayPoints;
    private float tileSize = 0.5f;

    public enum MapTile
    {
        EMPTY = 0,
        INDESTRUCTIBLE_TILE = 3,
        DESTRUCTIBLE_TILE = 6,
        EXIT = 14,
        GRASS = 15,
        TOWER = 33,
        PATH = 40,
        ENTRANCE = 47,
        HOVER_ACTIVE = 100,
        HOVER_INACTIVE = 101
    }

    [System.Serializable]
    public class MapMaterials
    {
        public MapTile type;
        public Material material;
    }
    public List<MapMaterials> mapMaterialsList;
    private Dictionary<MapTile, MapMaterials> mapMaterials;

    //Constants for tags
    public static string PLANE_CAMERA_FOCUS = "Plane_camera_focus";
    public static string PLANE_HOVER = "Plane_hover";
    public static string PLANE_NO_HOVER = "Plane_no_hover";

    //Constants for layers
    public static string RAYCAST_LAYER = "Raycast_layer";
    public static string DEFAULT_LAYER = "Default";

    // Use this for initialization
    void Awake () {
        instance = this;

        mapMaterials = new Dictionary<MapTile, MapMaterials>();
        for(int i = 0 ; i < mapMaterialsList.Count ; i++)
            mapMaterials.Add(mapMaterialsList[i].type, mapMaterialsList[i]);
    }

    private void Start()
    {
        StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid() {
        LevelData levelData = GameData.instance.GetCurrentLevel();

        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (levelData.loaded == false){
            yield return waitTime;
        }

        int gridWidth = levelData.width;
        int gridLength = levelData.length;
    
        wayPoints = new Transform[levelData.waypointsNo+2];
        for (int i =0; i < gridWidth; i++)
        {
            for (int j =0; j < gridLength; j++)
            {
                // Create plane
                switch (levelData.grid[i][j]){
                    case (int) MapTile.GRASS:    // Create grass plane
                        createPlane(i, 0, j, mapMaterials[MapTile.GRASS].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        break;
                    case (int) MapTile.PATH:    // Create path plane
                        createPlane(i, 0, j, mapMaterials[MapTile.PATH].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        break;
                    case (int) MapTile.TOWER:    // Create tower plane
                        createPlane(i, 0, j, mapMaterials[MapTile.TOWER].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        createPlane(i, 0.05f, j, mapMaterials[MapTile.HOVER_INACTIVE].material, PLANE_HOVER, RAYCAST_LAYER);
                        break;
                    case (int) MapTile.ENTRANCE:    // Create entrance
                        createPlane(i, 0, j, mapMaterials[MapTile.ENTRANCE].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        createWayPoint(i, 0.1f, j, 0);
                        break;
                    case (int) MapTile.EXIT:    // Create exit
                        createPlane(i, 0, j, mapMaterials[MapTile.EXIT].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        createWayPoint(i, 0.1f, j, levelData.waypointsNo + 1);
                        break;
                    case (int) MapTile.INDESTRUCTIBLE_TILE:
                        createPlane(i, 0, j, mapMaterials[MapTile.INDESTRUCTIBLE_TILE].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        break;
                    case (int) MapTile.DESTRUCTIBLE_TILE:
                        createPlane(i, 0, j, mapMaterials[MapTile.DESTRUCTIBLE_TILE].material, PLANE_NO_HOVER, RAYCAST_LAYER);
                        break;
                    default:
                        break;
                }
                if (levelData.waypoints[i][j] != 0){ // Grass plane
                    createWayPoint(i, 0.01f, j, levelData.waypoints[i][j]);
                }
            }
        }

        yield return null;
    }

    private void createPlane(float x, float y, float z, Material mat, string tagName, string layerName)
    {	
        // Generate plane
        Mesh mesh = new Mesh();
        mesh.name = "GridPlane";
        mesh.vertices = new Vector3[] { new Vector3(-tileSize, y, -tileSize), new Vector3(tileSize, y, -tileSize), new Vector3(tileSize, y, tileSize), new Vector3(-tileSize, y, tileSize) };
        mesh.uv = new Vector2[] { new Vector2 (0, 0), new Vector2 (0, 1), new Vector2(1, 1), new Vector2 (1, 0) };
        mesh.triangles = new int[] {0, 2, 1, 0, 3, 2};
        mesh.RecalculateNormals();
        
        GameObject obj = new GameObject ("GridPlane" + x.ToString () + z.ToString ());
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.parent = gridRoot;
        obj.tag = tagName;
        obj.layer = LayerMask.NameToLayer(layerName);

        obj.AddComponent<MeshFilter> ();
        obj.GetComponent<MeshFilter>().mesh = mesh;

        obj.AddComponent<MeshRenderer> (); 
        obj.GetComponent<Renderer>().material = mat;

        obj.AddComponent<BoxCollider>();
        obj.GetComponent<BoxCollider>().size = new Vector3(1, 0.001f, 1); 
    }
    
    private void createWayPoint(float x, float y, float z, int wayPointNumber)
    {
        GameObject newWayPoint = new GameObject("WayPoint" + wayPointNumber.ToString());
        newWayPoint.transform.position = new Vector3(x, y, z);
        newWayPoint.transform.parent = waypointsRoot;

        // Show that's a waypoint
        Waypoint wayPoint = newWayPoint.AddComponent<Waypoint>();
        wayPoint.number = wayPointNumber;

        wayPoints [wayPointNumber] = newWayPoint.transform;
    }
}
