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
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
            if (timer >= 1000f)
            {
                timer = 999.99f;
                timerActive = false;
            }
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (timerText.text != null) timerText.text = timer.ToString("n2");
    }

    public void ToggleTimer(bool active)
    {
        timerActive = active;
    }

    public float GetTime()
    {
        return (Mathf.Floor(timer * 100f)) / 100f;
    }
}
