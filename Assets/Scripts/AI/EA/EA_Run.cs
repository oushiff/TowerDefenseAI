using System;
using System.Collections.Generic;

public class EA_Run
{
	private class Tower {
		int towerIndex;
		int towerLevel;

		public Tower(int index, int level) {
			towerIndex = index;
			towerLevel = level;
		}
	}

	List<int[][]> posList;
	List<int> towerIndices;


	int posSize;
	int towerSize;


	Dictionary<int[][], Tower> map = new Dictionary<int[][], Tower>();


	public List<GeneSeq> CrossOverRes(GeneSeq seq1, GeneSeq seq2, int number){
		List<GeneSeq> listGeneSeq = new List<GeneSeq> ();
		int length = (seq1.Size () > seq2.Size () ? seq2.Size () : seq1.Size ());
		Random rnd = new Random();
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
		Random rnd = new Random ();
		int[][] pos  = posList[rnd.Next (0, posSize)];
		int towerIndex = towerIndices[rnd.Next (0, towerSize)];
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




	void RunGeneSeq(GeneSeq seq) {
		while (seq.hasNext()) {
			//   if money enough  
			GeneNode node = seq.GetNextStep ();
			int[][] pos = node.pos;
			int towerIndex = node.towerIndex;
			if (map.ContainsKey (pos)) {
				Tower tower = map [pos];
				// updateTower 
				// if (true) tower.towerLevel += 1;

			} else {
				map[pos] = new Tower(towerIndex, 1);
			}
		}


	}




}

