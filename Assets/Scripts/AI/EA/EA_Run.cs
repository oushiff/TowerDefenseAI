using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using AI;

public class EA_Run : MonoBehaviour
{
//	private class Tower {
//		int towerIndex;
//		int towerLevel;
//
//		public Tower(int index, int level) {
//			towerIndex = index;
//			towerLevel = level;
//		}
//	}

//	class SeqScore {
//		int seqIndex;
//		int score;
//		public SeqScore(int seqIndex, int score) {
//			this.seqIndex = seqIndex;
//			this.score = score;
//		}
//	}

	public MapMonitor m_map;

	public static EA_Run instance;

	public GameOperater GO;//= GameObject.; 

	public Dictionary<String, GameObject> gameObjMap;

	List<int[]> posList = new List<int[]> ();
	int towerSize;

	GeneSeq curSeq;

	double[] scores;
	int scoreIdx = 0;
	int scoreSize = 0;

	public void Init () {
		//		GameObject worldObj = GameObject.FindGameObjectWithTag ("ExcelReader");

		//		m_map = worldObj.GetComponent<MapMonitor>();
		MapMonitor.Instance.init();
		m_map = MapMonitor.instance;
		GO = new GameOperater ();
		//		GO = worldObj.GetComponent<EA_Operator>();
		gameObjMap = GO.gameObjMap;

		List <double[]> tmpList = m_map.GetAllCandidateSpacesAtBeginning ();

		Debug.Log (tmpList.Count + "AAAAA");


		foreach (double[] tmpDouble in tmpList) {
			int[] tmpInt = new int[tmpDouble.Length];
			int i = 0;
			foreach (double num in tmpDouble) {
				tmpInt [i] = (int)num;
				i++;
			}
			posList.Add (tmpInt);
		}
		List<TowerData.Level> selectedTower = GameData.instance.GetCurrentLevel ().towers;
		towerSize = selectedTower.Count;

	}


	void Start()
	{


	}

//	Dictionary<int[], Tower> map = new Dictionary<int[], Tower>();


	public List<GeneSeq> CrossOverRes(GeneSeq seq1, GeneSeq seq2, int number){
		List<GeneSeq> listGeneSeq = new List<GeneSeq> ();
		int length = (seq1.Size () > seq2.Size () ? seq2.Size () : seq1.Size ());
		System.Random rnd = new System.Random();
		for (int i = 0; i < number; i++) {
			int index = rnd.Next (1, length);
			List<GeneNode> nodes = new List<GeneNode> ();
			int j = 0;
			for (; j <= index; j++) {
				seq2.GetNextStep ();
				nodes.Add (seq2.GetNextStep ());
			}
			for (; i < seq2.Size (); i++) {
				nodes.Add(seq2.GetNextStep());
			}
			GeneSeq newSeq = new GeneSeq (nodes);
			listGeneSeq.Add (newSeq);
		}
		return listGeneSeq;
	}
		

	public List<GeneSeq> Mutate(GeneSeq seq,int number){	       
		List<GeneSeq> listGeneSeq = new List<GeneSeq> ();
		int length = seq.Size ();
		System.Random rnd = new System.Random();
		int[] pos  = posList[rnd.Next (0, posList.Count)];
		int towerIndex = rnd.Next (0, towerSize);
		int index = rnd.Next (0, length);
		for (int i = 0; i < number; i++) {		
			GeneSeq newSeq = new GeneSeq(seq);
			newSeq.SetNode (index, pos, towerIndex);
			listGeneSeq.Add (newSeq);
			newSeq = new GeneSeq(seq);
			newSeq.InsertNodeAfterIndex (index, new GeneNode(pos, towerIndex));
			listGeneSeq.Add (newSeq);
			newSeq = new GeneSeq(seq);
			newSeq.RemoveNodeAt (index);
			listGeneSeq.Add (newSeq);
		}
		return listGeneSeq;
	}


	public List<GeneSeq> InitSeqsRandom() {
		List<GeneSeq> pool = new List<GeneSeq> ();
		for (int i = 0; i < 2; i++) {
			GeneSeq newSeq = new GeneSeq ();
			newSeq.InitRandom (posList, towerSize, posList.Count * 3);
			pool.Add (newSeq);

//			Debug.Log ("init seq:  " +  posList.Count + "   "+towerSize);
		}
		return pool;
	}

