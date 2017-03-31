
using UnityEngine;

namespace JuloAudio {
	
	public class SoundsPlayerBehaviour : SoundSource {
		public override void playClip(AudioClip clip, float volume = 1f) {
			if(isOn)
				audioSource.PlayOneShot(clip, volume);
		}
		
		public override void play() { }
		public override void pause() { }
		public override void stop() { }
		public override void next() { }
		public override bool isPlaying() {
			return audioSource.isPlaying;
		}
	}
}
