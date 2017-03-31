using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	public class InputManager : MonoBehaviour { //Impl : InputManager {
		
		public int numPlayers;
		
		public float strokeTime = 0.1f;
		
		public bool inputEnabled = true;
		
		SmartAxis horizontalAxis;
		SmartAxis verticalAxis;
		
		[HideInInspector]
		public int whoIs;
		
		void Start() {
			if(numPlayers < 1)
				throw new ApplicationException("Invalid numPlayers: " + numPlayers);
			whoIs = -1;
			
			horizontalAxis = new SmartAxis(this, "Horizontal");
			verticalAxis = new SmartAxis(this, "Vertical");
		}
		void Update() {
			horizontalAxis.update();
			verticalAxis.update();
		}
		
		public float getAxis(string button) {
			float ret = 0f;
			for(int i = 0; i < numPlayers; i++) {
				float value = getAxis(button, i);
				if(Mathf.Abs(value) > Mathf.Abs(ret)) {
					ret = value;
				}
			}
			return ret;
		}
		
		public float getAxis(string button, int player) {
			if(!inputEnabled) {
				return 0f;
			} else if(button == "Horizontal") {
				return horizontalAxis.getValue(player);
			} else if(button == "Vertical") {
				return verticalAxis.getValue(player);
			} else {
				// TODO replace by GetAxis ?
				return getAxisRaw(button, player);
			}
		}
		/*
		public bool isNonzeroAny(string button) {
			if(!inputEnabled)
				return false;
			
			for(int i = 0; i < numPlayers; i++) {
				float val = getAxis(button, i);
				if(val != 0f) {
					whoIs = i;
					return true;
				}
			}
			whoIs = -1;
			return false;
		}
		*/
		public bool isDownAny(string button) {
			if(!inputEnabled)
				return false;
			for(int i = 0; i < numPlayers; i++) {
				if(isDown(button, i)) {
					whoIs = i;
					return true;
				}
			}
			whoIs = -1;
			return false;
		}
		public int who() {
			return whoIs;
		}
		
		public bool isKey(string keyName) {
			return Input.GetKey(keyName);
		}
		
		public bool isDownKey(string keyName) {
			return Input.GetKeyDown(keyName);
		}
		
		public bool isDown(string button, int player) {
			if(!inputEnabled) {
				return false;
			} else if(button == "Horizontal") {
				return horizontalAxis.isDown(player);
			} else if(button == "Vertical") {
				return verticalAxis.isDown(player);
			} else {
				string buttonName = nameFor(button, player);
				bool ret = Input.GetButtonDown(buttonName);
				/*if(buttonName == "Fire1") {
					Debug.Log("Testing for Fire1: " + ret);
				}*/
				return ret;
			}
		}
		
		public string nameFor(string button, int player) {
			return button + player;
		}
		
		public bool mouseIsDown(int buttonNumber = 0) {
			if(!inputEnabled)
				return false;
			return Input.GetMouseButtonDown(buttonNumber);
		}
		
		public bool mouseIsMoving() {
			if(!inputEnabled)
				return false;
			return Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f;
		}
		
		public Vector2 getMousePosition() {
			return new Vector2(Input.mousePosition.x, /*Screen.height - */Input.mousePosition.y);
		}
		
		public float getAxisRaw(string button, int player) {
			return Input.GetAxisRaw(nameFor(button, player));
		}
		
		void OnGUI() {
			Event e = Event.current;
			
			if(e.type == EventType.KeyDown) {
				//Debug.Log("e.");
			} else if(e.type == EventType.Layout || e.type == EventType.Repaint) {
				// do nothing
			} else if(e.type == EventType.KeyDown || e.type == EventType.KeyUp) {
			} else if(e.type == EventType.MouseDown || e.type == EventType.MouseUp || e.type == EventType.MouseDrag || e.type == EventType.ScrollWheel) {
			} else {
				Debug.Log("event " + e.type + ":");
			}
		}
	}
}
