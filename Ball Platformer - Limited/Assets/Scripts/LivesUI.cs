using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour {

    public Text numLives;
    public CanvasGroup cg;

	// Use this for initialization
	void Start () {
        if (SessionData.currentMode != SessionData.GameMode.Challenge) {
            cg.alpha = 0f;
        }else {
            cg.alpha = 1f;

            numLives.text = "x " + SessionData.numLives.ToString();
        }
	}
}
