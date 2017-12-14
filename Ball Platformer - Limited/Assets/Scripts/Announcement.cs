using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Announcement : MonoBehaviour {

    public PlayerController player;
    public CanvasGroup cg;
    public Button okButton;
    public MenuController menuController;
    public Text titleText;
    public Text bodyText;

    GameData gameData;
    List<string> msgQueue;
    float delay;
    bool showingMsg;
    bool buttonEnabled;
    bool okPressed;
    int numFrames;
    bool checkedAnnouncements;

    readonly WorldEntrance.World[] worldList = new WorldEntrance.World[] { WorldEntrance.World.Forest, WorldEntrance.World.Desert, WorldEntrance.World.Canyon, WorldEntrance.World.Island, WorldEntrance.World.Space };

	// Use this for initialization
	void Start () {
        msgQueue = new List<string>();
        delay = 0.25f;
        MenuHandler.ShowCG(cg, false);
        MenuHandler.DisableButton(okButton);
        numFrames = 0;
	}
	
	// Update is called once per frame
	void Update () {
        numFrames++;

        if (numFrames > 5 && !checkedAnnouncements) {
            gameData = GameData.currentGameFile;
             CheckForAnnouncements();
        }

        if (checkedAnnouncements) ShowAnnouncements();

	}

    void ShowAnnouncements() {
        // Keep iterating through our message queue until we are out of messages.
        if (msgQueue.Count > 0) {

            //Currently, nothing is showing
            if (!showingMsg) {
                //Show the message with the button disabled.
                if (delay <= 0) {
                    showingMsg = true;
                    MenuHandler.ShowCG(cg, true);
                    delay = 1f;
                    player.GetComponent<PlayerController>().PauseRB(true);

                    string[] msg = msgQueue[0].Split('\n');

                    titleText.text = msg[0];
                    bodyText.text = msg[1];
                }

                //Currently, the message is showing, but the OK button is disabled
            } else if (!buttonEnabled) {
                if (delay <= 0) {
                    buttonEnabled = true;
                    MenuHandler.EnableButton(okButton);
                    EventSystem.current.SetSelectedGameObject(okButton.gameObject);
                }

                //Currently, the message is showing, and the OK button is enabled
            } else {
                
                if (menuController.Submit()) {
                    OkButtonPressed();
                }

                if (okPressed) {
                    okPressed = false;
                    showingMsg = false;
                    buttonEnabled = false;
                    MenuHandler.DisableButton(okButton);
                    MenuHandler.ShowCG(cg, false);
                    delay = 0.25f;
                    player.GetComponent<PlayerController>().PauseRB(false);
                    EventSystem.current.SetSelectedGameObject(null);
                    // Remove the item AFTER we've pressed OK.
                    msgQueue.RemoveAt(0);
                }
            }


        // If we're out of messages, save the game.
        }else {
            gameData.Save();
        }

        if (delay > 0f) delay -= Time.deltaTime;
    }

    void CheckForAnnouncements() {
        foreach (WorldEntrance.World world in worldList) {
            if (!gameData.hasShownClearMsg(world) && gameData.hasClearedWorld(world)) {
                ShowClearWorldMsg(world);
            }

            if (!gameData.hasShownChallengeMsg(world) && gameData.hasClearedChallenge(world)) {
                ShowClearChallengeMsg(world);
            }

            if (!gameData.hasUnlockedAbililtyMsg() && gameData.hasUnlockedAnAbility()) {
                ShowHasUnlockedAbilityMsg();
            }

            if (!gameData.hasShownSpaceMsg() && gameData.hasUnlockedSpace()) {
                ShowHasUnlockedSpaceMsg();
            }

            if (!gameData.hasShownTimesMsg(world) && gameData.hasBeatChallengeTimes(world)) {
                ShowClearTimesMsg(world);
            }

        }

        checkedAnnouncements = true;
    }

    void ShowClearWorldMsg(WorldEntrance.World world) {
        if (world == WorldEntrance.World.Space) {
            msgQueue.Add(world.ToString() + " cleared!\nYou've explored space! Can you clear the ultimate challenge.");
        } else {
            msgQueue.Add(world.ToString() + " cleared!\nYou cleared the " + world.ToString() + " world and unlocked a new ability. Now see if you can go back and clear the " + world.ToString() + " challenge!");
        }
        gameData.SetShownClearWorldMsg(world);
    }

    void ShowClearChallengeMsg(WorldEntrance.World world) {
        if (world == WorldEntrance.World.Space) {
            msgQueue.Add("Awesome job!\nThanks for playing my game! If you haven't already, try seeing if you can beat all of the challenge times. Otherwise, feel free to replay any of the previous challenges.");
        } else {
            msgQueue.Add("Congratulations!\nYou beat the " + world.ToString() + " challenge. That must have been tough! If you are still looking for a challenge, try beating all of the challenge times for this world.");
        }

        gameData.SetShownChallengeMsg(world);
    }

    void ShowHasUnlockedSpaceMsg() {
        msgQueue.Add("Space Unlocked!\nYou've beaten all four world challenges -- but you're not done yet! You've unlocked Space, the final frontier. Time to put your skills to the test.");
        gameData.SetShownSpaceMsg();
    }

    void ShowClearTimesMsg(WorldEntrance.World world) {
        msgQueue.Add(world.ToString() + " challenge times cleared!\nYou've cleared all of the challenge times for this world, and unlocked a special skin. Go to the settings menu to change your ball's color to the new skin!");
        gameData.SetShownTimesMsg(world);
    }

    void ShowHasUnlockedAbilityMsg() {
        msgQueue.Add("Ability Unlocked!\nCheck the pause menu from the Hub world to set your new ability. Once you enter a world with an ability, you can't change it until you leave, so choose wisely.");
        gameData.SetShownAbilityMsg();
    }

    void OkButtonPressed() {
        okPressed = true;
    }
}
