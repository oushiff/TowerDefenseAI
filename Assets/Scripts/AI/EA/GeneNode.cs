using System;

public class GeneNode {
	public int[][] pos;
	public int towerIndex;

	public GeneNode(int[][] pos, int towerIndex) {
		this.pos = pos;
		this.towerIndex = towerIndex;
	}
}
