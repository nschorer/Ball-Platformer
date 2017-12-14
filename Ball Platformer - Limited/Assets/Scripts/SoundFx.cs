using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFx : MonoBehaviour {

    public AudioClip[] effects;

    SoundFx instance;
    AudioSource aSource;
    float volume;
    bool[] isPlaying;
    float climbTimer;
    float boostTimer;
    float rewindTimer;
    float movingPlatTimer;

    private static int MENU_SUBMIT = 0;
    private static int MENU_CANCEL = 1;
    private static int MENU_NAV = 2;
    private static int LEVEL_START = 3;
    private static int MOVING_PLAT = 4;
    private static int BOOST = 5;
    private static int BOUNCE_LOW = 6;
    private static int BOUNCE_MED = 7;
    private static int BOUNCE_HIGH = 8;
    private static int CLIMB = 9;
    private static int REWIND = 10;
    private static int DING = 11;

    void Awake() {
        // If this is our 2nd time at the start screen, destroy the new Music Player before it instantiates
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            // Make sure this doesn't go away when we change scenes
            GameObject.DontDestroyOnLoad(gameObject);
            aSource = GetComponent<AudioSource>();
        }
    }

    // Use this for initialization
    void Start () {
        if (effects != null) isPlaying = new bool[effects.Length];
	}
	
	// Update is called once per frame
	void Update () {
        if (climbTimer > 0) climbTimer -= Time.deltaTime;
        if (boostTimer > 0) boostTimer -= Time.deltaTime;
        if (rewindTimer > 0) rewindTimer -= Time.deltaTime;
        if (movingPlatTimer > 0) movingPlatTimer -= Time.deltaTime;
	}

    public void SetVolume(float vol) {
        volume = vol;
        aSource.volume = vol;
    }

    public void MenuSubmit() {
        aSource.Stop();
        aSource.clip = effects[MENU_SUBMIT];
        aSource.Play();
    }

    public void MenuCancel() {
        aSource.Stop();
        aSource.clip = effects[MENU_CANCEL];
        aSource.Play();
    }

    public void MenuNavigate() {
        aSource.Stop();
        aSource.clip = effects[MENU_NAV];
        aSource.Play();
    }

    public void LevelStart(Vector3 loc) {
        AudioSource.PlayClipAtPoint(effects[LEVEL_START], loc, volume);
    }

    public void MovingPlat(Vector3 loc) {
        if (movingPlatTimer <= 0) {
            AudioSource.PlayClipAtPoint(effects[MOVING_PLAT], loc, volume);
            movingPlatTimer = 0.4f;
        }
    }

    public void Boost(Vector3 loc) {
        if (boostTimer <= 0) {
            AudioSource.PlayClipAtPoint(effects[BOOST], loc, volume);
            boostTimer = 0.3333f;
        }
    }

    public void Bounce(Vector3 loc, int strength) {
        if (strength == 1) {
            AudioSource.PlayClipAtPoint(effects[BOUNCE_LOW], loc, volume);
        } else if (strength == 2) {
            AudioSource.PlayClipAtPoint(effects[BOUNCE_MED], loc, volume);
        } else if (strength == 3) {
            AudioSource.PlayClipAtPoint(effects[BOUNCE_HIGH], loc, volume);
        }
    }

    public void Climb(Vector3 loc) {
        if (climbTimer <= 0) {
            AudioSource.PlayClipAtPoint(effects[CLIMB], loc, volume);
            climbTimer = 0.25f;
        }
    }

    public void Rewind(Vector3 loc, bool start = true) {
        if (start) {
            AudioSource.PlayClipAtPoint(effects[REWIND], loc, volume);
            rewindTimer = 1f;
        } else AudioSource.PlayClipAtPoint(effects[DING], loc, volume);
    }
}
