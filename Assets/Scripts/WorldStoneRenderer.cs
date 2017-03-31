
using UnityEngine;

namespace JuloGo {
	
	public class WorldStoneRenderer : StoneRenderer {
		public float unitsPerBoardPoint;
		
		SpriteRenderer sr;
		
		void Start() {
			sr = GetComponent<SpriteRenderer>();
		}
		
		public override void setSprite(Sprite sprite) {
			sr.sprite = sprite;
		}
		public override void locate(int row, int col, int numRows, int numCols) {
			float x = (col - (numCols - unitsPerBoardPoint) / 2.0f) * unitsPerBoardPoint;
			float y = (row - (numRows - unitsPerBoardPoint) / 2.0f) * unitsPerBoardPoint;
			float z = 0f;
			
			transform.position = new Vector3(x, y, z);
		}
	}

}
