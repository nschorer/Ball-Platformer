using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestTime : MonoBehaviour {

    public Text bestTimeText;

    float bestTime;
    bool newBestTime;
    float flashTime;
    GameData gameData;

    const float FLASH_INTERVAL = 0.25f;

	// Use this for initialization
	void Start () {
        gameData = GameObject.FindObjectOfType<GameData>();
        bestTime = gameData.getBestTime();
        if (bestTime == 0f) bestTime = 1000f; //upper bound
        UpdateText();
	}
	
	// Update is called once per frame
	void Update () {
        if (newBestTime) FlashText();
	}

    void FlashText(){
        if (flashTime <= 0f){
            Color textColor = bestTimeText.color;
            textColor.r = 1 - textColor.r;
            textColor.g = 1 - textColor.g;
            textColor.b = 1 - textColor.b;
            bestTimeText.color = textColor;

            flashTime = FLASH_INTERVAL;
        }else{
            flashTime -= Time.deltaTime;
        }
    }

    void UpdateText(){
        if (bestTimeText.text != null){
            if (bestTime >= 1000f){
                bestTimeText.text = "---------";
            }
            else {
                bestTimeText.text = bestTime.ToString("n2");
            }
        }
    }

    public void CheckForNewBestTime(float newTime)
    {
        if (newTime < bestTime)
        {
            bestTime = newTime;
            UpdateText();
            gameData.setBestTime(bestTime);
            newBestTime = true;
        }
    }
}
