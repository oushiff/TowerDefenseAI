using System;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class EA_Run //: MonoBehaviour
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

	private MapMonitor m_map;
	private TowerMonitor m_tower;

	public static EA_Run instance;

	public AI.GameOperater GO;//= GameObject.; 

	public Dictionary<String, GameObject> gameObjMap;

	List<int[]> posList = new List<int[]> ();
	int posSize;
	int towerSize;



	public EA_Run()
	{
		m_map = new MapMonitor ();
		m_tower = new TowerMonitor ();
		List <double[]> tmpList = m_map.GetAllCandidateSpacesAtBeginning ();

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
		GO = new GameOperater ();
		gameObjMap = GO.gameObjMap;
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
		int[] pos  = posList[rnd.Next (0, posSize)];
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
			GeneSeq newSeq = new GeneSeq ();
			string newPart = part + ";\n";
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

	void ExecuteGeneSeq(GeneSeq seq) {
		while (seq.hasNext()) {
			GeneNode node = seq.GetNextStep ();
			int[] pos = node.pos;
			int towerIndex = node.towerIndex;
			String posStr = "";
			posStr += pos [0]; 
			posStr += pos [1];

			double[] posDouble = new double[2];
			posDouble [0] = (double)pos [0];
			posDouble [1] = (double)pos [1];

			if (!gameObjMap.ContainsKey (posStr)) {
				while (true) {
					if (GO.BuildTower (towerIndex, posDouble)) {
						break;
					} else {
						System.Threading.Thread.Sleep (3000);
					}
				}
			} else {
				if (!GO.IsUpgradable(posDouble)) 
					continue;
				while (true) {
					if (GO.UpgradeTower (0, posDouble)) {
						break;
					} else {
						System.Threading.Thread.Sleep (3000);
					}
				}
			}
		}

	}


	public int GetScore() {
		System.Random rnd = new System.Random();
		return rnd.Next (10);
	}


	public void Run_EA_Loop() {
		List<GeneSeq> geneSeqs = ImportSeqsFromFile ();
		int[] scores = new int[geneSeqs.Count];
		for (int i = 0; i < geneSeqs.Count; i++) {
			ExecuteGeneSeq (geneSeqs[i]);
			scores[i] = GetScore ();
		}

//		int loopCount = 100;
//		while (loopCount > 0) {
//			loopCount--;	
//		}


	}



}

