using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseBP : MonoBehaviour{

    public Button[] topButtons;
    public MenuController menuController;
    public Text pauseLevelTitle;
    public CanvasGroup topMenu, abMenu, settingsMenu;
    public Button abBackButton;
    public Button changeAbButton;
    public Button settingsBackButton;
    public Slider musicVolumeSlider;
    public Slider soundFXSlider;
    public Slider camSensSlider;
    public Button changeColorButton;
    public Text instructionsText;

    public enum PMenu { None, TopMenu, Abilities, Settings };

    private PMenu currentMenu;
    private bool paused;
    private GameObject[] movingObjects;
    private GameObject[] padObjects;
    private GameObject player;
    private LevelManager levelManager;
    private bool[] abArray;
    private bool[] colorArray;
    private int abIdx;
    private SoundFx soundfx;
    private GameData gameData;
    private bool firstTimePausing;
    private int colorIdx;
    private bool cgChangedThisFrame;
    private int buttonIdx;

    private const int RESTART_IDX = 1;
    private const int ABILITIES_IDX = 2;

    // Use this for initialization
    void Start(){
        paused = false;
        movingObjects = GameObject.FindGameObjectsWithTag("Moving");
        padObjects = GameObject.FindGameObjectsWithTag("Pad");
        player = GameObject.FindGameObjectWithTag("Player");
        soundfx = GameObject.FindGameObjectWithTag("Sound Fx").GetComponent<SoundFx>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        gameData = GameData.currentGameFile;
        ShowPauseUI(false);
        SetPauseLevelTitle();
        HandleRestartButton();
        if (topButtons == null) Debug.LogError("Pause buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
        if (musicVolumeSlider == null) Debug.LogError("Music Volume is not set up.");
        if (soundFXSlider == null) Debug.LogError("Sound FX is not set up.");
        firstTimePausing = true;
    }

    void ShowPauseUI(bool showPause)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        MenuHandler.ShowCG(cg, showPause);

        if (showPause){
            ShowPMenu(PMenu.TopMenu);

            //For things that rely on GameData being loaded.
            if (firstTimePausing) {
                HandleAbilitiesButton();
                if (LevelManager.IsHub()) InitAbArray();

                musicVolumeSlider.value = gameData.MusicVolume;
                soundFXSlider.value = gameData.SoundFXVolume;
                camSensSlider.value = gameData.CameraSensitivity;
                InitColorArray();
                colorIdx = (int)gameData.BallColor;
                SetColorButton();

                firstTimePausing = false;
            }

        }else {
            currentMenu = PMenu.None;
            // We might have changed some settings while we were paused.
            // Even if we didn't change anything, we might as well call Save because the code will immediately quit if there is nothing to save.
            gameData.Save();
        }
    }

    public void TogglePause(bool pauseOn){
        paused = pauseOn;
        if (pauseOn) {
            soundfx.MenuSubmit();
            cgChangedThisFrame = true;
        }else {
            soundfx.MenuCancel();
        }

        ShowPauseUI(pauseOn);
        if (topButtons != null)
        {
            EventSystem.current.SetSelectedGameObject(topButtons[0].gameObject);
        }

        FreezeOtherObjects(pauseOn);
    }

    void ShowPMenu(PMenu menu) {

        EventSystem.current.SetSelectedGameObject(null); //If we don't do this, we could retain focus on the button we just clicked
        currentMenu = menu;

        MenuHandler.ShowCG(topMenu, menu == PMenu.TopMenu);
        if (LevelManager.IsHub()) MenuHandler.ShowCG(abMenu, menu == PMenu.Abilities);
        MenuHandler.ShowCG(settingsMenu, menu == PMenu.Settings);

        buttonIdx = 0;

        GameObject startFocus = topButtons[buttonIdx].gameObject;
        bool noFocus = false;

        // We need to set the focus for submenus on something that is NOT the back button.
        // In the start scene only (for some reason), submit is being double triggered.
        // So, if we focus on the back button, it will immediately click that and go back to the top menu.
        if (menu == PMenu.TopMenu) {
            startFocus = topButtons[0].gameObject;

        } else if (menu == PMenu.Abilities) {
            startFocus = changeAbButton.gameObject;

        } else if (menu == PMenu.Settings) {
            startFocus = musicVolumeSlider.gameObject;  

        } else noFocus = true;

        if (!noFocus) EventSystem.current.SetSelectedGameObject(startFocus);
    }

    void HandleRestartButton() {
        if (SessionData.currentMode == SessionData.GameMode.Challenge && !LevelManager.IsHub()) {
            topButtons[RESTART_IDX].GetComponentInChildren<Text>().text = "Restart (-1)";
            if (SessionData.numLives <= 1) {
                MenuHandler.DisableButton(topButtons[RESTART_IDX]);
            }
        }
        // Outside of challenge mode, it should be set to "Restart" by default and is always selectable.
    }

    void HandleAbilitiesButton() {
        if (LevelManager.IsHub() && abMenu != null) {
            if (!gameData.hasUnlockedAnAbility()) {
                MenuHandler.DisableButton(topButtons[ABILITIES_IDX]);
            }
        }
    }

    void SetPauseLevelTitle() {
        GameObject levelTitleObject = GameObject.FindGameObjectWithTag("Level Title");
        Text levelTitle = null;
        if (levelTitleObject != null) levelTitle = levelTitleObject.GetComponent<Text>();
         
        if (pauseLevelTitle != null && levelTitle != null) {
            Color color = levelTitle.color;
            string title = levelTitle.text;

            pauseLevelTitle.text = title;
            pauseLevelTitle.color = color;
        }
    }

    void InitAbArray() {

        abArray = new bool[5];

        abArray[(int)SessionData.Ability.None] = true;
        abArray[(int)SessionData.Ability.Climb] = gameData.hasClearedForest();
        abArray[(int)SessionData.Ability.Boost] = gameData.hasClearedDesert();
        abArray[(int)SessionData.Ability.Bounce] = gameData.hasClearedCanyon();
        abArray[(int)SessionData.Ability.Rewind] = gameData.hasClearedIsland();

        abIdx = (int)SessionData.currentAbility;
        changeAbButton.GetComponentInChildren<Text>().text = SessionData.currentAbility.ToString();
    }

    void InitColorArray() {
        int maxColors = Enum.GetValues(typeof(PlayerController.BallColor)).Cast<int>().Max() + 1;
        colorArray = new bool[maxColors];

        // Unlocked by default
        colorArray[(int)PlayerController.BallColor.Maroon] = true;
        colorArray[(int)PlayerController.BallColor.Blue] = true;
        colorArray[(int)PlayerController.BallColor.Gray] = true;
        colorArray[(int)PlayerController.BallColor.Green] = true;
        colorArray[(int)PlayerController.BallColor.Purple] = true;
        colorArray[(int)PlayerController.BallColor.Yellow] = true;

        // Unlocked by beating the challenge times
        colorArray[(int)PlayerController.BallColor.Forest] = gameData.hasBeatChallengeTimes(WorldEntrance.World.Forest);
        colorArray[(int)PlayerController.BallColor.Forest] = gameData.hasBeatChallengeTimes(WorldEntrance.World.Forest);
        colorArray[(int)PlayerController.BallColor.Forest] = gameData.hasBeatChallengeTimes(WorldEntrance.World.Forest);
        colorArray[(int)PlayerController.BallColor.Forest] = gameData.hasBeatChallengeTimes(WorldEntrance.World.Forest);
        colorArray[(int)PlayerController.BallColor.Forest] = gameData.hasBeatChallengeTimes(WorldEntrance.World.Forest);

        int WORLD_OFFSET = (int)PlayerController.BallColor.Forest - (int)WorldEntrance.World.Forest;

        for (int i = (int)PlayerController.BallColor.Forest; i<maxColors; i++) {
            bool hasBeatTimes = gameData.hasBeatChallengeTimes((WorldEntrance.World) (i - WORLD_OFFSET));
            colorArray[i] = hasBeatTimes;
        }
    }

    // Update is called once per frame
    void Update(){
        //Only make changes in pause menu if actually paused!
        if (!paused) return;

        if (!cgChangedThisFrame) {
            //If we lose focus on a button/slider, reset the current menu
            if (EventSystem.current.currentSelectedGameObject == null) ShowPMenu(currentMenu);
            SetHighlightedColor();

        
            if (currentMenu == PMenu.TopMenu) HandleTopMenu();
            else if (currentMenu == PMenu.Abilities) HandleAbilitiesMenu();
            else if (currentMenu == PMenu.Settings) HandleSettingsMenu();

            //Copied from LevelSelect.cs
            if (menuController.Cancel()) {
                soundfx.MenuCancel();
                BackPressed();
            }else if (menuController.Submit()) {
                Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                if (button != null) {
                    soundfx.MenuSubmit();
                    button.onClick.Invoke();
                }
            }
        }
        
        cgChangedThisFrame = false;
    }

    // Copied from LevelSelect.cs
    // Get buttons/sliders to fade in and out to show that they are selected
    public void SetHighlightedColor() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        Button button = curObj.GetComponent<Button>();
        Slider slider = curObj.GetComponent<Slider>();

        float mod = Mathf.Sin(Time.time*4) * MenuHandler.buttonFlash;
        Color color = MenuHandler.buttonColor;
        color.r += mod;
        color.g += mod;
        color.b += mod;

        if (button != null) {
            ColorBlock colors = button.colors;
            colors.highlightedColor = color;
            button.colors = colors;
        }else if (slider != null) {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = color;
            slider.colors = colors;
        }
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

    //Copied from LevelSelect.cs
    void HandleTopMenu() {
        if (menuController.Up()) {
            soundfx.MenuNavigate();
            buttonIdx--;
            if ((LevelManager.IsHub()) && buttonIdx == ABILITIES_IDX && !gameData.hasUnlockedAnAbility()) {
                buttonIdx--;
            }else if (buttonIdx == RESTART_IDX && SessionData.numLives == 1 && SessionData.currentWorld != WorldEntrance.World.Hub) {
                buttonIdx--;
            }
        } else if (menuController.Down()) {
            soundfx.MenuNavigate();
            buttonIdx++;
            if ((LevelManager.IsHub()) && buttonIdx == ABILITIES_IDX && !gameData.hasUnlockedAnAbility()) {
                buttonIdx++;
            } else if (buttonIdx == RESTART_IDX && SessionData.numLives == 1 && SessionData.currentWorld != WorldEntrance.World.Hub) {
                buttonIdx++;
            }
        }

        if (buttonIdx < 0) buttonIdx = topButtons.Length - 1;
        else if (buttonIdx >= topButtons.Length) buttonIdx = 0;
        EventSystem.current.SetSelectedGameObject(topButtons[buttonIdx].gameObject);
    }

    void HandleAbilitiesMenu() {
        UpdateAbInstructions();

        GameObject curObj = EventSystem.current.currentSelectedGameObject;

        if (curObj == abBackButton.gameObject) {
            if (menuController.Up() || menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(changeAbButton.gameObject);
            }
        } else if (curObj == changeAbButton.gameObject) {
            if (menuController.Up() || menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(abBackButton.gameObject);
            } else if (menuController.Left()) {
                RotateAbLeft();
            } else if (menuController.Right()) {
                RotateAbRight();
            }
        }
    }

    void HandleSettingsMenu() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;

        /* Menu navigation */

        if (curObj == settingsBackButton.gameObject) {
            if (menuController.Up()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(changeColorButton.gameObject);
            }else if (menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(musicVolumeSlider.gameObject);
            }
        } else if (curObj == musicVolumeSlider.gameObject) {
            if (menuController.Up()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(settingsBackButton.gameObject);
            } else if (menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(soundFXSlider.gameObject);
            }
        } else if (curObj == soundFXSlider.gameObject) {
            if (menuController.Up()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(musicVolumeSlider.gameObject);
            } else if (menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(camSensSlider.gameObject);
            }
        } else if (curObj == camSensSlider.gameObject) {
            if (menuController.Up()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(soundFXSlider.gameObject);
            } else if (menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(changeColorButton.gameObject);
            }
       } else if (curObj == changeColorButton.gameObject) {
            if (menuController.Up()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(camSensSlider.gameObject);
            } else if (menuController.Down()) {
                soundfx.MenuNavigate();
                EventSystem.current.SetSelectedGameObject(settingsBackButton.gameObject);
            }
        }

        /* Actual settings */

        // Music
        if (curObj == musicVolumeSlider.gameObject) {
            if (menuController.Left()) {
                soundfx.MenuNavigate();
                //shiftslider
                musicVolumeSlider.value -= .1f;
                gameData.SetMusicVolume(musicVolumeSlider.value);
            } else if (menuController.Right()) {
                soundfx.MenuNavigate();
                musicVolumeSlider.value += .1f;
                gameData.SetMusicVolume(musicVolumeSlider.value);
            }
        }

        // Sound FX
        if (curObj == soundFXSlider.gameObject) {
            if (menuController.Left()) {
                soundfx.MenuNavigate();
                soundFXSlider.value -= .1f;
                gameData.SetSoundFX(soundFXSlider.value);
            } else if (menuController.Right()) {
                soundfx.MenuNavigate();
                soundFXSlider.value += .1f;
                gameData.SetSoundFX(soundFXSlider.value);
            }
        }

        // Camera Sensitivity
        if (curObj == camSensSlider.gameObject) {
            if (menuController.Left()) {
                soundfx.MenuNavigate();
                camSensSlider.value -= .1f;
                gameData.SetCameraSensitivity(camSensSlider.value);
            } else if (menuController.Right()) {
                soundfx.MenuNavigate();
                camSensSlider.value += .1f;
                gameData.SetCameraSensitivity(camSensSlider.value);
            }
        }

        // Color
        if (curObj == changeColorButton.gameObject) {
            if (menuController.Left()) {
                RotateColorLeft();
            } else if (menuController.Right()) {
                RotateColorRight();
            }
        }
    }

    void RotateAbLeft() {
        RotateAb(false);
    }

    void RotateAbRight() {
        RotateAb(true);
    }

    void RotateAb(bool toRight) {
        bool valid = false;
        soundfx.MenuNavigate();

        while (!valid) {
            //Right
            if (toRight) {
                abIdx++;
                if (abIdx >= abArray.Length) abIdx = 0;

            //Left
            } else {
                abIdx--;
                if (abIdx < 0) abIdx = abArray.Length - 1;
            }

            if (abArray[abIdx]) valid = true;
        }

        Text text = changeAbButton.GetComponentInChildren<Text>();
        SessionData.currentAbility = ((SessionData.Ability)abIdx);
        text.text = SessionData.currentAbility.ToString();
    }

    void UpdateAbInstructions() {
        SessionData.Ability ab = SessionData.currentAbility;
        string curText = instructionsText.text;
        string newText = "";

        switch (ab) {
            case SessionData.Ability.None:
                newText = "You don't need any fancy powers to get the job done! Plus, it's nice to have extra lives during Challenge mode.";
                break;
            case SessionData.Ability.Boost:
                newText = "ROLLING AROUND AT THE SPEED OF -- go fast.\n\nPress space (keyboard) or A (controller) to activate.";
                break;
            case SessionData.Ability.Bounce:
                newText = "Sometimes you just need a little a spring in your step -- er, I mean roll.\n\nPress space (keyboard) or A (controller) to activate.";
                break;
            case SessionData.Ability.Climb:
                newText = "Where's the ramp to get up this thing? Whatever, let's just ignore gravity.\n\nHold space (keyboard) or A (controller) to activate.";
                break;
            case SessionData.Ability.Rewind:
                newText = "Moving platforms running away from you? Bring 'em right back!\n\nHold space (keyboard) or A (controller) to activate.";
                break;
        }

        if (curText != newText) instructionsText.text = newText;
    }

    void RotateColorLeft() {
        RotateColor(false);
    }

    void RotateColorRight() {
        RotateColor(true);
    }

    void RotateColor(bool toRight) {
        bool valid = false;
        soundfx.MenuNavigate();

        while (!valid) {
            //Right
            if (toRight) {
                colorIdx++;
                if (colorIdx >= colorArray.Length) colorIdx = 0;

                //Left
            } else {
                colorIdx--;
                if (colorIdx < 0) colorIdx = colorArray.Length - 1;
            }

            if (colorArray[colorIdx]) valid = true;
        }

        gameData.SetBallColor((PlayerController.BallColor)colorIdx);
        SetColorButton();
    }

    void SetColorButton() {
        Color buttonColor = Color.black;
        PlayerController.BallColor color = (PlayerController.BallColor)colorIdx;

        switch (color) {
            case PlayerController.BallColor.Maroon:
                buttonColor = new Color(0.5f, 0.2f, 0.2f);
                break;
            case PlayerController.BallColor.Blue:
                buttonColor = Color.blue;
                break;
            case PlayerController.BallColor.Gray:
                buttonColor = new Color(0.25f, 0.25f, 0.25f);
                break;
            case PlayerController.BallColor.Green:
                buttonColor = Color.green;
                break;
            case PlayerController.BallColor.Purple:
                buttonColor = new Color(1f, 0.1f, 1f);
                break;
            case PlayerController.BallColor.Yellow:
                buttonColor = Color.yellow;
                break;
            case PlayerController.BallColor.Forest:
                buttonColor = Color.black;
                break;
            case PlayerController.BallColor.Desert:
                buttonColor = Color.black;
                break;
            case PlayerController.BallColor.Canyon:
                buttonColor = Color.black;
                break;
            case PlayerController.BallColor.Island:
                buttonColor = Color.black;
                break;
            case PlayerController.BallColor.Space:
                buttonColor = Color.black;
                break;
        }

        Text buttonText = changeColorButton.GetComponentInChildren<Text>();
        buttonText.text = color.ToString();
        buttonText.color = buttonColor;
    }

    public void ResumeButtonPressed()
    {
        TogglePause(false);
    }

    public void RestartButtonPressed()
    {
        levelManager.ReloadLevel();
    }

    public void ExitButtonPressed(){
        if (LevelManager.IsHub()) {
            levelManager.LoadTitle();
        } else {
            levelManager.LoadHub();
        }
    }

    public void BackPressed() {
        EventSystem.current.SetSelectedGameObject(null);
        if (currentMenu == PMenu.TopMenu) {
            TogglePause(false);
        } else if (currentMenu == PMenu.Abilities) {
            ShowPMenu(PMenu.TopMenu);
        }else if (currentMenu == PMenu.Settings) {
            ShowPMenu(PMenu.TopMenu);
        }
    }

    public void AbilitiesButtonPressed() {
        // This button is only ever selectable in the hub world
        if (LevelManager.IsHub()) {
            ShowPMenu(PMenu.Abilities);
        }
    }

    public void SettingsButtonPressed() {
        ShowPMenu(PMenu.Settings);
    }
}
