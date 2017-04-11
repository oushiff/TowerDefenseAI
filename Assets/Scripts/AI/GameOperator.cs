using System;
using UnityEngine;
using System.Collections.Generic;
// build && remove tower
using Excel.Log;
using UnityEngine.UI;
namespace AI
{
	public class GameOperater: TowerButton
	{
        public List<BuildTowerButton> towerButtons;
        public Transform towersRoot;
        private RectTransform uiTransform;
        private Vector2 initialSize;
        private CanvasGroup canvasGroup;
        private GameObject selectedPlane;

		private Dictionary<String, GameObject> gameObjMap = new Dictionary<String, GameObject>();


		public GameOperater ()
		{
        }

		public Boolean BuildTower (int index, double[] pos)
		{
			/**
			 * index    - tower type eg: 0 means gun tower
			 * position - where to build tower
			 */

			UIManager.instance.RegisterUIClick ();

			// set screen Position
			Vector3 Position = Vector3.zero;
			Position.x = (float)pos [0];
			Position.y = 0.0f;
			Position.z = (float)pos [1];

			TowerData.Level selectedTower = GameData.instance.GetCurrentLevel ().towers [index];
			// Calculate cost for new tower
			int towerCost = (int)selectedTower.GetProperty (GameData.GameProperies.COST);
			if (towerCost == 0)
				towerCost = 100;


			Debug.Log ("   xxxxxx" + selectedTower.name+ "  "+ selectedTower.index);

			// If there are enough money to build the tower, deduct them
			if (Currency.instance.UseCoins (towerCost)) {
                // Build the tower
                GameObject towerObject = Instantiate (selectedTower.tower.GetPrefab (), Position, Quaternion.identity) as GameObject;
				String posStr = "";
				posStr += (int)pos [0]; 
				posStr += (int)pos [1];
				gameObjMap [posStr] = towerObject;


				Tower tower = towerObject.GetComponent<Tower> ();
				tower.transform.parent = towersRoot;
                tower.level = 0;
//				selectedPlane.tag = Grid.PLANE_NO_HOVER;

				// Place the tower on the grid
				GamePlay.instance.activeTowers.Add (tower);
				return true;
			}
			return false;
		}

		public Boolean UpgradeTower (int index, double[] pos)
		{

            
			//UIManager.instance.RegisterUIClick ();

			String posStr = "";
			posStr += (int)pos [0]; 
			posStr += (int)pos [1];

			//TowerData.Level selectedTower = GameData.instance.GetCurrentLevel ().towers [index];

			//GameObject towerObject = selectedTower.tower.GetPrefab ();

		
		

			GameObject towerObject = gameObjMap [posStr];


			//GameObject towerObject = Instantiate (selectedTower.tower.GetPrefab (), Position, Quaternion.identity) as GameObject;
			Tower tower = towerObject.GetComponent<Tower> ();

	
			tower.UpgradeTower (1);
		


    
			return true;

		}

		public Boolean RemoveTower ()
		{
			//todo
			return true;
		}
		
	}
}
