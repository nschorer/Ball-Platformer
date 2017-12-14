using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilePreview : MonoBehaviour {

    public Image[] icons, balls, clocks;

    private CanvasGroup cg;
    private bool isShowing;
    private GameData currentFile;

    private static int FOREST_IDX = 0;
    private static int DESERT_IDX = 1;
    private static int CANYON_IDX = 2;
    private static int ISLAND_IDX = 3;
    private static int SPACE_IDX = 4;

	// Use this for initialization
	void Start () {
        cg = GetComponent<CanvasGroup>();
        isShowing = true;
        ShowFilePreview(false);
	}
	
	public void ShowFilePreview(bool show) {
        if (show != isShowing) {
            MenuHandler.ShowCG(cg, show);
            isShowing = show;
        }
    }

    public void ShowFileData(GameData data, bool forceShow = false) {

        if ((data != currentFile) || forceShow) {

            icons[FOREST_IDX].color = data.hasClearedForest() ? Color.white : Color.clear;
            icons[DESERT_IDX].color = data.hasClearedDesert() ? Color.white : Color.clear;
            icons[CANYON_IDX].color = data.hasClearedCanyon() ? Color.white : Color.clear;
            icons[ISLAND_IDX].color = data.hasClearedIsland() ? Color.white : Color.clear;
            icons[SPACE_IDX].color = data.hasClearedSpace() ? Color.white : Color.clear;

            balls[FOREST_IDX].color = data.ForestChallengeCleared ? Color.white : Color.clear;
            balls[DESERT_IDX].color = data.DesertChallengeCleared ? Color.white : Color.clear;
            balls[CANYON_IDX].color = data.CanyonChallengeCleared ? Color.white : Color.clear;
            balls[ISLAND_IDX].color = data.IslandChallengeCleared ? Color.white : Color.clear;
            balls[SPACE_IDX].color = data.SpaceChallengeCleared ? Color.white : Color.clear;

            clocks[FOREST_IDX].color = data.hasBeatChallengeTimes(WorldEntrance.World.Forest) ? Color.white : Color.clear;
            clocks[DESERT_IDX].color = data.hasBeatChallengeTimes(WorldEntrance.World.Desert) ? Color.white : Color.clear;
            clocks[CANYON_IDX].color = data.hasBeatChallengeTimes(WorldEntrance.World.Canyon) ? Color.white : Color.clear;
            clocks[ISLAND_IDX].color = data.hasBeatChallengeTimes(WorldEntrance.World.Island) ? Color.white : Color.clear;
            clocks[SPACE_IDX].color = data.hasBeatChallengeTimes(WorldEntrance.World.Space) ? Color.white : Color.clear;

            currentFile = data;
        }
    }
}
