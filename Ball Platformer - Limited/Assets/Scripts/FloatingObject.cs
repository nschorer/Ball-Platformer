using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour, IMovingObject {

    public float oscHeight = 1f;
    public float oscSpeed = 1f;
    public float phaseShift = 0f;
    public bool upFirst = true;

    private Vector3 startPos;
    private Vector3 startRot;
    private float maxRotation;
    private bool rewinding;
    private float fakeTime;
    private bool paused;

    private const float BASE_MAX_ROTATION = 10f;

    void Start(){

        startPos = transform.position;
        startRot = transform.eulerAngles;

        System.Random rand = new System.Random();
        maxRotation = BASE_MAX_ROTATION + (0.25f * BASE_MAX_ROTATION * (float)(rand.NextDouble()*2 - 1));
    }

	// FixedUpdate is called once per frame
	void FixedUpdate () {

        if (!paused)
        {
            float timeDiff = Time.deltaTime;
            if (rewinding) timeDiff *= -1;
            fakeTime += timeDiff;

            Oscillate();
            Twist();
        }
	}

    // Moves the object up and down
    void Oscillate() {
        transform.position = startPos + (upFirst ? 1: -1) * (oscHeight * Vector3.up * Mathf.Sin(fakeTime * oscSpeed + phaseShift));
    }

    // Makes the object rotate slightly as it moves up and down
    void Twist(){
        float angleDegrees = Mathf.Sin(fakeTime * oscSpeed * 4 / 5 + phaseShift) * maxRotation;
        transform.rotation = Quaternion.Euler(startRot.x, startRot.y + angleDegrees, startRot.z);
    }

    public void RewindTime(bool rewindOn){
        rewinding = rewindOn;
    }

    public void PauseRB(bool pauseOn)
    {
        paused = pauseOn;
        RigidbodyConstraints rbConstraints = pauseOn ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().constraints = rbConstraints;
    }
}
