using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

    public enum WhatToLoadNext { NextLevel, Start}

    public WhatToLoadNext whatNext = WhatToLoadNext.NextLevel;
	public TextMesh winText;

	bool beatLevel;
	bool startGame;
	float countdown;
	string textPrefix;
	LevelManager levelManager;
    TimerBP timerBP;

	void Start (){
		countdown = 3f;
		winText = transform.GetChild(0).GetComponent<TextMesh>();

		levelManager = GameObject.FindObjectOfType<LevelManager> ();
		textPrefix = levelManager.GetLoadingText(whatNext);

        timerBP = GameObject.FindGameObjectWithTag("Timer").GetComponent<TimerBP>();
	}

	void Update ()
	{
		if (beatLevel) {
			countdown -= Time.deltaTime;
			if (winText != null) {
				winText.text = textPrefix + Mathf.CeilToInt (countdown).ToString () + "...";
			}
			if (countdown <= 0f) {
				LevelManager levelManager = GameObject.FindObjectOfType<LevelManager> ();
				if (levelManager != null) {
					if (whatNext == WhatToLoadNext.NextLevel) {
						levelManager.LoadNextLevel ();
					} else if (whatNext == WhatToLoadNext.Start){
                        levelManager.LoadStart();
                    }else{
                        Debug.LogError("Could not figure out which level to load.");
                    }
				} else {
					Debug.LogError("You must create a Level Manager to change levels.", levelManager);
				}
			}
		}
	}

	// When player touches this pad, they've beat the level.
	// Load the next level and make sure the player can no longer die.
	void OnTriggerEnter (Collider collider){
		if (!beatLevel && collider.tag == "Player") {
			beatLevel = true;
			collider.GetComponent<PlayerController>().BeatLevel();
            timerBP.ToggleTimer(false);
		}
	}
}
