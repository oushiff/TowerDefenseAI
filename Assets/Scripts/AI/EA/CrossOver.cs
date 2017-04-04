using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class CrossOver
	{

		public List<int[][]> posList = new List<int[][]> ();
		public List<int> towerIndices = new List<int[][]> ();
		int posSize = posList.Count();
		int towerSize = towerIndices.Count;

		public List<GeneSeq> CrossOverRes(GeneSeq seq1, GeneSeq seq2, int number){

			List<GeneSeq> ListGeneSeq = new List<GeneSeq> ();

			int length = (seq1.Size () > seq2.Size () ? seq2.Size () : seq1.Size ());
			Random rnd = new Random();

			for (int i = 0; i < number; i++) {
				int index = rnd.Next (1, length);
				ListGeneSeq.Add (seq1.crossOver (seq2, index));
			}
			return ListGeneSeq;
		}

		public List<GeneSeq> Mutate(GeneSeq seq,int number){
		       
			List<GeneSeq> geneSeq = new List<GeneSeq> ();
			int length = seq.Size ();
			Random rnd = new Random ();
			int pos  = rnd.Next (0, posSize);
			int tower = rnd.Next (0, towerSize);
			int index = rnd.Next (0, length);
			int getNodeindex = rnd.Next (0, length);
			for (int i = 0; i < number; i++) {		
				geneSeq.AddRange(new List<>(seq.GenerateMutated(index,posList[pos],towerIndices[tower])));
				geneSeq.Add(seq.AddNodeAfterIndex (index, seq.GetNodeByIndex (getNodeindex)));
				geneSeq.Add(seq.RemoveNodeAt (index));
			}
			return geneSeq;
		}
	}
}

