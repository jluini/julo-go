
using UnityEngine;
using UnityEngine.UI;

namespace JuloGo {
	
	public class UIStoneRenderer : StoneRenderer {
		Image image;
		
		void Start() {
			image = GetComponent<Image>();
		}
		
		public override void setSprite(Sprite sprite) {
			image.sprite = sprite;
		}
		public override void locate(int row, int col, int numRows, int numCols) {
			
			transform.position = new Vector3(0, 0, 0);
		}
	}

}
