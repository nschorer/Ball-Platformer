using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    const int GAME_START = 0;
	const int FOREST_START = 2;
    const int DESERT_START = 12;
    const int CANYON_START = 22;
    const int ISLAND_START = 32;
    const int SPACE_START = 42;

	public void ReloadLevel (){
		LoadLevel(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadStart(){
		LoadLevel(GAME_START);
	}

	public void LoadNextLevel (){
		LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadLevel(int levelIndex){
		SceneManager.LoadScene(levelIndex);
	}

	public void ExitGame (){
		Application.Quit();
	}

	public string GetLoadingText (Goal.WhatToLoadNext whatNext){
		string str = "";

        if (whatNext == Goal.WhatToLoadNext.NextLevel) str = "Nice job!\nStarting next level in ";
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
