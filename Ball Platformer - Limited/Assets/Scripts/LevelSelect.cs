using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour {

    public Button backButton;
    public Button[] levelButtons;
    public Text titleText;
    public MenuController menuController;

    private LevelManager levelManager;
    private GameObject player;
    private int selButtonIdx;
    private bool menuShowing;
    private WorldEntrance.World currentWorld;
    private int currentStartID;

    // Use this for initialization
    void Start () {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        ShowLvlSelectUI(false);

        if (levelButtons == null || backButton == null) Debug.LogError("Level buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
    }
	

    void ShowLvlSelectUI(bool showUI)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = showUI ? 1f : 0f;
        cg.blocksRaycasts = showUI ? true : false;
    }

    public void SetWorldAndStartID(WorldEntrance.World world, int startID)
    {
        currentWorld = world;
        titleText.text = world.ToString();
        currentStartID = startID;
    }

    public void ToggleLevelSelect(bool showLevelSelect)
    {
        menuShowing = showLevelSelect;
        ShowLvlSelectUI(showLevelSelect);
        if (levelButtons != null)
        {
            selButtonIdx = 0;
            EventSystem.current.SetSelectedGameObject(levelButtons[selButtonIdx].gameObject);
        }

        player.GetComponent<PlayerController>().PauseRB(showLevelSelect);
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuShowing) return;

        if (menuController.Cancel()) {
            BackPressed();
        }
        else if (menuController.Up())
        {
            PressUp();
        }
        else if (menuController.Down())
        {
            PressDown();
        }
        else if (menuController.Left())
        {
            PressLeft();
        }
        else if (menuController.Right())
        {
            PressRight();
        }
        else if (menuController.Submit())
        {
            if (selButtonIdx >= 0 && selButtonIdx < levelButtons.Length) NumberButtonPressed();
            else BackPressed();
        }
    }

    void BackPressed()
    {
        ToggleLevelSelect(false);
    }

    void NumberButtonPressed()
    {
        Button clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string number = clickedButton.GetComponentInChildren<Text>().text;
        int buttonNumber = 0;
        int.TryParse(number, out buttonNumber);
        levelManager.LoadLevel(currentStartID + buttonNumber - 1);
    }

    void PressUp()
    {
        ChangeSelectedButton(-5);
    }

    void PressDown()
    {
        ChangeSelectedButton(5);
    }

    void PressLeft()
    {
        ChangeSelectedButton(-1);
    }

    void PressRight()
    {
        ChangeSelectedButton(1);
    }

    void ChangeSelectedButton(int numToChangeIdx)
    {
        if (levelButtons != null)
        {

            //If the back button is currently selected
            if (selButtonIdx < 0 || selButtonIdx >= levelButtons.Length)
            {
                if (numToChangeIdx > 0) selButtonIdx = 0;
                else selButtonIdx = levelButtons.Length - 1;
                EventSystem.current.SetSelectedGameObject(levelButtons[selButtonIdx].gameObject);
            }
            //If a number button is currently selected
            else
            {
                selButtonIdx += numToChangeIdx;
                if (selButtonIdx < 0 || selButtonIdx >= levelButtons.Length) EventSystem.current.SetSelectedGameObject(backButton.gameObject);
                else EventSystem.current.SetSelectedGameObject(levelButtons[selButtonIdx].gameObject);
            }
        }
    }
}
