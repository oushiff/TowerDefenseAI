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

		public Dictionary<String, GameObject> gameObjMap = new Dictionary<String, GameObject>();


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




			// If there are enough money to build the tower, deduct them
			if (Currency.instance.UseCoins (towerCost)) {
                // Build the tower
                GameObject towerObject = Instantiate (selectedTower.tower.GetPrefab (), Position, Quaternion.identity) as GameObject;
				String posStr = "";
				posStr += (int)pos [0];
				posStr += ",";
				posStr += (int)pos [1];
				gameObjMap [posStr] = towerObject;


				Tower tower = towerObject.GetComponent<Tower> ();
				tower.transform.parent = towersRoot;
                tower.level = 1;
//				selectedPlane.tag = Grid.PLANE_NO_HOVER;

				// Place the tower on the grid
				GamePlay.instance.activeTowers.Add (tower);

				int neg_value = -towerCost;
				Debug.Log ("{\t\"Type\": \"Tower\",\t\"TowerName\": \""+selectedTower.name+"\",\t\"TowerIndex\": "+selectedTower.index+",\t\"Level\": \"1,1\",\t\"Position\": \""+posStr+"\",\t\"Event\": \"Built\",\t\"Money\": "+neg_value+",\t\"Time\": "+(int)Time.time+"}, ");

				return true;
			}
			return false;
		}


		public Boolean IsUpgradable (double[] pos) {
			String posStr = "";
			posStr += (int)pos [0]; 
			posStr += ",";
			posStr += (int)pos [1];
			GameObject towerObject = gameObjMap [posStr];
			Tower tower = towerObject.GetComponent<Tower> ();
			return tower.IsTowerUpgradable ();
		}

		public Boolean UpgradeTower (int index, double[] pos)
		{
			if (!IsUpgradable (pos)) {
				return false;
			}

			//UIManager.instance.RegisterUIClick ();

			String posStr = "";
			posStr += (int)pos [0];
			posStr += ",";
			posStr += (int)pos [1];

			//TowerData.Level selectedTower = GameData.instance.GetCurrentLevel ().towers [index];

			//GameObject towerObject = selectedTower.tower.GetPrefab ();


			GameObject towerObject = gameObjMap [posStr];


			//GameObject towerObject = Instantiate (selectedTower.tower.GetPrefab (), Position, Quaternion.identity) as GameObject;
			Tower tower = towerObject.GetComponent<Tower> ();
    


			bool succ = tower.UpgradeTower (1); 
			if (succ) {
				int neg_value = (int)GameData.instance.GetTower (tower.type, tower.level).GetProperty (GameData.GameProperies.COST);
				neg_value = -neg_value;
				Debug.Log ("{\t\"Type\": \"Tower\",\t\"TowerName\": \""+tower.type+"\",\t\"TowerIndex\": -1,\t\"Level\": \""+tower.level+",1\",\t\"Position\": \""+posStr+"\",\t\"Event\": \"Upgraded\",\t\"Money\": "+neg_value+",\t\"Time\": "+(int)Time.time+"}, ");
			}
			return succ;

		}

		public Boolean RemoveTower ()
		{
			//todo
			return true;
		}
		
	}
}
