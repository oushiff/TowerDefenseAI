﻿using System;
using System.Collections.Generic;

public class GeneSeq {
	private List<GeneNode> seqList = new List<GeneNode>();
//	public int id;
//	public List<int> parentIds;

	public GeneSeq() {}

	public GeneSeq(List<GeneNode> nodes) {
		int size = nodes.Count;
		for (int i = size - 1; i >= 0; i--) {
			seqList.Add (nodes[i]);
		}
	}

	public GeneSeq(GeneSeq seq) {
		this.seqList = seq.GetReverseNodeList();
//		this.id = seq.id;
//		this.parentIds = seq.parentIds;
	}

	public void InitRandom(List<int[]> posList, int towerIndicesSize, int size) {
		int posSize = posList.Count;
		Random rnd = new Random();
		for (int i = 0; i < size; i++) {
			GeneNode node = new GeneNode (posList [rnd.Next (posSize)], rnd.Next (towerIndicesSize));
			seqList.Add (node);
		}
	}
		
	// Avoid to use this function, because it will return reverse list
	public List<GeneNode> GetReverseNodeList() {
		return seqList;
	}

	public GeneNode GetNodeByIndex(int index) {
		if (index >= Size () || index < 0) {
			return null;
		}
		return seqList [this.Size () - index - 1];
	}

	public void SetNode(int index, int[] pos, int towerIndex) {
		if (index >= Size () || index < 0) {
			return;
		}
		seqList [this.Size () - index - 1].pos = pos; 
		seqList [this.Size () - index - 1].towerIndex = towerIndex; 
	}

	public void SetNode(int index, int[] pos) {
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


	public bool hasNext() {
		if (this.Size () > 0) {
			return true;
		} else {
			return false;
		}
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
		
	public void InsertNodeAfterIndex(int index, GeneNode node) {
		if (index >= Size () || index < 0)
			return;
		this.seqList.Insert (Size () - index - 1, node);
	}

	public void RemoveNodeAt(int index) {
		if (index >= Size () || index < 0)
			return;
		this.seqList.RemoveAt (Size () - index - 1);
	}

	public string Serialize() {
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		int count = seqList.Count;
		sb.Append (count + "\n");
		for (int i = count - 1; i >= 0; i--) {
			sb.Append (seqList [i].pos [0] + " " + seqList [i].pos [1] + " " + seqList [i].towerIndex + "\n");
		}
		sb.Append (";\n");
		return sb.ToString();
	}

	public GeneSeq Deserialize(string stream) {
		string[] lines = stream.Split ('\n');
		for (int i = 1; i < lines.Length; i++) {			
			string line = lines [i];
			string[] elems = line.Split (' ');
			if (elems.Length != 3) {
				break;
			}
			int[] pos = new int[2];
			pos [0] = Int32.Parse (elems [0]);
			pos [1] = Int32.Parse (elems [1]);
			GeneNode node = new GeneNode (pos, Int32.Parse (elems [2]));
			seqList.Add (node);
		}
		return this;
	}


}
