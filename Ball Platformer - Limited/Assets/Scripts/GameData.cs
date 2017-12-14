using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameData : MonoBehaviour {


    public static GameData currentGameFile;

    public int fileNum = 0;

    int forestLvlsCleared;
    int desertLvlsCleared;
    int canyonLvlsCleared;
    int islandLvlsCleared;
    int spaceLvlsCleared;

    bool forestChallengeCleared;
    bool desertChallengeCleared;
    bool canyonChallengeCleared;
    bool islandChallengeCleared;
    bool spaceChallengeCleared;

    float[] forestBestTimes = new float[10];
    float[] desertBestTimes = new float[10];
    float[] canyonBestTimes = new float[10];
    float[] islandBestTimes = new float[10];
    float[] spaceBestTimes = new float[10];

    bool[] forestMsgs = new bool[3];
    bool[] desertMsgs = new bool[3];
    bool[] canyonMsgs = new bool[3];
    bool[] islandMsgs = new bool[3];
    bool[] spaceMsgs = new bool[3];
    bool _hasShownSpaceMsg; //Specifically, have we UNLOCKED space. That's why this is not included in the spaceMsgs array.
    bool _hasUnlockedAbilityMsg;

    float musicVolume;
    float soundFXVolume;
    float camSens;
    PlayerController.BallColor ballColor;

    public int ForestLvlsCleared { get { return forestLvlsCleared; } }
    public int DesertLvlsCleared { get { return desertLvlsCleared; } }
    public int CanyonLvlsCleared { get { return canyonLvlsCleared; } }
    public int IslandLvlsCleared { get { return islandLvlsCleared; } }
    public int SpaceLvlsCleared { get { return spaceLvlsCleared; } }

    public bool ForestChallengeCleared { get { return forestChallengeCleared; } }
    public bool DesertChallengeCleared { get { return desertChallengeCleared; } }
    public bool CanyonChallengeCleared { get { return canyonChallengeCleared; } }
    public bool IslandChallengeCleared { get { return islandChallengeCleared; } }
    public bool SpaceChallengeCleared { get { return spaceChallengeCleared; } }

    public float MusicVolume { get {return musicVolume; } }
    public float SoundFXVolume { get { return soundFXVolume; } }
    public float CameraSensitivity { get { return camSens; } }
    public PlayerController.BallColor BallColor { get { return ballColor; } }

    private bool needsSave;
    private PlayerController player;
    // This is bad design. Since all 3 game files have a reference to this instance, we end up calling some methods on the musicplayer multiple times.
    // We should change it so it does it only once.
    private MusicPlayer musicPlayer;
    private SoundFx soundfx;

    private const int NUM_FOREST_LVLS = 10;
    private const int NUM_DESERT_LVLS = 10;
    private const int NUM_CANYON_LVLS = 10;
    private const int NUM_ISLAND_LVLS = 10;
    private const int NUM_SPACE_LVLS = 10;

    private const int CLEARED_IDX = 0;
    private const int CHALLENGE_IDX = 1;
    private const int TIMES_IDX = 2;

    private string FILEPATH;
    private static string PREFIX = "/SaveGameData";
    private static string SUFFIX = ".dat";

    private void Awake() {
        FILEPATH = Application.persistentDataPath + PREFIX + fileNum.ToString() + SUFFIX;
        if (currentGameFile != null && currentGameFile.GetId() == FILEPATH) {
            Destroy(this);
        }
    }

    void Start() {
        musicPlayer = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicPlayer>();
        soundfx = GameObject.FindGameObjectWithTag("Sound Fx").GetComponent<SoundFx>();
        ApplyDefaultValues();
        GameObject.DontDestroyOnLoad(gameObject);
        Load();
    }

    public void SetCurrentFile() {
        currentGameFile = this;
        ApplySavedSettings();
    }

    public bool SaveExists() {
        return File.Exists(FILEPATH);
    }

    public string GetId() {
        return FILEPATH;
    }

    public static string File1Id() { return PREFIX + "1" + SUFFIX; }
    public static string File2Id() { return PREFIX + "2" + SUFFIX; }
    public static string File3Id() { return PREFIX + "3" + SUFFIX; }

    public bool hasClearedForest() { return forestLvlsCleared >= NUM_FOREST_LVLS; }
    public bool hasClearedDesert() { return desertLvlsCleared >= NUM_DESERT_LVLS; }
    public bool hasClearedCanyon() { return canyonLvlsCleared >= NUM_CANYON_LVLS; }
    public bool hasClearedIsland() { return islandLvlsCleared >= NUM_ISLAND_LVLS; }
    public bool hasClearedSpace() { return spaceLvlsCleared >= NUM_SPACE_LVLS; }

    public int numChallengesCleared() {
        int count = 0;
        count += forestChallengeCleared ? 1 : 0;
        count += desertChallengeCleared ? 1 : 0;
        count += canyonChallengeCleared ? 1 : 0;
        count += islandChallengeCleared ? 1 : 0;
        count += spaceChallengeCleared ? 1 : 0;
        return count;
    }

    public bool hasUnlockedAnAbility() {
        return hasClearedForest() ||
               hasClearedDesert() ||
               hasClearedCanyon() ||
               hasClearedIsland();
    }

    public bool hasUnlockedSpace() {
        return forestChallengeCleared &&
               desertChallengeCleared &&
               canyonChallengeCleared &&
               islandChallengeCleared;
    }

    public float getBestTime() {
        return getBestTime(SessionData.currentLevel - 1, SessionData.currentWorld);
    }

    public float getBestTime(int lvl, WorldEntrance.World world){
        float bestTime;
        
        switch (world){
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

    public float[] getBestTimes(WorldEntrance.World world) {
        switch (world) {
            case WorldEntrance.World.Forest:
                return forestBestTimes;
            case WorldEntrance.World.Desert:
                return desertBestTimes;
            case WorldEntrance.World.Canyon:
                return canyonBestTimes;
            case WorldEntrance.World.Island:
                return islandBestTimes;
            case WorldEntrance.World.Space:
                return spaceBestTimes;
            default:
                Debug.LogError("Could not get best times.");
                return null;
        }
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

    public bool hasBeatChallengeTimes(WorldEntrance.World world) {
        float[] challengeTimes = StaticInfo.getChallengeTimes(world);
        float[] bestTimes;

        switch (world) {
            case WorldEntrance.World.Forest:
                bestTimes = forestBestTimes;
                break;
            case WorldEntrance.World.Desert:
                bestTimes = desertBestTimes; ;
                break;
            case WorldEntrance.World.Canyon:
                bestTimes = canyonBestTimes; ;
                break;
            case WorldEntrance.World.Island:
                bestTimes = islandBestTimes; ;
                break;
            case WorldEntrance.World.Space:
                bestTimes = spaceBestTimes; ;
                break;
            default:
                Debug.LogError("Could not match case.");
                return false;
        }

        for (int i = 0; i<challengeTimes.Length; i++) {
            float bestTime = bestTimes[i];
            if (bestTime == 0f || bestTime > challengeTimes[i]) return false; //The best time is 0f by default (if they haven't even cleared the level), so we need to check that as well.
        }

        return true;    
    }

    public bool hasShownClearMsg(WorldEntrance.World world) {
        return hasShownMsg(world, CLEARED_IDX);
    }

    public void SetShownClearWorldMsg(WorldEntrance.World world) {
        SetShownMsg(world, CLEARED_IDX);
    }

    public bool hasShownChallengeMsg(WorldEntrance.World world) {
        return hasShownMsg(world, CHALLENGE_IDX);
    }

    public void SetShownChallengeMsg(WorldEntrance.World world) {
        SetShownMsg(world, CHALLENGE_IDX);
    }

    public bool hasShownSpaceMsg() {
        return _hasShownSpaceMsg;
    }

    public void SetShownSpaceMsg() {
        _hasShownSpaceMsg = true;
        needsSave = true;
    }

    public bool hasShownTimesMsg(WorldEntrance.World world) {
        return hasShownMsg(world, TIMES_IDX);
    }

    public void SetShownTimesMsg(WorldEntrance.World world) {
        SetShownMsg(world, TIMES_IDX);
    }

    public bool hasUnlockedAbililtyMsg() {
        return _hasUnlockedAbilityMsg;
    }

    public void SetShownAbilityMsg() {
        _hasUnlockedAbilityMsg = true;
        needsSave = true;
    }

    bool hasShownMsg(WorldEntrance.World world, int idx) {
        switch (world) {
            case WorldEntrance.World.Forest:
                return forestMsgs[idx];
            case WorldEntrance.World.Desert:
                return desertMsgs[idx];
            case WorldEntrance.World.Canyon:
                return canyonMsgs[idx];
            case WorldEntrance.World.Island:
                return islandMsgs[idx];
            case WorldEntrance.World.Space:
                return spaceMsgs[idx];
            default:
                return false;
        }
    }

    void SetShownMsg(WorldEntrance.World world, int idx) {
        switch (world) {
            case WorldEntrance.World.Forest:
                forestMsgs[idx] = true;
                break;
            case WorldEntrance.World.Desert:
                desertMsgs[idx] = true;
                break;
            case WorldEntrance.World.Canyon:
                canyonMsgs[idx] = true;
                break;  
            case WorldEntrance.World.Island:
                islandMsgs[idx] = true;
                break;  
            case WorldEntrance.World.Space:
                spaceMsgs[idx] = true;
                break;
            default:
                Debug.LogError("Failed to set msg-shown flag.");
                break;
        }
        needsSave = true;
    }

    public void showClearMsg(WorldEntrance.World world) {
        showMsg(world, CLEARED_IDX);
    }

    public void showChallengeMsg(WorldEntrance.World world) {
        showMsg(world, CHALLENGE_IDX);
    }

    public void showTimesMsg(WorldEntrance.World world) {
        showMsg(world, TIMES_IDX);
    } 

    void showMsg(WorldEntrance.World world, int idx) {
        switch (world) {
            case WorldEntrance.World.Forest:
                forestMsgs[idx] = true;
                break;
            case WorldEntrance.World.Desert:
                desertMsgs[idx] = true;
                break;
            case WorldEntrance.World.Canyon:
                canyonMsgs[idx] = true;
                break;
            case WorldEntrance.World.Island:
                islandMsgs[idx] = true;
                break;
            case WorldEntrance.World.Space:
                spaceMsgs[idx] = true;
                break; 
        }
        needsSave = true;
    }

    public void SetMusicVolume(float vol) {
        musicVolume = vol;
        musicPlayer.SetVolume(vol);
        needsSave = true;
    }

    public void SetSoundFX(float vol) {
        soundFXVolume = vol;
        soundfx.SetVolume(vol);
        needsSave = true;
    }

    // This should be a value between 0f and 1f -- in other words, it should be the value of the slider in the pause menu.
    // The camera controller will modify the value as needed.
    public void SetCameraSensitivity(float sliderVal) {
        camSens = sliderVal;
        CameraController.instance.SetCameraSensitivity(sliderVal);
        needsSave = true;
    }

    public void SetPlayerReference(PlayerController playerController) {
        player = playerController;
    }

    public void SetBallColor(PlayerController.BallColor color) {
        ballColor = color;
        player.SetBallColor(color);
        needsSave = true;
    }

    public void Load(){

        // If we are continuing a game, load the information from the file.
        if (File.Exists(FILEPATH)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FILEPATH, FileMode.Open);
            SaveGameData data = (SaveGameData)bf.Deserialize(file);
            file.Close();

            LoadSaveDataIntoGameData(data);

            // Uncomment to test messages
            //forestMsgs = new bool[3];
            //desertMsgs = new bool[3];
            //canyonMsgs = new bool[3];
            //islandMsgs = new bool[3];
            //spaceMsgs = new bool[3];
            
        
        // If this is a new game, what information needs to be set by default.
        }else {
            SetDefaultValues();
        }

        //ApplySavedSettings();
    }

    void SetDefaultValues() {
        musicVolume = 0.5f;
        soundFXVolume = 1f;
        camSens = 0.5f;
        ballColor = PlayerController.BallColor.Maroon;
    }

    public void ApplySavedSettings() {
        musicPlayer.SetVolume(musicVolume);
        soundfx.SetVolume(soundFXVolume);
        //CameraController.instance.SetCameraSensitivity(camSens);
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //player.SetBallColor(ballColor);
    }

    public void ApplyDefaultValues() {
        musicPlayer.SetVolume(0.5f);
        soundfx.SetVolume(1f);
    }

    // We have deserialized the data from the file. Now, we just need to load it into our main GameData class.
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

        forestMsgs = saveData.forestMsgs;
        desertMsgs = saveData.desertMsgs;
        canyonMsgs = saveData.canyonMsgs;
        islandMsgs = saveData.islandMsgs;
        spaceMsgs = saveData.spaceMsgs;
        _hasShownSpaceMsg = saveData._hasShownSpaceMsg;
        _hasUnlockedAbilityMsg = saveData._hasUnlockedAbilityMsg;

        musicVolume = saveData.musicVolume;
        soundFXVolume = saveData.soundFXVolume;
        camSens = saveData.camSens;
        ballColor = saveData.ballColor;
    }

    public void Delete() {
        if (File.Exists(FILEPATH)) {
            File.Delete(FILEPATH);
        }

        forestLvlsCleared = 0;
        desertLvlsCleared = 0;
        canyonLvlsCleared = 0;
        islandLvlsCleared = 0;
        spaceLvlsCleared = 0;

        forestChallengeCleared = false;
        desertChallengeCleared = false;
        canyonChallengeCleared = false;
        islandChallengeCleared = false;
        spaceChallengeCleared = false;

        forestBestTimes = new float[10];
        desertBestTimes = new float[10];
        canyonBestTimes = new float[10];
        islandBestTimes = new float[10];
        spaceBestTimes = new float[10];

        forestMsgs = new bool[3];
        desertMsgs = new bool[3];
        canyonMsgs = new bool[3];
        islandMsgs = new bool[3];
        spaceMsgs = new bool[3];
        _hasShownSpaceMsg = false;
        _hasUnlockedAbilityMsg = false;

        SetDefaultValues();
    }

    public void Save(){
        if (needsSave) {

            //Create a new file or overwrite the old one
            FileStream file;
            if (File.Exists(FILEPATH)) file = File.OpenWrite(FILEPATH);
            else file = File.Create(FILEPATH);

            SaveGameData data = new MySaveGameData(currentGameFile);
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

        public bool[] forestMsgs;
        public bool[] desertMsgs;
        public bool[] canyonMsgs;
        public bool[] islandMsgs;
        public bool[] spaceMsgs;
        public bool _hasShownSpaceMsg; //Specifically, have we UNLOCKED space. That's why this is not included in the spaceMsgs array.
        public bool _hasUnlockedAbilityMsg;

        public float musicVolume;
        public float soundFXVolume;
        public float camSens;
        public PlayerController.BallColor ballColor;

        protected SaveGameData(GameData gameData)
        {
            forestLvlsCleared = gameData.forestLvlsCleared;
            desertLvlsCleared = gameData.desertLvlsCleared;
            canyonLvlsCleared = gameData.canyonLvlsCleared;
            islandLvlsCleared = gameData.islandLvlsCleared;
            spaceLvlsCleared = gameData.spaceLvlsCleared;

            forestChallengeCleared = gameData.forestChallengeCleared;
            desertChallengeCleared = gameData.desertChallengeCleared;
            canyonChallengeCleared = gameData.canyonChallengeCleared;
            islandChallengeCleared = gameData.islandChallengeCleared;
            spaceChallengeCleared = gameData.spaceChallengeCleared;

            forestBestTimes = gameData.forestBestTimes;
            desertBestTimes = gameData.desertBestTimes;
            canyonBestTimes = gameData.canyonBestTimes;
            islandBestTimes = gameData.islandBestTimes;
            spaceBestTimes = gameData.spaceBestTimes;

            forestMsgs = gameData.forestMsgs;
            desertMsgs = gameData.desertMsgs;
            canyonMsgs = gameData.canyonMsgs;
            islandMsgs = gameData.islandMsgs;
            spaceMsgs = gameData.spaceMsgs;
            _hasShownSpaceMsg = gameData._hasShownSpaceMsg;
            _hasUnlockedAbilityMsg = gameData._hasUnlockedAbilityMsg;

            musicVolume = gameData.musicVolume;
            soundFXVolume = gameData.soundFXVolume;
            camSens = gameData.camSens;
            ballColor = gameData.ballColor;
        }
    }

    [System.Serializable]
    private class MySaveGameData : SaveGameData
    {
        public MySaveGameData(GameData gameData) : base(gameData) { }
    }
}


