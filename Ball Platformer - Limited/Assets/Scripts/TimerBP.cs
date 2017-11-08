using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBP : MonoBehaviour
{
    public Text timerText;

    private float timer;
    private bool timerActive;

    // Use this for initialization
    void Start()
    {
        timer = 0f;
        if (timerText == null) Debug.LogError("Timer Text is not configured.");
        UpdateText();
    }

    // Update is called once per frame
    void Update(){
        if (timerActive)
        {
            timer += Time.deltaTime;
            if (timer >= 1000f){
                timer = 999.99f;
                timerActive = false;
            }
            UpdateText();
        }
    }

    void UpdateText(){
        // We need to check this here in order to make sure that the timer doesn't increment after we ToggleTimer()
        if (timerActive){
            if (timerText.text != null) timerText.text = GetTime().ToString("n2");
        }
    }

    public float ToggleTimer(bool active){
        timerActive = active;
        return GetTime();
    }

    public float GetTime(){
        return (Mathf.Floor(timer * 100f)) / 100f;
    }
}
