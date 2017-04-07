using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class GeneSeqTrans
	{
		public 

		List<int[][]> posList;
		List<int> towerIndices ;
		int posSize;
		int towerSize;

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
				GeneSeq newSeq = new GeneSeq ();
				newSeq.InitByNodes (nodes);
				listGeneSeq.Add (newSeq);
			}
			return listGeneSeq;
		}
			

		public List<GeneSeq> Mutate(GeneSeq seq,int number){	       
			List<GeneSeq> listGeneSeq = new List<GeneSeq> ();
			int length = seq.Size ();
			Random rnd = new Random ();
			int[][] pos  = posList[rnd.Next (0, posSize)];
			int tower = towerIndices[rnd.Next (0, towerSize)];
			int index = rnd.Next (0, length);
			for (int i = 0; i < number; i++) {		
				GeneSeq newSeq = seq.DeepCopy();
				newSeq.SetNode (index, pos, tower);
				listGeneSeq.Add (newSeq);
				newSeq = seq.DeepCopy();
				newSeq.InsertNodeAfterIndex (index, new GeneNode(pos, tower));
				listGeneSeq.Add (newSeq);
				newSeq = seq.DeepCopy();
				newSeq.RemoveNodeAt (index);
				listGeneSeq.Add (newSeq);
			}
			return listGeneSeq;
		}


	}
}

