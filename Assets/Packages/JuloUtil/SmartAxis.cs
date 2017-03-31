using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	public class SmartAxis {
		InputManager inputManager;
		string axisName;
		
		float[] values;
		bool[] flags;
		float[] timestamp;
		
		public SmartAxis(InputManager inputManager, string axisName) {
			this.inputManager = inputManager;
			this.axisName = axisName;
			
			values = new float[inputManager.numPlayers];
			flags = new bool[inputManager.numPlayers];
			timestamp = new float[inputManager.numPlayers];
			
			float now = JuloTime.applicationTime();
			
			for(int i = 0; i < inputManager.numPlayers; i++) {
				values[i] = 0f;
				flags[i] = false;
				timestamp[i] = now;
			}
		}
		
		public void update() {
			for(int i = 0; i < inputManager.numPlayers; i++) {
				float preValue = values[i];
				float newValue = inputManager.getAxisRaw(axisName, i);
				
				int preSign = JuloMath.sign(preValue);
				int newSign = JuloMath.sign(newValue);
				
				bool isDown = false;
				float now = JuloTime.applicationTime();
				if(preSign != newSign) {
					isDown = newSign != 0;
				} else if(newSign != 0){
					isDown = JuloTime.applicationTimeSince(timestamp[i]) > inputManager.strokeTime;
				}
				
				values[i] = newValue;
				flags[i] = isDown;
				if(isDown) {
					timestamp[i] = now;
				}
			}
		}
		public float getValue(int player) {
			return values[player];
		}
		public bool isDown(int player) {
			return flags[player];
		}
	}
}
