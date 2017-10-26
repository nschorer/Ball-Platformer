using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{

    private bool isPaused;
    private GameObject[] movingObjects;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        isPaused = false;
        movingObjects = GameObject.FindGameObjectsWithTag("Moving");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void togglePause(bool pauseOn)
    {

        isPaused = pauseOn;
        gameObject.SetActive(pauseOn);

        foreach (GameObject movingObject in movingObjects)
        {
            IMovingObject mObject = movingObject.GetComponent<IMovingObject>();
            mObject.PauseRB(pauseOn);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
