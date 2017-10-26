using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

	public int loadSpecificLevel;
	public TextMesh exitText;

	bool exitGame;
	float countdown;

	void Start (){
		ResetExitPad();
	}

	void Update (){
		// Player must stay on the pad for 3 seconds to exit the game.
		if (exitGame) {
			countdown -= Time.deltaTime;
			if (exitText != null) {
				exitText.text = "Stay on pad to exit.\nExiting in " + Mathf.CeilToInt (countdown).ToString () + "...";
			}
			if (countdown <= 0f) {
				GameObject.FindObjectOfType<LevelManager> ().ExitGame ();
			}
		}
	}

	void OnTriggerEnter (Collider collider){
		if (!exitGame && collider.tag == "Player") {
			exitGame = true;
		}
	}

	void OnTriggerExit (Collider collider){
		if (collider.tag == "Player") {
			ResetExitPad();
		}
	}

	void ResetExitPad (){
		countdown = 3f;
		exitText.text = "Exit Game";
		exitGame = false;
	}

}
