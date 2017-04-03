using System;
using System.Collections.Generic;

public class GeneSeq {
	private List<GeneNode> seqList = new List<GeneNode>();
	public int id;


	public void AddNodes(List<GeneNode> nodes) {
		int size = nodes.Count;
		for (int i = size - 1; i >= 0; i--) {
			seqList.Add (nodes[i]);
		}
	}

	public GeneNode GetNodeByIndex(int index) {
		if (index >= this.Size ()) {
			return null;
		}
		return seqList [this.Size () - index - 1];
	}

	public void SetNode(int index, int[][] pos, int towerIndex) {
		if (index >= this.Size ()) {
			return null;
		}
		seqList [this.Size () - index - 1].pos = pos; 
		seqList [this.Size () - index - 1].towerIndex = towerIndex; 
	}

	public void SetNode(int index, int[][] pos) {
		if (index >= this.Size ()) {
			return null;
		}
		seqList [this.Size () - index - 1].pos = pos; 
	}

	public void SetNode(int index, int towerIndex) {
		if (index >= this.Size ()) {
			return null;
		}
		seqList [this.Size () - index - 1].towerIndex = towerIndex; 
	}

	public GeneNode getNextStep() {
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
}
