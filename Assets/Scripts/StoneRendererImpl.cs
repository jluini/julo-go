
using UnityEngine;

using JuloUtil;

namespace JuloGo {
	
	public class StoneRendererImpl : StoneRenderer {
		public float unitsPerBoardPoint = 1.0f;
		
		SpriteRenderer sr;
		SpriteRenderer mark;
		
		void Awake() {
			sr = GetComponent<SpriteRenderer>();
			mark = JuloFind.byName<SpriteRenderer>("Mark", this);
		}
		
		public override void setSprite(Sprite sprite) {
			sr.sprite = sprite;
		}
		public override void setMark(Sprite sprite) {
			mark.sprite = sprite;
		}
		public override void locate(int row, int col, int numRows, int numCols) {
			float x = (col - (numCols - unitsPerBoardPoint) / 2.0f) * unitsPerBoardPoint;
			float y = (row - (numRows - unitsPerBoardPoint) / 2.0f) * unitsPerBoardPoint;
			float z = 0f;
			
			transform.position = new Vector3(x, y, z);
		}
	}

}
