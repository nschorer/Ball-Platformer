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
    BestTime bestTime;
    GameData gameData;

	void Start (){
		countdown = 3f;
		winText = transform.GetChild(0).GetComponent<TextMesh>();

		levelManager = GameObject.FindObjectOfType<LevelManager> ();
		textPrefix = levelManager.GetLoadingText(whatNext);

        timerBP = GameObject.FindGameObjectWithTag("Timer").GetComponent<TimerBP>();
        bestTime = GameObject.FindGameObjectWithTag("Best Time").GetComponent<BestTime>();
        gameData = GameObject.FindObjectOfType<GameData>().GetComponent<GameData>();
	}

	void Update (){
		if (beatLevel) {
			countdown -= Time.deltaTime;
			if (winText != null) {
				winText.text = textPrefix + Mathf.CeilToInt (countdown).ToString () + "...";
			}

			if (countdown <= 0f) {
                if (SessionData.currentMode == SessionData.GameMode.Exploration) gameData.incrementLevelCleared();
                LoadNext();
			}
		}
	}

    // Determine what scene to lad next.
    // And determine what data needs to be saved.
    void LoadNext(){
        if (levelManager != null){

            // In practice mode, simply repeat the same level.
            if (SessionData.currentMode == SessionData.GameMode.Practice) {
                gameData.Save();
                levelManager.ReloadLevel();

            // If level 1-9, go to the next level.
            }else if (whatNext == WhatToLoadNext.NextLevel){
                gameData.Save();
                SessionData.currentLevel++;
                levelManager.LoadNextLevel();
            }

            // If level 10, return to Hub.
            else if (whatNext == WhatToLoadNext.Start){
                SessionData.currentLevel = 0;
                if (SessionData.currentMode == SessionData.GameMode.Challenge) gameData.clearChallenge();
                gameData.Save();
                levelManager.LoadStart();
            }
            else{
                Debug.LogError("Could not figure out which level to load.");
            }
        }else{
            Debug.LogError("You must create a Level Manager to change levels.", levelManager);
        }
    }

	// When player touches this pad, they've beat the level.
	// Load the next level and make sure the player can no longer die.
	void OnTriggerEnter (Collider collider){
		if (!beatLevel && collider.tag == "Player") {
			beatLevel = true;
			collider.GetComponent<PlayerController>().BeatLevel();
            float time = timerBP.ToggleTimer(false);
            if (bestTime != null) bestTime.CheckForNewBestTime(time);
		}
	}
}
