using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLater : MonoBehaviour {

    public GameObject forestSphere, desertSphere, canyonSphere, islandSphere, spaceSphere;
    public GameObject elevator, spacePodium;

    bool isFirstFrame;
    GameData gameData;

	// Use this for initialization
	void Start () {
        gameData = GameData.currentGameFile;
        isFirstFrame = true;
    }

    private void Update() {

        //The game data is not loaded on start, so we have to do this check in the first frame of Update
        if (isFirstFrame) {
            HandleSphere(forestSphere);
            HandleSphere(desertSphere);
            HandleSphere(canyonSphere);
            HandleSphere(islandSphere);
            HandleSphere(spaceSphere);
            HandleElevator();

            isFirstFrame = false;
        }
    }
	
	void HandleSphere(GameObject sphere) {

        if (sphere == null) { return; }

        if (sphere == forestSphere) {
            sphere.SetActive(gameData.ForestChallengeCleared);

        }else if (sphere == desertSphere) {
            sphere.SetActive(gameData.DesertChallengeCleared);

        } else if (sphere == canyonSphere) {
            sphere.SetActive(gameData.CanyonChallengeCleared);

        } else if (sphere == islandSphere) {
            sphere.SetActive(gameData.IslandChallengeCleared);

        } else if (sphere == spaceSphere) {
            sphere.SetActive(gameData.SpaceChallengeCleared);
        }

    }

    void HandleElevator() {
        if (elevator == null) return;

        elevator.SetActive(gameData.hasUnlockedSpace());
    }

}
