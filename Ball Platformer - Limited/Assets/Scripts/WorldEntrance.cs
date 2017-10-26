using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorldEntrance : MonoBehaviour {

    public enum World { Forest, Desert, Canyon, Island, Space};
    public World loadWorld;
    public LevelManager levelManager;
    public LevelSelect levelSelect;

    private bool isEnabled;
    private int startID;

    // Use this for initialization
    void Start () {
        if (levelManager != null) startID = levelManager.GetWorldStartID(loadWorld);
        if (startID != 0 && levelSelect != null) isEnabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (isEnabled)
        {
            if (collider.tag == "Player")
            {
                levelSelect.SetWorldAndStartID(loadWorld, startID);
                levelSelect.ToggleLevelSelect(true);
            }
        }
    }

    
}
