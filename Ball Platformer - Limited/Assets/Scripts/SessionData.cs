using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionData {

    public static SessionData sessionData;

    public enum GameMode { Exploration, Challenge, Practice};
    public static GameMode currentMode;

    public enum Ability { None, Climb, Boost, Bounce, Rewind};
    public static Ability currentAbility;

    public static WorldEntrance.World currentWorld;
    public static int currentLevel;

    public static int numLives;

    public static bool hasShownTitle;

    // Should be used when entering a world from the beginning (exploration/challenge mode)
    public static void EnterWorld(WorldEntrance.World world, GameMode mode, int level = 1){
        currentWorld = world;
        currentMode = mode;

        if (mode == GameMode.Challenge) {
            currentLevel = 1;
            if (currentAbility != Ability.None) numLives = 3;
            else numLives = 5;
        } else {
            currentLevel = level;
        }
    }

    public static void PracticeLevel(WorldEntrance.World world, int level){
        currentWorld = world;
        currentLevel = level;
        currentMode = GameMode.Practice;
    }
}
