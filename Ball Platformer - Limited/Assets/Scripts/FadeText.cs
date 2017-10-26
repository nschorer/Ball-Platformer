using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeText : MonoBehaviour {

    public float timeBeforeFade = 2;

    Text text;
    Color textColor;
    double timer;

	// Use this for initialization
	void Start () {
        text = gameObject.GetComponent<Text>();
        textColor = text.color;
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer > timeBeforeFade && timer < 6){
            FadeColor();
        }
	}

    void FadeColor(){
        Color fadedColor = textColor;
        fadedColor.a -= Time.deltaTime;
        textColor = fadedColor;
        text.color = textColor;
    }
}