	public List<GeneSeq> ImportSeqsFromFile() {
		System.IO.StreamReader readFile = 
			new System.IO.StreamReader("Strategy_Pool/info");
		int fileIndex = Int32.Parse (readFile.ReadLine ());
		readFile.Close ();

		fileIndex -= 1;
		readFile = new System.IO.StreamReader("Strategy_Pool/strategy_" + fileIndex);
		string stream = readFile.ReadToEnd();
		readFile.Close ();

		List<GeneSeq> pool = new List<GeneSeq> ();
		string[] seqStreams = stream.Split (';');
		foreach (string part in seqStreams) {
			if (part.Trim () == "")
				continue;
			
			GeneSeq newSeq = new GeneSeq ();
			string newPart = part + ";\n";
			Debug.Log ("Gene STring :   " + newPart);
			pool.Add(newSeq.Deserialize (newPart));

		}
		return pool;
	}

	public void ExportSeqToFile(List<GeneSeq> geneSeqs) {
		System.IO.StreamReader readFile = 
			new System.IO.StreamReader("Strategy_Pool/info");
		int fileIndex = Int32.Parse (readFile.ReadLine ());
		readFile.Close ();

		System.IO.StreamWriter writeFile = new System.IO.StreamWriter("Strategy_Pool/info");
		writeFile.WriteLine(fileIndex + 1);
		writeFile.Close();

		System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
		foreach (GeneSeq seq in geneSeqs) {
			sb.Append(seq.Serialize());
		}
		writeFile = new System.IO.StreamWriter("Strategy_Pool/strategy_" + fileIndex);
		writeFile.WriteLine(sb.ToString());
		writeFile.Close();
	}

	IEnumerator ExecuteGeneSeq() {
		scoreIdx = 0;
		Collector.Instance.init();
		while (curSeq.hasNext()) {
			yield return new WaitForSeconds(1);
			Debug.Log ("New Node!!!");
			GeneNode node = curSeq.GetNextStep ();
			if (curSeq.Size () <= 0) {
				break;
			}

			int[] pos = node.pos;
			int towerIndex = node.towerIndex;
			String posStr = "";
			posStr += pos [0]; 
			posStr += ",";
			posStr += pos [1];

			double[] posDouble = new double[2];
			posDouble [0] = (double)pos [0];
			posDouble [1] = (double)pos [1];

			if (!gameObjMap.ContainsKey (posStr)) {
				
				while (!GO.BuildTower (towerIndex, posDouble)) {
					Debug.Log ("Build Tower Failed!!!!");
					yield return new WaitForSeconds(5);
				} 
				Debug.Log ("Build Tower Succ!!!!");
			} else {
				if (!GO.IsUpgradable (posDouble)) {
					Debug.Log ("Level Highest!!!!");
					continue;
				}
				while (!GO.UpgradeTower (0, posDouble)) {
					Debug.Log ("Upgrade Tower Failed!!!!");
					yield return new WaitForSeconds(5);
				}
				Debug.Log ("Upgrade Tower Succ!!!!");
			}
		}
		Debug.Log (scoreIdx + "  "  + scoreSize);
		if (scoreIdx < scoreSize) {
			
			scores [scoreIdx] = GetScore ();
			Debug.LogWarning ("【Score】: " + scores [scoreIdx]);
			scoreIdx++;
		}
	}


	public double GetScore() {
		double alpha = 1;
		double beta = 1;
		double theta = 1;
		//double gamma = 0;
		//double delta = 0;
		MoneyMonitor money = new MoneyMonitor();
		int initMoney = money.GetStartingMoney ();
		int mapLength = MapMonitor.instance.GetRoadsCoordinates().Count;


		Collector co = Collector.instance;
		//double res = alpha * co.spendMoney * (-1) + beta / (co.enemyLeftDistance + 1) + theta * co.enemyDeadAmount - gamma * co.enemyArrivedAmount + delta * co.upgradeTowerNum;
		double res = alpha * co.spendMoney / initMoney - beta * co.enemyLeftDistance / co.monsterCount / mapLength - theta * co.enemyArrivedAmount / co.monsterCount;
		//Debug.LogWarning ("【test】: " + co.enemyArrivedAmount);

		return (double)1 / ( 1 + Math.Pow(Math.E, (-1) * res));
		//System.Random rnd = new System.Random();
		//return rnd.Next (10);
	}


	public void Run_EA_Loop() {
		Debug.LogWarning ("In loop !!!!!!!!!!");
		List<GeneSeq> geneSeqs = ImportSeqsFromFile ();
		scoreSize = geneSeqs.Count;
		scores = new double[scoreSize];
		for (int i = 0; i < geneSeqs.Count; i++) {
			curSeq = geneSeqs [i];
			Debug.Log ("!!!!!!!!!!!!!" + curSeq.Size());
			StartCoroutine(ExecuteGeneSeq ());

		}

//		int loopCount = 100;
//		while (loopCount > 0) {
//			loopCount--;	
//		}


	}



}

