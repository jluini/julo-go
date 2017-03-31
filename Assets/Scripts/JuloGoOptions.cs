
using System;
using System.Collections;
using System.Collections.Generic;
﻿
using UnityEngine;
using UnityEngine.Assertions;

namespace JuloGo {
	public class JuloGoOptions : MonoBehaviour {
		public int numRows;
		public int numCols;
		
		public StoneRenderer stoneRendererPrefab;
		public StoneRenderer markPrefab;
		
		public Sprite blackSprite;
		public Sprite whiteSprite;
		
		public AudioClip moveSound;
		public AudioClip invalidMoveSound;
		public AudioClip passSound;
	}
}