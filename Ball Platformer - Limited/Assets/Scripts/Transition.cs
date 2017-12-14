using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour {

    public float letterPause = 0.2f;
    public AudioClip sound;
    public Text worldText;
    public Image fadeOut;
    public LevelManager levelManager;

    string message;
    float delay;
    float timer = 7f;

    // Use this for initialization
    void Start() {
        message = worldText.text;
        worldText.text = "";
        delay = 5f;
        StartCoroutine(TypeText());
    }

    void Update() {
        timer -= Time.deltaTime;
        if (delay > 0) {
            delay -= Time.deltaTime;
        }else {
            Color color = fadeOut.color;
            color.a += (1f / 60f);
            fadeOut.color = color;
        }

        if (timer <= 0) {
            levelManager.LoadLevel(SessionData.currentLevel);
        }
    }

    IEnumerator TypeText() {
        foreach (char letter in message.ToCharArray()) {
            worldText.text += letter;
            if (sound)
                
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
    }
}
