
using UnityEngine;

namespace JuloAudio {
	
	public abstract class SoundSource : MonoBehaviour {
		bool on = true;
		public bool isOn {
			get {
				return on;
			}
			set {
				on = value;
				onSwitch();
			}
		}
		public float volume = 1.0f;
		
		private AudioSource _audioSource;
		protected AudioSource audioSource {
			get {
				if(_audioSource == null)
					_audioSource = GetComponent<AudioSource>();
				return _audioSource;
			}
		}
		
		public void init() {
			audioSource.volume = volume;
			
			onInit();
		}
		public void setVolume(float newVolume) {
			volume = newVolume;
			audioSource.volume = volume;
		}
		
		public virtual void onInit() { }
		public virtual void onSwitch() { }
		public abstract void play();
		public abstract void pause();
		public abstract void stop();
		public abstract void next();
		public abstract void playClip(AudioClip clip, float volume = 1f);
		
		public abstract bool isPlaying();
	}
	
}
