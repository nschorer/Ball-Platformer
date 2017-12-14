using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour {

    public Button[] topButtons;
    public Button[] fileButtons;
    public Button[] deleteButtons;
    public GameData[] saveFiles;
    public LevelManager levelManager;
    public MenuController menuController;
    public CanvasGroup topMenu, fileMenu, deleteLayer;
    public FilePreview filePreview;

    public enum TMenu { None, TopMenu, FileMenu };
    public enum TLayer { None, Delete};

    private TMenu currentMenu;
    private TLayer currentLayer;
    private bool showingTitle;
    private bool cgChangedThisFrame;
    private int buttonIdx;
    private int fileIdx;
    private GameData currentFile; //which one is highlighted
    private SoundFx soundfx;

    private const int FILE_IDX = 0;
    private const int EXIT_IDX = 1;

    private const int START_IDX = 0;
    private const int DELETE_IDX = 1;

    private const int DELETE_NO_IDX = 0;
    private const int DELETE_YES_IDX = 1;

    // Use this for initialization
    void Start() {
        ToggleTitle(true);
        ShowTLayer(TLayer.None);
        buttonIdx = 0;
        currentFile = saveFiles[buttonIdx];
        soundfx = GameObject.FindGameObjectWithTag("Sound Fx").GetComponent<SoundFx>();
        
        if (levelManager == null) Debug.LogError("Level manager is not set up");
        if (topButtons == null) Debug.LogError("Pause buttons are not set up.");
        if (menuController == null) Debug.LogError("Menu Controller is not set up.");
    }

    void ShowTitleUI(bool showTitleMenu) {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        MenuHandler.ShowCG(cg, showTitleMenu);

        if (showTitleMenu) {
            ShowTMenu(TMenu.TopMenu);

        } else {
            currentMenu = TMenu.None;
            currentLayer = TLayer.None;
        }
    }

    public void ToggleTitle(bool showTitleMenu) {
        showingTitle = showTitleMenu;
        if (showTitleMenu) cgChangedThisFrame = true;

        ShowTitleUI(showTitleMenu);
        if (topButtons != null) {
            EventSystem.current.SetSelectedGameObject(topButtons[FILE_IDX].gameObject);
        }

       
    }

    void ShowTMenu(TMenu menu) {

        EventSystem.current.SetSelectedGameObject(null); //If we don't do this, we could retain focus on the button we just clicked
        currentMenu = menu;

        MenuHandler.ShowCG(topMenu, menu == TMenu.TopMenu);
        MenuHandler.ShowCG(fileMenu, menu == TMenu.FileMenu);

        buttonIdx = 0;

        GameObject startFocus = topButtons[buttonIdx].gameObject;
        bool noFocus = false;

        if (menu == TMenu.TopMenu) {
            startFocus = topButtons[FILE_IDX].gameObject;

        } else if (menu == TMenu.FileMenu) {
            startFocus = fileButtons[START_IDX].gameObject;

        } else noFocus = true;

        if (!noFocus) EventSystem.current.SetSelectedGameObject(startFocus);
    }

    // Note, with this implementation, we can only stack on ONE layer.
    void ShowTLayer(TLayer layer) {
        if (currentMenu == TMenu.None) return;

        EventSystem.current.SetSelectedGameObject(null); //If we don't do this, we could retain focus on the button we just clicked
        currentLayer = layer;

        MenuHandler.ShowCG(deleteLayer, layer == TLayer.Delete);

        buttonIdx = 0;

        GameObject startFocus = topButtons[buttonIdx].gameObject;
        bool noFocus = false;

        if (layer == TLayer.Delete) {
            startFocus = deleteButtons[DELETE_NO_IDX].gameObject;

        } else noFocus = true;

        if (!noFocus) EventSystem.current.SetSelectedGameObject(startFocus);
    }

    // Update is called once per frame
    void Update() {

        //Only make changes in pause menu if actually showing title!
        if (!showingTitle) return;

        if (!cgChangedThisFrame) {

            if (currentLayer != TLayer.None) {
                //If we lose focus on a button/slider, reset the current layer
                if (EventSystem.current.currentSelectedGameObject == null) ShowTLayer(currentLayer);

                if (currentLayer == TLayer.Delete) HandleDeleteLayer();
            } else {
                //If we lose focus on a button/slider, reset the current menu
                if (EventSystem.current.currentSelectedGameObject == null) ShowTMenu(currentMenu);

                if (currentMenu == TMenu.TopMenu) HandleTopMenu();
                else if (currentMenu == TMenu.FileMenu) HandleFileMenu();
            }
            
            
            SetHighlightedColor();

            //Copied from LevelSelect.cs
            if (menuController.Cancel()) {
                soundfx.MenuCancel();
                BackPressed();
            } else if (menuController.Submit()) {
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
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (button != null) {
            float mod = Mathf.Sin(Time.time * 4) * MenuHandler.buttonFlash;
            Color color = MenuHandler.buttonColor;
            color.r += mod;
            color.g += mod;
            color.b += mod;

            ColorBlock colors = button.colors;
            colors.highlightedColor = color;
            button.colors = colors;
        }
    }

    //Copied from LevelSelect.cs
    void HandleTopMenu() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        bool showFile = curObj == topButtons[FILE_IDX].gameObject;

        filePreview.ShowFilePreview(showFile);
        if (showFile) filePreview.ShowFileData(currentFile);

        if (menuController.Up()) {
            soundfx.MenuNavigate();
            buttonIdx--;

        } else if (menuController.Down()) {
            soundfx.MenuNavigate();
            buttonIdx++;

        }else if (curObj == topButtons[FILE_IDX].gameObject) {
            if (menuController.Left()) {
                RotateFileLeft();
            } else if (menuController.Right()) {
                RotateFileRight();
            }
        }

        if (buttonIdx < 0) buttonIdx = topButtons.Length - 1;
        else if (buttonIdx >= topButtons.Length) buttonIdx = 0;
        EventSystem.current.SetSelectedGameObject(topButtons[buttonIdx].gameObject);
    }

    void HandleFileMenu() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        bool isDelEnabled = fileButtons[DELETE_IDX].GetComponent<Button>().enabled;

        if (!currentFile.SaveExists() && isDelEnabled) {
            MenuHandler.DisableButton(fileButtons[DELETE_IDX].GetComponent<Button>());
            isDelEnabled = false;
        }

        if (menuController.Up()) {
            soundfx.MenuNavigate();
            buttonIdx--;
            if (!isDelEnabled && buttonIdx == DELETE_IDX) {
                buttonIdx--;
            }

        } else if (menuController.Down()) {
            soundfx.MenuNavigate();
            buttonIdx++;
            if (!isDelEnabled && buttonIdx == DELETE_IDX) {
                buttonIdx++;
            }
        }

        if (buttonIdx < 0) buttonIdx = fileButtons.Length - 1;
        else if (buttonIdx >= fileButtons.Length) buttonIdx = 0;
        EventSystem.current.SetSelectedGameObject(fileButtons[buttonIdx].gameObject);
    }

    void RotateFileLeft() {
        RotateFile(false);
    }

    void RotateFileRight() {
        RotateFile(true);
    }

    void RotateFile(bool toRight) {
        fileIdx += toRight ? 1 : -1;
        if (fileIdx >= saveFiles.Length) fileIdx = 0;
        else if (fileIdx < 0) fileIdx = saveFiles.Length - 1;

        soundfx.MenuNavigate();
        currentFile = saveFiles[fileIdx];
        topButtons[FILE_IDX].GetComponentInChildren<Text>().text = "File " + (fileIdx + 1).ToString();
    }

    void HandleDeleteLayer() {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;

        if (menuController.Left()) {
            buttonIdx--;

        } else if (menuController.Right()) {
            buttonIdx++;
        }

        if (buttonIdx < 0) buttonIdx = deleteButtons.Length - 1;
        else if (buttonIdx >= deleteButtons.Length) buttonIdx = 0;
        EventSystem.current.SetSelectedGameObject(deleteButtons[buttonIdx].gameObject);
    }

    public void FileButtonPressed() {
        ShowTMenu(TMenu.FileMenu);
    }

    public void ExitButtonPressed() {
        levelManager.ExitGame();
    }

    public void StartButtonPressed() {
        foreach (GameData saveFile in saveFiles) {
            if (saveFile != currentFile) Destroy(saveFile.gameObject);
            else saveFile.SetCurrentFile();
        }

        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>().SwitchSong(WorldEntrance.World.Hub);
        levelManager.LoadHub();
    }

    public void DeleteButtonPressed() {
        ShowTLayer(TLayer.Delete);
    }

    public void DeleteYes() {
        currentFile.Delete();
        BackPressed();
        filePreview.ShowFileData(currentFile, true);
    }

    public void DeleteNo() {
        BackPressed();
    }

    public void BackPressed() {
        EventSystem.current.SetSelectedGameObject(null);

        if (currentLayer != TLayer.None) {
            ShowTLayer(TLayer.None);
            ShowTMenu(currentMenu);
        } else {
            if (currentMenu == TMenu.TopMenu) {
                //ToggleTitle(false);
            } else if (currentMenu == TMenu.FileMenu) {
                MenuHandler.EnableButton(fileButtons[DELETE_IDX].GetComponent<Button>());
                ShowTMenu(TMenu.TopMenu);
            }
        }
    }

}
