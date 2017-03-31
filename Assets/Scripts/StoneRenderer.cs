
using UnityEngine;

namespace JuloGo {
	
	public abstract class StoneRenderer : MonoBehaviour {
		
		public abstract void setSprite(Sprite sprite);
		public abstract void locate(int row, int col, int numRows, int numCols);
		
	}

}
