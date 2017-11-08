using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour {

    public float timeBeforeFade = 2;
    public bool isTitleText = false;

    Text text;
    Color textColor;
    double timer;
    bool dontShow;

	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();
        textColor = text.color;
        timer = 0;

        if (isTitleText) {
            if (!SessionData.hasShownTitle) {
                SessionData.hasShownTitle = true;
            } else {
                text.color = Color.clear;
                dontShow = true;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!dontShow) {

            timer += Time.deltaTime;

            if (timer > timeBeforeFade && timer < 6) {
                FadeColor();
            }
        }
	}

    void FadeColor(){
        Color fadedColor = textColor;
        fadedColor.a -= Time.deltaTime;
        textColor = fadedColor;
        text.color = textColor;
    }
}
