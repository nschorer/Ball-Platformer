using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameData : MonoBehaviour {

    public static GameData gameData;
    public static bool loaded;

    public static int forestLvlsCleared;
    public static int desertLvlsCleared;
    public static int canyonLvlsCleared;
    public static int islandLvlsCleared;
    public static int spaceLvlsCleared;

    public static bool forestChallengeCleared;
    public static bool desertChallengeCleared;
    public static bool canyonChallengeCleared;
    public static bool islandChallengeCleared;
    public static bool spaceChallengeCleared;

    public static float[] forestBestTimes = new float[10];
    public static float[] desertBestTimes = new float[10];
    public static float[] canyonBestTimes = new float[10];
    public static float[] islandBestTimes = new float[10];
    public static float[] spaceBestTimes = new float[10];

    private bool needsSave;

    private const int NUM_FOREST_LVLS = 10;
    private const int NUM_DESERT_LVLS = 10;
    private const int NUM_CANYON_LVLS = 10;
    private const int NUM_ISLAND_LVLS = 10;
    private const int NUM_SPACE_LVLS = 10;

    private static string FILEPATH;

    void Awake () {
	    if (gameData == null){
            gameData = this;
            DontDestroyOnLoad(gameObject);
        }else if (gameData != this){
            Destroy(gameObject);
        }
	}

    void Start() {
        if (!loaded){
            FILEPATH = Application.persistentDataPath + "/SaveGameData.dat";
            Load();
        }
    }
	
    public bool hasClearedForest() { return forestLvlsCleared >= NUM_FOREST_LVLS; }
    public bool hasClearedDesert() { return desertLvlsCleared >= NUM_DESERT_LVLS; }
    public bool hasClearedCanyon() { return canyonLvlsCleared >= NUM_CANYON_LVLS; }
    public bool hasClearedIsland() { return islandLvlsCleared >= NUM_ISLAND_LVLS; }
    public bool hasClearedSpace() { return spaceLvlsCleared >= NUM_SPACE_LVLS; }

    public bool hasUnlockedAnAbility() {
        return hasClearedForest() ||
               hasClearedDesert() ||
               hasClearedCanyon() ||
               hasClearedIsland();
    }

    public float getBestTime(){
        float bestTime;
        int lvl = SessionData.currentLevel - 1;
        switch (SessionData.currentWorld){
            case WorldEntrance.World.Forest:
                bestTime = forestBestTimes[lvl];
                break;
            case WorldEntrance.World.Desert:
                bestTime = desertBestTimes[lvl];
                break;
            case WorldEntrance.World.Canyon:
                bestTime = canyonBestTimes[lvl];
                break;
            case WorldEntrance.World.Island:
                bestTime = islandBestTimes[lvl];
                break;
            case WorldEntrance.World.Space:
                bestTime = spaceBestTimes[lvl];
                break;
            default:
                Debug.LogError("Could not get best time.");
                bestTime = 999.99f;
                break;
        }
        return bestTime;
    }

    public void setBestTime(float bestTime){
        int lvl = SessionData.currentLevel - 1;
        switch (SessionData.currentWorld){
            case WorldEntrance.World.Forest:
                forestBestTimes[lvl] = bestTime;
                break;
            case WorldEntrance.World.Desert:
                desertBestTimes[lvl] = bestTime;
                break;
            case WorldEntrance.World.Canyon:
                canyonBestTimes[lvl] = bestTime;
                break;
            case WorldEntrance.World.Island:
                islandBestTimes[lvl] = bestTime;
                break;
            case WorldEntrance.World.Space:
                spaceBestTimes[lvl] = bestTime;
                break;
            default:
                Debug.LogError("Could not set best time.");
                break;
        }
        needsSave = true;
    }

    public void incrementLevelCleared(){
        switch (SessionData.currentWorld){
            case WorldEntrance.World.Forest:
                forestLvlsCleared++;
                break;
            case WorldEntrance.World.Desert:
                desertLvlsCleared++;
                break;
            case WorldEntrance.World.Canyon:
                canyonLvlsCleared++;
                break;
            case WorldEntrance.World.Island:
                islandLvlsCleared++;
                break;
            case WorldEntrance.World.Space:
                spaceLvlsCleared++;
                break;
            default:
                Debug.LogError("Could not increment level.");
                break;
        }
        needsSave = true;
    }

    public void clearChallenge(){
        switch (SessionData.currentWorld){
            case WorldEntrance.World.Forest:
                forestChallengeCleared = true;
                break;
            case WorldEntrance.World.Desert:
                desertChallengeCleared = true;
                break;
            case WorldEntrance.World.Canyon:
                canyonChallengeCleared = true;
                break;
            case WorldEntrance.World.Island:
                islandChallengeCleared = true;
                break;
            case WorldEntrance.World.Space:
                spaceChallengeCleared = true;
                break;
        }
        needsSave = true;
    }

    public bool hasClearedChallenge() {
        return hasClearedChallenge(SessionData.currentWorld);
    }

    public bool hasClearedChallenge(WorldEntrance.World world) {
        switch (world) {
            case WorldEntrance.World.Forest:
                return forestChallengeCleared;
            case WorldEntrance.World.Desert:
                return desertChallengeCleared;
            case WorldEntrance.World.Canyon:
                return canyonChallengeCleared;
            case WorldEntrance.World.Island:
                return islandChallengeCleared;
            case WorldEntrance.World.Space:
                return spaceChallengeCleared;
            default:
                Debug.LogError("Could not match case.");
                return false;
        }
    }

    public int getLvlsCleared() {
        return getLvlsCleared(SessionData.currentWorld);
    }

    public int getLvlsCleared(WorldEntrance.World world) {
        if (world == WorldEntrance.World.Forest) return forestLvlsCleared;
        else if (world == WorldEntrance.World.Desert) return desertLvlsCleared;
        else if (world == WorldEntrance.World.Canyon) return canyonLvlsCleared;
        else if (world == WorldEntrance.World.Island) return islandLvlsCleared;
        else if (world == WorldEntrance.World.Space) return spaceLvlsCleared;

        Debug.LogError("Could not match case.");
        return 0;
    }

    public bool hasClearedWorld() {
        return hasClearedWorld(SessionData.currentWorld);
    }

    public bool hasClearedWorld(WorldEntrance.World world) {
        if (world == WorldEntrance.World.Forest) return forestLvlsCleared >= NUM_FOREST_LVLS;
        else if (world == WorldEntrance.World.Desert) return desertLvlsCleared >= NUM_DESERT_LVLS;
        else if (world == WorldEntrance.World.Canyon) return canyonLvlsCleared >= NUM_CANYON_LVLS;
        else if (world == WorldEntrance.World.Island) return islandLvlsCleared >= NUM_ISLAND_LVLS;
        else if (world == WorldEntrance.World.Space) return spaceLvlsCleared >= NUM_SPACE_LVLS;

        Debug.LogError("Could not match case.");
        return false;
    }

    public void Load(){
        if (File.Exists(FILEPATH)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FILEPATH, FileMode.Open);
            SaveGameData data = (SaveGameData)bf.Deserialize(file);
            file.Close();

            LoadSaveDataIntoGameData(data);
        }
        //If this is our first time loading the game, there won't be any save data.
        //Still we need to mark loaded=true, so we don't try to get the data again.
        loaded = true;
    }

    private void LoadSaveDataIntoGameData(SaveGameData saveData){
        forestLvlsCleared = saveData.forestLvlsCleared;
        desertLvlsCleared = saveData.desertLvlsCleared;
        canyonLvlsCleared = saveData.canyonLvlsCleared;
        islandLvlsCleared = saveData.islandLvlsCleared;
        spaceLvlsCleared = saveData.spaceLvlsCleared;

        forestChallengeCleared = saveData.forestChallengeCleared;
        desertChallengeCleared = saveData.desertChallengeCleared;
        canyonChallengeCleared = saveData.canyonChallengeCleared;
        islandChallengeCleared = saveData.islandChallengeCleared;
        spaceChallengeCleared = saveData.spaceChallengeCleared;

        forestBestTimes = saveData.forestBestTimes;
        desertBestTimes = saveData.desertBestTimes;
        canyonBestTimes = saveData.canyonBestTimes;
        islandBestTimes = saveData.islandBestTimes;
        spaceBestTimes = saveData.spaceBestTimes;
    }

    public void Save(){
        if (loaded && needsSave) {

            //Create a new file or overwrite the old one
            FileStream file;
            if (File.Exists(FILEPATH)) file = File.OpenWrite(FILEPATH);
            else file = File.Create(FILEPATH);

            SaveGameData data = new MySaveGameData();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();

            //Create a new file or overwrite the old one
            //using (FileStream fs = new FileStream(FILEPATH, FileMode.OpenOrCreate)) {
            //    SaveGameData data = new MySaveGameData();

            //    //I don't know what I'm doing.
            //    //https://stackoverflow.com/questions/33646428/c-sharp-overwriting-file-with-streamwriter-created-from-filestream
            //    fs.SetLength(0);
            //    bf.Serialize(fs, data);
            //}
        }
    }

    // Basically, we only want this constructor to be visible within this GameData
    [System.Serializable]
    public class SaveGameData
    {
        public int forestLvlsCleared;
        public int desertLvlsCleared;
        public int canyonLvlsCleared;
        public int islandLvlsCleared;
        public int spaceLvlsCleared;

        public bool forestChallengeCleared;
        public bool desertChallengeCleared;
        public bool canyonChallengeCleared;
        public bool islandChallengeCleared;
        public bool spaceChallengeCleared;

        public float[] forestBestTimes;
        public float[] desertBestTimes;
        public float[] canyonBestTimes;
        public float[] islandBestTimes;
        public float[] spaceBestTimes;

        protected SaveGameData()
        {
            forestLvlsCleared = GameData.forestLvlsCleared;
            desertLvlsCleared = GameData.desertLvlsCleared;
            canyonLvlsCleared = GameData.canyonLvlsCleared;
            islandLvlsCleared = GameData.islandLvlsCleared;
            spaceLvlsCleared = GameData.spaceLvlsCleared;

            forestChallengeCleared = GameData.forestChallengeCleared;
            desertChallengeCleared = GameData.desertChallengeCleared;
            canyonChallengeCleared = GameData.canyonChallengeCleared;
            islandChallengeCleared = GameData.islandChallengeCleared;
            spaceChallengeCleared = GameData.spaceChallengeCleared;

            forestBestTimes = GameData.forestBestTimes;
            desertBestTimes = GameData.desertBestTimes;
            canyonBestTimes = GameData.canyonBestTimes;
            islandBestTimes = GameData.islandBestTimes;
            spaceBestTimes = GameData.spaceBestTimes;
        }
    }

    [System.Serializable]
    private class MySaveGameData : SaveGameData
    {
        public MySaveGameData() : base() { }
    }
}


