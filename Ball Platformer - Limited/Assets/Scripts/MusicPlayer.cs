using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	// Make sure we only ever have one music player
	static MusicPlayer instance = null;

	void Awake ()
	{
		// If this is our 2nd time at the start screen, destroy the new Music Player before it instantiates
		if (instance != null) {
			Destroy (gameObject);		
		} else {
			instance = this;
			// Make sure this doesn't go away when we change scenes
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
}
