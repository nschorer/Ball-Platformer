using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingGun : MonoBehaviour {

    public string goText = "GO!";

    private Text text;
    private float startDelay;
    private bool started;
    private TimerBP timerBP;
    private PauseBP pauseBP;
    private bool isFirstFrame;
    private Color lvlTitleColor;
    private static MusicPlayer mPlayer;
    private PlayerController player;

	// Use this for initialization
	void Start () {
        startDelay = 1f;
        text = GetComponent<Text>();
        text.text = goText;
        ToggleTextVisibility(false);

        Text levelTitle = GameObject.FindGameObjectWithTag("Level Title").GetComponent<Text>();
        if (levelTitle != null) {
            lvlTitleColor = levelTitle.color;
            lvlTitleColor.a = 0f;
            text.color = lvlTitleColor;
        }else {
            Debug.LogError("Could not find level title");
        }

        timerBP = GameObject.FindGameObjectWithTag("Timer").GetComponent<TimerBP>();
        if (timerBP == null) Debug.LogError("TimerBP has not been configured.");

        pauseBP = GameObject.FindGameObjectWithTag("Pause").GetComponent<PauseBP>();
        if (pauseBP == null) Debug.LogError("PauseBP has not been configured.");

        if (mPlayer == null) mPlayer = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>();
        if (mPlayer == null) Debug.LogError("MusicPlayer has not been configured.");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        isFirstFrame = true;
	}
	
	// Update is called once per frame
	void Update () {

        //It looks like this can't be done in Start() because the objects aren't initialized yet. So we'll do it on the first frame.
        if (isFirstFrame){
            pauseBP.FreezeOtherObjects(true);
            isFirstFrame = false;
        }

        if (startDelay > 0f) startDelay -= Time.deltaTime;
        else if (!started) FireGun();
	}

    void ToggleTextVisibility(bool isVisibile){
        Color color = text.color;
        color.a = isVisibile ? 255f : 0f;
        text.color = color;
    }

    void FireGun(){
        started = true;
        ToggleTextVisibility(true);
        timerBP.ToggleTimer(true);
        pauseBP.FreezeOtherObjects(false);
        if (!mPlayer.IsPlaying()) mPlayer.Play();

        GameObject obj = GameObject.FindGameObjectWithTag("Sound Fx");
        SoundFx soundfx = null;
        if (obj != null) soundfx = obj.GetComponent<SoundFx>();
        if (soundfx != null) soundfx.LevelStart(player.transform.position);
    }
}
