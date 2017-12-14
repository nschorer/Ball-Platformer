using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour {

    public CanvasGroup topMenu;
    public CanvasGroup practiceMenu;
    public Button practiceBackButton;
    public Button[] levelButtons;
    public Image[] levelChecks;
    public Button[] topButtons;
    public Text titleText;
    public Text instructionsText;
    public MenuController menuController;
    public Text bestTimeText, challengeTimeText;

    public enum LSMenu { None, TopMenu, Practice };

    private LSMenu currentMenu;
    private LevelManager levelManager;
    private GameObject player;
    private WorldEntrance.World currentWorld;
    private int currentStartID;
    private int clearedLvls;
    private GameObject lastSelectedObj;
    private static MusicPlayer mPlayer;
    private SoundFx soundfx;
    private GameData gameData;
    private float[] bestTimes, challengeTimes;
    private int buttonIdx;
    private Button[] currentButtons;

    private const int EXPLORE_BUTTON_IDX = 0;
    private const int PRACTICE_BUTTON_IDX = 1;
    private const int BACK_BUTTON_IDX = 2;

    // Use this for initialization
    void Start() {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        soundfx = GameObject.FindGameObjectWithTag("Sound Fx").GetComponent<SoundFx>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameData = GameData.currentGameFile;
        ShowLvlSelectUI(false);

        if (titleText == null) Debug.LogError("Title text is not set up.");
        if (topButtons == null) Debug.LogError("Top menu buttons are not set up.");
        if (levelButtons == null || practiceBackButton == null) Debug.LogError("Practice menu buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
        if (gameData == null) Debug.LogError("Game Data is not set up.");

        if (mPlayer == null) mPlayer = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>();
        if (mPlayer == null) Debug.LogError("MusicPlayer is not set up.");
    }

    void ShowLvlSelectUI(bool showUI) {

        CanvasGroup cg = GetComponent<CanvasGroup>();
        MenuHandler.ShowCG(cg, showUI);

        if (showUI) {
            SetupPracticeMenu();
            HandleExploreButton();
            ShowLSMenu(LSMenu.TopMenu);

        } else currentMenu = LSMenu.None;
    }

    public void SetWorldAndStartID(WorldEntrance.World world, int startID) {
        currentWorld = world;
        titleText.text = world.ToString();
        currentStartID = startID;
        clearedLvls = gameData.getLvlsCleared(currentWorld);
    }

    public void ToggleLevelSelect(bool showLevelSelect) {
        ShowLvlSelectUI(showLevelSelect);
        if (topButtons != null && showLevelSelect) {
            EventSystem.current.SetSelectedGameObject(topButtons[0].gameObject);
        }

        if (showLevelSelect) {
            soundfx.MenuSubmit();
        }else {
            soundfx.MenuCancel();
        }

        player.GetComponent<PlayerController>().PauseRB(showLevelSelect);
    }

    public void ShowLSMenu(int menuID) {
        ShowLSMenu((LSMenu)menuID);
    }

    void ShowLSMenu(LSMenu menu) {

        EventSystem.current.SetSelectedGameObject(null);
        currentMenu = menu;

        MenuHandler.ShowCG(topMenu, menu == LSMenu.TopMenu);
        MenuHandler.ShowCG(practiceMenu, menu == LSMenu.Practice);

        if (menu == LSMenu.TopMenu) titleText.text = "Level Select";
        else if (menu == LSMenu.Practice) titleText.text = "Practice Level";

        if (menu == LSMenu.TopMenu) {
            currentButtons = topButtons;
        } else if (menu == LSMenu.Practice) {
            currentButtons = levelButtons;
        }

        buttonIdx = 0;
        if (currentButtons != null) EventSystem.current.SetSelectedGameObject(currentButtons[0].gameObject);
    }

    void HandleExploreButton() {
        Text expButtonText = topButtons[EXPLORE_BUTTON_IDX].GetComponentInChildren<Text>();

        if (gameData.hasClearedWorld(currentWorld)) {
            expButtonText.text = "*Challenge*";
            if (!gameData.hasClearedChallenge(currentWorld)) {
                expButtonText.color = Color.black;
            }else {
                expButtonText.color = new Color(.8f, .8f, 0f);
            }
        }else {
            expButtonText.text = "Explore";
            expButtonText.color = Color.black;
        }
    }

    void SetupPracticeMenu() {
        if (clearedLvls <= 0) {
            MenuHandler.DisableButton(topButtons[PRACTICE_BUTTON_IDX]);
        } else {
            MenuHandler.EnableButton(topButtons[PRACTICE_BUTTON_IDX]);

            bestTimes = gameData.getBestTimes(currentWorld);
            challengeTimes = StaticInfo.getChallengeTimes(currentWorld);

            for (int i=0; i<levelButtons.Length; i++) {
                levelButtons[i].interactable = (clearedLvls > i);
                SetCheckmark(i);
            }
        }
    }

    void SetCheckmark(int idx) {
        Image check = levelChecks[idx];

        //Show a checkmark if 1. we beat the level AND 2. we beat the challenge time
        if ((clearedLvls <= idx) || bestTimes[idx] > challengeTimes[idx]) {
            Color invisible = new Color(0f, 0f, 0f, 0f);
            check.color = invisible;
        } else {
            check.color = Color.white;
        }
    }

    void SetInstructionsText() {
        string curStr = instructionsText.text;
        string newStr = "";

        if (currentMenu == LSMenu.Practice) {
            newStr = "Select a level that you have already cleared and try to set a new best time!";
        }else if (currentMenu == LSMenu.TopMenu) {

            if (EventSystem.current.currentSelectedGameObject != null) {

                Button curButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

                if (curButton == topButtons[EXPLORE_BUTTON_IDX]) {
                    if (!gameData.hasClearedWorld(currentWorld)) {
                        if (clearedLvls <= 0) {
                            newStr = "Start exploring this world to see what if offers!\nYou may not use abilities in exploration mode.";
                        } else {
                            newStr = "Continue your exploration of this world!\nYou may not use abilities in exploration mode.";
                        }
                    } else {
                        newStr = "Take on all of the levels of this world in one go! \n\n";
                        newStr += "Choose to bring an ability: 3 lives.\n";
                        newStr += "Choose to not bring an ability: 5 lives.";
                    }
                } else if (curButton == topButtons[PRACTICE_BUTTON_IDX]) {
                    newStr = "Play a previously cleared level.";
                } else if (topButtons[BACK_BUTTON_IDX]) {
                    newStr = "Stay in the hub world.";
                }
            }
        }

        if (newStr != curStr) instructionsText.text = newStr;
    }

    // Update is called once per frame
    void Update() {

        if (currentMenu != LSMenu.None) {

            GameObject curObject = EventSystem.current.currentSelectedGameObject;
            bool menuObjectChanged = lastSelectedObj != curObject;

            if (EventSystem.current.currentSelectedGameObject == null) ShowLSMenu(currentMenu);
            SetHighlightedColor();

            if (menuObjectChanged) {
                lastSelectedObj = curObject;
                SetInstructionsText();
            }

            //Copied from PauseBP.cs
            if (menuController.Cancel()) {
                soundfx.MenuCancel();
                BackPressed();
            }
            if (menuController.Submit()) {
                Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                if (button != null) {
                    soundfx.MenuSubmit();
                    button.onClick.Invoke();
                }
            }

            if (currentMenu == LSMenu.TopMenu) HandleTopMenu();
            if (currentMenu == LSMenu.Practice) HandlePracticeMenu(menuObjectChanged);
        }
    }

    // Copied from PauseBP.cs
    // Get buttons/sliders to fade in and out to show that they are selected
    public void SetHighlightedColor() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        Button button = curObj.GetComponent<Button>();
        Slider slider = curObj.GetComponent<Slider>();

        float mod = Mathf.Sin(Time.time * 4) * MenuHandler.buttonFlash;
        Color color = MenuHandler.buttonColor;
        color.r += mod;
        color.g += mod;
        color.b += mod;

        if (button != null) {
            ColorBlock colors = button.colors;
            colors.highlightedColor = color;
            button.colors = colors;
        } else if (slider != null) {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = color;
            slider.colors = colors;
        }
    }

    //Copied from PauseBP.cs
    void HandleTopMenu() {

        if (menuController.Up()) {
            soundfx.MenuNavigate();
            buttonIdx--;
            if (buttonIdx == PRACTICE_BUTTON_IDX && clearedLvls <= 0) {
                buttonIdx--;
            }
        }else if (menuController.Down()) {
            soundfx.MenuNavigate();
            buttonIdx++;
            if (buttonIdx == PRACTICE_BUTTON_IDX && clearedLvls <= 0) {
                buttonIdx++;
            }
        }

        if (buttonIdx < 0) buttonIdx = topButtons.Length - 1;
        else if (buttonIdx >= topButtons.Length) buttonIdx = 0;
        EventSystem.current.SetSelectedGameObject(topButtons[buttonIdx].gameObject);
    }

    void HandlePracticeMenu(bool menuObjectChanged) {

        GameObject curObject = EventSystem.current.currentSelectedGameObject;

        if (menuObjectChanged) {

            int lvl = -1;
            for (int i = 0; i < levelButtons.Length; i++) {
                if (curObject == levelButtons[i].gameObject) lvl = i;
            }

            UpdateBestChallengeTimes(lvl);
        }

        HandlePracticeMenuNavigation();
    }

    void HandlePracticeMenuNavigation() {
        GameObject curObject = EventSystem.current.currentSelectedGameObject;

        bool onBackButton = (curObject == practiceBackButton.gameObject);

        if (menuController.Up()) {
            soundfx.MenuNavigate();
            if (onBackButton) {
                if (clearedLvls >= 8) buttonIdx = 7;
                else buttonIdx = clearedLvls - 1;
            } else {
                if (buttonIdx <= 4) buttonIdx = -1;
                else buttonIdx -= 5;
            }
        } else if (menuController.Down()) {
            soundfx.MenuNavigate();
            if (onBackButton) {
                int idx;
                if (clearedLvls >= 3) idx = 2;
                else idx = clearedLvls-1;
                buttonIdx = idx;
            } else {
                if (buttonIdx <= 4) {
                    if (buttonIdx + 5 <= clearedLvls - 1) buttonIdx = buttonIdx + 5;
                } else buttonIdx = -1;
            }
        } else if (menuController.Left()) {
            if (onBackButton) {
                soundfx.MenuNavigate();
                buttonIdx = 0;
            } else {
                if (buttonIdx != 0 && buttonIdx != 5) {
                    soundfx.MenuNavigate();
                    buttonIdx--;
                    if (!levelButtons[buttonIdx].interactable) buttonIdx++;
                }
            }
        } else if (menuController.Right()) {
            if (onBackButton) {
                soundfx.MenuNavigate();
                if (clearedLvls >= 5) buttonIdx = 4;
            } else {
                if (buttonIdx != 4 && buttonIdx != 9) {
                    soundfx.MenuNavigate();
                    buttonIdx++;
                    if (!levelButtons[buttonIdx].interactable) buttonIdx--;
                }
            }
        }

        if (buttonIdx < 0 || buttonIdx >= levelButtons.Length) EventSystem.current.SetSelectedGameObject(practiceBackButton.gameObject);
        else EventSystem.current.SetSelectedGameObject(levelButtons[buttonIdx].gameObject);
    }

    void UpdateBestChallengeTimes(int lvl) {
        if (lvl < 0 || lvl > 9) {
            bestTimeText.text = "---";
            challengeTimeText.text = "---";
        } else {
            bestTimeText.text = bestTimes[lvl].ToString("n2");
            challengeTimeText.text = challengeTimes[lvl].ToString("n2");
        }
    }

    public void exploreButtonPressed() {
        //Explore
        if (!gameData.hasClearedWorld(currentWorld)) {
            SessionData.currentAbility = SessionData.Ability.None; //Exploration must be done without abilities.
            SessionData.EnterWorld(currentWorld, SessionData.GameMode.Exploration, clearedLvls + 1);
            levelManager.LoadLevel(currentStartID + clearedLvls);

        //Challenge
        }else {
            SessionData.EnterWorld(currentWorld, SessionData.GameMode.Challenge);
            levelManager.LoadLevel(currentStartID);
        }

        mPlayer.SwitchSong(currentWorld);
    }

    public void BackPressed() {
        EventSystem.current.SetSelectedGameObject(null);
        if (currentMenu == LSMenu.TopMenu) {
            ToggleLevelSelect(false);
        }else if (currentMenu == LSMenu.Practice) {
            ShowLSMenu(LSMenu.TopMenu);
        }
    }

    public void NumberButtonPressed(int buttonNumber) {
        SessionData.PracticeLevel(currentWorld, buttonNumber);
        levelManager.LoadLevel(currentStartID + buttonNumber - 1);
    }
}
