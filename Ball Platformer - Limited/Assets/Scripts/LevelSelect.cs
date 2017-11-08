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
    public Button[] topButtons;
    public Text titleText;
    public Text instructionsText;
    public MenuController menuController;
    public GameData gameData;

    public enum LSMenu { None, TopMenu, Practice };

    private LSMenu currentMenu;
    private LevelManager levelManager;
    private GameObject player;
    private int selButtonIdx;
    private WorldEntrance.World currentWorld;
    private int currentStartID;
    private bool justChangedLSMenu;
    private int clearedLvls;
    private GameObject lastSelectedObj;

    private const int EXPLORE_BUTTON_IDX = 0;
    private const int PRACTICE_BUTTON_IDX = 1;
    private const int BACK_BUTTON_IDX = 2;

    // Use this for initialization
    void Start() {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        ShowLvlSelectUI(false);

        if (titleText == null) Debug.LogError("Title text is not set up.");
        if (topButtons == null) Debug.LogError("Top menu buttons are not set up.");
        if (levelButtons == null || practiceBackButton == null) Debug.LogError("Practice menu buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
        if (gameData == null) Debug.LogError("Game Data is not set up.");
    }

    void ShowLvlSelectUI(bool showUI) {

        CanvasGroup cg = GetComponent<CanvasGroup>();
        ShowCG(cg, showUI);

        if (showUI) {
            HandlePracticeButtons();
            HandleExploreButton();
            ShowLSMenu(LSMenu.TopMenu);

        } else currentMenu = LSMenu.None;
    }

    void ShowCG(CanvasGroup cg, bool showCG) {
        cg.alpha = showCG ? 1f : 0f;
        cg.blocksRaycasts = showCG;
        cg.interactable = showCG;
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
            //selButtonIdx = 0;
            EventSystem.current.SetSelectedGameObject(topButtons[0].gameObject);
        }

        player.GetComponent<PlayerController>().PauseRB(showLevelSelect);
    }

    public void ShowLSMenu(int menuID) {
        ShowLSMenu((LSMenu)menuID);
    }

    void ShowLSMenu(LSMenu menu) {

        EventSystem.current.SetSelectedGameObject(null);
        currentMenu = menu;
        //selButtonIdx = 0;
        //EventSystem.current.SetSelectedGameObject(topButtons[selButtonIdx].gameObject);

        ShowCG(topMenu, menu == LSMenu.TopMenu);
        ShowCG(practiceMenu, menu == LSMenu.Practice);

        if (menu == LSMenu.TopMenu) titleText.text = "Level Select";
        else if (menu == LSMenu.Practice) titleText.text = "Practice Level";

        Button[] buttons = new Button[10];
        bool noButtons = false;
        if (menu == LSMenu.TopMenu) buttons = topButtons;
        else if (menu == LSMenu.Practice) buttons = levelButtons;
        else noButtons = true;
        if (!noButtons) EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);

        justChangedLSMenu = true;
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
        }
    }

    void HandlePracticeButtons() {
        if (clearedLvls <= 0) {
            ButtonHandler.DisableButton(topButtons[PRACTICE_BUTTON_IDX]);
        } else {
            topButtons[PRACTICE_BUTTON_IDX].enabled = true;

            for (int i=0; i<levelButtons.Length; i++) {
                levelButtons[i].interactable = (clearedLvls > i);
            }
        }
    }

    void SetInstructionsText() {
        string curStr = instructionsText.text;
        string newStr = "";

        if (currentMenu == LSMenu.Practice) {
            newStr = "Select a level that you have already cleared and try to set a new best time!";
        }else if (currentMenu == LSMenu.TopMenu) {
            Button curButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            if (curButton == topButtons[EXPLORE_BUTTON_IDX]) {
                if (!gameData.hasClearedWorld(currentWorld)) {
                    if (clearedLvls <= 0) {
                        newStr = "Start exploring this world to see what if offers!";
                    }else {
                        newStr = "Continue your exploration of this world!";
                    }
                }else {
                    newStr =  "Take on all of the levels of this world in one go! \n\n";
                    newStr += "Choose to bring an ability: 3 lives.\n";
                    newStr += "Choose to not bring an ability: 5 lives.";
                }
            }else if (curButton == topButtons[PRACTICE_BUTTON_IDX]) {
                newStr = "Play a previously cleared level.";
            }else if (topButtons[BACK_BUTTON_IDX]) {
                newStr = "Stay in the hub world.";
            }
        }

        if (newStr != curStr) instructionsText.text = newStr;
    }

    // Update is called once per frame
    void Update() {
        GameObject curObject = EventSystem.current.currentSelectedGameObject;
        if (lastSelectedObj != curObject) {
            lastSelectedObj = curObject;
            SetInstructionsText();
        }

        //if (!justChangedLSMenu) {
        //    if (currentMenu == LSMenu.None) {
        //        return;

        //    } else {
        //        if (currentMenu == LSMenu.Practice) {
        //            if (menuController.Up()) {
        //                PressUp();
        //            } else if (menuController.Down()) {
        //                PressDown();
        //            } else if (menuController.Left()) {
        //                PressLeft();
        //            } else if (menuController.Right()) {
        //                PressRight();
        //            }

        //        } else if (currentMenu == LSMenu.TopMenu) {
        //            if (menuController.Up()) {
        //                PressUp();
        //            } else if (menuController.Down()) {
        //                PressDown();
        //            }
        //        }

        //        if (menuController.Cancel()) {
        //            BackPressed();
        //        } else if (menuController.Submit()) {
        //            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        //        }
        //    }
        //    print(selButtonIdx);

        //    //Make sure we don't double-press A
        //} else {
        //    justChangedLSMenu = true;
        //    //print("Yes");
        //}
    }

    public void exploreButtonPressed() {
        //Explore
        if (!gameData.hasClearedWorld(currentWorld)) {
            SessionData.EnterWorld(currentWorld, SessionData.GameMode.Exploration, clearedLvls + 1);
            levelManager.LoadLevel(currentStartID + clearedLvls);

        //Challenge
        }else {
            SessionData.EnterWorld(currentWorld, SessionData.GameMode.Challenge);
            levelManager.LoadLevel(currentStartID);
        }
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

    //void PressUp() {
    //    if (currentMenu == LSMenu.Practice) {
    //        ChangeSelectedButton(-5);
    //    }else if (currentMenu == LSMenu.TopMenu) {
    //        ChangeSelectedButton(-1);
    //    }
    //}

    //void PressDown() {
    //    if (currentMenu == LSMenu.Practice) {
    //        ChangeSelectedButton(5);
    //    } else if (currentMenu == LSMenu.TopMenu) {
    //        ChangeSelectedButton(1);
    //    }
    //}

    //void PressLeft() {
    //    ChangeSelectedButton(-1);
    //}

    //void PressRight() {
    //    ChangeSelectedButton(1);
    //}

    //void ChangeSelectedButton(int numToChangeIdx) {
    //    Button[] eButtons;

    //    if (currentMenu == LSMenu.Practice) {
    //        if (levelButtons != null) {

    //            eButtons = GetEnabledButtonAry(levelButtons);

    //            //If the back button is currently selected
    //            if (selButtonIdx < 0 || selButtonIdx >= eButtons.Length) {
    //                if (numToChangeIdx > 0) selButtonIdx = 0;
    //                else selButtonIdx = eButtons.Length - 1;
    //                EventSystem.current.SetSelectedGameObject(eButtons[selButtonIdx].gameObject);
    //            }
    //            //If a number button is currently selected
    //            else {
    //                selButtonIdx += numToChangeIdx;
    //                if (selButtonIdx < 0 || selButtonIdx >= eButtons.Length) EventSystem.current.SetSelectedGameObject(practiceBackButton.gameObject);
    //                else EventSystem.current.SetSelectedGameObject(eButtons[selButtonIdx].gameObject);
    //            }
    //        }
    //    }else if (currentMenu == LSMenu.TopMenu) {
    //        if (topButtons != null) {

    //            eButtons = GetEnabledButtonAry(topButtons);
    //            selButtonIdx += numToChangeIdx;

    //            int aryLength = eButtons.Length;
    //            if (selButtonIdx < 0) selButtonIdx = aryLength - 1;
    //            else if (selButtonIdx >= aryLength) selButtonIdx = 0;

    //            EventSystem.current.SetSelectedGameObject(eButtons[selButtonIdx].gameObject);
    //        }
    //    }
    //}

    //Button[] GetEnabledButtonAry(Button[] buttons) {
    //    Button[] eButtons;
    //    int idx = 0;
    //    int numInteractable = 0;

    //    foreach (Button button in buttons) {
    //        if (button.IsInteractable()) numInteractable++;
    //    }

    //    eButtons = new Button[numInteractable];
    //    if (numInteractable > 0) {
    //        foreach (Button button in buttons) {
    //            if (button.IsInteractable()) {
    //                eButtons[idx] = button;
    //                idx++;
    //            }
    //        }
    //    }

    //    return eButtons;
    //}
}
