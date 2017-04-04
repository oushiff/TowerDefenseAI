using System;
using System.Collections.Generic;

public class GeneSeq {
	private List<GeneNode> seqList = new List<GeneNode>();
	public int id;
	public List<int> parentIds;

	public void InitRandom(List<int[][]> posList, List<int> towerIndices, int size) {
		int posSize = posList.Count;
		int towerIndicesSize = towerIndices.Count;
		Random rnd = new Random();
		for (int i = 0; i < size; i++) {
			GeneNode node = new GeneNode (posList [rnd.Next (posSize)], towerIndices [rnd.Next (towerIndicesSize)]);
			seqList.Add (node);
		}
	}

	public void InitByNodes(List<GeneNode> nodes) {
		int size = nodes.Count;
		for (int i = size - 1; i >= 0; i--) {
			seqList.Add (nodes[i]);
		}
	}

	public GeneNode GetNodeByIndex(int index) {
		if (index >= Size () || index < 0) {
			return null;
		}
		return seqList [this.Size () - index - 1];
	}

	public void SetNode(int index, int[][] pos, int towerIndex) {
		if (index >= Size () || index < 0) {
			return;
		}
		seqList [this.Size () - index - 1].pos = pos; 
		seqList [this.Size () - index - 1].towerIndex = towerIndex; 
	}

	public void SetNode(int index, int[][] pos) {
		if (index >= Size () || index < 0) {
			return;
		}
		seqList [this.Size () - index - 1].pos = pos; 
	}

	public void SetNode(int index, int towerIndex) {
		if (index >= Size () || index < 0) {
			return;
		}
		seqList [this.Size () - index - 1].towerIndex = towerIndex; 
	}

	public GeneNode GetNextStep() {
		int size = this.Size ();
		if (size <= 0) {
			return null;
		} 
		GeneNode node = seqList [size - 1];
		seqList.RemoveAt(size - 1);
		return node;
	}

	public int Size() {
		return seqList.Count;
	}
		
	public GeneSeq CrossOverWith(GeneSeq seq, int afterIndex) {
		List<GeneNode> nodes = new List<GeneNode> ();
		int i = 0;
		for (; i <= afterIndex; i++) {
			seq.GetNextStep ();
			nodes.Add (this.GetNextStep ());
		}
		for (; i < seq.Size (); i++) {
			nodes.Add(seq.GetNextStep());
		}
		GeneSeq newSeq = new GeneSeq ();
		newSeq.InitByNodes (nodes);
		return newSeq;
	}

	public void AddNodeAfterIndex(int index, GeneNode node) {
		if (index >= Size () || index < 0)
			return;
		this.seqList.Insert (Size () - index - 1, node);
	}

	public void RemoveNodeAt(int index) {
		if (index >= Size () || index < 0)
			return;
		this.seqList.RemoveAt (Size () - index - 1);
	}

	public GeneSeq DeepCopy() {
		return null; //-----
	}

	public List<GeneSeq> GenerateMutated(int index, int[][] pos, int towerIndex) {
		List<GeneSeq> res = new List<GeneSeq> ();
		GeneSeq seq1 = DeepCopy();
		GeneSeq seq2 = DeepCopy ();
		GeneSeq seq3 = DeepCopy ();
//		seq1.SetNode ();
//		seq2.AddNode ();
//		seq3.RemoveNode ();
		res.Add (seq1);
		res.Add (seq2);
		res.Add (seq3);
		return res;
	}
}
