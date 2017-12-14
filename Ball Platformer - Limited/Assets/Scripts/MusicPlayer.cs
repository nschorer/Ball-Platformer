using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public AudioClip[] titleMusic;
    public AudioClip[] hubMusic;
    public AudioClip[] forestMusic;
    public AudioClip[] desertMusic;
    public AudioClip[] canyonMusic;
    public AudioClip[] islandMusic;
    public AudioClip[] spaceMusic;

	// Make sure we only ever have one music player
	static MusicPlayer instance = null;

    AudioSource aSource;
    bool fadeOut;
    float startFadeOut;
    float volume;
    AudioClip[] nextSong;
    bool playAfterFade;
    bool hasNextClip;
    int clipIdx;
    bool switchingSong;

    const float FADEOUT_LENGTH = .8f;

	void Awake (){
		// If this is our 2nd time at the start screen, destroy the new Music Player before it instantiates
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			// Make sure this doesn't go away when we change scenes
			GameObject.DontDestroyOnLoad(gameObject);
            aSource = GetComponent<AudioSource>();
        }
	}

    void Start() {
        SwitchSong(null, true);
    }

    void Update() {

        if (hasNextClip && !aSource.isPlaying && !switchingSong) {
            MoveToNextSongSegment();
        }

        if (fadeOut) {
            float timeSinceFadeStart = Time.time - startFadeOut;

            aSource.volume = Mathf.Lerp(volume, 0f, timeSinceFadeStart / FADEOUT_LENGTH);

            if (timeSinceFadeStart >= FADEOUT_LENGTH) {
                fadeOut = false;
                aSource.Stop();
                
                if (playAfterFade) Play();
            }
        }
    }

    // A song is an array of audio clips. This is in case we have something 
    // like an introduction segment to a song that should not be looped.
    // If no parameter is passed, it is assumed that nextSong has been set.
    public void Play(AudioClip[] clips = null) {
        if (clips != null) nextSong = clips;

        switchingSong = false;
        aSource.volume = volume;
        clipIdx = -1;
        MoveToNextSongSegment();
    }

    public bool IsPlaying() {
        return aSource.isPlaying;
    }

    void MoveToNextSongSegment() {
        clipIdx++;
        if (clipIdx >= nextSong.Length - 1) clipIdx = nextSong.Length - 1;

        aSource.clip = nextSong[clipIdx];
        hasNextClip = (clipIdx < nextSong.Length - 1); //Only loop the last clip in the array
        aSource.loop = !hasNextClip;

        aSource.Play();
    }

    public void FadeOut() {
        volume = aSource.volume;
        fadeOut = true;
        startFadeOut = Time.time;
    }

    public void SwitchSong(WorldEntrance.World? world, bool autoplay = false) {
        if (world == WorldEntrance.World.Hub) {
            nextSong = hubMusic;
        } else if (world == WorldEntrance.World.Forest) {
            nextSong = forestMusic;
        } else if (world == WorldEntrance.World.Desert) {
            nextSong = desertMusic;
        } else if (world == WorldEntrance.World.Canyon) {
            nextSong = canyonMusic;
        } else if (world == WorldEntrance.World.Island) {
            nextSong = islandMusic;
        } else if (world == WorldEntrance.World.Space) {
            nextSong = spaceMusic;
        } else {
            nextSong = titleMusic;
        }

        // If we're just going to play the same song, don't bother fading out.
        if (nextSong != null && !isSongPlaying(nextSong)) {
            FadeOut();
            playAfterFade = autoplay;
            switchingSong = true;
        }
    }

    bool isSongPlaying(AudioClip[] clips) {
        foreach (AudioClip clip in clips) {
            if (clip == aSource.clip) return true;
        }
        return false;
    }

    // What's the difference between volume and aSource.volume?
    // volume: the volume set by the user
    // aSource.volume: the actual current volume of the music player
    // So, when we fade out, aSource.volume is decreasing, but volume stays the same,
    //     so we know what value to return to when we start the next song.
    public void SetVolume(float vol) {
        volume = vol;
        aSource.volume = vol;  
    }
}
