using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    const int TITLE = 0;
    const int HUB = 1;
	const int FOREST_START = 2;
    const int DESERT_START = 2;
    const int CANYON_START = 22;
    const int ISLAND_START = 32;
    const int SPACE_START = 42;

	public void ReloadLevel (){
        if (SessionData.currentMode == SessionData.GameMode.Challenge) {
            SessionData.numLives--;
            if (SessionData.numLives <= 0) {
                LoadHub();
            }else {
                LoadLevel(SceneManager.GetActiveScene().buildIndex);
            }

        } else {
            LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
	}

    public void LoadTitle() {
        SessionData.currentAbility = SessionData.Ability.None;
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>().SwitchSong(null, true);
        Destroy(GameData.currentGameFile.gameObject);
        LoadLevel(TITLE);
    }

    public void LoadHub(){
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>().SwitchSong(WorldEntrance.World.Hub, true);
		LoadLevel(HUB);
	}

    public static bool IsTitle() {
        return SceneManager.GetActiveScene().buildIndex == TITLE;
    }

    public static bool IsHub() {
        return SceneManager.GetActiveScene().buildIndex == HUB;
    }

	public void LoadNextLevel (int incLevels = 1){
		LoadLevel(SceneManager.GetActiveScene().buildIndex + incLevels);
	}

	public void LoadLevel(int levelIndex){
		SceneManager.LoadScene(levelIndex);
	}

	public void ExitGame (){
		Application.Quit();
	}

	public string GetLoadingText (Goal.WhatToLoadNext whatNext){
		string str = "";

        if (SessionData.currentMode == SessionData.GameMode.Practice) str = "Restarting level in ";
        else if (whatNext == Goal.WhatToLoadNext.NextLevel) str = "Nice job!\nStarting next level in ";
        else if (whatNext == Goal.WhatToLoadNext.Start) str = "Returning to Hub World in ";

		return str;
	}

    public int GetWorldStartID(WorldEntrance.World whichWorld)
    {
        int lvlID;

        switch (whichWorld)
        {
            case WorldEntrance.World.Forest:
                lvlID = FOREST_START;
                break;
            case WorldEntrance.World.Desert:
                lvlID = DESERT_START;
                break;
            case WorldEntrance.World.Canyon:
                lvlID = CANYON_START;
                break;
            case WorldEntrance.World.Island:
                lvlID = ISLAND_START;
                break;
            case WorldEntrance.World.Space:
                lvlID = SPACE_START;
                break;
            default:
                lvlID = 0;
                break;
        }
        return lvlID;
    }
}
