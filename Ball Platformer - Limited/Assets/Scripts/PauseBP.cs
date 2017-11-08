using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseBP : MonoBehaviour{

    public Button[] pauseButtons;
    public MenuController menuController;
    public Text pauseLevelTitle;

    private bool paused;
    private GameObject[] movingObjects;
    private GameObject[] padObjects;
    private GameObject player;
    private bool pausedThisFrame;
    private bool heldUp, heldDown;
    private int selButtonIdx;
    private LevelManager levelManager;

    private const int RESTART_IDX = 1;

    // Use this for initialization
    void Start(){
        paused = false;
        movingObjects = GameObject.FindGameObjectsWithTag("Moving");
        padObjects = GameObject.FindGameObjectsWithTag("Pad");
        player = GameObject.FindGameObjectWithTag("Player");
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        ShowPauseUI(false);
        SetPauseLevelTitle();
        if (pauseButtons == null) Debug.LogError("Pause buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
    }

    void ShowPauseUI(bool showPause)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = showPause ? 1f : 0f;
        cg.blocksRaycasts = showPause ? true: false;
    }

    public void TogglePause(bool pauseOn)
    {
        paused = pauseOn;
        pausedThisFrame = true;
        heldUp = heldDown = true;

        ShowPauseUI(pauseOn);
        if (pauseButtons != null)
        {
            selButtonIdx = 0;
            EventSystem.current.SetSelectedGameObject(pauseButtons[selButtonIdx].gameObject);
        }

        FreezeOtherObjects(pauseOn);
    }

    void HandleRestartButton() {
        if (SessionData.currentMode == SessionData.GameMode.Challenge) {
            pauseButtons[RESTART_IDX].GetComponentInChildren<Text>().text = "Restart (-1)";

            if (SessionData.numLives <= 1) {
                ButtonHandler.DisableButton(pauseButtons[RESTART_IDX]);
            }
        }
        // Outside of challenge mode, it should be set to "Restart" by default and is always selectable.
    }

    void SetPauseLevelTitle() {
        Text levelTitle = GameObject.FindGameObjectWithTag("Level Title").GetComponent<Text>();
         
        if (pauseLevelTitle != null && levelTitle != null) {
            Color color = levelTitle.color;
            string title = levelTitle.text;

            pauseLevelTitle.text = title;
            pauseLevelTitle.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused) return;

        if (pausedThisFrame){
            pausedThisFrame = false;
            return;
        }

        //DecrementTimers();

        //if (Input.GetKeyDown(KeyCode.Escape)) TogglePause(false);
        //else if (Input.GetKeyDown(KeyCode.W)){
        //    ChangeSelectedButton(true);
        //}else if (Input.GetKeyDown(KeyCode.S)){
        //    ChangeSelectedButton(false);
        //}else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
        //    EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        //}

        //Handle controller inputs
        //if (menuController.Pause()) TogglePause(false);
        //else if (menuController.Up()){
        //    ChangeSelectedButton(true);
        //}else if (menuController.Down()){
        //    ChangeSelectedButton(false);
        //}else if (menuController.Submit()){
        //    EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        //}else if (menuController.Cancel()){
        //    TogglePause(false);
        //}

    }

    public void FreezeOtherObjects(bool pauseOn)
    {
        IMovingObject[] moveScripts;
        IMovingObject mObject;
        IPad pObject;
        foreach (GameObject movingObject in movingObjects)
        {
            moveScripts = movingObject.GetComponents<IMovingObject>();

            foreach (IMovingObject moveScript in moveScripts)
            {
                if (moveScript != null) moveScript.PauseRB(pauseOn);
                else Debug.LogError("Object does not have IMoving attached: " + movingObject.ToString());
            }
        }

        foreach (GameObject padObject in padObjects)
        {
            pObject = padObject.GetComponent<IPad>();
            if (pObject != null) pObject.PausePad(pauseOn);
            else Debug.LogError("Object does not have IPad attached: " + padObject.ToString());
        }

        mObject = player.GetComponent<IMovingObject>();
        if (mObject != null) mObject.PauseRB(pauseOn);
        else Debug.LogError("PlayerController doesn't have IMovingObject attached.");
    }

    //void ChangeSelectedButton(bool pressedUp){
    //    if (pauseButtons != null)
    //    {
    //        selButtonIdx += pressedUp ? -1 : 1;
    //        if (selButtonIdx < 0) selButtonIdx = pauseButtons.Length - 1;
    //        else if (selButtonIdx >= pauseButtons.Length) selButtonIdx = 0;
    //        EventSystem.current.SetSelectedGameObject(pauseButtons[selButtonIdx].gameObject);
    //    }
    //}

    public void ResumeButtonPressed()
    {
        TogglePause(false);
    }

    public void RestartButtonPressed()
    {
        levelManager.ReloadLevel();
    }

    public void ExitButtonPressed()
    {
        levelManager.LoadStart();
    }
}
